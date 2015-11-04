
open System.IO
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.CSharp.Syntax
open Microsoft.CodeAnalysis.MSBuild

type ConvertInternalToPublic() =
    inherit CSharpSyntaxRewriter()

    let tInternal = SyntaxFactory.Token SyntaxKind.InternalKeyword
    let tProtected = SyntaxFactory.Token SyntaxKind.ProtectedKeyword
    let tPublic = SyntaxFactory.Token SyntaxKind.PublicKeyword

    let isInternal (t:SyntaxToken) = t.RawKind = tInternal.RawKind
    let isProtected (t:SyntaxToken) = t.RawKind = tProtected.RawKind
    let isPublic (t:SyntaxToken) = t.RawKind = tPublic.RawKind

    let hasInternal (tokens: SyntaxTokenList) = tokens |> Seq.exists isInternal
    let hasProtected (tokens: SyntaxTokenList) = tokens |> Seq.exists isProtected
    let hasPublic (tokens: SyntaxTokenList) = tokens |> Seq.exists isPublic

    let replaceInternalWithPublic (tokens: SyntaxTokenList) =
        seq {
            for t in tokens do
                if t.RawKind = tInternal.RawKind then
                    yield SyntaxFactory.Token(t.LeadingTrivia, SyntaxKind.PublicKeyword, t.TrailingTrivia)
                else yield t }
        |> SyntaxFactory.TokenList

    let hasProtectedAndPublic (tokens: SyntaxTokenList) =
        tokens |> Seq.exists isProtected
        && tokens |> Seq.exists isPublic

    let removeProtectedLeavePublic (tokens: SyntaxTokenList) =
        let tProtected = tokens |> Seq.find isProtected
        let tPublic = tokens |> Seq.find isPublic
        let leadingTrivia, trailingTrivia =
            if tProtected.HasLeadingTrivia then tProtected.LeadingTrivia, tProtected.TrailingTrivia
            else tPublic.LeadingTrivia, tPublic.TrailingTrivia
        seq {
            yield SyntaxFactory.Token(leadingTrivia, SyntaxKind.PublicKeyword, trailingTrivia)
            for t in tokens do
                if isProtected t || isPublic t then ()
                else yield t }
        |> SyntaxFactory.TokenList

    let updateInternal (tokens: SyntaxTokenList) =
        let tokens = replaceInternalWithPublic tokens
        if hasProtectedAndPublic tokens then
            removeProtectedLeavePublic tokens
        else tokens

    override x.VisitClassDeclaration d =
        let d = base.VisitClassDeclaration d :?> Syntax.ClassDeclarationSyntax
        d.Modifiers
        |> updateInternal
        |> d.WithModifiers
        :> _

    override x.VisitInterfaceDeclaration d =
        let d = base.VisitInterfaceDeclaration d :?> Syntax.InterfaceDeclarationSyntax
        d.Modifiers
        |> updateInternal
        |> d.WithModifiers
        :> _

    override x.VisitStructDeclaration d =
        let d = base.VisitStructDeclaration d :?> Syntax.StructDeclarationSyntax
        d.Modifiers
        |> updateInternal
        |> d.WithModifiers
        :> _

    override x.VisitFieldDeclaration d =
        d.Modifiers
        |> updateInternal
        |> d.WithModifiers
        :> _

    override x.VisitDelegateDeclaration d =
        d.Modifiers
        |> updateInternal
        |> d.WithModifiers
        :> _

    override x.VisitMethodDeclaration d =
        d.Modifiers
        |> updateInternal
        |> d.WithModifiers
        :> _

    override x.VisitEnumDeclaration d =
        d.Modifiers
        |> updateInternal
        |> d.WithModifiers
        :> _

    override x.VisitPropertyDeclaration d =
        // change internal to public for property
        let d =
            d.Modifiers
            |> updateInternal
            |> d.WithModifiers

        // change internal to public for accessors
        let d =
            let al = d.AccessorList
            if isNull al then d
            else
                al.Accessors
                |> Seq.map (fun a ->
                    a.Modifiers
                    |> updateInternal
                    |> a.WithModifiers )
                |> SyntaxFactory.List
                |> al.WithAccessors
                |> d.WithAccessorList

        // remove public from accessor if property is public
        let al = d.AccessorList
        if isNull al then d
        else
            al.Accessors
            |> Seq.map (fun a ->
                a.Modifiers
                |> Seq.filter (fun m -> not (hasPublic d.Modifiers && isPublic m))
                |> SyntaxFactory.TokenList
                |> a.WithModifiers )
            |> SyntaxFactory.List
            |> al.WithAccessors
            |> d.WithAccessorList
        :> _

    override x.VisitConstructorDeclaration d =
        d.Modifiers
        |> updateInternal
        |> d.WithModifiers
        :> _

[<EntryPoint>]
let main argv = 
    for proj in argv do
        printfn "update project %s" proj
        use ws = MSBuildWorkspace.Create()
        let pr = ws.OpenProjectAsync proj |> Async.RunTask
        let converter = new ConvertInternalToPublic()
        for doc in pr.Documents do
            printfn "update file %s" doc.FilePath
            let root = doc.GetSyntaxRootAsync() |> Async.RunTask
            let publicRoot = converter.Visit root
            let publicDoc = doc.WithSyntaxRoot publicRoot
            let text = publicDoc.GetTextAsync() |> Async.RunTask
            use fw = new StreamWriter(doc.FilePath)
            text.Write fw
        printfn ""
    0