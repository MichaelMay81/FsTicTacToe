module FsTicTacToe.GameLoop

open System

let private parseRowColumnIndex (str:string) : (ColumnIndex*RowIndex) option =
    str.Split(' ')
    |> function
    | [| rowInt; columnInt |] ->
        printfn $"column: {columnInt} rowInt: {rowInt}"
        columnInt
        |> Helpers.tryParseColumnIndex
        |> Option.bind (fun columnIndex ->
            rowInt
            |> Helpers.tryParseRowIndex
            |> Option.map(fun rowIndex ->
                columnIndex, rowIndex))
    | _ -> None

let gameLoop (newBoardCallback: Board -> unit) (gameBoard:Board) : unit =
    let rec gameLoopRec (board:Board) =
        // get user input
        Console.ReadLine()
        |> parseRowColumnIndex
        |> function
        | Some value -> Ok value 
        | None -> Error "Please, just insert two numbers separated by a space."
        |> function
        | Ok (columnIndex, rowIndex) ->
            // set square
            Boards.setSquare rowIndex columnIndex  board
            |> function
            | Error msg ->
                printfn "%s" msg
                gameLoopRec board
            | Ok newBoard ->
                newBoard |> newBoardCallback
                printfn "%s" (newBoard |> Boards.toString)
                
                // check for winner
                Boards.calculateWinner newBoard
                |> function
                | Some player ->
                    printfn "Player %A wins!" player
                | None ->
                    gameLoopRec newBoard
        | Error error ->
            printfn "%s" error
            gameLoopRec board

    gameBoard
    |> gameLoopRec