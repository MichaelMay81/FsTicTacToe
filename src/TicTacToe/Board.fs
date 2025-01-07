module FsTicTacToe.Boards

let empty : Board =
    { Row1 = { Square1 = None; Square2 = None; Square3 = None }
      Row2 = { Square1 = None; Square2 = None; Square3 = None }
      Row3 = { Square1 = None; Square2 = None; Square3 = None }}

let private allSquares (board:Board) : Square seq =
    seq {
        board.Row1.Square1; board.Row1.Square2; board.Row1.Square3
        board.Row2.Square1; board.Row2.Square2; board.Row2.Square3
        board.Row3.Square1; board.Row3.Square2; board.Row3.Square3 }

let private selectRow (board:Board) (row:RowIndex) : Row =
    match row with
    | R1 -> board.Row1
    | R2 -> board.Row2
    | R3 -> board.Row3

let private selectSquare (board:Board) (row:RowIndex) (column:ColumnIndex) : Square =
    let row = selectRow board row
    match column with
    | C1 -> row.Square1
    | C2 -> row.Square2
    | C3 -> row.Square3

let private selectSquares
    (board:Board)
    ((a1,a2), (b1,b2), (c1,c2))
    : Square*Square*Square =
    selectSquare board a1 a2,
    selectSquare board b1 b2,
    selectSquare board c1 c2

let private calcualateSquaresWinner = function
    | Some X, Some X, Some X -> Some X
    | Some O, Some O, Some O -> Some O
    | _, _, _ -> None

/// Whoever has three in a row, column or diagonal wins.
let calculateWinner (board:Board) : Player option =
        // lines
    [   ((R1, C1), (R1, C2), (R1, C3)); ((R2, C1), (R2, C2), (R2, C3)); ((R3, C1), (R3, C2), (R3, C3))
        // rows
        ((R1, C1), (R2, C1), (R3, C1)); ((R1, C2), (R2, C2), (R3, C2)); ((R1, C3), (R2, C3), (R3, C3))
        // diagonals
        ((R1, C1), (R2, C2), (R3, C3)); ((R1, C3), (R2, C2), (R3, C1))]
    |> Seq.map (selectSquares board)
    |> Seq.map calcualateSquaresWinner
    |> Seq.choose id
    |> Seq.tryHead

/// Whoever has the fewer squares on the board is the next player.
let nextPlayer (board:Board) : Player =
    allSquares board
    |> Seq.fold (fun (xCount,oCount) square ->
            match square with
            | Some X -> (xCount + 1, oCount)
            | Some O -> (xCount, oCount + 1)
            | None -> (xCount, oCount)
        ) (0,0)
    |> fun (xCount,oCount) ->
        if xCount > oCount then O
        else X

let setSquare (row:RowIndex) (column:ColumnIndex) (board:Board) : Result<Board, string> =
    if selectSquare board row column <> None then
        Error $"Square {row}/{column} already taken"
    else
        let nextPlayer = nextPlayer board
        
        match row, column with
        | R1, C1 -> { board with Row1 = { board.Row1 with Square1 = Some nextPlayer }}
        | R1, C2 -> { board with Row1 = { board.Row1 with Square2 = Some nextPlayer }}
        | R1, C3 -> { board with Row1 = { board.Row1 with Square3 = Some nextPlayer }}
        | R2, C1 -> { board with Row2 = { board.Row2 with Square1 = Some nextPlayer }}
        | R2, C2 -> { board with Row2 = { board.Row2 with Square2 = Some nextPlayer }}
        | R2, C3 -> { board with Row2 = { board.Row2 with Square3 = Some nextPlayer }}
        | R3, C1 -> { board with Row3 = { board.Row3 with Square1 = Some nextPlayer }}
        | R3, C2 -> { board with Row3 = { board.Row3 with Square2 = Some nextPlayer }}
        | R3, C3 -> { board with Row3 = { board.Row3 with Square3 = Some nextPlayer }}
        |> Ok