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

let private percent x =
    if Double.IsNaN(x) then "" else x.ToString("0.00%;-0.00%;0.00%")

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

let private mergesteps magnitude threshold items =

    let combine (basis, i, _) value =
        let i = i + 1
        let maxlength = items |> Array.length
        let magnitude = magnitude basis value
        let condition = magnitude > threshold
        if (condition || i = maxlength) then (value, i, true) else (basis, i, false)

    items
    |> Array.scan combine (items.[0], 0, true)
    |> Array.filter (fun (_, _, take) -> take)
    |> Array.map (fun (x, _, _) -> x)

let private downsample samples items =

    let compute i =
        let count = items |> Array.length
        let ratio = float i / float samples
        let index = ratio * float count
        items.[index |> round |> int]

    let compute = function
        | i when i = samples -> items |> Array.last
        | i -> compute i

    compute |> Array.init (samples + 1)

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

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key textcolor '#ffffff'

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

let renderHeatmapTraces path heatmap plateau trace samples tag =

    let densityX = (heatmap |> Array2D.length1) - 1
    let densityY = (heatmap |> Array2D.length2) - 1
    let heatmap = matrix heatmap densityX densityY

    let plateau =
        plateau
        |> Array.map (fun (x, y) -> sprintf "%e %e" x y)
        |> String.concat "\n"

    let start = sprintf "%e %e" <|| (trace |> Array.head)
    let final = sprintf "%e %e" <|| (trace |> Array.last)

    let magnitude basis value =
        let dx = fst value - fst basis
        let dy = snd value - snd basis
        sqrt <| (dx ** 2.0) + (dy ** 2.0)

    let trace =
        trace
        |> mergesteps magnitude 0.0001
        |> downsample samples
        |> Array.map (fun (x, y) -> sprintf "%e %e" x y)
        |> String.concat "\n"

    render path plotHeatmapTraces [| heatmap; plateau; densityX; densityY; tag; trace; start; final |]

//-------------------------------------------------------------------------------------------------

let private plotStepsize = "
$data << EOD
{0}
EOD

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics
set xtics scale 0.01, 0.01
set ytics scale 0.01, 0.01

set xlabel 'Iteration'
set xtics 10000
set format x '%.0s%c'

set ylabel 'Step Size \u00f7 10^{{-4}}'
set yrange [0.0:1.1]
set ytics 1
set mytics 10
set format y '%.2f'

set key box linecolor '#808080' samplen 1
set key top right reverse Left

set linetype 1 linewidth 2 linecolor '#ff0000'

plot '$data' using 1:($2/0.0001) with lines title 'Step Size'
"

let renderStepsize path data samples =

    let data =
        data
        |> downsample samples
        |> Array.map (fun (x, y) -> sprintf "%i %e" x y)
        |> String.concat "\n"

    render path plotStepsize [| data |]

//-------------------------------------------------------------------------------------------------

let private plotPmfunc = "
$data << EOD
{0}
EOD

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics
set xtics scale 0.01, 0.01
set ytics scale 0.01, 0.01

set xlabel 'Possible Outcome'
set xrange [-{1}-2:+{1}+2]
set xtics ({2})

set ylabel 'Probability'
set yrange [0:0.6]
set format y '%0.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 1 linecolor '#808080'
set style fill solid border linecolor '#ffffff'

plot '$data' using 1:2 with boxes title 'Probability Mass',\
     '$data' using 1:(0.024):3 with labels notitle textcolor '#ffffff'
"

let renderPmfunc path data =

    let n = data |> Array.length |> (+) -1

    let xtic = function
        | 0 -> "0"
        | i -> sprintf "'%+i' %i" i i

    let xtics =
        [| -n .. 2 .. +n |]
        |> Array.map xtic
        |> Array.reduce (fun l r -> l + ", " + r)

    let data =
        data
        |> Array.mapi (fun i x -> sprintf "%i %e %s" (2 * i - n) x (percent x))
        |> String.concat "\n"

    render path plotPmfunc [| data; n; xtics |]
