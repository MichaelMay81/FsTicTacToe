namespace FsTicTacToe

type Player = | X | O

[<CLIMutable>]
type Square = Player option
[<CLIMutable>]
type Row = {
    Square1: Square
    Square2: Square
    Square3: Square }
[<CLIMutable>]
type Board = {
    Row1: Row
    Row2: Row
    Row3: Row }

type RowIndex = | R1 | R2 | R3
type ColumnIndex = | C1 | C2 | C3

type Error = | Error of string