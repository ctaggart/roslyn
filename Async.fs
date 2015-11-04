[<RequireQualifiedAccess>]
module Async

/// raise the InnerException instead of AggregateException if there is just one
let AwaitTaskOne task = async {
    try
        return! Async.AwaitTask task
    with e ->
        return
            match e with
            | :? System.AggregateException as ae ->
                if ae.InnerExceptions.Count = 1 then raise ae.InnerException
                else raise ae
            | _ -> raise e }

/// run a task synchronously
let RunTask task =
    task |> AwaitTaskOne |> Async.RunSynchronously