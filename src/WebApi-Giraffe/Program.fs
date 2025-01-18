open System
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.DependencyInjection
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Giraffe
open FsTicTacToe
open FsTicTacToe.WebApi

let postSquare (inMemoeryMbp:MailboxProcessor<Games.Message<Guid>>) (id:Guid) (row:int) (column:int) : HttpHandler =
    Helpers.parseIndex row column
    |> function
    | Result.Error error -> Result.Error error
    | Result.Ok (rowIndex, columnIndex) ->
        inMemoeryMbp.PostAndReply (fun replyChannel ->
            Games.SetSquare (id, rowIndex, columnIndex, replyChannel))
    |> function
    | Result.Ok None ->
        Successful.NO_CONTENT
    | Result.Ok (Some player) ->
        player
        |> function
        | X -> "Winner: X" |> text |> Successful.ok
        | O -> "Winner: O" |> text |> Successful.ok
    | Result.Error (Error error) ->
        error |> text |> ServerErrors.internalError

[<CLIMutable>]
type SquareInput = { Row:int; Column:int }

let parsingError (err : string) =
    RequestErrors.BAD_REQUEST err

let notFoundHandler : HttpHandler =
    setHttpHeader "X-CustomHeader" "Some value"
    >=> RequestErrors.NOT_FOUND "Not Found"

let configureApp (app : IApplicationBuilder) =
    let games: MailboxProcessor<Games.Message<Guid>> =
        Games.start ()

    let webApp =
        choose [
            route "/" >=> (text "FsTicTacToe")
            
            GET >=> route "/Board"
            >=> warbler (fun _ ->
                games.PostAndReply Games.GetBoards
                |> json
                |> Successful.ok)
            
            POST >=> route "/Square"
            >=> tryBindQuery<SquareInput> parsingError None
                (fun input -> postSquare games (Guid "7192ed9b-3fbc-4dfe-88ef-bab9ab6ee061") input.Row input.Column)

            notFoundHandler
        ]

    // Add Giraffe to the ASP.NET Core pipeline
    app.UseGiraffe webApp

let configureServices (services : IServiceCollection) =
    // Add Giraffe dependencies
    services.AddGiraffe() |> ignore
        
    // services.AddSingleton<Json.ISerializer>(
        // SystemTextJson.Serializer(
        //     JsonFSharpOptions.Default()
        //         .ToJsonSerializerOptions())) |> ignore
    // services.Configure(fun (options:JsonOptions) ->
    //     JsonFSharpOptions() //types=JsonFSharpTypes.Unions)
    //         .AddToJsonSerializerOptions(options.SerializerOptions)) |> ignore
    

[<EntryPoint>]
let main _ =
    Host.CreateDefaultBuilder()
        .ConfigureWebHostDefaults(
            fun webHostBuilder ->
                webHostBuilder
                    .Configure(configureApp)
                    .ConfigureServices(configureServices)
                    |> ignore)
        .Build()
        .Run()
    0