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
    let engineInfo = Var.Create "-"
    let random = Var.Create "-"
    let clamp = Var.Create "-"
    let opacity = Var.Create "-"
    let eases = Var.Create "-"
    let cubicBezier = Var.Create "-"
    let spring = Var.Create "-"
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
    let mutable core : option<JSAnimation> = None
    let mutable timeline : option<Timeline> = None
    let mutable animatable : option<Animatable> = None

    // ---------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------

    let byId (id: string) =
        let el = JS.Document.GetElementById id
        if isNull el then None else Some el

    let applyTheme () =
        JS.Document.DocumentElement.SetAttribute("data-theme", theme.Value)
        themeLabel.Value <-
            if theme.Value = "dark" then "☀️ Light mode" else "🌙 Dark mode"

    let toggleTheme () =
        theme.Value <- (if theme.Value = "dark" then "light" else "dark")
        try JS.Window.LocalStorage.SetItem(themeStorageKey, theme.Value) with _ -> ()
        applyTheme ()

    let withCore (f: JSAnimation -> unit) =
        match core with
        | Some a -> f a
        | None -> status.Value <- "Run a core animation first."

    let withTimeline (f: Timeline -> unit) =
        match timeline with
        | Some t -> f t
        | None -> status.Value <- "Play the timeline first."

    let progress (t: Timer) =
        details.Value <-
            sprintf "progress %.0f%% · %.0f ms" t.Progress t.CurrentTime

    // Create (and play) a new animation on #box, replacing any previous one.
    let runCore (mk: Dom.Element -> AnimationParams) =
        match byId "box" with
        | None -> status.Value <- "The #box element is missing."
        | Some box ->
            Anime.Remove(box) |> ignore
            let a = Anime.Animate(box, mk box)
            core <- Some a
            a.Play() |> ignore

    // ---------------------------------------------------------------
    // Core animation
    // ---------------------------------------------------------------

    let moveRight () =
        runCore (fun _ ->
            AnimationParams(
                X = 220.,
                Scale = 1.5,
                Duration = 900,
                Ease = "outQuad",
                OnBegin = (fun _ -> status.Value <- "Translate & scale: begin."),
                OnUpdate = progress,
                OnComplete = (fun _ -> status.Value <- "Translate & scale: complete.")
            ))

    let spin () =
        runCore (fun _ ->
            AnimationParams(
                Rotate = 360.,
                Duration = 900,
                Ease = "inOutBack",
                OnComplete = (fun _ -> status.Value <- "Rotate: complete.")
            ))

    let fadeColor () =
        runCore (fun _ ->
            AnimationParams(
                Opacity = (1., 0.35),
                BackgroundColor = ("#4f46e5", "#f59e0b"),
                Duration = 900,
                Alternate = true,
                Loop = 2,
                Ease = "inOutQuad",
                OnComplete = (fun _ -> status.Value <- "Opacity & colour: complete.")
            ))

    let loop () =
        runCore (fun _ ->
            AnimationParams(
                Y = (-150., 150.),
                Rotate = (0., 360.),
                Duration = 1400,
                Alternate = true,
                Loop = true,
                Ease = Anime.Spring(SpringParams(Mass = 1., Stiffness = 80., Damping = 10.)),
                OnUpdate = progress
            ))

    let play () = withCore (fun a -> a.Play() |> ignore; status.Value <- "Playing.")
    let pause () = withCore (fun a -> a.Pause() |> ignore; status.Value <- "Paused.")
    let restart () = withCore (fun a -> a.Restart() |> ignore; status.Value <- "Restarted from the beginning.")
    let reverse () = withCore (fun a -> a.Reverse() |> ignore; status.Value <- "Reversed.")
    let alternate () = withCore (fun a -> a.Alternate() |> ignore; status.Value <- "Alternating.")
    let cancel () = withCore (fun a -> a.Cancel() |> ignore; status.Value <- "Cancelled.")
    let revert () = withCore (fun a -> a.Revert() |> ignore; status.Value <- "Reverted to the original state.")

    let seek () =
        withCore (fun a ->
            a.Seek(a.Duration * 0.5) |> ignore
            status.Value <- "Seeked to 50%."
            details.Value <- sprintf "currentTime %.0f ms" a.CurrentTime)

    // ---------------------------------------------------------------
    // Timeline
    // ---------------------------------------------------------------

    let runTimeline () =
        match byId "box2" with
        | None -> status.Value <- "The #box2 element is missing."
        | Some box2 ->
            Anime.Remove(box2) |> ignore
            let tl =
                Anime.CreateTimeline(TimelineParams(Duration = 800, PlaybackEase = "inOutQuad"))
                    .Add(box2, AnimationParams(X = 260., Rotate = 360., Duration = 900), "+=200")
                    .Add(box2, AnimationParams(BackgroundColor = ("#0ea5e9", "#22c55e"), Scale = 0.6, Duration = 600))
                    .Add(box2, AnimationParams(Opacity = (1., 0.3), Duration = 400, Alternate = true, Loop = 2))
            timeline <- Some tl
            tl.Play() |> ignore
            status.Value <- "Timeline playing."

    let pauseTimeline () =
        withTimeline (fun t ->
            t.Pause() |> ignore
            status.Value <- "Timeline paused.")

    let restartTimeline () =
        withTimeline (fun t ->
            t.Restart() |> ignore
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

    let runStagger (targetScale: float * float) (targetOpacity: float * float) (origin: string) =
        match dots () with
        | None -> status.Value <- "The #dots grid is missing."
        | Some items ->
            Anime.Animate(
                items,
                AnimationParams(
                    Scale = targetScale,
                    Opacity = targetOpacity,
                    Delay = Anime.Stagger(60., StaggerParams(Grid = ([| 6; 4 |] : int[]), From = origin, Axis = "x")),
                    Duration = 700,
                    Ease = "outElastic(1, .6)"))
            |> ignore
            status.Value <- "Stagger running."

    let staggerIn () = runStagger (0.35, 1.) (0.25, 1.) "first"
    let staggerOut () = runStagger (1., 0.35) (1., 0.25) "last"

    // ---------------------------------------------------------------
    // SVG
    // ---------------------------------------------------------------

    let morphShapes = [|
        "M10,60 C10,10 190,10 190,60 C190,110 10,110 10,60 Z"
        "M100,10 C150,10 190,50 190,60 C190,70 150,110 100,110 C50,110 10,70 10,60 C10,50 50,10 100,10 Z"
        "M100,10 C190,10 190,110 100,110 C10,110 10,10 100,10 Z"
    |]

    let mutable morphIndex = 0

    let draw () =
        match byId "morph-path" with
        | None -> status.Value <- "The #morph-path element is missing."
        | Some path ->
            let drawable = Anime.Svg.CreateDrawable(path)
            Anime.Animate(
                box drawable,
                AnimationParams(
                    Draw = "0 1",
                    Duration = 1200,
                    Ease = "inOutQuad",
                    OnComplete = (fun _ -> status.Value <- "Draw complete.")))
            |> ignore

    let morph () =
        match byId "morph-path", byId "morph-target" with
        | Some path, Some target ->
            morphIndex <- (morphIndex + 1) % morphShapes.Length
            target.SetAttribute("d", morphShapes.[morphIndex])
            Anime.Animate(
                path,
                AnimationParams(
                    D = Anime.Svg.MorphTo("#morph-target"),
                    Duration = 800,
                    Ease = "inOutQuad",
                    OnComplete = (fun _ -> status.Value <- sprintf "Morphed to shape %d." morphIndex)))
            |> ignore
        | _ -> status.Value <- "The #morph-path element is missing."

    // ---------------------------------------------------------------
    // Draggable
    // ---------------------------------------------------------------

    let setupDraggable () =
        match byId "drag-box" with
        | None -> ()
        | Some box ->
            Anime.CreateDraggable(
                box,
                DraggableParams(
                    Container = "#drag-stage",
                    OnGrab = (fun _ -> status.Value <- "Dragging…"),
                    OnRelease = (fun _ -> status.Value <- "Released.")))
            |> ignore

    // ---------------------------------------------------------------
    // Helpers & API tour
    // ---------------------------------------------------------------

    let read () =
        match byId "box" with
        | None -> status.Value <- "The #box element is missing."
        | Some box ->
            opacity.Value <- Anime.Get(box, "opacity")
            engineInfo.Value <-
                sprintf "%s · precision %.2f" Anime.Engine.TimeUnit Anime.Engine.Precision
            status.Value <- "Read anime.get() and engine."

    let setHalf () =
        match byId "box" with
        | None -> status.Value <- "The #box element is missing."
        | Some box ->
            Anime.Set(box, AnimationParams(Opacity = 0.5)) |> ignore
            status.Value <- "anime.set(box, { opacity: 0.5 })."

    let removeAll () =
        match byId "box" with
        | None -> status.Value <- "The #box element is missing."
        | Some box ->
            Anime.Remove(box) |> ignore
            core <- None
            status.Value <- "anime.remove(box) — animations cleared."

    let animatableX () =
        match animatable with
        | Some a ->
            a.X(120., 500.) |> ignore
            status.Value <- "animatable.x(120, 500)."
        | None -> status.Value <- "The animatable is not ready."

    let sampleHelpers () =
        random.Value <- sprintf "%.0f" (Anime.Random(1., 100.))
        clamp.Value <- sprintf "%.2f" (Anime.Utils.Clamp(1.5, 0., 1.))
        let outQuad = Anime.Eases.OutQuad
        eases.Value <- sprintf "%.3f" (outQuad.Invoke(0.5))
        let curve = Anime.CubicBezier(0.5, 0.05, 0.1, 0.3)
        cubicBezier.Value <- sprintf "%.3f" (curve.Invoke(0.5))
        let sp = Anime.Spring(SpringParams(Mass = 1., Stiffness = 100., Damping = 10.))
        spring.Value <- sprintf "%.3f" (sp.Solve(0.5))
        engineInfo.Value <-
            sprintf "%s · precision %.2f" Anime.Engine.TimeUnit Anime.Engine.Precision

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
            .engine(engineInfo.V)
            .random(random.V)
            .clamp(clamp.V)
            .opacity(opacity.V)
            .eases(eases.V)
            .cubicBezier(cubicBezier.V)
            .spring(spring.V)
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
            .Alternate(fun _ -> alternate ())
            .Seek(fun _ -> seek ())
            .Cancel(fun _ -> cancel ())
            .Revert(fun _ -> revert ())
            .RunTimeline(fun _ -> runTimeline ())
            .PauseTimeline(fun _ -> pauseTimeline ())
            .RestartTimeline(fun _ -> restartTimeline ())
            .StaggerIn(fun _ -> staggerIn ())
            .StaggerOut(fun _ -> staggerOut ())
            .Draw(fun _ -> draw ())
            .Morph(fun _ -> morph ())
            .Read(fun _ -> read ())
            .SetHalf(fun _ -> setHalf ())
            .RemoveAll(fun _ -> removeAll ())
            .AnimatableX(fun _ -> animatableX ())
            .Bind()

        // The DOM is ready now. Create the animatable and the draggable.
        match byId "box" with
        | Some box ->
            animatable <-
                Some (Anime.CreateAnimatable(box, AnimatableParams(X = 500., Duration = 500, Ease = "outQuad")))
        | None -> ()
        setupDraggable ()
        sampleHelpers ()
