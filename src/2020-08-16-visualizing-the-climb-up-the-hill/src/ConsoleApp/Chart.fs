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

let private downsampleEvaluate samples items = function
    | i when i = samples -> items |> Array.last
    | i ->
        let count = items |> Array.length
        let ratio = float i / float samples
        let index = ratio * float count
        items.[index |> round |> int]

let private downsample samples items =
    downsampleEvaluate samples items |> Array.init (samples + 1)

//-------------------------------------------------------------------------------------------------

let private plotBiases = "
$data << EOD
{0}
EOD

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set xlabel 'State'
set xtics scale 0.01, 0.01
set xtics ({2})
set xrange [-{1}:+{1}]

set ylabel 'Probability of Heads'
set ytics scale 0.01, 0.01
set ytics 0.10
set yrange [0:1]
set format y '%0.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 1 linecolor '#80a080'
set style fill solid border linecolor '#ffffff'

plot '$data' using 1:2 with boxes title 'Coin Bias',\
     '$data' using 1:($3 == '0.00%' ? 1/0 : 0.04):3 with labels notitle center rotate by 0 textcolor '#ffffff',\
     '$data' using 1:($3 != '0.00%' ? 1/0 : 0.04):3 with labels notitle center rotate by 0 textcolor '#607860'
"

let renderBiases path data =

    let n = 3

    let xtic = function
        | 0 -> "0"
        | i -> sprintf "'%+i' %i" i i

    let xtics =
        [| -(n - 1) .. 1 .. +(n - 1) |]
        |> Array.map xtic
        |> Array.reduce (fun l r -> l + ", " + r)

    let data =
        data
        |> Array.mapi (fun i x -> sprintf "%i %e %s" (i - n) x (percent x))
        |> String.concat "\n"

    render path plotBiases [| data; n; xtics |]

//-------------------------------------------------------------------------------------------------

let private plotPmfunc = "
$data << EOD
{0}
EOD

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set xlabel 'Possible Outcome'
set xtics scale 0.01, 0.01
set xtics ({2})
set xrange [-{1}-2:+{1}+2]

set ylabel 'Probability'
set ytics scale 0.01, 0.01
set yrange [0:0.6]
set format y '%0.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 1 linecolor '#808080'
set style fill solid border linecolor '#ffffff'

plot '$data' using 1:2 with boxes title 'Probability Mass',\
     '$data' using 1:(0.024):3 with labels notitle center rotate by 0 textcolor '#ffffff'
"

let renderPmfunc path data =

    let n = 3

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

//-------------------------------------------------------------------------------------------------

let private plotTosses = "
$data << EOD
{0}
EOD

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set xlabel 'Sequence'
set xtics scale 0.01, 0.01
set xrange [-1:(2**{1})]

set ylabel 'Probability'
set ytics scale 0.01, 0.01
set yrange [0:0.30]
set format y '%0.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 1 linecolor '#80b0e0'
set linetype 2 linewidth 1 linecolor '#8098b0'
set linetype 3 linewidth 1 linecolor '#b08080'
set linetype 4 linewidth 1 linecolor '#e08080'
set style fill solid border linecolor '#ffffff'

plot '$data' using 1:($3 == 0 ? $4 : 0):xtic(2) with boxes title '0 Heads, 4 Tails',\
     '$data' using 1:($3 == 1 ? $4 : 0):xtic(2) with boxes title '1 Heads, 3 Tails',\
     '$data' using 1:($3 == 2 ? $4 : 0):xtic(2) with boxes title '2 Heads, 2 Tails',\
     '$data' using 1:($3 == 3 ? $4 : 0):xtic(2) with boxes title '3 Heads, 1 Tails',\
     '$data' using 1:($5 == '0.00%' && $3 == 0 ? 0.012 : 1/0):5 with labels notitle center rotate by 0 textcolor '#80b0e0',\
     '$data' using 1:($5 == '0.00%' && $3 == 1 ? 0.012 : 1/0):5 with labels notitle center rotate by 0 textcolor '#8098b0',\
     '$data' using 1:($5 == '0.00%' && $3 == 2 ? 0.012 : 1/0):5 with labels notitle center rotate by 0 textcolor '#b08080',\
     '$data' using 1:($5 == '0.00%' && $3 == 3 ? 0.012 : 1/0):5 with labels notitle center rotate by 0 textcolor '#e08080',\
     '$data' using 1:($5 != '0.00%' ? 0.012 : 1/0):5 with labels notitle center rotate by 0 textcolor '#ffffff'
