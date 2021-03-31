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

let private matrix (items : float[,]) =

    let densityX = (items |> Array2D.length1) - 1
    let densityY = (items |> Array2D.length2) - 1

    let createLine j =
        { 0 .. densityX }
        |> Seq.map (fun i -> items.[i, j])
        |> Seq.map (sprintf "%e")
        |> Seq.reduce (sprintf "%s %s")

    let combinedRows =
        { 0 .. densityY }
        |> Seq.map createLine
        |> Seq.reduce (sprintf "%s\n%s")

    combinedRows

//-------------------------------------------------------------------------------------------------

let private plotHeatmapTraces = "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

$data2 << EOD
{2}
EOD

$data3 << EOD
{3}
EOD

$data4 << EOD
{4}
EOD

tag = '{5}'

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

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key textcolor '#ffffff'

set linetype 1 linewidth 2 linecolor '#00ff00'
set linetype 2 linewidth 2 linecolor '#ffffff'
set linetype 3 pointtype 6 linecolor '#ffffff'
set linetype 4 pointtype 7 linecolor '#ffffff'

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

stats [][0:0] $data0 matrix using (0) nooutput
densityX = STATS_size_x - 1
densityY = STATS_size_y - 1

plot $data0 using ($1/densityX):($2/densityY):($3/10) matrix with image pixels notitle,\
     $data1 using 1:2 with lines title 'Plateau',\
     $data2 using 1:2 with lines title sprintf('Trace %s', tag),\
     $data3 using 1:2:3 with labels point linetype 3 title 'Start',\
     $data4 using 1:2:3 with labels point linetype 4 title 'Finish'
"

let renderHeatmapTraces path heatmap plateau trace tag =

    let data0 = matrix heatmap

    let data1 =
        plateau
        |> Array.map (fun (x, y) -> sprintf "%e %e" x y)
        |> String.concat "\n"

    let data2 =
        trace
        |> Array.map (fun (x, y) -> sprintf "%e %e" x y)
        |> String.concat "\n"

    let data3 = sprintf "%e %e" <|| (trace |> Array.head)
    let data4 = sprintf "%e %e" <|| (trace |> Array.last)

    render path plotHeatmapTraces [| data0; data1; data2; data3; data4; tag |]
