namespace WebSharper.AnimeJs.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html
open WebSharper.UI.Templating
// [<JavaScript>]
// module Templates =   
//     type MainTemplate = Template<"index.html", ClientLoad.FromDocument>

[<JavaScript>]
module Client =
    open WebSharper.AnimeJs
    [<SPAEntryPoint>]
    let Main () =
         
        Doc.Concat [
            h1 [
                attr.id "WSProba"
                attr.style "opacity:0%"
                on.afterRender (fun el ->
                    Console.Log el.Id
                    Anime.Anime(AnimeParams(
                        el,
                        Opacity = ("0%","100%"),
                        Easing = Easing.Linear,
                        Duration = 1000,
                        Delay = 300
                    )) |> ignore
                    Anime.Anime(AnimeParams(
                        el,
                        TranslateX = 350,
                        Duration = 1500
                    )) |> ignore
                )
            ] [
                text "WebSharper Anime.js Sample"
            ]
            let box1 = JS.Document.GetElementById("box1")
            button [ 
                on.click (fun _ _ -> 
                    Anime.Anime(AnimeParams(
                        box1, 
                        TranslateX=(0,250),
                        Scale=(0,1.5),
                        Duration=800
                    )) |> ignore
                )
            ] [ text "move box to right" ]
            button [
                on.click (fun _ _ -> 
                    Anime.Anime(AnimeParams(
                        box1,
                        TranslateY=(0,50),
                        Rotate=(0,360),
                        Duration=500
                    )) |> ignore
                )
            ] [ text "move box down with rotation" ]
                  
        ]
        |> Doc.RunById "main"