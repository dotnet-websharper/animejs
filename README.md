# WebSharper.AnimeJS

[![NuGet](https://img.shields.io/nuget/v/WebSharper.AnimeJs.svg)](https://www.nuget.org/packages/WebSharper.AnimeJs)

WebSharper bindings for [anime.js](https://animejs.com/) 4.x (4.5.0+) — the
lightweight JavaScript animation engine — for F# and C# web apps.

The binding consumes the npm package [`animejs`](https://www.npmjs.com/package/animejs)
as an ES module (`import * as animejs from "animejs"`) and is bundled by your
toolchain (esbuild in the sample); there is no CDN `<script>` tag.

This repository contains:

| Project | Purpose |
| --- | --- |
| `WebSharper.AnimeJs` | The binding: WIG definitions for `Anime` (the module), `JSAnimation`, `Timeline`, `Timer`, `Animatable`, `Draggable`, `Scope`, `WAAPIAnimation`, `TextSplitter`, `ScrollObserver`, `AutoLayout`, `Spring`, `Tween`, the config objects and the `eases`/`utils`/`svg`/`waapi`/`engine`/`text` namespaces. |
| `WebSharper.AnimeJs.Sample` | A WebSharper SPA (esbuild → `dist/`) that drives the whole surface: core animations, a timeline, `stagger`, SVG draw/morph, a draggable box and the `animejs` helpers. |

## Live demos

**Try it live:** [Open the sample in your browser](https://dotnet-websharper.github.io/animejs/)

![anime.js sample](docs/sample.png)

The sample doubles as the binding's [test case](#running-the-test-case): it
creates animations, a timeline, a staggered grid, draws and morphs an SVG path,
makes a box draggable and exercises the `animejs` helpers (`get`, `set`,
`remove`, `random`, `utils`, `eases`, `cubicBezier`, `spring` and `engine`). It
is light-themed by default and has a light/dark theme switcher (persisted in
`localStorage`).

- **Sample (this repository):** <https://dotnet-websharper.github.io/animejs/>
- **Source repository:** <https://github.com/dotnet-websharper/animejs>
- **WebSharper bindings:** <https://websharper.com/bindings>

## Install

```bash
dotnet add package WebSharper.AnimeJs
```

Or with Paket:

```fsharp
nuget WebSharper.AnimeJs
```

The package targets `netstandard2.0` and works with any WebSharper 10 or 11
project. It supports anime.js `>= 4.5.0 < 5.0.0` (the 4.x line); anime.js itself
is pulled from npm (`animejs`), so the JS bundle step must resolve it
(WebSharper's SPA/esbuild build does this automatically).

## Usage

All types live in the `WebSharper.AnimeJs` namespace. The module functions are
static members of `Anime`; the namespace objects are reachable as
`Anime.Eases`, `Anime.Utils`, `Anime.Svg`, `Anime.Waapi`, `Anime.Engine` and
`Anime.Text`.

### Animate an element

```fsharp
open WebSharper
open WebSharper.JavaScript
open WebSharper.AnimeJs

[<JavaScript>]
let AnimateBox (box: Dom.Element) =
    Anime.Animate(
        box,
        AnimationParams(
            X = 220.,
            Scale = 1.5,
            Opacity = (1., 0.35),
            BackgroundColor = ("#4f46e5", "#f59e0b"),
            Duration = 900,
            Alternate = true,
            Loop = 2,
            Ease = "outQuad",
            OnUpdate = (fun (t: Timer) -> Console.Log t.Progress)
        )
    )
    |> ignore
```

`Anime.Animate(...)` returns a `JSAnimation` with the full `Timer` surface:
`Play()`, `Pause()`, `Resume()`, `Restart()`, `Reverse()`, `Alternate()`,
`Seek(ms)`, `Cancel()`, `Revert()`, `Complete()`, `Stretch(ms)`, and the
`Progress`, `CurrentTime`, `Duration`, `Paused`, `Began`, `Completed`,
`Reversed`, `IterationCount` and `Then(...)` members.

Property keys are the JS names in **camelCase** (`backgroundColor`, `fontSize`,
`zIndex`), and the transform shorthands are `X`, `Y`, `Z`, `Rotate`, `Scale`,
`Skew`, `TranslateX`, … Values can be a number, a string, a `[from, to]` tuple,
a function, or a `StaggerFunction`.

### Timelines

```fsharp
Anime.CreateTimeline(TimelineParams(Duration = 800, PlaybackEase = "inOutQuad"))
    .Add(box, AnimationParams(X = 260., Rotate = 360., Duration = 900), "+=200")
    .Add(box, AnimationParams(Opacity = (1., 0.3), Duration = 400, Loop = 2))
    .Play()
    |> ignore
```

`TimelineParams` takes `PlaybackEase` (the timeline-wide easing); per-step
easing goes in each `AnimationParams`.

### Stagger

```fsharp
Anime.Animate(
    items,
    AnimationParams(
        Scale = (0.35, 1.),
        Delay = Anime.Stagger(60., StaggerParams(Grid = [| 6; 4 |], From = "center")),
        Duration = 700,
        Ease = "outElastic(1, .6)"
    )
) |> ignore
```

### SVG

```fsharp
// Draw a path with the `draw` attribute.
let drawable = Anime.Svg.CreateDrawable("#path")
Anime.Animate(drawable, AnimationParams(Draw = "0 1", Duration = 1200)) |> ignore

// Morph a path to another path element.
Anime.Animate(path, AnimationParams(D = Anime.Svg.MorphTo("#target-path"), Duration = 800))
    |> ignore
```

### Draggable and animatable

```fsharp
Anime.CreateDraggable(box, DraggableParams(Container = "#stage")) |> ignore

let animatable = Anime.CreateAnimatable(box, AnimatableParams(X = 500., Duration = 500))
animatable.X(120., 500.) |> ignore
```

### The `animejs` helpers

`Anime.Get(target, property)`, `Anime.Set(target, AnimationParams(...))`,
`Anime.Remove(target)`, `Anime.Random(min, max)`, `Anime.Sync(...)`,
`Anime.OnScroll(...)`, `Anime.CreateLayout(...)`, `Anime.SplitText(...)`,
`Anime.ScrambleText(...)`, `Anime.CubicBezier(...)`, `Anime.Steps(...)`,
`Anime.Spring(...)`, and the `Anime.Utils` / `Anime.Eases` / `Anime.Engine`
namespaces.

## Notes on the API

- **The package is npm-based.** The binding imports the whole module
  (`import * as animejs from "animejs"`); nothing is fetched from a CDN. Add
  `animejs` to your front-end dependencies (WebSharper registers it as an
  `NpmDependencies` entry).
- **`animate(targets, params)` takes the targets as the first argument** (this
  changed in v4; v3 used a single `targets` field in the params object).
- **Easings are strings, functions or springs.** `Ease` accepts a string
  (`"outQuad"`, `"inOutCubic"`, `"outElastic(1, .6)"`), an `EasingFunction`
  (`Anime.Eases.OutQuad`, `Anime.CubicBezier(...)`), or a `Spring`
  (`Anime.Spring(SpringParams(...))`).
- **Callbacks receive a `Timer`.** `OnBegin`/`OnUpdate`/`OnComplete`/… are typed
  as `Timer -> unit`, so you can read `Progress`/`CurrentTime` directly.
- **`svg.morphTo` takes the target path element or selector**, not a `d` string.
  Set the target path's `d` first, then animate `D = Anime.Svg.MorphTo("#target")`.
- **`createAnimatable` needs the animated properties in its params** (e.g.
  `AnimatableParams(X = 500.)`); the returned object then exposes `X`, `Y`,
  `Rotate`, `Scale` and `Opacity` setters.
- **`createDraggable`'s `Container` is a CSS selector** (`"#stage"`, not
  `"stage"`).
- **Function-typed members are `Func<...>`.** WIG maps `a ^-> b` to
  `System.Func<a, b>`, so values returned by `Anime.CubicBezier(...)` or
  `Anime.Eases.OutQuad` are invoked with `.Invoke(...)`.

## Running the test case

The sample is a static SPA. Building it runs `npm install` and esbuild and emits
`dist/` at the repository root:

```bash
dotnet build WebSharper.AnimeJs.Sample/WebSharper.AnimeJs.Sample.fsproj -c Release
dotnet serve -d dist -p 8080
```

`dotnet serve` comes from the [`dotnet-serve`](https://github.com/dotnet-serve/dotnet-serve)
tool; install it once with `dotnet tool install -g dotnet-serve` (or use
`dotnet tool run dotnet-serve` with a local tool manifest).

Open <http://localhost:8080/>. anime.js is fully bundled, so no network access
is needed. The buttons drive the whole binding — use them to verify a change end
to end.

## Building from source

### Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download) or later.
- [Node.js](https://nodejs.org/) (for the sample's esbuild step).
- [Paket](https://fsprojects.github.io/Paket/) as a local dotnet tool (configured
  in `.config/dotnet-tools.json`).

### Restore and build

```bash
dotnet tool restore
dotnet paket restore
dotnet build WebSharper.AnimeJs.sln -c Release
```

The binding targets `netstandard2.0`; the sample is a static bundle.

### Pack the NuGet package

After a Release build (so the assembly exists in `bin/Release/netstandard2.0`):

```bash
dotnet paket pack ./out
```

## Deployment

Pushing to the default branch triggers `.github/workflows/ghpages.yml`, which
builds the sample in Release and publishes `dist/` to the `gh-pages` branch via
GitHub Pages.

## Repository layout

```
WebSharper.AnimeJs/              # the binding (WIG definitions)
  Main.fs
WebSharper.AnimeJs.Sample/       # static SPA demo / test case
  Client.fs
  index.html
  site.css
  esbuild.config.mjs
  wsconfig.json
docs/
  sample.png                     # screenshot of the running sample
nuget/                           # Paket pack template
.github/workflows/ghpages.yml    # build + deploy the sample
```

## License

Apache 2.0 — see [LICENSE.md](LICENSE.md).
