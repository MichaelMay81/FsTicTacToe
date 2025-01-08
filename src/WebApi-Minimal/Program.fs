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

let postSquare (inMemoeryMbp:MailboxProcessor<InMemoryMbp.Message<string>>) (id:string) (row:int) (column:int) : PostSqaureResult =
    Helpers.parseIndex row column
    |> function
    | Result.Error error -> Some error
    | Result.Ok (rowIndex, columnIndex) ->
        inMemoeryMbp.PostAndReply (fun replyChannel ->
            InMemoryMbp.SetSquare (id, rowIndex, columnIndex, replyChannel))
    |> function
    | None ->
        TypedResults.NoContent()
    | Some (Error error) ->
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

    app.MapGet("/boards", Func<Map<string,Board>>(fun _ ->
            inMemoeryMbp.PostAndReply InMemoryMbp.GetBoards))
        .WithTags("Board")
        .WithName("GetBoard")

    app.MapPost("/", Func<int, int, PostSqaureResult>
            (postSquare inMemoeryMbp "96adcda5-8aa7-43db-a251-4b1aeec1b5c5"))
        .WithTags("Square")
        .WithName("PostSquare")
        
    app.Run()
    0
