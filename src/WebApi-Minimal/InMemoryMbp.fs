module FsTicTacToe.WebApi.InMemoryMbp

open TicTacToe

type Error = | Error of string

type Message =
| GetBoard of AsyncReplyChannel<Board>
| SetSquare of int * int * AsyncReplyChannel<Error option>

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
                | SetSquare (x, y, replyChannel) ->
                    printfn "SetSquare %d %d" x y
                    let newBoard =
                        board
                        |> Boards.setSquare x y
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