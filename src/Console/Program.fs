open TicTacToe

printfn """
TicTacToe

Insert the coordinates of the square you want to play,
separated by a space. Ctrl-C to exit.

GameBoard:
(0 0) (0 1) (0 2)
(1 0) (1 1) (1 2)
(2 0) (2 1) (2 2)"""

// Boards.empty
// |> GameLoop.gameLoop ignore
Boards.fromFile "board.json"
|> GameLoop.gameLoop (fun board -> Boards.toFile "board.json" board)