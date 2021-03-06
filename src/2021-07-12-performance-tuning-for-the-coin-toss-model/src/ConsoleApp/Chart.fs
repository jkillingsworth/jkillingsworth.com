﻿module Chart

open System
open System.Diagnostics
open System.IO
open System.Text

//-------------------------------------------------------------------------------------------------

let private preamble = "
set terminal svg size 720 405 font 'Consolas, Menlo, monospace'
set encoding utf8
set output '{0}'

set xtics scale 0, 0.0001
set ytics scale 0, 0.0001
"

let private terminal = "
exit
"

let private render path template args =

    let preamble = String.Format(preamble, Path.GetFullPath(path))
    let template = String.Format(template, args)
    let plot = preamble + template + terminal
    use proc = new Process()
    proc.StartInfo.FileName <- "gnuplot.exe"
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

let private matrix (items : float[,]) =

    let densityX = (items |> Array2D.length1) - 1
    let densityY = (items |> Array2D.length2) - 1

    let createLine j =
        { 0 .. densityX }
        |> Seq.map (fun i -> items.[i, j])
        |> Seq.map (sprintf "%O")
        |> String.concat "\t"

    let combinedRows =
        { 0 .. densityY }
        |> Seq.map createLine
        |> String.concat "\n"

    combinedRows

let private downsample samples items =

    let count = (items |> Array.length) - 1

    let compute i =
        let ratio = float i / float samples
        let value = ratio * float count
        let index = value |> round |> int
        items.[index]

    if (samples < count) then
        compute |> Array.init (samples + 1)
    else
        items

//-------------------------------------------------------------------------------------------------

let private plotPmfunc = "
$data0 << EOD
{0}
EOD

n = {1}; upper = {2}

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics

set xlabel 'Possible Outcome'
set xrange [-(n + 2):+(n + 2)]
if (n % 2 == 0) {{ set xtics ('0' 0) }} else {{ set xtics () }}
if (n <= 10) {{
    set for [i=+n:+1:-2] xtics add (sprintf('%+i', i) i)
    set for [i=-n:-1:+2] xtics add (sprintf('%+i', i) i)
}} else {{
    set xtics add -n, n, +n
    set mxtics (n / 2)
    set format x '%+0.0f'
}}

set ylabel 'Probability'
set yrange [0:upper]
set format y '%0.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 1 linecolor '#808080'
set style fill solid border linecolor '#ffffff'

plot $data0 using 1:2 with boxes title 'Probability Mass'
"

let renderPmfunc path pmfunc upper =

    let n = (pmfunc |> Array.length) - 1

    let data0 =
        pmfunc
        |> Array.mapi (fun i x -> sprintf "%O %O" (2 * i - n) x)
        |> String.concat "\n"

    render path plotPmfunc [| data0; n; upper |]

//-------------------------------------------------------------------------------------------------

let private plotBiases = "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

n = {2}; style = {3}

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics

set xlabel 'State'
set xrange [-n:+n]
set xtics ('0' 0)
if (n <= 10) {{
    set for [i=+1:+(n - 1):+1] xtics add (sprintf('%+i', i) i)
    set for [i=-1:-(n - 1):-1] xtics add (sprintf('%+i', i) i)
}} else {{
    set xtics add -(n - 1), (n - 1), +(n - 1)
    set mxtics (n - 1)
    set format x '%+0.0f'
}}

set ylabel 'Probability of Heads'
set yrange [0:1]
set ytics 0.10
set format y '%0.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 1 linecolor '#80a080'
set linetype 2 pointtype 7 linecolor '#a060a0'
set linetype 3 linewidth 2 linecolor '#a060a0'
set linetype 4 linewidth 1 linecolor '#908090'
set style fill solid border linecolor '#ffffff'

if (style == 0) {{
    plot $data0 using 1:2 with boxes title 'Coin Bias'
}}

if (style == 1 && n <= 10) {{
    x1 = n + 1
    x2 = n + 2
    plot $data0 using 1:2 with boxes title 'Coin Bias',\
         $data0 using 1:2:(sprintf('p_%i', $1)) every ::x1::x2 with labels offset -0.5,-1.0 point linetype 2 textcolor '#ffffff' notitle,\
         $data1 using 1:2 with lines notitle
}}

if (style == 2 && n <= 10) {{
    x1 = n + 1
    x3 = n + 3
    plot $data0 using 1:2 with boxes title 'Coin Bias',\
         $data0 using 1:2:(sprintf('p_%i', $1)) every ::x1::x3 with labels offset -0.5,-1.0 point linetype 2 textcolor '#ffffff' notitle,\
         $data1 using 1:2 with lines notitle
}}

if (style == 3 && n <= 10) {{
    x1 = n + 1
    x4 = n + 4
    plot $data0 using 1:2 with boxes title 'Coin Bias',\
         $data0 using 1:2:(sprintf('p_%i', $1)) every ::x1::x4 with labels offset -0.5,-1.0 point linetype 2 textcolor '#ffffff' notitle,\
         $data1 using 1:2 with lines notitle
}}

if (style == 1 && n > 10) {{
    plot $data0 using 1:(1 <= $1 && $1 <= 2 ? 0 : $2) with boxes title 'Coin Bias',\
         $data0 using 1:(1 <= $1 && $1 <= 2 ? $2 : 0) with boxes notitle linetype 4,\
         $data1 using 1:2 with lines notitle
}}