"

let renderTosses path data =

    let n = 3

    let color s =
        s
        |> Seq.filter (fun x -> x = 'H')
        |> Seq.length

    let data =
        data
        |> Array.mapi (fun i (s, r) -> sprintf "%i %s %i %e %s" i s (color s) r (percent r))
        |> String.concat "\n"

    render path plotTosses [| data; n |]

//-------------------------------------------------------------------------------------------------

let private plotSurface = "
$heatmap << EOD
{0}
EOD

set xlabel 'Coin Bias (+1)'
set xtics scale 0.01, 0.01
set xtics 0.2
set xrange [0:1]
set format x '%0.1f'

set ylabel 'Coin Bias (+2)'
set ytics scale 0.01, 0.01
set ytics 0.2
set yrange [0:1]
set format y '%0.1f'

if ({1} == 1) {{ set zrange [0:0.25] }}
if ({1} == 2) {{ set zrange [0.25:0] }}
set format z '%0.2f'

set cblabel 'Cost'
set format cb '%0.2f'

set pm3d
set grid
if ({1} == 1) {{ set view 30,30,1,1.8 }}
if ({1} == 2) {{ set view 30,60,1,1.8 }}

set key box linecolor '#808080' samplen 1 width -1
set key top left reverse Left

splot '$heatmap' using ($1/{2}):($2/{3}):3 matrix with lines title 'Surface Plot'
"

let renderSurface path data style =

    let densityX = (data |> Array2D.length1) - 1
    let densityY = (data |> Array2D.length2) - 1
    let heatmap = matrix data densityX densityY

    render path plotSurface [| heatmap; style; densityX; densityY |]

//-------------------------------------------------------------------------------------------------

let private plotHeatmap = "
$heatmap << EOD
{0}
EOD

$plateau << EOD
{1}
EOD

set xlabel 'Coin Bias (+1)'
set xtics scale 0.01, 0.01
set xtics 0.2
set xrange [0:1]
set format x '%0.2f'

set ylabel 'Coin Bias (+2)'
set ytics scale 0.01, 0.01
set ytics 0.2
set yrange [0:1]
set format y '%0.2f'

set cblabel 'Cost'
set format cb '%0.2f'

set key box linecolor '#808080' textcolor '#ffffff' samplen 1
set key top left reverse Left

set linetype 1 linewidth 2 linecolor '#00ff00'

plot '$heatmap' using ($1/{2}):($2/{3}):3 matrix with image pixels notitle,\
     '$plateau' using 1:2 with lines title 'Plateau'
"

let renderHeatmap path heatmap plateau =

    let densityX = (heatmap |> Array2D.length1) - 1
    let densityY = (heatmap |> Array2D.length2) - 1
    let heatmap = matrix heatmap densityX densityY

    let plateau =
        plateau
        |> Array.map (fun (x, y) -> sprintf "%e %e" x y)
        |> String.concat "\n"

    render path plotHeatmap [| heatmap; plateau; densityX; densityY |]

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

set xlabel 'Coin Bias (+1)'
set xtics scale 0.01, 0.01
set xtics 0.2
set xrange [0:1]
set format x '%0.2f'

set ylabel 'Coin Bias (+2)'
set ytics scale 0.01, 0.01
set ytics 0.2
set yrange [0:1]
set format y '%0.2f'

set cblabel 'Cost'
set format cb '%0.2f'

set key box linecolor '#808080' textcolor '#ffffff' samplen 1
set key top left reverse Left

set linetype 1 linewidth 2 linecolor '#00ff00'
set linetype 2 linewidth 2 linecolor '#ffffff'

plot '$heatmap' using ($1/{2}):($2/{3}):3 matrix with image pixels notitle,\
     '$plateau' using 1:2 with lines title 'Plateau',\
     '$trace' using 1:2 with lines title 'Trace {4}',\
     '$start' with labels point pointtype 6 linecolor '#ffffff' title 'Start',\
     '$final' with labels point pointtype 7 linecolor '#ffffff' title 'Finish'
"

