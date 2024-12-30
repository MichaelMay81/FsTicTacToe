namespace TicTacToe

type Player = | X | O
type Square = Player option
type Board = Square array array