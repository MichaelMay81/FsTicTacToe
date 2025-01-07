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

type PostSqaureResult = Results<NoContent, InternalServerError<string>>
let postSquare (inMemoeryMbp:MailboxProcessor<InMemoryMbp.Message>) (row:int) (column:int) : PostSqaureResult =
    inMemoeryMbp.PostAndReply (fun replyChannel ->
        InMemoryMbp.SetSquare (R1, C1, replyChannel))
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
