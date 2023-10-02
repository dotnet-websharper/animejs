namespace WebSharper.AnimeJs.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Html
open WebSharper.UI.Templating

open WebSharper.AnimeJs.Sample.Components
 [<JavaScript>]
 module Templates =   
     type MainTemplate = Template<"index.html", ClientLoad.FromDocument>

[<JavaScript>]
module Client =
    open WebSharper.AnimeJs
    open type WebSharper.AnimeJs.Anime


    [<SPAEntryPoint>]
    let Main () =
        let isOpen = Var.Create false
        Doc.Concat [
            h1 [
                attr.id "WSProba"
                attr.style "opacity:0%"
                on.afterRender (fun _ -> 
                    Anime(AnimeParams(
                        JS.Document.GetElementById("box2"),
                        Points = "0,00 0,50 50,50 50,0",
                        Easing = Easing.Cubic(0.5, 0.05, 0.1, 0.3),
                        Loop = true,
                        Duration = 1000,
                        //Delay = 500,
                        Direction = Direction.Alternate
                    )) |> ignore
                )
                on.afterRender (fun el ->
                    Console.Log el.Id

                    let timeline = 
                        Anime.Timeline(TimelineParams(
                            Targets = el, 
                            TranslateX = 350,
                            Duration = 1500
                        )).Add(TimelineParams(
                            Opacity = ("0%","100%"),
                            Easing = Easing.Elastic(ElasticEasingType.EaseOutElastic, 1, 0.6),
                            Duration = 1000
                        ), "+=300")
                    timeline.Play()
                   
                )
            ] [
                text "WebSharper Anime.js Sample"
            ]
            let box1 = JS.Document.GetElementById("box1")
            SVGMenu.Create(300).Doc
            button [ 
                on.click (fun _ _ -> 
                    Anime(AnimeParams(
                        box1, 
                        TranslateX=(0,250),
                        Scale=(0,1.5),
                        Duration=800
                    )) |> ignore
                )
            ] [ text "move box to right" ]
            button [
                on.click (fun _ _ -> 
                    Anime(AnimeParams(
                        box1,
                        TranslateY=(0,50),
                        Rotate=(0,360),
                        Duration=500
                    )) |> ignore
                )
            ] [ text "move box down with rotation" ]
                  
        ]
        |> Doc.RunById "main"