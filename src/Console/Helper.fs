namespace TicTacToe

open System.IO
open System.Text.Json
open System.Text.Json.Serialization

module Boards =
    
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

    let options =
        JsonFSharpOptions.Default()
            .ToJsonSerializerOptions()

    let toFile (path:string) (board:Board) : unit =
        JsonSerializer.Serialize (board, options)
        |> fun json -> File.WriteAllText (path, json)

    let fromFile (path:string) : Board =
        File.ReadAllText path
        |> fun str -> JsonSerializer.Deserialize<Board> (str, options)