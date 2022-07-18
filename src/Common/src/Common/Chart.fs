module Common.Chart

open System
open System.Diagnostics
open System.IO
open System.Text

//-------------------------------------------------------------------------------------------------

let hsv h s v =

    let h = h / 60.0
    let c = v * s
    let x = c * (1.0 - abs ((h % 2.0) - 1.0))
    let m = v - c

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

let mix (r1, g1, b1) (r2, g2, b2) ratio =

    let r = (r1 * ratio) + (r2 * (1.0 - ratio))
    let g = (g1 * ratio) + (g2 * (1.0 - ratio))
    let b = (b1 * ratio) + (b2 * (1.0 - ratio))

    (r, g, b)

//-------------------------------------------------------------------------------------------------

let parWhite = hsv 0.0 0.0 1.0
let parBlack = hsv 0.0 0.0 0.0

let baseGray = mix parWhite parBlack 0.50
let liteGray = mix baseGray parWhite 0.50
let darkGray = mix baseGray parBlack 0.75
let highGray = mix baseGray parWhite 0.20

let hueRojo = 000.0
let hueGold = 045.0
let hueLeaf = 120.0
let hueBlue = 220.0
let huePurp = 270.0

let richRojo = hsv hueRojo 1.00 1.00
let richBlue = hsv hueBlue 1.00 1.00

let baseRojo = hsv hueRojo 0.75 1.00
let baseGold = hsv hueGold 1.00 1.00
let baseLeaf = hsv hueLeaf 1.00 0.75
let baseBlue = hsv hueBlue 0.75 1.00
let basePurp = hsv huePurp 0.75 1.00

let liteRojo = mix baseRojo parWhite 0.50
let liteGold = mix baseGold parWhite 0.50
let liteLeaf = mix baseLeaf parWhite 0.50
let liteBlue = mix baseBlue parWhite 0.50
let litePurp = mix basePurp parWhite 0.50

let darkRojo = mix baseRojo parBlack 0.75
let darkGold = mix baseGold parBlack 0.75
let darkLeaf = mix baseLeaf parBlack 0.75
let darkBlue = mix baseBlue parBlack 0.75
let darkPurp = mix basePurp parBlack 0.75

//-------------------------------------------------------------------------------------------------

let private serialize name (r, g, b) =

    let r = int (round (r * 255.0)) <<< 16
    let g = int (round (g * 255.0)) <<< 08
    let b = int (round (b * 255.0)) <<< 00

    sprintf "%s = 0x%06x" name (r + g + b)

let private colordef =

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
        serialize (nameof baseGold) baseGold
        serialize (nameof baseLeaf) baseLeaf
        serialize (nameof baseBlue) baseBlue
        serialize (nameof basePurp) basePurp
        ""
        serialize (nameof liteRojo) liteRojo
        serialize (nameof liteGold) liteGold
        serialize (nameof liteLeaf) liteLeaf
        serialize (nameof liteBlue) liteBlue
        serialize (nameof litePurp) litePurp
        ""
        serialize (nameof darkRojo) darkRojo
        serialize (nameof darkGold) darkGold
        serialize (nameof darkLeaf) darkLeaf
        serialize (nameof darkBlue) darkBlue
        serialize (nameof darkPurp) darkPurp
        |]

    colors |> String.concat "\n"

//-------------------------------------------------------------------------------------------------

let private preamble = "
set terminal svg size 720 405 font 'monospace'
set encoding utf8
set output '{0}'

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
