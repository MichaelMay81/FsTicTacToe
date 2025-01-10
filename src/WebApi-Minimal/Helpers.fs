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

let private toOption =
    function
    | true, value -> Some value
    | false, _ -> None

let getOrSetSessionCoockie
    (cookieKey:string)
    (cookieExpires:TimeSpan)
    (request:HttpRequest)
    (response:HttpResponse)
    : Guid =
    
    request.Cookies.TryGetValue cookieKey
    |> toOption
    |> Option.bind (Guid.TryParse >> toOption) 
    |> Option.defaultWith (fun _ ->
        let value = Guid.NewGuid()
        let options = CookieOptions()
        options.HttpOnly <- true
        options.Secure <- request.IsHttps
        options.Expires <- DateTimeOffset.UtcNow.Add(cookieExpires)
        response.Cookies.Append(cookieKey, value.ToString(), options)
        value )
