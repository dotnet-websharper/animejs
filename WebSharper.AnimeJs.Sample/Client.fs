namespace WebSharper.AnimeJs.Sample

open WebSharper
open WebSharper.JavaScript
open WebSharper.UI
open WebSharper.UI.Client
open WebSharper.UI.Templating
open WebSharper.AnimeJs

[<JavaScript>]
module Client =

    // The template is the whole document, loaded from the DOM, so editing
    // index.html and refreshing the browser is enough.
    type IndexTemplate = Template<"index.html", ClientLoad.FromDocument>

    // ---------------------------------------------------------------
    // State bound into the template
    // ---------------------------------------------------------------

    let status = Var.Create "Ready — press a button to run an animation."
    let details = Var.Create ""
    let version = Var.Create "-"
    let running = Var.Create "-"
    let opacity = Var.Create "-"
    let random = Var.Create "-"
    let suspend = Var.Create "-"
    let notice = Var.Create ""

    // Theme: light is the default; the choice is persisted in localStorage.
    // The storage key is shared with the bootstrap script in index.html.
    let themeStorageKey = "websharper-animejs-theme"
    let theme = Var.Create "light"
    let themeLabel = Var.Create "🌙 Dark mode"

    let prefersReducedMotion =
        JS.Window.MatchMedia("(prefers-reduced-motion: reduce)").Matches

    // The animation driving the "Core animation" box / the timeline, so the
    // transport controls can act on them.
    let mutable core : option<Anime> = None
    let mutable timeline : option<Timeline> = None

    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    let byId (id: string) =
        let el = JS.Document.GetElementById id
        if isNull el then None else Some el

    // ---------------------------------------------------------------
    // Theme switcher
    // ---------------------------------------------------------------

    let applyTheme () =
        JS.Document.DocumentElement.SetAttribute("data-theme", theme.Value)
        themeLabel.Value <-
            if theme.Value = "dark" then "☀️ Light mode" else "🌙 Dark mode"

    let toggleTheme () =
        theme.Value <- (if theme.Value = "dark" then "light" else "dark")
        try JS.Window.LocalStorage.SetItem(themeStorageKey, theme.Value) with _ -> ()
        applyTheme ()

    let withCore (f: Anime -> unit) =
        match core with
        | Some a -> f a
        | None -> status.Value <- "Run a core animation first."

    let withTimeline (f: Timeline -> unit) =
        match timeline with
        | Some t -> f t
        | None -> status.Value <- "Play the timeline first."

    let progress (a: Anime) =
        details.Value <-
            sprintf "progress %.0f%% · %.0f ms · remaining %.0f ms"
                a.Progress a.CurrentTime a.Remaining

    // Create (and play) a new animation on #box, replacing any previous one.
    let runCore (mk: Dom.Element -> AnimeParams) =
        match byId "box" with
        | None -> status.Value <- "The #box element is missing."
        | Some box ->
            Anime.Remove(box)
            let a = Anime(mk box)
            core <- Some a
            a.Play()

    // ---------------------------------------------------------------
    // Core animation
    // ---------------------------------------------------------------

    let moveRight () =
        runCore (fun box ->
            AnimeParams(
                box,
                TranslateX = (0., 220.),
                Scale = (1., 1.5),
                Duration = 900,
                Easing = Easing.Cubic(0.5, 0.05, 0.1, 0.3),
                Begin = (fun _ -> status.Value <- "Translate & scale: begin."),
                Update = progress,
                Complete = (fun _ -> status.Value <- "Translate & scale: complete.")
            ))

    let spin () =
        runCore (fun box ->
            AnimeParams(
                box,
                Rotate = (0., 360.),
                Duration = 900,
                Easing = Easing.EaseInOutBack,
                Complete = (fun _ -> status.Value <- "Rotate: complete.")
            ))

    let fadeColor () =
        runCore (fun box ->
            AnimeParams(
                box,
                Opacity = (1., 0.35),
                Background_color = ("#4f46e5", "#f59e0b"),
                Duration = 900,
                Direction = Direction.Alternate,
                Loop = 2,
                Easing = Easing.EaseInOutQuad,
                Complete = (fun _ -> status.Value <- "Opacity & colour: complete.")
            ))

    let loop () =
        runCore (fun box ->
            AnimeParams(
                box,
                TranslateY = (-150., 150.),
                Rotate = (0., 360.),
                Duration = 1400,
                Direction = Direction.Alternate,
                Loop = true,
                Easing = Easing.Spring(1., 80., 10., 0.),
                Update = progress
            ))

    let play () = withCore (fun a -> a.Play(); status.Value <- "Playing.")
    let pause () = withCore (fun a -> a.Pause(); status.Value <- "Paused.")
    let restart () = withCore (fun a -> a.Restart(); status.Value <- "Restarted from the beginning.")
    let reverse () = withCore (fun a -> a.Reverse(); status.Value <- "Reversed.")

    let seek () =
        withCore (fun a ->
            a.Seek(a.Duration * 0.5)
            status.Value <- "Seeked to 50%."
            details.Value <- sprintf "currentTime %.0f ms" a.CurrentTime)

    // ---------------------------------------------------------------
    // Timeline
    // ---------------------------------------------------------------

    let runTimeline () =
        match byId "box2" with
        | None -> status.Value <- "The #box2 element is missing."
        | Some box2 ->
            Anime.Remove(box2)
            let tl =
                Anime.Timeline(
                    TimelineParams(
                        Targets = box2,
                        Duration = 800,
                        Easing = Easing.EaseInOutQuad
                    ))
                    .Add(
                        TimelineParams(TranslateX = (0., 260.), Rotate = (0., 360.), Duration = 900),
                        "+=200")
                    .Add(
                        TimelineParams(Background_color = ("#0ea5e9", "#22c55e"), Scale = (1., 0.6), Duration = 600))
                    .Add(
                        TimelineParams(Opacity = (1., 0.3), Duration = 400, Direction = Direction.Alternate, Loop = 2))
            timeline <- Some tl
            tl.Play()
            status.Value <- "Timeline playing."

    let pauseTimeline () =
        withTimeline (fun t ->
            t.Pause()
            status.Value <- "Timeline paused.")

    let restartTimeline () =
        withTimeline (fun t ->
            t.Restart()
            status.Value <- "Timeline restarted.")

    // ---------------------------------------------------------------
    // Stagger
    // ---------------------------------------------------------------

    let dots () =
        match byId "dots" with
        | None -> None
        | Some container ->
            let list = container.QuerySelectorAll(".dot")
            Some [| for i in 0 .. list.Length - 1 -> list.Item(i) |]

    let runStagger (targetScale: float * float) (targetOpacity: float * float) (origin: FromValue) =
        match dots () with
        | None -> status.Value <- "The #dots grid is missing."
        | Some items ->
            Anime(
                AnimeParams(
                    items,
                    Scale = targetScale,
                    Opacity = targetOpacity,
                    Delay = Anime.Stagger(60, StaggerOptions(Grid = (6, 4), From = origin, Axis = Axis.X)),
                    Duration = 700,
                    Easing = Easing.Elastic(ElasticEasingType.EaseOutElastic, 1., 0.6)))
            |> ignore
            status.Value <- "Stagger running."

    let staggerIn () = runStagger (0.35, 1.) (0.25, 1.) FromValue.First
    let staggerOut () = runStagger (1., 0.35) (1., 0.25) FromValue.Last

    // ---------------------------------------------------------------
    // SVG path
    // ---------------------------------------------------------------

    let morphShapes = [|
        "M10,60 C10,10 190,10 190,60 C190,110 10,110 10,60 Z"
        "M100,10 C150,10 190,50 190,60 C190,70 150,110 100,110 C50,110 10,70 10,60 C10,50 50,10 100,10 Z"
        "M100,10 C190,10 190,110 100,110 C10,110 10,10 100,10 Z"
    |]

    let mutable morphIndex = 0

    let morph () =
        match byId "morph-path" with
        | None -> status.Value <- "The #morph-path element is missing."
        | Some path ->
            morphIndex <- (morphIndex + 1) % morphShapes.Length
            Anime(
                AnimeParams(
                    path,
                    D = PathValues([| morphShapes.[morphIndex] |]),
                    Duration = 800,
                    Easing = Easing.EaseInOutCirc,
                    Complete = (fun _ -> status.Value <- sprintf "Morphed to shape %d." morphIndex)))
            |> ignore

    let probePath () =
        let point = Anime.Path("#morph-path")
        let p = point.Invoke(PathOptions.X)
        status.Value <-
            sprintf "anime.path('#morph-path')('x') → property '%s', total length %.0f"
                p.Property p.TotalLength

    // ---------------------------------------------------------------
    // Helpers & API tour
    // ---------------------------------------------------------------

    let read () =
        match byId "box" with
        | None -> status.Value <- "The #box element is missing."
        | Some box ->
            running.Value <- string Anime.Running.Length
            opacity.Value <- Anime.Get(box, "opacity")
            status.Value <- "Read anime.get() and anime.running."

    let setHalf () =
        match byId "box" with
        | None -> status.Value <- "The #box element is missing."
        | Some box ->
            Anime.Set(box, TimelineParams(Opacity = 0.5))
            status.Value <- "anime.set(box, { opacity: 0.5 })."

    let removeAll () =
        match byId "box" with
        | None -> status.Value <- "The #box element is missing."
        | Some box ->
            Anime.Remove(box)
            core <- None
            status.Value <- "anime.remove(box) — animations cleared."

    let sampleHelpers () =
        random.Value <- sprintf "%.0f" (Anime.Random(1., 100.))
        suspend.Value <- string Anime.SuspendWhenDocumentHidden

    // ---------------------------------------------------------------
    // Entry point
    // ---------------------------------------------------------------

    [<SPAEntryPoint>]
    let Main () =
        if prefersReducedMotion then
            notice.Value <-
                "Your system requests reduced motion; animations run only when you press a button."

        // Restore a previously chosen theme (light is the default).
        let savedTheme =
            try JS.Window.LocalStorage.GetItem(themeStorageKey) with _ -> null
        if savedTheme = "dark" || savedTheme = "light" then
            theme.Value <- savedTheme
        applyTheme ()

        // `.Bind()` renders the whole document body, so the elements below are
        // in the DOM only after this call.
        IndexTemplate()
            .notice(notice.V)
            .details(details.V)
            .status(status.V)
            .version(version.V)
            .running(running.V)
            .opacity(opacity.V)
            .random(random.V)
            .suspend(suspend.V)
            .themeLabel(themeLabel.V)
            .ToggleTheme(fun _ -> toggleTheme ())
            .MoveRight(fun _ -> moveRight ())
            .Spin(fun _ -> spin ())
            .FadeColor(fun _ -> fadeColor ())
            .Loop(fun _ -> loop ())
            .Play(fun _ -> play ())
            .Pause(fun _ -> pause ())
            .Restart(fun _ -> restart ())
            .Reverse(fun _ -> reverse ())
            .Seek(fun _ -> seek ())
            .RunTimeline(fun _ -> runTimeline ())
            .PauseTimeline(fun _ -> pauseTimeline ())
            .RestartTimeline(fun _ -> restartTimeline ())
            .StaggerIn(fun _ -> staggerIn ())
            .StaggerOut(fun _ -> staggerOut ())
            .Morph(fun _ -> morph ())
            .ProbePath(fun _ -> probePath ())
            .Read(fun _ -> read ())
            .SetHalf(fun _ -> setHalf ())
            .RemoveAll(fun _ -> removeAll ())
            .Bind()

        // The DOM is ready now. Seed the helper readouts.
        version.Value <- Anime.Version
        sampleHelpers ()
        running.Value <- string Anime.Running.Length
