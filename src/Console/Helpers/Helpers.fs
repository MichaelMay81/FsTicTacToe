module FsTicTacToe.Helpers

open System

let tryParseRowIndex (str:string) : RowIndex option =
    str
    |> Int32.TryParse
    |> function
        | true, 1 -> Some R1
        | true, 2 -> Some R2
        | true, 3 -> Some R3
        | _, _ -> None

let tryParseColumnIndex (str:string) : ColumnIndex option =
    str
    |> Int32.TryParse
    |> function
        | true, 1 -> Some C1
        | true, 2 -> Some C2
        | true, 3 -> Some C3
        | _, _ -> None
