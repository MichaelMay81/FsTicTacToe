module FsTicTacToe.WebApi.Helpers

open System
open Microsoft.AspNetCore.Http
open FsTicTacToe

let parseIndex (row:int) (column:int) : Result<RowIndex*ColumnIndex, Error> =
    row
    |> function | 1 -> Some R1 | 2 -> Some R2 | 3 -> Some R3 | _ -> None
    |> Option.bind (fun rowIndex ->
        column
        |> function | 1 -> Some C1 | 2 -> Some C2 | 3 -> Some C3 | _ -> None
        |> Option.map (fun columnIndex -> rowIndex, columnIndex))
    |> function
    | None -> "Values for row and column must be values of 1-3." |> Error |> Result.Error
    | Some values -> Result.Ok values

let getOrSetSessionCoockie
    (cookieKey:string)
    (cookieExpires:TimeSpan)
    (request:HttpRequest)
    (response:HttpResponse)
    : string =
    
    request.Cookies.TryGetValue cookieKey
    |> function
    | true, value ->
        // printfn "Cookie %s found: %s" cookieKey value
        value
    | false, _ ->
        let value = Guid.NewGuid().ToString()
        // printfn "Cookie %s set to %s" cookieKey value
        let options = CookieOptions()
        options.HttpOnly <- true
        options.Secure <- request.IsHttps
        options.Expires <- DateTimeOffset.UtcNow.Add(cookieExpires)
        response.Cookies.Append(cookieKey, value, options)
        value