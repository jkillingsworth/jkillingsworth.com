module Common.Chart

open System
open System.Diagnostics
open System.IO
open System.Text
open Wacton.Unicolour

//-------------------------------------------------------------------------------------------------

let private hsl h s l =

    let h = h / 60.0
    let c = s * (1.0 - abs (2.0 * l - 1.0))
    let x = c * (1.0 - abs ((h % 2.0) - 1.0))
    let m = l - (c / 2.0)

    let (r, g, b) =
        match h with
        | h when h >= 0 && h < 1 -> (c, x, 0.0)
        | h when h >= 1 && h < 2 -> (x, c, 0.0)
        | h when h >= 2 && h < 3 -> (0.0, c, x)
        | h when h >= 3 && h < 4 -> (0.0, x, c)
        | h when h >= 4 && h < 5 -> (x, 0.0, c)
        | h when h >= 5 && h < 6 -> (c, 0.0, x)
        | _ -> failwith "Unexpected hue."

    let r = r + m
    let g = g + m
    let b = b + m

    (r, g, b)

let private blend ratio (r1, g1, b1) (r2, g2, b2) =

    let r = (r1 * ratio) + (r2 * (1.0 - ratio))
    let g = (g1 * ratio) + (g2 * (1.0 - ratio))
    let b = (b1 * ratio) + (b2 * (1.0 - ratio))

    (r, g, b)

let private adjustChroma f (r, g, b) =

    let color = (r, g, b).ToValueTuple() |> Unicolour.FromRgb

    let l = color.Oklch.L
    let c = color.Oklch.C |> f
    let h = color.Oklch.H

    let color = (l, c, h).ToValueTuple() |> Unicolour.FromOklch

    let r = Math.Round(color.Rgb.R, 15)
    let g = Math.Round(color.Rgb.G, 15)
    let b = Math.Round(color.Rgb.B, 15)

    (r, g, b)

let private rgbToInt (r, g, b) =

    let r = int (round (r * 255.0)) <<< 16
    let g = int (round (g * 255.0)) <<< 08
    let b = int (round (b * 255.0)) <<< 00

    r + g + b

//-------------------------------------------------------------------------------------------------

let parWhite = hsl 000.0 0.0 1.00000
let parBlack = hsl 000.0 0.0 0.00000

let private hueRojo = 000.0
let private hueRust = 030.0
let private hueGold = 045.0
let private hueLime = 075.0
let private hueMint = 120.0
let private hueTeal = 185.0
let private hueBlue = 220.0
let private huePurp = 265.0
let private huePink = 315.0

let private fade = adjustChroma ((*) 0.50)
let private lite = adjustChroma (min 0.12) << blend 0.500 parWhite
let private dark = adjustChroma (min 0.15) << blend 0.375 parBlack
let private high = adjustChroma (min 0.06) << blend 0.800 parWhite
let private deep = adjustChroma (min 0.05) << blend 0.875 parBlack

let baseGray = parWhite |> blend 0.500 parBlack
let liteGray = baseGray |> lite
let darkGray = baseGray |> dark
let highGray = baseGray |> high
let deepGray = baseGray |> deep

let richRojo = hsl hueRojo 1.0 0.50000
let richMint = hsl hueMint 1.0 0.50000
let richBlue = hsl hueBlue 1.0 0.50000

let baseRojo = hsl hueRojo 1.0 0.62500
let baseRust = hsl hueRust 1.0 0.50000
let baseGold = hsl hueGold 1.0 0.50000
let baseLime = hsl hueLime 1.0 0.40625
let baseMint = hsl hueMint 1.0 0.37500
let baseTeal = hsl hueTeal 1.0 0.40625
let baseBlue = hsl hueBlue 1.0 0.62500
let basePurp = hsl huePurp 1.0 0.68750
let basePink = hsl huePink 1.0 0.68750

let fadeRojo = baseRojo |> fade
let fadeRust = baseRust |> fade
let fadeGold = baseGold |> fade
let fadeLime = baseLime |> fade
let fadeMint = baseMint |> fade
let fadeTeal = baseTeal |> fade
let fadeBlue = baseBlue |> fade
let fadePurp = basePurp |> fade
let fadePink = basePink |> fade

let liteRojo = baseRojo |> lite
let liteRust = baseRust |> lite
let liteGold = baseGold |> lite
let liteLime = baseLime |> lite
let liteMint = baseMint |> lite
let liteTeal = baseTeal |> lite
let liteBlue = baseBlue |> lite
let litePurp = basePurp |> lite
let litePink = basePink |> lite

let darkRojo = baseRojo |> dark
let darkRust = baseRust |> dark
let darkGold = baseGold |> dark
let darkLime = baseLime |> dark
let darkMint = baseMint |> dark
let darkTeal = baseTeal |> dark
let darkBlue = baseBlue |> dark
let darkPurp = basePurp |> dark
let darkPink = basePink |> dark

let highRojo = baseRojo |> high
let highRust = baseRust |> high
let highGold = baseGold |> high
let highLime = baseLime |> high
let highMint = baseMint |> high
let highTeal = baseTeal |> high
let highBlue = baseBlue |> high
let highPurp = basePurp |> high
let highPink = basePink |> high

let deepRojo = baseRojo |> deep
let deepRust = baseRust |> deep
let deepGold = baseGold |> deep
let deepLime = baseLime |> deep
let deepMint = baseMint |> deep
let deepTeal = baseTeal |> deep
let deepBlue = baseBlue |> deep
let deepPurp = basePurp |> deep
let deepPink = basePink |> deep

