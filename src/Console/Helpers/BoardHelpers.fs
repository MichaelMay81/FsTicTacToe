module FsTicTacToe.Boards

open System.IO
open System.Text.Json
open System.Text.Json.Serialization

let private squareToString = function
    | Some X -> "X"
    | Some O -> "O"
    | None -> " "

let private rowToString (delimiter:string) (row:Row) : string =
    squareToString row.Square1 + delimiter +
    squareToString row.Square2 + delimiter +
    squareToString row.Square3 + delimiter

let toString (board:Board) : string =
    let rowDelimiter = " | "
    let lineDelimiter = "\n"
    rowToString rowDelimiter board.Row1 + lineDelimiter +
    rowToString rowDelimiter board.Row2 + lineDelimiter +
    rowToString rowDelimiter board.Row3 + lineDelimiter

let private options =
    JsonFSharpOptions.Default()
        .ToJsonSerializerOptions()

let toFile (path:string) (board:Board) : unit =
    JsonSerializer.Serialize (board, options)
    |> fun json -> File.WriteAllText (path, json)

let fromFile (path:string) : Result<Board, string> =
    try
        File.ReadAllText path
        |> Ok
    with ex ->
        Error (ex.Message)
    |> Result.bind (fun str ->
        try
            JsonSerializer.Deserialize<Board> (str, options)
            |> Ok
        with ex ->
            Error (ex.Message))