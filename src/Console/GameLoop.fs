namespace TicTacToe

open System

module GameLoop =
    
    let gameLoop (newBoardCallback: Board -> unit) (gameBoard:Board) : unit=
        let rec gameLoopRec (board:Board) =
            printfn "%s" (board |> Boards.toString)
            Console.ReadLine().Split(' ')
            |> Array.map Int32.TryParse
            |> function
            | [|true, x; true, y|] ->
                Boards.setSquare x y board
                |> function
                | Error msg ->
                    printfn "%s" msg
                    gameLoopRec board
                | Ok newBoard ->
                    newBoard |> newBoardCallback
                    printfn "%s" (newBoard |> Boards.toString)
                    let winner = Boards.calculateWinner newBoard
                    match winner with
                    | Some player ->
                        printfn "Player %A wins!" player
                    | None ->
                        gameLoopRec newBoard
            | _ ->
                printfn "Please, just insert two numbers separated by a space."
                gameLoopRec board

        gameBoard
        |> gameLoopRec