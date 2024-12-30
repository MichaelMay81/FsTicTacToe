namespace TicTacToe

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