namespace TicTacToe

module Boards =

    let empty : Board =
        Array.init 3 (fun _ ->
            Array.init 3 (fun _ ->
                None))

    let private selectSquares
        (board:Board)
        ((a1,a2), (b1,b2), (c1,c2))
        : Square*Square*Square =
        board[a1][a2],
        board[b1][b2],
        board[c1][c2]

    let private calcualateSquaresWinner = function
        | Some X, Some X, Some X -> Some X
        | Some O, Some O, Some O -> Some O
        | _, _, _ -> None

    let calculateWinner (board:Board) : Player option =
            // lines
        [   ((0, 0), (0, 1), (0, 2)); ((1, 0), (1, 1), (1, 2)); ((2, 0), (2, 1), (2, 2))
            // rows
            ((0, 0), (1, 0), (2, 0)); ((0, 1), (1, 1), (2, 1)); ((0, 2), (1, 2), (2, 2))
            // diagonals
            ((0, 0), (1, 1), (2, 2)); ((0, 2), (1, 1), (2, 0))]
        |> Seq.map (selectSquares board)
        |> Seq.map calcualateSquaresWinner
        |> Seq.choose id
        |> Seq.tryHead

    let nextPlayer (board:Board) : Player =
        seq { for x in 0..2 do for y in 0..2 do board[x][y] }
        |> Seq.fold (fun (xCount,oCount) square ->
                match square with
                | Some X -> (xCount + 1, oCount)
                | Some O -> (xCount, oCount + 1)
                | None -> (xCount, oCount)
            ) (0,0)
        |> fun (xCount,oCount) ->
            if xCount > oCount then O
            else X

    let setSquare (x:int) (y:int) (board:Board) : Result<Board, string> =
        if x < 0 || x > 2 || y < 0 || y > 2 then
            Error $"Square {x}/{y} invalid"
        elif board[x][y] <> None then
            Error $"Square {x}/{y} already taken"
        else
            let nextPlayer = nextPlayer board
            board
            |> Array.updateAt x (
                board.[x]
                |> Array.updateAt y
                    (Some nextPlayer))
            |> Ok