let renderHeatmapTraces path heatmap plateau trace tag =

    let densityX = (heatmap |> Array2D.length1) - 1
    let densityY = (heatmap |> Array2D.length2) - 1
    let heatmap = matrix heatmap densityX densityY
    let samples = (plateau |> Array.length) - 1

    let plateau =
        plateau
        |> Array.map (fun (x, y) -> sprintf "%e %e" x y)
        |> String.concat "\n"

    let start = sprintf "%e %e" <|| (trace |> Array.item 0)
    let final = sprintf "%e %e" <|| (trace |> Array.last)

    let trace =
        trace
        |> downsample samples
        |> Array.map (fun (x, y) -> sprintf "%e %e" x y)
        |> String.concat "\n"

    render path plotHeatmapTraces [| heatmap; plateau; densityX; densityY; tag; trace; start; final |]

//-------------------------------------------------------------------------------------------------

let private plotHeatmapScores = "
$heatmap << EOD
{0}
EOD

$plateau << EOD
{1}
EOD

$optimum << EOD
{5}
EOD

set xlabel 'Coin Bias (+1)'
set xtics scale 0.01, 0.01
set xtics 0.2
set xrange [0:1]
set format x '%0.2f'

set ylabel 'Coin Bias (+2)'
set ytics scale 0.01, 0.01
set ytics 0.2
set yrange [0:1]
set format y '%0.2f'

if ({6} == 1) {{ set cblabel 'Score' }}
if ({6} == 2) {{ set cblabel 'Cost' }}
set format cb '%0.2f'

set key box linecolor '#808080' textcolor '#ffffff' samplen 1
set key top left reverse Left

if ({6} == 1) {{ set palette defined (0 '#000000', 10 '#202020', 50 '#606060', 100 '#c0c0c0') }}
if ({6} == 2) {{ set palette rgb 7,5,15 }}

green(x) = (int((1 - x) * 240) << 24) + (int(255) << 8)

n = {4}

plot '$heatmap' using ($1/{2}):($2/{3}):3 matrix with image pixels notitle,\
     for [i=0:0] '$plateau' using 1:2:(val=$3) every ::i-0::i+0 w l lc rgb green(1.0) lw 2 title 'Plateau',\
     for [i=0:n] '$plateau' using 1:2:(val=$3) every ::i-1::i+1 w l lc rgb green(val) lw 1.5 + val notitle,\
     '$optimum' with labels point pointtype 7 linecolor '#ffffff' title 'Optimum'
"

let renderHeatmapScores path heatmap scores (p1, p2, score) style =

    let densityX = (heatmap |> Array2D.length1) - 1
    let densityY = (heatmap |> Array2D.length2) - 1
    let heatmap = matrix heatmap densityX densityY
    let samples = (scores |> Array.length) - 1

    let min = scores |> Array.map (fun (_, _, s) -> s) |> Array.min
    let max = scores |> Array.map (fun (_, _, s) -> s) |> Array.max
    let scale x = (max - x) / (max - min)

    let plateau =
        scores
        |> Array.map (fun (p1, p2, s) -> sprintf "%e %e %e" p1 p2 (scale s))
        |> String.concat "\n"

    let optimum = sprintf "%e %e" p1 p2

    render path plotHeatmapScores [| heatmap; plateau; densityX; densityY; samples; optimum; style |]

//-------------------------------------------------------------------------------------------------

let private plotScores = "
$scores << EOD
{0}
EOD

$optima << EOD
{1} {2} {2:f8}
EOD

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set xlabel 'Coin Bias (+1)'
set xtics scale 0.01, 0.01
set xrange [{3}:{4}]
set xtics ({3}, {4}, {1})
set format x '%0.4f'

set ylabel 'Score'
set ytics scale 0.01, 0.01
set yrange [-0.05:0.8]
set format y '%0.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 1 linecolor '#ff0000'

plot '$scores' using 1:2 with lines title 'Score {5}',\
     '$optima' with labels offset 0,1 point pointtype 2 title 'Optimum'
"

let renderScores path scores (p1, p2, score) tag =

    let xvals = scores |> Array.map (fun (p1, p2, s) -> p1)
    let lower = Array.min xvals
    let upper = Array.max xvals

    let scores =
        scores
        |> Array.map (fun (p1, p2, s) -> sprintf "%e %e" p1 s)
        |> String.concat "\n"

    render path plotScores [| scores; p1; score; lower; upper; tag |]
