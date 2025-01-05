module FsTicTacToe.WebApi.Program

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
open TicTacToe

type PostSqaureResult = Results<NoContent, InternalServerError<string>>
let postSquare (inMemoeryMbp:MailboxProcessor<InMemoryMbp.Message>) (x:int) (y:int) : PostSqaureResult =
    inMemoeryMbp.PostAndReply (fun replyChannel ->
        InMemoryMbp.SetSquare (x, y, replyChannel))
    |> function
    | None ->
        TypedResults.NoContent()
    | Some (InMemoryMbp.Error error) ->
        TypedResults.InternalServerError<string>(error)
    
[<EntryPoint>]
let main args =
    // start game thread
    let inMemoeryMbp = InMemoryMbp.start ()

    let builder = WebApplication.CreateBuilder(args)
    // Add F# type serializabitlity.
    builder.Services.Configure(fun (options:JsonOptions) ->
        JsonFSharpOptions.Default()
                .AddToJsonSerializerOptions(options.SerializerOptions))
    // Add OpenApi.
    builder.Services.AddOpenApi ()

    let app = builder.Build()

    if app.Environment.IsDevelopment() then
        // OpenApi endpoint.
        app.MapOpenApi()
        // Scalar API reference endpoint.
        app.MapScalarApiReference ()
        ()

    app.UseHttpsRedirection()

    app.MapGet("/board", Func<Board>(fun _ ->
            inMemoeryMbp.PostAndReply InMemoryMbp.GetBoard))
        .WithTags("Board")
        .WithName("GetBoard")

    app.MapPost("/", Func<int, int, PostSqaureResult>
            (postSquare inMemoeryMbp))
        .WithTags("Square")
        .WithName("PostSquare")
        
    app.Run()
    0
