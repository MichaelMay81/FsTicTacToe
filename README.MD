# # FsTicTacToe # #

This is a simple game of Tic Tac Toe to try out different patterns in FSharp.

## Library

This library defines the basic game.

## Console

Simple console application to play Tic Tac Toe.

Supports two modes:
- In memory game.
- Persistant game state loaded/safed from file.

## Web Api - Minimal

Web Api that servers a single game of Tic Tac Toe in a seperate thread via a MailboxProcessor. Supports serialization of all types and OpenApi Schmea (except for discriminated unions).