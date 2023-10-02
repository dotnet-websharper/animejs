namespace WebSharper.AnimeJs.Extension

open WebSharper
open WebSharper.InterfaceGenerator


module Definition =
    
    let Anime =
        Class "Anime"
        |> ImportDefault "animejs/lib/anime.es.js"
        
    let Tween =
        Class "Tween"
        |+> Instance [
            "duration" =? T<float>
            "delay" =? T<float>
            for n in ["value";"from";"to";"start";"end";"round"] do
                n =? T<int>
            for n in ["isPath";"isPathTargetInsideSVG";"isColor"] do
                n =? T<bool>
        ]
        
    let AnimationType = Pattern.EnumStrings "AnimationType" [
        "attribute";"transform";"css";"object"
    ]
    
    let Animation =
        AbstractClass "Animation"
        |+> Instance [
            "type" =? AnimationType
            "property" =? T<string>
            "animatable" =? T<bool>
            "tweens" =? !|Tween
            "duration" =? T<float>
            "delay" =? T<float>
            "endDelay" =? T<float>
            "seek" => T<float> ^-> T<unit>
            "play" => T<unit> ^-> T<unit>
            "pause" => T<unit> ^-> T<unit>
            "restart" => T<unit> ^-> T<unit>
        ]
        
    Anime
        |=> Inherits Animation
        |> ignore
    let transforms = [
        "translateX"
        "translateY"
        "translateZ"
        "rotate"
        "rotateX"
        "rotateY"
        "rotateZ"
        "scale"
        "scaleX"
        "scaleY"
        "scaleZ"
        "skew"
        "skewX"
        "skewY"
        "perspective"
        "matrix"
        "matrix3d"
    ]
    let cssProperties = [
        let inline csw s = [
            $"{s}-color"
            $"{s}-style"
            $"{s}-width"
        ]
        "accent-color"
        "align-content"
        "align-items"
        "align-self"
        "all"
        "animation"
        for n in ["delay";"direction";"duration";"fill-mode";"iteration-count";"name";"play-state";"timing-function"] 
            do $"animation-{n}"
        "aspect-ratio"
        
        "backdrop-filter"
        "backface-visibility"
        "background"
        for n in ["attachment";"blend-mode";"clip";"color";"image";"origin";
                    "position";"position-x";"position-y";"repeat";"size"] 
            do $"background-{n}"
        "block-size"
        "border"
        "border-block"
        for n in ["end";"start"] do
            for res in csw $"border-block-{n}" do res
        "border-block-style"
        "border-block-width"
        "border-bottom"
        "border-bottom-color"
        "border-bottom-left-radius"
        "border-bottom-right-radius"
        "border-bottom-style"
        "border-bottom-width"
        "border-collapse"
        "border-color"
        "border-image"
        for n in ["outset";"repeat";"slice";"source";"width"] do
            $"border-image-{n}"
        "border-inline"
        for n in ["";"end-";"start-"] do
            for res in csw $"border-inline-{n}" do res
        
        "border-left"
        for res in csw "border-left-" do res
        "border-radius"
        "border-right"
        for res in csw "border-right-" do res
        "border-spacing"
        "border-style"
        "border-top"
        for res in csw "border-top-" do res
        "border-top-left-radius"
        "border-top-right-radius"
        "border-width"
        "bottom"
        for n in ["decoration-break";"reflect";"shadow";"sizing"] do
            $"box-{n}"
        for n in ["after";"before";"inside"] do
            $"break-{n}"

        "caption-side"
        "caret-color"
        // "@charset"
        "clear"
        "clip"
        "color"
        "column-count"
        "column-fill"
        "column-gap"
        "column-rule"
        for res in csw "column-rule-" do res
        "column-span"
        "column-width"
        "columns"
        "content"
        "counter-increment"
        "counter-reset"
        "cursor"

        "display"

        "empty-cells"

        "filter"
        "flex"
        "flex-basis"
        "flex-direction"
        "flex-flow"
        "flex-grow"
        "flex-shrink"
        "flex-wrap"
        "float"
        "font"
        // "@font-face"
        "font-family"
        "font-feature-settings"
        "font-kerning"
        "font-size"
        "font-size-adjust"
        "font-stretch"
        "font-style"
        "font-variant"
        "font-variant-caps"
        "font-weight"

        let egs n = [
            $"{n}-end"
            $"{n}-gap"
            $"{n}-start"
        ]
        "grid"
        "grid-area"
        for res in egs "grid-auto" do res
        "grid-column"
        for res in egs "grid-column" do res
        "grid-gap"
        "grid-row"
        for res in egs "grid-row" do res
        "grid-template"
        for n in ["areas";"columns";"rows"] do
            $"grid-template-{n}"
        
        "hanging-punctuation"
        "height"
        "hyphens"
        
        "inline-size"
        "inset"
        "inset-block"
        "inset-inline"
        for n in ["end";"start"] do
            $"inset-block-{n}"
            $"inset-inline-{n}"
        
        "justify-content"
        "justify-items"
        "justify-self"

        "left"
        "letter-spacing"
        "line-height"
        "list-style"
        "list-style-image"
        "list-style-position"
        "list-style-type"
        
        
        "margin"
        "margin-block"
        "margin-inline"
        for n in ["end";"start"] do
            $"margin-block-{n}"
            $"margin-inline-{n}"
        
        "margin-bottom"
        "margin-left"
        "margin-right"
        "margin-top"
        "max-height"
        "max-width"
        "max-block-size"
        "min-block-size"
        "max-inline-size"
        "min-inline-size"
        "min-height"
        "min-width"
        
        "object-fit"
        "object-position"
        "offset"
        "offset-anchor"
        "offset-distance"
        "offset-path"
        "offset-rotate"
        "opacity"
        "order"
        "orphans"
        "outline"
        "outline-color"
        "outline-offset"
        "outline-style"
        "outline-width"
        "overflow"
        "overflow-anchor"
        "overflow-wrap"
        "overflow-x"
        "overflow-y"
        
        "padding"
        "padding-block"
        "padding-block-end"
        "padding-block-start"
        "padding-bottom"
        "padding-inline"
        "padding-inline-end"
        "padding-inline-start"
        "padding-left"
        "padding-right"
        "padding-top"
        "page-break-after"
        "page-break-before"
        "page-break-inside"
        "paint-order"
        "perspective-origin"
        "place-content"
        "place-items"
        "place-self"
        "pointer-events"
        "position"
        
        "quotes"
        "resize"
        "right"
        // "rotate"
        "row-gap"
        
        for n1 in ["margin";"padding"] do
            $"scroll-{n1}"
            for n2 in ["top";"bottom";"left";"right"] do
                $"scroll-{n1}-{n2}"
            for n2 in ["block";"inline"] do
                $"scroll-{n1}-{n2}"
                for n3 in ["end";"start"] do
                    $"scroll-{n1}-{n2}-{n3}"
        "scroll-snap-align"
        "scroll-snap-stop"
        "scroll-snap-type"
        "scrollbar-color"
        
        "tab-size"
        "table-layout"
        "text-align"
        "text-align-last"
        "text-decoration"
        "text-decoration-color"
        "text-decoration-line"
        "text-decoration-style"
        "text-decoration-thickness"
        "text-indent"
        "text-justify"
        "text-orientation"
        "text-overflow"
        "text-shadow"
        "text-transform"
        "top"
        "transform-origin"
        "transition"
        "transition-delay"
        "transition-duration"
        "transition-property"
        "transition-timing-function"
        
        "unicode-bidi"
        
        "vertical-align"
        "visibility"
        
        "white-space"
        "widows"
        "width"
        "word-break"
        "word-spacing"
        "word-wrap"
        
        "z-index"
    ]
    
    // easings
    let Easing = Pattern.EnumStrings "Easing" [
        "linear"
        "easeInQuad" 
        "easeOutQuad" 
        "easeInOutQuad" 
        "easeOutInQuad"
        "easeInCubic" 
        "easeOutCubic" 
        "easeInOutCubic" 
        "easeOutInCubic"
        "easeInQuart" 
        "easeOutQuart" 
        "easeInOutQuart" 
        "easeOutInQuart"
        "easeInQuint" 
        "easeOutQuint" 
        "easeInOutQuint" 
        "easeOutInQuint"
        "easeInSine" 
        "easeOutSine" 
        "easeInOutSine" 
        "easeOutInSine"
        "easeInExpo" 
        "easeOutExpo" 
        "easeInOutExpo" 
        "easeOutInExpo"
        "easeInCirc" 
        "easeOutCirc" 
        "easeInOutCirc" 
        "easeOutInCirc"
        "easeInBack" 
        "easeOutBack" 
        "easeInOutBack" 
        "easeOutInBack"
        "easeInBounce" 
        "easeOutBounce" 
        "easeInOutBounce" 
        "easeOutInBounce"
    ]
    
    let ElasticEasingType = Pattern.EnumStrings "Easing.ElasticEasingType" [for n in ["In";"Out";"InOut";"OutIn"] -> $"ease{n}Elastic"]

               
    Easing
    |+> Static [
        "Cubic" => T<float>?x1 * T<float>?y1 * T<float>?x2 * T<float>?y2 ^-> TSelf
                      |> WithInline """{
                            var x1=$x1; var x2=$x2; var y1=$y1; var y2=$y2;
                            return 'cubicBezier(' + x1 + ',' + y1 + ',' + x2 + ',' + y2 + ')';
                      }"""
        "Spring" => T<float>?mass * T<float>?stiffness * T<float>?damping * T<float>?velocity ^-> TSelf
                       |> WithInline """{
                            var m=$mass; var s=$stiffness; var d=$damping; var v=$velocity;
                            return 'spring('+m+','+s+','+d+','+v+')';
                       }"""
        "Elastic" => ElasticEasingType?elasticType * T<float>?amplitude * T<float>?period ^-> TSelf
                        |> WithInline """{
                            var e=$elasticType; var a=$amplitude; var p=$period;
                            return e+'('+a+','+p+')';
                        }"""
        "Step" => T<int>?steps ^-> TSelf
                    |> WithInline """{
                        var s=$steps;
                        return 'steps('+s+')';
                    }"""

    ]
    |> ignore
    
    let StaggerDirection = Pattern.EnumStrings "StaggerDirection" [
        "normal"
        "reverse"
    ]
    let Direction = Pattern.EnumStrings "Direction" [
        "normal"
        "alternate"
        "reverse"
    ]
    
    let transformParam = T<float> + (T<float> * T<float>) + T<string> + (T<string> * T<string>)
    let PropertyParameter = Pattern.Config "PropertyParameter" {
        Required = [
            "value", transformParam
        ]
        Optional = [
            "delay", T<float>
            "duration", T<float>
            "easing", Easing.Type
        ]
    }
    
    let Keyframes = !| PropertyParameter
    
    let FromValue = Pattern.EnumStrings "FromValue" [
        "first";"last";"center"
    ]
    let Axis = Pattern.EnumStrings "Axis" ["x";"y"]
    
    let StaggerOptions =
        Pattern.Config "StaggerOptions" {
            Required = []
            Optional = [
                "grid", T<int>*T<int>
                "from",FromValue.Type + T<int>
                "axis", Axis.Type
                "easing", Easing.Type
                "direction", StaggerDirection.Type
                "start", T<int> + T<string>
            ]
        }
        
        
        
    let Stagger = Method "Stagger" (
        ((T<int> + (T<int>*T<int>)) + T<float> + T<string> + T<JavaScript.Array>)?value
            * (!?StaggerOptions) ^-> T<unit>
        )
    Anime
    |+> Static [
        "stagger" => Stagger.Type
    ]
    |> ignore
    
    
    let targetType = T<string> + T<JavaScript.Dom.Node> + T<obj>
    let PathValues = 
        Class "PathValues"
        |+> Static [
            ObjectConstructor (T<string> + (!|T<string>))?value
        ]
    let commonParams = [
                let functionBased = targetType?target * (!?T<int>)?index * (!?T<int>)?targetsLength ^-> T<int>
                let numericParam = T<int> + Stagger.Type + functionBased
                // property parameters
                for n in ["duration";"delay";"endDelay"] do
                    n, numericParam
                for n in cssProperties do
                    n, numericParam 
                "easing", Easing.Type
                "round", T<int>
                // animation params
                "direction", Direction.Type
                "loop", T<bool> + T<int>
                "autoplay", T<bool>
                
                "keyframes", Keyframes
                // css transforms
                for n in transforms do
                    n, transformParam + Keyframes
                //svg
                "points", T<string> + !|(T<float>*T<float>) + PathValues.Type
                "d", PathValues.Type
                "baseFrequency", T<float>
                "strokeDashOffset", T<string>*T<float> // todo
                
                // callbacks & promises
                for n in ["update";"begin";"complete";"loopBegin";"loopComplete";"changeBegin";"changeComplete"] do
                    n, Animation?anim ^-> T<unit> // TODO Anim object
                "change", (!?Animation?anim) ^-> T<unit>
                    
            ]
    let AnimeParams = 
        Pattern.Config "AnimeParams" {
            Required = [
                "targets", targetType + (!|T<JavaScript.Dom.Node>) + T<JavaScript.Array>
            ]
            Optional =  [
                yield! commonParams
            ]
        }
    let TimelineParams = Pattern.Config "TimelineParams" {
        Required = []
        Optional = [
            "targets", targetType + (!|T<JavaScript.Dom.Node>) + T<JavaScript.Array>
            yield! commonParams
        ] 
    }
    
    Anime
    |+> Static [
        Constructor AnimeParams?options
        //"Anime" => AnimeParams?options ^-> TSelf
        //|> ImportDefault "animejs/lib/anime.es.js"
    ]
    |+> Instance [
        "reverse" => T<unit> ^-> T<unit>
        "duration" =? T<float>
    ]
    |> ignore
    
    let Timeline =
        Class "Timeline"
        |=> Inherits Animation
        //|+> Static [
        //    Constructor (!?TimelineParams)?parameters
        //]
        |+> Instance [
            "add" => TimelineParams?parameters * (!?(T<int> + T<string>))?timeOffset ^-> TSelf
        ]
    Anime
    |+> Static [
        "timeline" => (!?TimelineParams)?parameters ^-> Timeline
    ]
    |> ignore
    
    let PathOptions = Pattern.EnumStrings "PathOptions" ["x";"y";"angle"]
    
    let Path = Method "Path" (PathOptions ^-> T<float>)
    Anime
    |+> Static [
        "path" => T<string> ^-> Path.Type
    ]
    |> ignore
    
    
    let Assembly =
        Assembly [
            Namespace "WebSharper.AnimeJs" [
                Tween
                AnimationType
                Animation
                
                Direction
                StaggerDirection
                PropertyParameter
                FromValue
                Axis
                ElasticEasingType
                Easing
                StaggerOptions
                
                PathOptions
                PathValues
                
                TimelineParams
                Timeline
                
                AnimeParams
                Anime
                
            ]
        ]

[<Sealed>]
type Extension() =
    interface IExtension with
        member x.Assembly = Definition.Assembly
        
[<assembly: Extension(typeof<Extension>)>]
do ()