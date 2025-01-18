// Implicitly ignore Warning shows up a lot, when using C# style function chaining.
#nowarn "20"
// Implicit conversion is used by TypedResults.
#nowarn "3391"

open System
open System.Text.Json.Serialization
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Http.Json
open Microsoft.AspNetCore.Http.HttpResults
open Scalar.AspNetCore
open FsTicTacToe
open FsTicTacToe.WebApi

type PostSqaureResult = Results<NoContent, Ok<string>, InternalServerError<string>>

let postSquare (inMemoeryMbp:MailboxProcessor<Games.Message<Guid>>) (id:Guid) (row:int) (column:int) : PostSqaureResult =
    Helpers.parseIndex row column
    |> function
    | Result.Error error -> Result.Error error
    | Result.Ok (rowIndex, columnIndex) ->
        inMemoeryMbp.PostAndReply (fun replyChannel ->
            Games.SetSquare (id, rowIndex, columnIndex, replyChannel))
    |> function
    | Result.Ok None ->
        TypedResults.NoContent()
    | Result.Ok (Some player) ->
        player
        |> function
        | X -> TypedResults.Ok<string> "X"
        | O -> TypedResults.Ok<string> "O"
    | Result.Error (Error error) ->
        TypedResults.InternalServerError<string>(error)
    
[<EntryPoint>]
let main args =
    // start game thread
    let games = Games.start ()

    let builder = WebApplication.CreateBuilder(args)
    // Add F# unions serializabitlity.
    // Most F# types are supported out of the box.
    // This doesn't add the right API schema though...
    builder.Services.Configure(fun (options:JsonOptions) ->
        JsonFSharpOptions(types=JsonFSharpTypes.Unions)
            .AddToJsonSerializerOptions(options.SerializerOptions))
    builder.Services.AddOpenApi()

    let app = builder.Build()

    if app.Environment.IsDevelopment() then
        // OpenApi endpoint.
        app.MapOpenApi()
        // Scalar API reference endpoint.
        app.MapScalarApiReference ()
        ()

    app.UseHttpsRedirection()

    app.MapGet("/boards", Func<Map<Guid,Board>>(fun _ ->
            games.PostAndReply Games.GetBoards))
        .WithTags("Board")
        .WithName("GetBoard")

    app.MapGet("/", Func<HttpContext, int, int, PostSqaureResult>(fun httpContext row column ->
            let coockie = Helpers.getOrSetSessionCoockie "FsTicTacToeSession" (TimeSpan.FromMinutes 5.) httpContext.Request httpContext.Response
            postSquare games coockie row column))
        .WithTags("Square")
        .WithName("PostSquare")

    app.Run()
    0
