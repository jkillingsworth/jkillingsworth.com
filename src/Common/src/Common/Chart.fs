module Common.Chart

open System
open System.Diagnostics
open System.IO
open System.Text

//-------------------------------------------------------------------------------------------------

let hsl h s l =

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

    (r + m, g + m, b + m)

let mix ratio (r1, g1, b1) (r2, g2, b2) =

    let r = (r1 * ratio) + (r2 * (1.0 - ratio))
    let g = (g1 * ratio) + (g2 * (1.0 - ratio))
    let b = (b1 * ratio) + (b2 * (1.0 - ratio))

    (r, g, b)

let rgbToInt (r, g, b) =

    let r = int (round (r * 255.0)) <<< 16
    let g = int (round (g * 255.0)) <<< 08
    let b = int (round (b * 255.0)) <<< 00

    r + g + b

//-------------------------------------------------------------------------------------------------

let parWhite = hsl 000.0 0.000 1.000
let parBlack = hsl 000.0 0.000 0.000

let baseGray = parWhite |> mix 0.500 parBlack
let liteGray = baseGray |> mix 0.500 parWhite
let darkGray = baseGray |> mix 0.375 parBlack
let highGray = baseGray |> mix 0.800 parWhite

let hueRojo = 000.0
let hueRust = 030.0
let hueGold = 045.0
let hueLime = 075.0
let hueMint = 120.0
let hueTeal = 175.0
let hueBlue = 220.0
let huePurp = 270.0
let huePink = 310.0

let richRojo = hsl hueRojo 1.000 0.500
let richBlue = hsl hueBlue 1.000 0.500

let baseRojo = hsl hueRojo 1.000 0.625
let baseRust = hsl hueRust 1.000 0.500
let baseGold = hsl hueGold 1.000 0.500
let baseLime = hsl hueLime 1.000 0.375
let baseMint = hsl hueMint 1.000 0.375
let baseTeal = hsl hueTeal 1.000 0.375
let baseBlue = hsl hueBlue 1.000 0.625
let basePurp = hsl huePurp 1.000 0.625
let basePink = hsl huePink 1.000 0.625

let liteRojo = baseRojo |> mix 0.500 parWhite
let liteRust = baseRust |> mix 0.500 parWhite
let liteGold = baseGold |> mix 0.500 parWhite
let liteLime = baseLime |> mix 0.500 parWhite
let liteMint = baseMint |> mix 0.500 parWhite |> mix 0.250 liteGray
let liteTeal = baseTeal |> mix 0.500 parWhite
let liteBlue = baseBlue |> mix 0.500 parWhite
let litePurp = basePurp |> mix 0.500 parWhite
let litePink = basePink |> mix 0.500 parWhite

let darkRojo = baseRojo |> mix 0.375 parBlack
let darkRust = baseRust |> mix 0.375 parBlack
let darkGold = baseGold |> mix 0.375 parBlack
let darkLime = baseLime |> mix 0.375 parBlack
let darkMint = baseMint |> mix 0.375 parBlack
let darkTeal = baseTeal |> mix 0.375 parBlack
let darkBlue = baseBlue |> mix 0.375 parBlack
let darkPurp = basePurp |> mix 0.375 parBlack
let darkPink = basePink |> mix 0.375 parBlack

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
        ""
        serialize (nameof richRojo) richRojo
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
