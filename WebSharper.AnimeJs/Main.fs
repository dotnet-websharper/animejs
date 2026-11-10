namespace WebSharper.AnimeJs.Extension

open WebSharper
open WebSharper.InterfaceGenerator


module Definition =

    // ------------------------------------------------------------------
    // Shared types
    // ------------------------------------------------------------------

    /// `(time: number) => number`
    let EasingFunction = T<float> ^-> T<float>
    /// `(power?: number) => EasingFunction`
    let PowerEasing = T<float> ^-> EasingFunction
    /// `(overshoot?: number) => EasingFunction`
    let BackEasing = T<float> ^-> EasingFunction
    /// `(amplitude?: number, period?: number) => EasingFunction`
    let ElasticEasing = T<float>?amplitude * T<float>?period ^-> EasingFunction

    /// `(target?, index?, targets?) => number`
    let StaggerFunction =
        T<obj>?target * (!?T<int>)?index * (!?T<obj>)?targets ^-> T<float>
    /// `(target?, index?, targets?) => value`
    let FunctionValue =
        T<obj>?target * (!?T<int>)?index * (!?T<obj>)?targets ^-> T<obj>

    let Spring = Class "Spring"

    /// `string | EasingFunction | Spring`
    let EaseParam = T<string> + EasingFunction + Spring

    /// A tween value: number, string, `[from, to]`, a function, or anything else.
    let TweenValue =
        T<float> + T<string> + (T<float> * T<float>) + (T<string> * T<string>)
        + StaggerFunction + FunctionValue + T<obj>

    /// `string | Node | NodeList | Node[] | object`
    let TargetsParam =
        T<string> + T<JavaScript.Dom.Node> + T<JavaScript.Dom.NodeList>
        + (!|T<JavaScript.Dom.Node>) + T<obj>

    // ------------------------------------------------------------------
    // Classes (defined early so configs can reference them)
    // ------------------------------------------------------------------

    let Timer = Class "Timer"
    let JSAnimation = Class "JSAnimation" |=> Inherits Timer
    let Timeline = Class "Timeline" |=> Inherits Timer
    let Animatable = Class "Animatable"
    let Draggable = Class "Draggable"
    let Scope = Class "Scope"
    let WAAPIAnimation = Class "WAAPIAnimation"
    let TextSplitter = Class "TextSplitter"
    let ScrollObserver = Class "ScrollObserver"
    let AutoLayout = Class "AutoLayout"
    let Tween = Class "Tween"

    let DrawableSVGGeometry =
        Class "DrawableSVGGeometry"
        |+> Instance [
            "draw" =? T<string>
        ]

    // ------------------------------------------------------------------
    // Config objects
    // ------------------------------------------------------------------

    let timerOptions = [
        "id", T<string> + T<int>
        "duration", TweenValue
        "delay", TweenValue
        "loopDelay", T<float>
        "reversed", T<bool>
        "alternate", T<bool>
        "loop", T<bool> + T<int>
        "autoplay", T<bool>
        "frameRate", T<float>
        "playbackRate", T<float>
        "priority", T<float>
    ]

    let callbacks = [
        "onBegin", Timer?self ^-> T<unit>
        "onBeforeUpdate", Timer?self ^-> T<unit>
        "onUpdate", Timer?self ^-> T<unit>
        "onLoop", Timer?self ^-> T<unit>
        "onPause", Timer?self ^-> T<unit>
        "onComplete", Timer?self ^-> T<unit>
        "onRender", Timer?self ^-> T<unit>
    ]

    let transformProps = [
        for n in [
            "x"; "y"; "z"
            "rotate"; "rotateX"; "rotateY"; "rotateZ"
            "scale"; "scaleX"; "scaleY"; "scaleZ"
            "skew"; "skewX"; "skewY"
            "translateX"; "translateY"; "translateZ"
            "perspective"; "matrix"; "matrix3d"
        ] do
            n, TweenValue
    ]

    let cssProps = [
        for n in [
            "accentColor"; "alignContent"; "alignItems"; "alignSelf"; "all"
            "animation"; "animationDelay"; "animationDirection"; "animationDuration"
            "animationFillMode"; "animationIterationCount"; "animationName"
            "animationPlayState"; "animationTimingFunction"; "aspectRatio"
            "backdropFilter"; "backfaceVisibility"; "background"
            "backgroundAttachment"; "backgroundBlendMode"; "backgroundClip"
            "backgroundColor"; "backgroundImage"; "backgroundOrigin"
            "backgroundPosition"; "backgroundRepeat"; "backgroundSize"
            "blockSize"; "border"; "borderBlock"; "borderBottom"
            "borderBottomColor"; "borderBottomLeftRadius"; "borderBottomRightRadius"
            "borderBottomStyle"; "borderBottomWidth"; "borderCollapse"
            "borderColor"; "borderImage"; "borderInline"; "borderLeft"
            "borderLeftColor"; "borderLeftStyle"; "borderLeftWidth"; "borderRadius"
            "borderRight"; "borderRightColor"; "borderRightStyle"; "borderRightWidth"
            "borderSpacing"; "borderStyle"; "borderTop"; "borderTopColor"
            "borderTopLeftRadius"; "borderTopRightRadius"; "borderTopStyle"
            "borderTopWidth"; "borderWidth"; "bottom"; "boxShadow"; "boxSizing"
            "caretColor"; "clear"; "clip"; "clipPath"; "color"; "columnCount"
            "columnGap"; "columnRule"; "columnSpan"; "columnWidth"; "columns"
            "content"; "cursor"; "display"; "filter"; "flex"; "flexBasis"
            "flexDirection"; "flexFlow"; "flexGrow"; "flexShrink"; "flexWrap"
            "font"; "fontFamily"; "fontFeatureSettings"; "fontKerning"; "fontSize"
            "fontSizeAdjust"; "fontStretch"; "fontStyle"; "fontVariant"
            "fontVariantCaps"; "fontWeight"; "gap"; "grid"; "gridArea"
            "gridAutoColumns"; "gridAutoFlow"; "gridAutoRows"; "gridColumn"
            "gridColumnEnd"; "gridColumnGap"; "gridColumnStart"; "gridGap"
            "gridRow"; "gridRowEnd"; "gridRowGap"; "gridRowStart"; "gridTemplate"
            "gridTemplateAreas"; "gridTemplateColumns"; "gridTemplateRows"
            "height"; "inset"; "insetBlock"; "insetInline"; "justifyContent"
            "justifyItems"; "justifySelf"; "left"; "letterSpacing"; "lineHeight"
            "listStyle"; "listStyleImage"; "listStylePosition"; "listStyleType"
            "margin"; "marginBlock"; "marginBottom"; "marginInline"; "marginLeft"
            "marginRight"; "marginTop"; "maxHeight"; "maxWidth"; "minHeight"
            "minWidth"; "objectFit"; "objectPosition"; "opacity"; "order"
            "outline"; "outlineColor"; "outlineOffset"; "outlineStyle"
            "outlineWidth"; "overflow"; "overflowWrap"; "overflowX"; "overflowY"
            "padding"; "paddingBlock"; "paddingBottom"; "paddingInline"
            "paddingLeft"; "paddingRight"; "paddingTop"; "perspectiveOrigin"
            "placeContent"; "placeItems"; "placeSelf"; "pointerEvents"; "position"
            "resize"; "right"; "rowGap"; "scrollBehavior"; "scrollMargin"
            "scrollPadding"; "scrollSnapAlign"; "scrollSnapType"; "scrollbarColor"
            "tabSize"; "tableLayout"; "textAlign"; "textAlignLast"
            "textDecoration"; "textDecorationColor"; "textDecorationLine"
            "textDecorationStyle"; "textDecorationThickness"; "textIndent"
            "textJustify"; "textOrientation"; "textOverflow"; "textShadow"
            "textTransform"; "top"; "transform"; "transformOrigin"; "transition"
            "transitionDelay"; "transitionDuration"; "transitionProperty"
            "transitionTimingFunction"; "userSelect"; "verticalAlign"
            "visibility"; "whiteSpace"; "width"; "wordBreak"; "wordSpacing"
            "wordWrap"; "zIndex"
        ] do
            n, TweenValue
    ]

    let DefaultsParams =
        Pattern.Config "DefaultsParams" {
            Required = []
            Optional = [
                yield! timerOptions
                yield! callbacks
                "keyframes", T<obj>
                "playbackEase", EaseParam
                "persist", T<bool>
                "ease", EaseParam
                "composition", T<string>
                "modifier", T<obj>
            ]
        }

    let AnimationParams =
        Pattern.Config "AnimationParams" {
            Required = []
            Optional = [
                yield! timerOptions
                yield! callbacks
                yield! transformProps
                yield! cssProps
                "ease", EaseParam
                "playbackEase", EaseParam
                "composition", T<string>
                "modifier", T<obj>
                "keyframes", T<obj>
                "persist", T<bool>
                "draw", TweenValue
                "d", TweenValue
            ]
        }

    let TimelineParams =
        Pattern.Config "TimelineParams" {
            Required = []
            Optional = [
                yield! timerOptions
                yield! callbacks
                "defaults", DefaultsParams.Type
                "playbackEase", EaseParam
                "composition", T<bool>
            ]
        }

    let TimerParams =
        Pattern.Config "TimerParams" {
            Required = []
            Optional = [
                yield! timerOptions
                yield! callbacks
            ]
        }

    let AnimatableParams =
        Pattern.Config "AnimatableParams" {
            Required = []
            Optional = [
                yield! callbacks
                yield! transformProps
                yield! cssProps
                "unit", T<string>
                "duration", TweenValue
                "ease", EaseParam
                "modifier", T<obj>
                "composition", T<string>
            ]
        }

    let StaggerParams =
        Pattern.Config "StaggerParams" {
            Required = []
            Optional = [
                "start", T<float> + T<string>
                "from", T<int> + T<string> + (!|T<int>)
                "reversed", T<bool>
                "grid", (!|T<int>) + T<bool>
                "axis", T<string>
                "use", T<string> + T<obj>
                "total", T<float>
                "ease", EaseParam
                "modifier", T<obj>
                "jitter", T<float> + (!|T<float>)
                "seed", T<bool> + T<int>
            ]
        }

    let DraggableParams =
        Pattern.Config "DraggableParams" {
            Required = []
            Optional = [
                "trigger", TargetsParam
                "container", TargetsParam + (!|T<float>)
                "x", T<bool> + T<obj>
                "y", T<bool> + T<obj>
                "modifier", T<obj>
                "snap", T<float> + (!|T<float>) + T<obj>
                "containerPadding", T<float> + (!|T<float>)
                "containerFriction", T<float>
                "releaseContainerFriction", T<float>
                "dragSpeed", T<float>
                "dragThreshold", T<float> + T<obj>
                "scrollSpeed", T<float>
                "scrollThreshold", T<float>
                "minVelocity", T<float>
                "maxVelocity", T<float>
                "velocityMultiplier", T<float>
                "releaseMass", T<float>
                "releaseStiffness", T<float>
                "releaseDamping", T<float>
                "releaseEase", EaseParam
                "cursor", T<bool> + T<obj>
                "onGrab", Draggable?self ^-> T<unit>
                "onDrag", Draggable?self ^-> T<unit>
                "onRelease", Draggable?self ^-> T<unit>
                "onUpdate", Draggable?self ^-> T<unit>
                "onSettle", Draggable?self ^-> T<unit>
                "onSnap", Draggable?self ^-> T<unit>
                "onResize", Draggable?self ^-> T<unit>
                "onAfterResize", Draggable?self ^-> T<unit>
            ]
        }

    let ScopeParams =
        Pattern.Config "ScopeParams" {
            Required = []
            Optional = [
                "root", TargetsParam
                "defaults", DefaultsParams.Type
                "mediaQueries", T<obj>
            ]
        }

    let SpringParams =
        Pattern.Config "SpringParams" {
            Required = []
            Optional = [
                "mass", T<float>
                "stiffness", T<float>
                "damping", T<float>
                "velocity", T<float>
                "bounce", T<float>
                "duration", T<float>
                "onComplete", JSAnimation?self ^-> T<unit>
            ]
        }

    let TextSplitterParams =
        Pattern.Config "TextSplitterParams" {
            Required = []
            Optional = [
                "lines", T<bool> + T<string> + T<obj>
                "words", T<bool> + T<string> + T<obj>
                "chars", T<bool> + T<string> + T<obj>
                "accessible", T<bool>
                "includeSpaces", T<bool>
                "debug", T<bool>
            ]
        }

    let ScrambleTextParams =
        Pattern.Config "ScrambleTextParams" {
            Required = []
            Optional = [
                "text", T<string> + T<obj>
                "chars", T<string> + T<obj>
                "ease", EaseParam
                "from", T<int> + T<string>
                "reversed", T<bool>
                "cursor", T<bool> + T<int> + T<string>
                "perturbation", T<float>
                "seed", T<int>
                "override", T<bool> + T<string>
                "revealRate", T<float>
                "settleDuration", T<float>
                "settleRate", T<float>
                "duration", T<float> + T<obj>
                "revealDelay", T<float> + T<obj>
                "delay", T<float> + T<obj>
                "onChange", (T<string> * T<float>) ^-> T<unit>
            ]
        }

    let ScrollObserverParams =
        Pattern.Config "ScrollObserverParams" {
            Required = []
            Optional = [
                "id", T<string> + T<int>
                "sync", T<bool> + T<float> + T<string>
                "container", TargetsParam
                "target", TargetsParam
                "axis", T<string> + T<obj>
                "enter", T<string> + T<float> + T<obj>
                "leave", T<string> + T<float> + T<obj>
                "repeat", T<bool> + T<obj>
                "debug", T<bool>
                "onEnter", ScrollObserver?self ^-> T<unit>
                "onLeave", ScrollObserver?self ^-> T<unit>
                "onEnterForward", ScrollObserver?self ^-> T<unit>
                "onLeaveForward", ScrollObserver?self ^-> T<unit>
                "onEnterBackward", ScrollObserver?self ^-> T<unit>
                "onLeaveBackward", ScrollObserver?self ^-> T<unit>
                "onUpdate", ScrollObserver?self ^-> T<unit>
                "onResize", ScrollObserver?self ^-> T<unit>
                "onSyncComplete", ScrollObserver?self ^-> T<unit>
            ]
        }

    let MotionPath =
        Pattern.Config "MotionPath" {
            Required = []
            Optional = [
                "translateX", FunctionValue
                "translateY", FunctionValue
                "rotate", FunctionValue
            ]
        }

    // ------------------------------------------------------------------
    // Class members
    // ------------------------------------------------------------------

    Timer
    |+> Instance [
        "id" =? T<string> + T<int>
        "parent" =? Timeline
        "duration" =? T<float>
        "backwards" =? T<bool>
        "paused" =? T<bool>
        "began" =? T<bool>
        "completed" =? T<bool>
        "cancelled" =? T<bool>
        "iterationDuration" =? T<float>
        "iterationCount" =? T<float>
        "currentTime" =? T<float>
        "iterationCurrentTime" =? T<float>
        "progress" =? T<float>
        "iterationProgress" =? T<float>
        "currentIteration" =? T<float>
        "reversed" =? T<bool>
        "fps" =? T<float>
        "speed" =? T<float>
        "onBegin" =? (Timer?self ^-> T<unit>)
        "onBeforeUpdate" =? (Timer?self ^-> T<unit>)
        "onUpdate" =? (Timer?self ^-> T<unit>)
        "onLoop" =? (Timer?self ^-> T<unit>)
        "onPause" =? (Timer?self ^-> T<unit>)
        "onComplete" =? (Timer?self ^-> T<unit>)
        "reset" => (!?T<bool>)?softReset ^-> TSelf
        "init" => (!?T<bool>)?internalRender ^-> TSelf
        "resetTime" => T<unit> ^-> TSelf
        "pause" => T<unit> ^-> TSelf
        "resume" => T<unit> ^-> TSelf
        "restart" => T<unit> ^-> TSelf
        "seek" => T<float>?time * (!?T<bool>)?muteCallbacks ^-> TSelf
        "alternate" => T<unit> ^-> TSelf
        "play" => T<unit> ^-> TSelf
        "reverse" => T<unit> ^-> TSelf
        "cancel" => T<unit> ^-> TSelf
        "stretch" => T<float>?newDuration ^-> TSelf
        "revert" => T<unit> ^-> TSelf
        "complete" => (!?T<bool>)?muteCallbacks ^-> TSelf
        "then" => (!?(T<obj> ^-> T<unit>))?callback ^-> T<JavaScript.Promise<obj>>
    ]
    |> ignore

    JSAnimation
    |+> Instance [
        "targets" =? !|T<obj>
        "onRender" =? (JSAnimation?self ^-> T<unit>)
        "refresh" => T<unit> ^-> TSelf
    ]
    |> ignore

    Timeline
    |+> Instance [
        "labels" =? T<obj>
        "defaults" =? DefaultsParams
        "composition" =? T<bool>
        "onRender" =? (Timeline?self ^-> T<unit>)
        "add" =>
            TargetsParam?targets * AnimationParams?parameters
            * (!?(T<float> + T<string> + StaggerFunction))?position ^-> TSelf
        "sync" =>
            (!?T<obj>)?synced * (!?(T<float> + T<string>))?position ^-> TSelf
        "set" =>
            TargetsParam?targets * AnimationParams?parameters
            * (!?(T<float> + T<string>))?position ^-> TSelf
        "call" =>
            (Timer?self ^-> T<unit>)?callback
            * (!?(T<float> + T<string>))?position ^-> TSelf
        "label" =>
            T<string>?labelName * (!?(T<float> + T<string>))?position ^-> TSelf
        "remove" => TargetsParam?targets * (!?T<string>)?propertyName ^-> TSelf
        "refresh" => T<unit> ^-> TSelf
    ]
    |> ignore

    Animatable
    |+> Instance [
        "targets" =? !|T<obj>
        "animations" =? T<obj>
        "callbacks" =? JSAnimation
        "revert" => T<unit> ^-> TSelf
        "x" => T<float>?value * (!?T<float>)?duration * (!?EaseParam)?ease ^-> TSelf
        "y" => T<float>?value * (!?T<float>)?duration * (!?EaseParam)?ease ^-> TSelf
        "rotate" => T<float>?value * (!?T<float>)?duration * (!?EaseParam)?ease ^-> TSelf
        "scale" => T<float>?value * (!?T<float>)?duration * (!?EaseParam)?ease ^-> TSelf
        "opacity" => T<float>?value * (!?T<float>)?duration * (!?EaseParam)?ease ^-> TSelf
    ]
    |> ignore

    Draggable
    |+> Instance [
        "x" =? T<float>
        "y" =? T<float>
        "progressX" =? T<float>
        "progressY" =? T<float>
        "velocity" =? T<float>
        "angle" =? T<float>
        "grabbed" =? T<bool>
        "dragged" =? T<bool>
        "released" =? T<bool>
        "enabled" =? T<bool>
        "setX" => T<float>?x * (!?T<bool>)?muteUpdateCallback ^-> TSelf
        "setY" => T<float>?y * (!?T<bool>)?muteUpdateCallback ^-> TSelf
        "stop" => T<unit> ^-> TSelf
        "reset" => T<unit> ^-> TSelf
        "enable" => T<unit> ^-> TSelf
        "disable" => T<unit> ^-> TSelf
        "revert" => T<unit> ^-> TSelf
        "refresh" => T<unit> ^-> T<unit>
        "update" => T<unit> ^-> T<unit>
        "scrollInView" =>
            (!?T<float>)?duration * (!?T<float>)?gap * (!?EaseParam)?ease ^-> TSelf
        "animateInView" =>
            (!?T<float>)?duration * (!?T<float>)?gap * (!?EaseParam)?ease ^-> TSelf
    ]
    |> ignore

    Scope
    |+> Instance [
        "defaults" =? DefaultsParams
        "root" =? T<obj>
        "register" => T<obj>?revertible ^-> T<unit>
        "execute" => (T<obj> ^-> T<obj>)?cb ^-> T<obj>
        "refresh" => T<unit> ^-> TSelf
        "add" => T<string>?name * (T<obj> ^-> T<obj>)?method ^-> TSelf
        "addOnce" => (T<obj> ^-> T<obj>)?cb ^-> TSelf
        "keepTime" => (T<obj> ^-> T<obj>)?cb ^-> T<obj>
        "revert" => T<unit> ^-> T<unit>
    ]
    |> ignore

    WAAPIAnimation
    |+> Instance [
        "targets" =? !|T<obj>
        "animations" =? !|T<obj>
        "duration" =? T<float>
        "completed" =? T<bool>
        "paused" =? T<bool>
        "reversed" =? T<bool>
        "speed" =? T<float>
        "currentTime" =? T<float>
        "progress" =? T<float>
        "onComplete" =? (WAAPIAnimation?self ^-> T<unit>)
        "forEach" => (T<obj> ^-> T<unit>)?callback ^-> TSelf
        "resume" => T<unit> ^-> TSelf
        "pause" => T<unit> ^-> TSelf
        "alternate" => T<unit> ^-> TSelf
        "play" => T<unit> ^-> TSelf
        "reverse" => T<unit> ^-> TSelf
        "seek" => T<float>?time * (!?T<bool>)?muteCallbacks ^-> TSelf
        "restart" => T<unit> ^-> TSelf
        "commitStyles" => T<unit> ^-> TSelf
        "complete" => T<unit> ^-> TSelf
        "cancel" => T<unit> ^-> TSelf
        "revert" => T<unit> ^-> TSelf
    ]
    |> ignore

    TextSplitter
    |+> Instance [
        "debug" =? T<bool>
        "includeSpaces" =? T<bool>
        "accessible" =? T<bool>
        "linesOnly" =? T<bool>
        "html" =? T<string>
        "lines" =? !|T<obj>
        "words" =? !|T<obj>
        "chars" =? !|T<obj>
        "addEffect" => (T<obj> ^-> T<obj>)?effect ^-> TSelf
        "revert" => T<unit> ^-> TSelf
        "split" => (!?T<bool>)?clearCache ^-> TSelf
        "refresh" => T<unit> ^-> T<unit>
    ]
    |> ignore

    ScrollObserver
    |+> Instance [
        "id" =? T<string> + T<int>
        "target" =? T<JavaScript.Dom.Node>
        "repeat" =? T<bool>
        "horizontal" =? T<bool>
        "velocity" =? T<float>
        "backward" =? T<bool>
        "scroll" =? T<float>
        "progress" =? T<float>
        "isInView" =? T<bool>
        "link" => T<obj>?linked ^-> TSelf
        "refresh" => T<unit> ^-> TSelf
        "removeDebug" => T<unit> ^-> TSelf
        "debug" => T<unit> ^-> T<unit>
        "updateBounds" => T<unit> ^-> T<unit>
        "handleScroll" => T<unit> ^-> T<unit>
        "revert" => T<unit> ^-> TSelf
    ]
    |> ignore

    AutoLayout
    |+> Instance [
        "params" =? T<obj>
        "root" =? T<JavaScript.Dom.Node>
        "id" =? T<string> + T<int>
        "revert" => T<unit> ^-> TSelf
        "record" => T<unit> ^-> TSelf
        "animate" => (!?T<obj>)?parameters ^-> Timeline
        "update" => (T<obj> ^-> T<unit>)?callback * (!?T<obj>)?parameters ^-> Timeline
    ]
    |> ignore

    Spring
    |+> Instance [
        "timeStep" =? T<float>
        "restThreshold" =? T<float>
        "restDuration" =? T<float>
        "maxDuration" =? T<float>
        "completed" =? T<bool>
        "solverDuration" =? T<float>
        "settlingDuration" =? T<float>
        "bounce" =? T<float>
        "duration" =? T<float>
        "stiffness" =? T<float>
        "damping" =? T<float>
        "mass" =? T<float>
        "velocity" =? T<float>
        "ease" =? EasingFunction
        "solve" => T<float>?time ^-> T<float>
        "compute" => T<unit> ^-> T<unit>
    ]
    |> ignore

    Tween
    |+> Instance [
        "id" =? T<int>
        "parent" =? JSAnimation
        "property" =? T<string>
        "target" =? T<obj>
        "duration" =? T<float>
        "delay" =? T<float>
    ]
    |> ignore

    // ------------------------------------------------------------------
    // The `animejs` module: top-level functions
    // ------------------------------------------------------------------

    let Anime = Class "Anime" |> Import "*" "animejs"

    Anime
    |+> Static [
        "animate" => TargetsParam?targets * AnimationParams?parameters ^-> JSAnimation
        "createTimeline" => (!?TimelineParams)?parameters ^-> Timeline
        "createTimer" => (!?TimerParams)?parameters ^-> Timer
        "createAnimatable" =>
            TargetsParam?targets * (!?AnimatableParams)?parameters ^-> Animatable
        "createDraggable" =>
            TargetsParam?target * (!?DraggableParams)?parameters ^-> Draggable
        "createScope" => (!?ScopeParams)?parameters ^-> Scope
        "stagger" =>
            (T<float> + T<string> + (T<float> * T<float>) + (T<string> * T<string>))?value
            * (!?StaggerParams)?parameters ^-> StaggerFunction
        "remove" =>
            TargetsParam?targets * (!?T<obj>)?renderable
            * (!?T<string>)?propertyName ^-> !|T<obj>
        "get" =>
            TargetsParam?target * T<string>?propertyName
            * (!?(T<string> + T<bool>))?unit ^-> T<string>
        "set" => TargetsParam?targets * AnimationParams?parameters ^-> JSAnimation
        "random" =>
            (!?T<float>)?min * (!?T<float>)?max * (!?T<int>)?decimalLength ^-> T<float>
        "sync" => (!?(Timer?self ^-> T<unit>))?callback ^-> Timer
        "onScroll" => (!?ScrollObserverParams)?parameters ^-> ScrollObserver
        "createLayout" => TargetsParam?root * (!?T<obj>)?parameters ^-> AutoLayout
        "splitText" =>
            TargetsParam?target * (!?TextSplitterParams)?parameters ^-> TextSplitter
        "scrambleText" => (!?ScrambleTextParams)?parameters ^-> FunctionValue
        "cubicBezier" =>
            (!?T<float>)?x1 * (!?T<float>)?y1 * (!?T<float>)?x2 * (!?T<float>)?y2
            ^-> EasingFunction
        "steps" => (!?T<int>)?steps * (!?T<bool>)?fromStart ^-> EasingFunction
        "linear" => T<JavaScript.Array>?args ^-> EasingFunction
        "irregular" => (!?T<int>)?length * (!?T<float>)?randomness ^-> EasingFunction
        "spring" => (!?SpringParams)?parameters ^-> Spring
        "createSpring" => (!?SpringParams)?parameters ^-> Spring
        "createDrawable" =>
            TargetsParam?selector * (!?T<float>)?start * (!?T<float>)?endValue
            ^-> !|DrawableSVGGeometry
        "createMotionPath" => TargetsParam?path * (!?T<float>)?offset ^-> MotionPath
        "morphTo" => TargetsParam?path2 * (!?T<float>)?precision ^-> FunctionValue
        "clamp" => T<float>?v * T<float>?min * T<float>?max ^-> T<float>
        "round" => T<float>?v * (!?T<int>)?decimalLength ^-> T<float>
        "snap" => T<float>?v * (T<float> + (!|T<float>))?increment ^-> T<float>
        "lerp" => T<float>?start * T<float>?endValue * T<float>?factor ^-> T<float>
        "damp" =>
            T<float>?start * T<float>?endValue * T<float>?deltaTime * T<float>?factor
            ^-> T<float>
        "mapRange" =>
            T<float>?value * T<float>?inLow * T<float>?inHigh * T<float>?outLow
            * T<float>?outHigh ^-> T<float>
        "wrap" => T<float>?v * T<float>?min * T<float>?max ^-> T<float>
        "padStart" =>
            T<float>?v * T<int>?totalLength * T<string>?padString ^-> T<string>
        "padEnd" =>
            T<float>?v * T<int>?totalLength * T<string>?padString ^-> T<string>
        "roundPad" => (T<float> + T<string>)?v * T<int>?decimalLength ^-> T<string>
        "degToRad" => T<float>?degrees ^-> T<float>
        "radToDeg" => T<float>?radians ^-> T<float>
        "randomPick" => T<JavaScript.Array>?items ^-> T<obj>
        "shuffle" => T<JavaScript.Array>?items ^-> T<JavaScript.Array>
        "createSeededRandom" =>
            (!?T<int>)?seed * (!?T<float>)?seededMin * (!?T<float>)?seededMax
            * (!?T<int>)?seededDecimalLength ^-> (T<float> ^-> T<float>)
        "cleanInlineStyles" => (!?T<obj>)?renderable ^-> T<unit>
        "keepTime" => T<obj>?constructor ^-> T<obj>
    ]
    |> ignore

    // ------------------------------------------------------------------
    // Namespace objects
    // ------------------------------------------------------------------

    let Eases = Class "Eases"
    Eases
    |+> Instance [
        "linear" =? EasingFunction
        "none" =? EasingFunction
        "in" =? PowerEasing
        "out" =? PowerEasing
        "inOut" =? PowerEasing
        "outIn" =? PowerEasing
        for n in [
            "inQuad"; "outQuad"; "inOutQuad"; "outInQuad"
            "inCubic"; "outCubic"; "inOutCubic"; "outInCubic"
            "inQuart"; "outQuart"; "inOutQuart"; "outInQuart"
            "inQuint"; "outQuint"; "inOutQuint"; "outInQuint"
            "inSine"; "outSine"; "inOutSine"; "outInSine"
            "inCirc"; "outCirc"; "inOutCirc"; "outInCirc"
            "inExpo"; "outExpo"; "inOutExpo"; "outInExpo"
            "inBounce"; "outBounce"; "inOutBounce"; "outInBounce"
        ] do
            n =? EasingFunction
        for n in ["inBack"; "outBack"; "inOutBack"; "outInBack"] do
            n =? BackEasing
        for n in ["inElastic"; "outElastic"; "inOutElastic"; "outInElastic"] do
            n =? ElasticEasing
    ]
    |> ignore

    let Utils = Class "Utils"
    Utils
    |+> Instance [
        "clamp" => T<float>?v * T<float>?min * T<float>?max ^-> T<float>
        "round" => T<float>?v * (!?T<int>)?decimalLength ^-> T<float>
        "snap" => T<float>?v * (T<float> + (!|T<float>))?increment ^-> T<float>
        "lerp" => T<float>?start * T<float>?endValue * T<float>?factor ^-> T<float>
        "damp" =>
            T<float>?start * T<float>?endValue * T<float>?deltaTime * T<float>?factor
            ^-> T<float>
        "mapRange" =>
            T<float>?value * T<float>?inLow * T<float>?inHigh * T<float>?outLow
            * T<float>?outHigh ^-> T<float>
        "wrap" => T<float>?v * T<float>?min * T<float>?max ^-> T<float>
        "padStart" =>
            T<float>?v * T<int>?totalLength * T<string>?padString ^-> T<string>
        "padEnd" =>
            T<float>?v * T<int>?totalLength * T<string>?padString ^-> T<string>
        "roundPad" => (T<float> + T<string>)?v * T<int>?decimalLength ^-> T<string>
        "degToRad" => T<float>?degrees ^-> T<float>
        "radToDeg" => T<float>?radians ^-> T<float>
        "random" =>
            (!?T<float>)?min * (!?T<float>)?max * (!?T<int>)?decimalLength ^-> T<float>
        "randomPick" => T<JavaScript.Array>?items ^-> T<obj>
        "shuffle" => T<JavaScript.Array>?items ^-> T<JavaScript.Array>
        "createSeededRandom" =>
            (!?T<int>)?seed * (!?T<float>)?seededMin * (!?T<float>)?seededMax
            * (!?T<int>)?seededDecimalLength ^-> (T<float> ^-> T<float>)
        "get" =>
            TargetsParam?target * T<string>?propertyName
            * (!?(T<string> + T<bool>))?unit ^-> T<string>
        "set" => TargetsParam?targets * AnimationParams?parameters ^-> JSAnimation
        "remove" =>
            TargetsParam?targets * (!?T<obj>)?renderable
            * (!?T<string>)?propertyName ^-> !|T<obj>
        "cleanInlineStyles" => (!?T<obj>)?renderable ^-> T<unit>
        "sync" => (!?(Timer?self ^-> T<unit>))?callback ^-> Timer
        "keepTime" => T<obj>?constructor ^-> T<obj>
    ]
    |> ignore

    let Svg = Class "Svg"
    Svg
    |+> Instance [
        "createDrawable" =>
            TargetsParam?selector * (!?T<float>)?start * (!?T<float>)?endValue
            ^-> !|DrawableSVGGeometry
        "morphTo" => TargetsParam?path2 * (!?T<float>)?precision ^-> FunctionValue
        "createMotionPath" => TargetsParam?path * (!?T<float>)?offset ^-> MotionPath
    ]
    |> ignore

    let Waapi = Class "Waapi"
    Waapi
    |+> Instance [
        "animate" => TargetsParam?targets * T<obj>?parameters ^-> WAAPIAnimation
    ]
    |> ignore

    let Engine = Class "Engine"
    Engine
    |+> Instance [
        "timeUnit" =? T<string>
        "precision" =? T<float>
        "pauseOnDocumentHidden" =? T<bool>
        "defaults" =? DefaultsParams
        "paused" =? T<bool>
        "pause" => T<unit> ^-> TSelf
        "resume" => T<unit> ^-> TSelf
        "wake" => T<unit> ^-> TSelf
        "update" => T<unit> ^-> T<unit>
    ]
    |> ignore

    let TextApi = Class "TextApi"
    TextApi
    |+> Instance [
        "split" =>
            TargetsParam?target * (!?TextSplitterParams)?parameters ^-> TextSplitter
        "splitText" =>
            TargetsParam?target * (!?TextSplitterParams)?parameters ^-> TextSplitter
        "scrambleText" => (!?ScrambleTextParams)?parameters ^-> FunctionValue
    ]
    |> ignore

    // Namespace objects exposed on the module (animejs.eases, animejs.utils, ...)
    Anime
    |+> Static [
        "eases" =? Eases
        "utils" =? Utils
        "svg" =? Svg
        "waapi" =? Waapi
        "engine" =? Engine
        "text" =? TextApi
    ]
    |> ignore

    // ------------------------------------------------------------------
    // Assembly
    // ------------------------------------------------------------------

    let Assembly =
        Assembly [
            Namespace "WebSharper.AnimeJs" [
                Anime
                Eases
                Utils
                Svg
                Waapi
                Engine
                TextApi

                Timer
                JSAnimation
                Timeline
                Animatable
                Draggable
                Scope
                WAAPIAnimation
                TextSplitter
                ScrollObserver
                AutoLayout
                Spring
                Tween
                DrawableSVGGeometry

                DefaultsParams
                AnimationParams
                TimelineParams
                TimerParams
                AnimatableParams
                StaggerParams
                DraggableParams
                ScopeParams
                SpringParams
                TextSplitterParams
                ScrambleTextParams
                ScrollObserverParams
                MotionPath
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member x.Assembly = Definition.Assembly

[<assembly: Extension(typeof<Extension>)>]
do ()
