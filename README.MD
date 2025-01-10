# # FsTicTacToe # #

This is a simple game of Tic Tac Toe to try out different patterns in FSharp.

## Library

This library defines the basic game and a MailboxProcessor for a thread-safe multi-game setup.

## Console

Simple console application to play Tic Tac Toe.

Supports two modes:
- In memory game.
- Persistant game state loaded/safed from file.

## Web Api - Minimal

Web Api that serves a game of Tic Tac Toe per session. Sessions are differentiated via Coockies. Supports serialization of all types and OpenApi Schema (except for discriminated unions).