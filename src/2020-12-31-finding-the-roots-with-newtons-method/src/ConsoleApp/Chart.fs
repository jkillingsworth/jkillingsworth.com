module Chart

open System
open System.Diagnostics
open System.IO
open System.Text

//-------------------------------------------------------------------------------------------------

let private preamble = "
set terminal svg size 720 405 font 'Consolas, Menlo, monospace'
set encoding utf8
set output '{0}'
"

let private render path template args =

    let preamble = String.Format(preamble, Path.GetFullPath(path))
    let template = String.Format(template, args)
    let plot = preamble + template
    use proc = new Process()
    proc.StartInfo.FileName <- "gnuplot.exe"
    proc.StartInfo.UseShellExecute <- false
    proc.StartInfo.RedirectStandardInput <- true
    proc.StartInfo.StandardInputEncoding <- new UTF8Encoding()
    proc.Start() |> ignore
    proc.StandardInput.Write(plot)
    proc.StandardInput.Flush()

//-------------------------------------------------------------------------------------------------

let private matrix (data : float[,]) densityX densityY =

    let createLine j =
        { 0 .. densityX }
        |> Seq.map (fun i -> data.[i, j])
        |> Seq.map (sprintf "%e")
        |> Seq.reduce (sprintf "%s %s")

    let combinedRows =
        { 0 .. densityY }
        |> Seq.map createLine
        |> Seq.reduce (sprintf "%s\n%s")

    combinedRows

//-------------------------------------------------------------------------------------------------

let private plotHeatmapTraces = "
$heatmap << EOD
{0}
EOD

$plateau << EOD
{1}
EOD

$trace << EOD
{5}
EOD

$start << EOD
{6}
EOD

$final << EOD
{7}
EOD

set border linewidth 1.2
set xtics scale 0.01, 0.01
set ytics scale 0.01, 0.01

set xlabel 'Coin Bias (+1)'
set xrange [0:1]
set xtics 0.2
set format x '%0.2f'

set ylabel 'Coin Bias (+2)'
set yrange [0:1]
set ytics 0.2
set format y '%0.2f'

set cblabel offset 1 'Cost \u00f7 10^{{1}}'
set format cb '%0.2f'

set key box linecolor '#808080' textcolor '#ffffff' samplen 1
set key top left reverse Left

set linetype 1 linewidth 2 linecolor '#00ff00'
set linetype 2 linewidth 2 linecolor '#ffffff'

set palette defined\
(\
0 '#000004',\
1 '#1c1044',\
2 '#4f127b',\
3 '#812581',\
4 '#b5367a',\
5 '#e55964',\
6 '#fb8761',\
7 '#fec287',\
8 '#fbfdbf' \
)

plot '$heatmap' using ($1/{2}):($2/{3}):($3/10) matrix with image pixels notitle,\
     '$plateau' using 1:2 with lines title 'Plateau',\
     '$trace' using 1:2 with lines title 'Trace {4}',\
     '$start' with labels point pointtype 6 linecolor '#ffffff' title 'Start',\
     '$final' with labels point pointtype 7 linecolor '#ffffff' title 'Finish'
"

let renderHeatmapTraces path heatmap plateau trace tag =

    let densityX = (heatmap |> Array2D.length1) - 1
    let densityY = (heatmap |> Array2D.length2) - 1
    let heatmap = matrix heatmap densityX densityY

    let plateau =
        plateau
        |> Array.map (fun (x, y) -> sprintf "%e %e" x y)
        |> String.concat "\n"

    let start = sprintf "%e %e" <|| (trace |> Array.head)
    let final = sprintf "%e %e" <|| (trace |> Array.last)

    let trace =
        trace
        |> Array.map (fun (x, y) -> sprintf "%e %e" x y)
        |> String.concat "\n"

    render path plotHeatmapTraces [| heatmap; plateau; densityX; densityY; tag; trace; start; final |]