if (style == 2 && n > 10) {{
    plot $data0 using 1:(1 <= $1 && $1 <= 3 ? 0 : $2) with boxes title 'Coin Bias',\
         $data0 using 1:(1 <= $1 && $1 <= 3 ? $2 : 0) with boxes notitle linetype 4,\
         $data1 using 1:2 with lines notitle
}}

if (style == 3 && n > 10) {{
    plot $data0 using 1:(1 <= $1 && $1 <= 4 ? 0 : $2) with boxes title 'Coin Bias',\
         $data0 using 1:(1 <= $1 && $1 <= 4 ? $2 : 0) with boxes notitle linetype 4,\
         $data1 using 1:2 with lines notitle
}}
"

let renderBiases path biases points style =

    let n = (biases |> Array.length) / 2

    let data0 =
        biases
        |> Array.mapi (fun i x -> sprintf "%O %O" (i - n) x)
        |> String.concat "\n"

    let data1 =
        points
        |> Array.map (fun (x, y) -> sprintf "%O %O" x y)
        |> String.concat "\n"

    render path plotBiases [| data0; data1; n; style |]

//-------------------------------------------------------------------------------------------------

let private plotSurface = "
$data0 << EOD
{0}
EOD

stats $data0 using 1:2 matrix nooutput prefix 'data0'
densityX = data0_size_x - 1
densityY = data0_size_y - 1

set border linewidth 1.0
set grid

set xlabel 'Coin Bias (+1)'
set xrange [0:1]
set xtics 0.2
set format x '%0.1f'

set ylabel 'Coin Bias (+2)'
set yrange [0:1]
set ytics 0.2
set format y '%0.1f'

set format z '%0.2f'

set cblabel offset 1 'Error'
set format cb '%0.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key width -1

set pm3d
set view 30,30,1,1.8

set palette defined\
(\
0 '#440154',\
1 '#472c7a',\
2 '#3b518b',\
3 '#2c718e',\
4 '#21908d',\
5 '#27ad81',\
6 '#5cc863',\
7 '#aadc32',\
8 '#fde725' \
)

set linetype 1 linewidth 1 linecolor '#4021908d'

splot $data0 using ($1/densityX):($2/densityY):3 matrix with lines title 'Surface Plot'
"

let renderSurface path heatmap =

    let data0 = matrix heatmap

    render path plotSurface [| data0 |]

//-------------------------------------------------------------------------------------------------

let private plotHeatmap = "
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

tag = '{4}'

stats $data0 using 1:2 matrix nooutput prefix 'data0'
densityX = data0_size_x - 1
densityY = data0_size_y - 1

set border linewidth 1.2

set xlabel 'Coin Bias (+1)'
set xrange [0:1]
set xtics 0.2
set format x '%0.2f'

set ylabel 'Coin Bias (+2)'
set yrange [0:1]
set ytics 0.2
set format y '%0.2f'

set cblabel offset 1 'Error'
set format cb '%0.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key textcolor '#ffffff'

set palette defined\
(\
0 '#440154',\
1 '#472c7a',\
2 '#3b518b',\
3 '#2c718e',\
4 '#21908d',\
5 '#27ad81',\
6 '#5cc863',\
7 '#aadc32',\
8 '#fde725' \
)

set linetype 1 linewidth 2 linecolor '#ffffff'
set linetype 2 pointtype 6 linecolor '#ffffff'
set linetype 3 pointtype 7 linecolor '#ffffff'

plot $data0 using ($1/densityX):($2/densityY):3 matrix with image pixels notitle,\
     $data1 using 1:2 with lines title sprintf('Trace %s', tag),\
     $data2 using 1:2 with points linetype 2 title 'Start',\
     $data3 using 1:2 with points linetype 3 title 'Finish'
"

let renderHeatmap path heatmap trace samples tag =

    let data0 = matrix heatmap

    let data1 =
        trace
        |> downsample samples
        |> Array.map (fun (x, y) -> sprintf "%O %O" x y)
        |> String.concat "\n"

    let data2 = sprintf "%O %O" <|| (trace |> Array.head)
    let data3 = sprintf "%O %O" <|| (trace |> Array.last)

    render path plotHeatmap [| data0; data1; data2; data3; tag |]

//-------------------------------------------------------------------------------------------------

let private plotFlopCounts = "
$data0 << EOD
{0}
EOD

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics

set xlabel 'Coin Tosses (n)'
set xtics 10
set xtics add (4)

set ylabel 'Floating Point Operations'
set ytics 10
set mytics 1
set logscale y 10
set format y '  10^{{%T}}'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key width -3

set linetype 1 linewidth 1.5 linecolor '#ff0000'
set linetype 2 linewidth 1.5 linecolor '#0000ff'
set linetype 3 linewidth 1.5 linecolor '#00c000'

plot $data0 using 1:2 with lines title 'Matrix Product (R-Associative)',\
     $data0 using 1:3 with lines title 'Matrix Product (L-Associative)',\
     $data0 using 1:4 with lines title 'Optimized Method'
"

let renderFlopCounts path counts =

    let data0 =
        counts
        |> Array.mapi (fun i (ra, la, om) -> sprintf "%O %O %O %O" i ra la om)
        |> String.concat "\n"

    render path plotFlopCounts [| data0 |]
