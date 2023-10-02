namespace WebSharper.AnimeJs.Sample.Components

open WebSharper
[<JavaScript>]
module SVGMenu =
    open WebSharper.UI
    open WebSharper.JavaScript
    open WebSharper.AnimeJs
    
    type private MenuTemplate = Templating.Template<"svgmenu.html">
    
    type Paths = {
            Open:string
            Close:string
            Reset:Dom.Attr
        }
        let Create(height:int) =
            let data = Var.Create {
                Open = ""
                Close = ""
                Reset = null
            }
            let menu = 
                MenuTemplate.SVGMenu()
                    .Initialize(fun (el:Dom.Element) -> 
                        let shapeEl = el.QuerySelector(".morph-shape")
                        let pathEl = shapeEl.QuerySelector("svg path")
                        data.SetFinal {
                            Open = shapeEl.GetAttribute "data-morph-open"
                            Close = pathEl.GetAttribute "d"
                            Reset = pathEl.Attributes.GetNamedItem "d"
                        })
            
            let isOpen = Var.Create false

            let duration = 600
            menu
                .ToggleMenu(fun evt ->
                    let el = evt.Target.ParentElement
                    let menuInnerEl = el.QuerySelector(".menu_inner")
                    let shapeEl = el.QuerySelector(".morph-shape")
                    let pathEl = shapeEl.QuerySelector("svg path")

                    let Open = not isOpen.Value
                    isOpen.Set Open

                    let children = menuInnerEl.QuerySelectorAll("a")
                    let len = children.Length
                    for i = 0 to len-1 do
                        let currChild = (if Open then children[i] else children[len-i-1])
                        let currDelay = (duration / len) * i

                        Anime(AnimeParams(
                            currChild,
                            Duration = 300,
                            Delay = currDelay,
                            TranslateZ = (if Open then 0 else -20),
                            Opacity = (if Open then 1. else 0.),
                            Easing = Easing.EaseOutCirc
                        )) |> ignore

                    Anime(AnimeParams(
                        pathEl,
                        D = PathValues([| data.Value.Open |]),
                        Duration = duration/2,
                        Complete = (fun anim ->
                            (anim |> As<Anime>).Pause()
                            Anime(AnimeParams(
                                pathEl,
                                D = PathValues( [| data.Value.Close |] ),
                                Duration = duration/2,
                                //Delay = duration-300,
                                Easing = Easing.EaseInOutCirc
                            )) |> ignore)
                    )) |> ignore
                    Anime(AnimeParams(
                        el,
                        Duration = duration,
                        Height = (if Open then height else 70),
                        Easing = Easing.Cubic(0.7,0,0.3,1)
                    )) |> ignore
                    Anime(AnimeParams(
                        menuInnerEl,
                        Duration = duration,
                        Height = (if Open then height-70 else 0),
                        Easing = Easing.Cubic(0.7,0,0.3,1)
                    )) |> ignore
                    Console.Log $"is open? {Open}"
                )
                .Create()