//-------------------------------------------------------------------------------------------------

let private colordef =

    let serialize name rgb = sprintf "%s = 0x%06x" name (rgbToInt rgb)

    let colors = [|
        serialize (nameof parWhite) parWhite
        serialize (nameof parBlack) parBlack
        ""
        serialize (nameof baseGray) baseGray
        serialize (nameof liteGray) liteGray
        serialize (nameof darkGray) darkGray
        serialize (nameof highGray) highGray
        serialize (nameof deepGray) deepGray
        ""
        serialize (nameof richRojo) richRojo
        serialize (nameof richMint) richMint
        serialize (nameof richBlue) richBlue
        ""
        serialize (nameof baseRojo) baseRojo
        serialize (nameof baseRust) baseRust
        serialize (nameof baseGold) baseGold
        serialize (nameof baseLime) baseLime
        serialize (nameof baseMint) baseMint
        serialize (nameof baseTeal) baseTeal
        serialize (nameof baseBlue) baseBlue
        serialize (nameof basePurp) basePurp
        serialize (nameof basePink) basePink
        ""
        serialize (nameof fadeRojo) fadeRojo
        serialize (nameof fadeRust) fadeRust
        serialize (nameof fadeGold) fadeGold
        serialize (nameof fadeLime) fadeLime
        serialize (nameof fadeMint) fadeMint
        serialize (nameof fadeTeal) fadeTeal
        serialize (nameof fadeBlue) fadeBlue
        serialize (nameof fadePurp) fadePurp
        serialize (nameof fadePink) fadePink
        ""
        serialize (nameof liteRojo) liteRojo
        serialize (nameof liteRust) liteRust
        serialize (nameof liteGold) liteGold
        serialize (nameof liteLime) liteLime
        serialize (nameof liteMint) liteMint
        serialize (nameof liteTeal) liteTeal
        serialize (nameof liteBlue) liteBlue
        serialize (nameof litePurp) litePurp
        serialize (nameof litePink) litePink
        ""
        serialize (nameof darkRojo) darkRojo
        serialize (nameof darkRust) darkRust
        serialize (nameof darkGold) darkGold
        serialize (nameof darkLime) darkLime
        serialize (nameof darkMint) darkMint
        serialize (nameof darkTeal) darkTeal
        serialize (nameof darkBlue) darkBlue
        serialize (nameof darkPurp) darkPurp
        serialize (nameof darkPink) darkPink
        ""
        serialize (nameof highRojo) highRojo
        serialize (nameof highRust) highRust
        serialize (nameof highGold) highGold
        serialize (nameof highLime) highLime
        serialize (nameof highMint) highMint
        serialize (nameof highTeal) highTeal
        serialize (nameof highBlue) highBlue
        serialize (nameof highPurp) highPurp
        serialize (nameof highPink) highPink
        ""
        serialize (nameof deepRojo) deepRojo
        serialize (nameof deepRust) deepRust
        serialize (nameof deepGold) deepGold
        serialize (nameof deepLime) deepLime
        serialize (nameof deepMint) deepMint
        serialize (nameof deepTeal) deepTeal
        serialize (nameof deepBlue) deepBlue
        serialize (nameof deepPurp) deepPurp
        serialize (nameof deepPink) deepPink
        |]

    colors |> String.concat "\n"

//-------------------------------------------------------------------------------------------------

let private preamble = "
set terminal svg size 720 405 font 'monospace'
set encoding utf8
set output '{0}'

pcolorDef(x) = sprintf('#%06x', x)
pcolorGet(x) = palette(x * 20 - 10)

{1}

set style line 1 linewidth 1 linecolor rgb highGray
set style line 2 linewidth 2 linecolor rgb highGray

set xtics scale 0, 0.0001
set ytics scale 0, 0.0001
"

let private terminal = "
exit
"

let render path template args =

    let preamble = String.Format(preamble, Path.GetFullPath(path), colordef)
    let template = String.Format(template, args)
    let plot = preamble + template + terminal
    use proc = new Process()
    proc.StartInfo.FileName <- Environment.GetEnvironmentVariable("GNUPLOT_EXE")
    proc.StartInfo.UseShellExecute <- false
    proc.StartInfo.RedirectStandardInput <- true
    proc.StartInfo.RedirectStandardError <- true
    proc.StartInfo.StandardInputEncoding <- new UTF8Encoding()
    proc.StartInfo.StandardErrorEncoding <- new UTF8Encoding()
    proc.Start() |> ignore
    proc.StandardInput.Write(plot)
    proc.StandardInput.Flush()
    proc.WaitForExit()
    let stderr = proc.StandardError.ReadToEnd()
    Console.ForegroundColor <- ConsoleColor.Red
    Console.Error.Write(stderr)
    Console.ResetColor()

//-------------------------------------------------------------------------------------------------

let baseplotRegular = "
set border linewidth 1.2

set grid linestyle 1
set grid xtics mxtics
set grid ytics mytics

set key box linecolor rgb baseGray
set key opaque
set key samplen 1
"

let baseplotHeatmap = "
set border linewidth 1.2

set key nobox
set key noopaque
set key samplen 1
"

let baseplotSurface = "
set border linewidth 1.0

set grid

set key box linecolor rgb baseGray
set key noopaque
set key samplen 1
"
