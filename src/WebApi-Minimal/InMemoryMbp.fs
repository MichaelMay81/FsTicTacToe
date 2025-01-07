module FsTicTacToe.WebApi.InMemoryMbp

open FsTicTacToe

type Error = | Error of string

type Message =
| GetBoard of AsyncReplyChannel<Board>
| SetSquare of RowIndex * ColumnIndex * AsyncReplyChannel<Error option>

let start () =
    MailboxProcessor<Message>.Start(fun inbox ->
        let rec loop (board:Board) =
            async {
                let! message = inbox.Receive()
                match message with
                | GetBoard replyChannel ->
                    printfn "GetBoard"
                    replyChannel.Reply board
                    return! loop board
                | SetSquare (row, column, replyChannel) ->
                    printfn "SetSquare %O %O" row column
                    let newBoard =
                        board
                        |> Boards.setSquare row column
                        |> function
                        | Ok newBoard ->
                            replyChannel.Reply None
                            newBoard
                        | Result.Error message ->
                            replyChannel.Reply (Some (Error message))
                            board
                    return! loop newBoard
            }
        loop Boards.empty)