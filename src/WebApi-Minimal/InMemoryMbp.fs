module FsTicTacToe.WebApi.InMemoryMbp

open FsTicTacToe

type Message<'BoardId when 'BoardId : comparison> =
| GetBoards of AsyncReplyChannel<Map<'BoardId, Board>>
| GetBoard of 'BoardId * AsyncReplyChannel<Result<Board, Error>>
| SetSquare of 'BoardId * RowIndex * ColumnIndex * AsyncReplyChannel<Error option>

let private tryGetBoard boardId boards =
    boards
    |> Map.tryFind boardId
    |> function
    | Some board ->
        Ok board
    | None ->
        Result.Error (Error $" Error: Board {boardId} not found")

let private processMessage boards message =
    match message with
    | GetBoards replyChannel ->
        printfn "GetBoards"
        replyChannel.Reply boards
        boards
    | GetBoard (boardId, replyChannel) ->
        printfn "GetBoard %O" boardId
        boards
        |> tryGetBoard boardId
        |> replyChannel.Reply
        boards
    | SetSquare (boardId, row, column, replyChannel) ->
        printfn "SetSquare %O %O on Board %O" row column boardId
        let newBoards =
            boards
            |> tryGetBoard boardId
            |> Result.defaultWith (fun _ ->
                printfn $"Board not found, creating new one {boardId}"
                Boards.empty)
            |> Boards.setSquare row column
            |> function
            | Ok newBoard ->
                replyChannel.Reply None
                boards
                |> Map.add boardId newBoard
            | Result.Error message ->
                replyChannel.Reply (Some message)
                boards
        newBoards

let start () =
    MailboxProcessor<Message<'BoardId>>.Start(fun inbox ->
        let rec loop (boards:Map<'BoardId,Board>) =
            async {
                let! message = inbox.Receive()
                let newBoards = processMessage boards message
                return! loop newBoards
            }
        loop Map.empty)