
open System.IO
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.CSharp
open Microsoft.CodeAnalysis.MSBuild

type ConvertInternalToPublic() =
    inherit CSharpSyntaxRewriter()

    let tInternal = SyntaxFactory.Token SyntaxKind.InternalKeyword
    let tProtected = SyntaxFactory.Token SyntaxKind.ProtectedKeyword

    let replaceInternalWithPublic (tokens: SyntaxTokenList) =
        seq {
        for t in tokens do
            if t.RawKind = tInternal.RawKind then
                yield SyntaxFactory.Token(t.LeadingTrivia, SyntaxKind.PublicKeyword, t.TrailingTrivia)
            else
                yield t }
        |> SyntaxFactory.TokenList

    let hasProtected (tokens: SyntaxTokenList) =
        tokens |> Seq.exists (fun t -> t.RawKind = tProtected.RawKind)

    let updateInternal (tokens: SyntaxTokenList) =
        if hasProtected tokens then tokens
        else replaceInternalWithPublic tokens

    override x.VisitClassDeclaration d =
        let d = base.VisitClassDeclaration d :?> Syntax.ClassDeclarationSyntax
        d.Modifiers
        |> replaceInternalWithPublic
        |> d.WithModifiers
        :> _

    override x.VisitInterfaceDeclaration d =
        let d = base.VisitInterfaceDeclaration d :?> Syntax.InterfaceDeclarationSyntax
        d.Modifiers
        |> replaceInternalWithPublic
        |> d.WithModifiers
        :> _

    override x.VisitStructDeclaration d =
        let d = base.VisitStructDeclaration d :?> Syntax.StructDeclarationSyntax
        d.Modifiers
        |> replaceInternalWithPublic
        |> d.WithModifiers
        :> _

    override x.VisitFieldDeclaration d =
        d.Modifiers
        |> replaceInternalWithPublic
        |> d.WithModifiers
        :> _

    override x.VisitDelegateDeclaration d =
        d.Modifiers
        |> replaceInternalWithPublic
        |> d.WithModifiers
        :> _

    override x.VisitMethodDeclaration d =
        d.Modifiers
        |> updateInternal
        |> d.WithModifiers
        :> _

    override x.VisitEnumDeclaration d =
        d.Modifiers
        |> replaceInternalWithPublic
        |> d.WithModifiers
        :> _

    override x.VisitPropertyDeclaration d =
        d.Modifiers
        |> updateInternal
        |> d.WithModifiers
        :> _

    override x.VisitConstructorDeclaration d =
        d.Modifiers
        |> replaceInternalWithPublic
        |> d.WithModifiers
        :> _

[<EntryPoint>]
let main argv = 
    use ws = MSBuildWorkspace.Create()
    let proj = 
        //@"..\..\..\roslyn\src\Compilers\Core\Portable\CodeAnalysis.csproj"
        argv.[0]
    let pr = ws.OpenProjectAsync proj |> Async.RunTask
    let converter = new ConvertInternalToPublic()
    for doc in pr.Documents do
        printfn "updating %s" doc.FilePath
        let root = doc.GetSyntaxRootAsync() |> Async.RunTask
        let publicRoot = converter.Visit root
        let publicDoc = doc.WithSyntaxRoot publicRoot
        let text = publicDoc.GetTextAsync() |> Async.RunTask
        use fw = new StreamWriter(doc.FilePath)
        text.Write fw
    0