open System
open TicTacToe

printfn """
TicTacToe

Insert the coordinates of the square you want to play,
separated by a space. Ctrl-C to exit.

GameBoard:
(0 0) (0 1) (0 2)
(1 0) (1 1) (1 2)
(2 0) (2 1) (2 2)"""

let rec gameLoop (board:Board) =
    Console.ReadLine().Split(' ')
    |> Array.map Int32.TryParse
    |> function
    | [|true, x; true, y|] ->
        Boards.setSquare x y board
        |> function
        | Error msg ->
            printfn "%s" msg
            gameLoop board
        | Ok newBoard ->
            printfn "%s" (newBoard |> Boards.toString)
            let winner = Boards.calculateWinner newBoard
            match winner with
            | Some player ->
                printfn "Player %A wins!" player
            | None ->
                gameLoop newBoard
    | _ ->
        printfn "Please, just insert two numbers separated by a space."
        gameLoop board

Boards.empty
|> gameLoop