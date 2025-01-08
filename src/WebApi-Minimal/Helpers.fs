module FsTicTacToe.WebApi.Helpers

open FsTicTacToe

let parseIndex (row:int) (column:int) : Result<RowIndex*ColumnIndex, Error> =
    row
    |> function | 1 -> Some R1 | 2 -> Some R2 | 3 -> Some R3 | _ -> None
    |> Option.bind (fun rowIndex ->
        column
        |> function | 1 -> Some C1 | 2 -> Some C2 | 3 -> Some C3 | _ -> None
        |> Option.map (fun columnIndex -> rowIndex, columnIndex))
    |> function
    | None -> "Values for row and column must be values of 1-3." |> Error |> Result.Error
    | Some values -> Result.Ok values 