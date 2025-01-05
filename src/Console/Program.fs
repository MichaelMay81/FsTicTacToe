open FsTicTacToe
open System.IO

let helpText = "Usage: TicTacToe [-persistance <filePath>]"
let introText = """
FsTicTacToe

Insert the coordinates of the square you want to play,
separated by a space. Ctrl-C to exit.

GameBoard:
(0 0) (0 1) (0 2)
(1 0) (1 1) (1 2)
(2 0) (2 1) (2 2)"""

[<EntryPoint>]
let main (args: string array) : int =
    let filePath =
        match args with
        | [||] -> None
        | [| "--persistance"; filePath|] -> Some filePath
        | _ ->
            printfn "%s" helpText
            exit 1

    printfn "%s" introText

    match filePath with
    | Some path ->
        Boards.fromFile path
        |> function
            | Error msg ->
                printfn "Error loading Board state: %s" msg
                printfn "Starting new game..."
                Boards.empty
            | Ok board ->
                printfn "%s" (board |> Boards.toString)
                board
        |> GameLoop.gameLoop (fun board -> Boards.toFile path board)
        File.Delete path
    | None ->
        Boards.empty
        |> GameLoop.gameLoop ignore

    0