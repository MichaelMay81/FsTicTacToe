module FsTicTacToe.Boards

open System.IO
open System.Text.Json
open System.Text.Json.Serialization
    
let toString (board:Board) : string =
    board
    |> Seq.map (fun row ->
        row
        |> Seq.map (function
            | Some X -> "X"
            | Some O -> "O"
            | None -> " "
        )
        |> String.concat " | ")
    |> String.concat "\n"

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