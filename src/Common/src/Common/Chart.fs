module Common.Chart

open System
open System.Diagnostics
open System.IO
open System.Text

//-------------------------------------------------------------------------------------------------

let private preamble = "
set terminal svg size 720 405 font 'monospace'
set encoding utf8
set output '{0}'

rgb(r, g, b) = (int(r * 255) * 65536) + (int(g * 255) * 256) + int(b * 255)
hsv(h, s, v) = hsv2rgb(h, s, v)

hueRojo = 000.0 / 360.0
hueGold = 045.0 / 360.0
hueLeaf = 120.0 / 360.0
hueBlue = 220.0 / 360.0
huePurp = 270.0 / 360.0
huePink = 310.0 / 360.0

richRojo = hsv(hueRojo, 1.00000, 1.00000)
richBlue = hsv(hueBlue, 1.00000, 1.00000)

baseRojo = hsv(hueRojo, 0.75000, 1.00000)
baseGold = hsv(hueGold, 1.00000, 1.00000)
baseLeaf = hsv(hueLeaf, 1.00000, 0.75000)
baseBlue = hsv(hueBlue, 0.75000, 1.00000)
basePurp = hsv(huePurp, 0.75000, 1.00000)

liteRojo = rgb(1.00000, 0.62500, 0.62500)
liteGold = rgb(1.00000, 0.87500, 0.50000)
liteLeaf = rgb(0.50000, 0.87500, 0.50000)
liteBlue = rgb(0.62500, 0.75000, 1.00000)
litePurp = rgb(0.81250, 0.62500, 1.00000)

darkRojo = rgb(0.75000, 0.18750, 0.18750)
darkGold = rgb(0.75000, 0.56250, 0.00000)
darkLeaf = rgb(0.00000, 0.56250, 0.00000)
darkBlue = rgb(0.18750, 0.37500, 0.75000)
darkPurp = rgb(0.46875, 0.18750, 0.75000)

baseGray = hsv(0, 0, 0.50000)
liteGray = hsv(0, 0, 0.75000)
darkGray = hsv(0, 0, 0.37500)
highGray = hsv(0, 0, 0.90000)

set style line 1 linewidth 1 linecolor '#e6e6e6'
set style line 2 linewidth 2 linecolor '#e6e6e6'

set xtics scale 0, 0.0001
set ytics scale 0, 0.0001
"

let private terminal = "
exit
"

let render path template args =

    let preamble = String.Format(preamble, Path.GetFullPath(path))
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

set key box linecolor '#808080'
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

set key box linecolor '#808080'
set key noopaque
set key samplen 1
"
