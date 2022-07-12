module Chart

open System
open Common.Chart

//-------------------------------------------------------------------------------------------------

let private percent x =
    if Double.IsNaN(x) then "" else x.ToString("0.00%;-0.00%;0.00%")

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

let private plotPmfunc = baseplotRegular + "
$data0 << EOD
{0}
EOD

n = {1}

set xlabel 'Possible Outcome'
set xrange [-n-2:+n+2]
if (n % 2 == 0) {{ set xtics (0) }} else {{ set xtics () }}
set for [i=+n:+1:-2] xtics add (sprintf('%+i', i) i)
set for [i=-n:-1:+2] xtics add (sprintf('%+i', i) i)

set ylabel 'Probability'
set yrange [0:0.6]
set format y '%4.2f'

set key top left
set key reverse Left

set linetype 1 linewidth 1 linecolor '#808080'
set style fill solid border linecolor '#ffffff'

plot $data0 using 1:2 with boxes title 'Probability Mass',\
     $data0 using 1:(0.024):3 with labels notitle textcolor '#ffffff'
"

let renderPmfunc path pmfunc =

    let n = 3

    let data0 =
        pmfunc
        |> Array.mapi (fun i x -> sprintf "%O %O %s" (2 * i - n) x (percent x))
        |> String.concat "\n"

    render path plotPmfunc [| data0; n |]

//-------------------------------------------------------------------------------------------------

let private plotBiases = baseplotRegular + "
$data0 << EOD
{0}
EOD

n = {1}

set xlabel 'State'
set xrange [-n:+n]
set xtics (0)
set for [i=+1:+n-1:+1] xtics add (sprintf('%+i', i) i)
set for [i=-1:-n+1:-1] xtics add (sprintf('%+i', i) i)

set ylabel 'Probability of Heads'
set yrange [0:1]
set ytics 0.10
set format y '%4.2f'

set key top left
set key reverse Left

set linetype 1 linewidth 1 linecolor '#80a080'
set style fill solid border linecolor '#ffffff'

plot $data0 using 1:2 with boxes title 'Coin Bias',\
     $data0 using 1:($3 == '0.00%' ? 0.04 : 1/0):3 with labels notitle textcolor '#607860',\
     $data0 using 1:($3 != '0.00%' ? 0.04 : 1/0):3 with labels notitle textcolor '#ffffff'
"

let renderBiases path biases =

    let n = 3

    let data0 =
        biases
        |> Array.mapi (fun i x -> sprintf "%O %O %s" (i - n) x (percent x))
        |> String.concat "\n"

    render path plotBiases [| data0; n |]

//-------------------------------------------------------------------------------------------------

let private plotTosses = baseplotRegular + "
$data0 << EOD
{0}
EOD

n = {1}

set xlabel 'Sequence'
set xrange [-1:(2**n)]

set ylabel 'Probability'
set yrange [0:0.30]
set format y '%4.2f'

set key top left
set key reverse Left

set linetype 1 linewidth 1 linecolor '#80b0e0'
set linetype 2 linewidth 1 linecolor '#8098b0'
set linetype 3 linewidth 1 linecolor '#b08080'
set linetype 4 linewidth 1 linecolor '#e08080'
set style fill solid border linecolor '#ffffff'

plot $data0 using 1:($3 == 0 ? $2 : 0):xtic(4) with boxes title '0 Heads, 4 Tails',\
     $data0 using 1:($3 == 1 ? $2 : 0):xtic(4) with boxes title '1 Heads, 3 Tails',\
     $data0 using 1:($3 == 2 ? $2 : 0):xtic(4) with boxes title '2 Heads, 2 Tails',\
     $data0 using 1:($3 == 3 ? $2 : 0):xtic(4) with boxes title '3 Heads, 1 Tails',\
     $data0 using 1:($5 == '0.00%' && $3 == 0 ? 0.012 : 1/0):5 with labels notitle textcolor '#80b0e0',\
     $data0 using 1:($5 == '0.00%' && $3 == 1 ? 0.012 : 1/0):5 with labels notitle textcolor '#8098b0',\
     $data0 using 1:($5 == '0.00%' && $3 == 2 ? 0.012 : 1/0):5 with labels notitle textcolor '#b08080',\
     $data0 using 1:($5 == '0.00%' && $3 == 3 ? 0.012 : 1/0):5 with labels notitle textcolor '#e08080',\
     $data0 using 1:($5 != '0.00%' ? 0.012 : 1/0):5 with labels notitle textcolor '#ffffff'
"

let renderTosses path tosses =

    let n = 3

    let color s =
        s
        |> Seq.filter (fun x -> x = 'H')
        |> Seq.length

    let data0 =
        tosses
        |> Array.rev
        |> Array.mapi (fun i (s, x) -> sprintf "%O %O %O %s %s" i x (color s) s (percent x))
        |> String.concat "\n"

    render path plotTosses [| data0; n |]

//-------------------------------------------------------------------------------------------------

let private plotSurface = baseplotSurface + "
$data0 << EOD
{0}
EOD

style = {1}

stats $data0 using 1:2 matrix nooutput prefix 'data0'
densityX = data0_size_x - 1
densityY = data0_size_y - 1

set xlabel 'Coin Bias (+1)'
set xrange [0:1]
set xtics 0.2
set format x '%0.1f'

set ylabel 'Coin Bias (+2)'
set yrange [0:1]
set ytics 0.2
set format y '%0.1f'

if (style == 1) {{ set zrange [0:0.25] }}
if (style == 2) {{ set zrange [0.25:0] }}
set format z '%4.2f'

set cblabel offset 1 'Cost'
set format cb '%4.2f'

set key top left
set key reverse Left

set pm3d
if (style == 1) {{ set view 30,30,1,1.8 }}
if (style == 2) {{ set view 30,60,1,1.8 }}

set palette rgb 7,5,15

set linetype 1 linewidth 1 linecolor '#9400d3'

splot $data0 using ($1/densityX):($2/densityY):3 matrix with lines title 'Surface Plot'
"

let renderSurface path heatmap style =

    let data0 = matrix heatmap

    render path plotSurface [| data0; style |]

//-------------------------------------------------------------------------------------------------

let private plotHeatmap = baseplotHeatmap + "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

stats $data0 using 1:2 matrix nooutput prefix 'data0'
densityX = data0_size_x - 1
densityY = data0_size_y - 1

set xlabel 'Coin Bias (+1)'
set xrange [0:1]
set xtics 0.2
set format x '%0.2f'

set ylabel 'Coin Bias (+2)'
set yrange [0:1]
set ytics 0.2
set format y '%4.2f'

set cblabel offset 1 'Cost'
set format cb '%4.2f'

set key top left
set key reverse Left
set key textcolor '#ffffff'

set palette rgb 7,5,15

set linetype 1 linewidth 2 linecolor '#00ff00'

plot $data0 using ($1/densityX):($2/densityY):3 matrix with image pixels notitle,\
     $data1 using 1:2 with lines title 'Plateau'
"

let renderHeatmap path heatmap plateau =

    let data0 = matrix heatmap

    let data1 =
        plateau
        |> Array.map (fun (x, y) -> sprintf "%O %O" x y)
        |> String.concat "\n"

    render path plotHeatmap [| data0; data1 |]

//-------------------------------------------------------------------------------------------------

let private plotHeatmapTraces = baseplotHeatmap + "
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

stats $data0 using 1:2 matrix nooutput prefix 'data0'
densityX = data0_size_x - 1
densityY = data0_size_y - 1

set xlabel 'Coin Bias (+1)'
set xrange [0:1]
set xtics 0.2
set format x '%0.2f'

set ylabel 'Coin Bias (+2)'
set yrange [0:1]
set ytics 0.2
set format y '%4.2f'

set cblabel offset 1 'Cost'
set format cb '%4.2f'

set key top left
set key reverse Left
set key textcolor '#ffffff'

set palette rgb 7,5,15

set linetype 1 linewidth 2 linecolor '#00ff00'
set linetype 2 linewidth 2 linecolor '#ffffff'
set linetype 3 pointtype 6 linecolor '#ffffff'
set linetype 4 pointtype 7 linecolor '#ffffff'

plot $data0 using ($1/densityX):($2/densityY):3 matrix with image pixels notitle,\
     $data1 using 1:2 with lines title 'Plateau',\
     $data2 using 1:2 with lines title sprintf('Trace %s', tag),\
     $data3 using 1:2 with points linetype 3 title 'Start',\
     $data4 using 1:2 with points linetype 4 title 'Finish'
"

let renderHeatmapTraces path heatmap plateau trace samples tag =

    let data0 = matrix heatmap

    let data1 =
        plateau
        |> Array.map (fun (x, y) -> sprintf "%O %O" x y)
        |> String.concat "\n"

    let data2 =
        trace
        |> downsample samples
        |> Array.map (fun (x, y) -> sprintf "%O %O" x y)
        |> String.concat "\n"

    let data3 = sprintf "%O %O" <|| (trace |> Array.head)
    let data4 = sprintf "%O %O" <|| (trace |> Array.last)

    render path plotHeatmapTraces [| data0; data1; data2; data3; data4; tag |]

//-------------------------------------------------------------------------------------------------

let private plotHeatmapScores = baseplotHeatmap + "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

$data2 << EOD
{2}
EOD

style = {3}

stats $data0 using 1:2 matrix nooutput prefix 'data0'
densityX = data0_size_x - 1
densityY = data0_size_y - 1

stats $data1 using 1:2 nooutput prefix 'data1'
n = data1_records - 1

set xlabel 'Coin Bias (+1)'
set xrange [0:1]
set xtics 0.2
set format x '%0.2f'

set ylabel 'Coin Bias (+2)'
set yrange [0:1]
set ytics 0.2
set format y '%4.2f'

if (style == 1) {{ set cblabel offset 1 'Score' }}
if (style == 2) {{ set cblabel offset 1 'Cost' }}
set format cb '%4.2f'

set key top left
set key reverse Left
set key textcolor '#ffffff'

if (style == 1) {{ set palette defined (0 '#000000', 1 '#c0c0c0') }}
if (style == 2) {{ set palette rgb 7,5,15 }}

set linetype 1 linewidth 2 linecolor '#00ff00'
set linetype 2 pointtype 7 linecolor '#ffffff'

green(x) = (int((1 - x) * 240) << 24) + (int(255) << 8)

plot $data0 using ($1/densityX):($2/densityY):3 matrix with image pixels notitle,\
     for [i=0:0] $data1 using 1:2:(mag=$3) every ::i-0::i+0 with lines linecolor rgb green(1.0) linewidth 2 title 'Plateau',\
     for [i=0:n] $data1 using 1:2:(mag=$3) every ::i-1::i+1 with lines linecolor rgb green(mag) linewidth 1.5 + mag notitle,\
     $data2 using 1:2 with points linetype 2 title 'Optimum'
"

let renderHeatmapScores path heatmap scores (p1, p2, score) style =

    let min = scores |> Array.map (fun (p1, p2, s) -> s) |> Array.min
    let max = scores |> Array.map (fun (p1, p2, s) -> s) |> Array.max
    let mag (x : float) = (max - x) / (max - min)

    let data0 = matrix heatmap

    let data1 =
        scores
        |> Array.map (fun (p1, p2, s) -> sprintf "%O %O %O" p1 p2 (mag s))
        |> String.concat "\n"

    let data2 = sprintf "%O %O" p1 p2

    render path plotHeatmapScores [| data0; data1; data2; style |]

//-------------------------------------------------------------------------------------------------

let private plotScores = baseplotRegular + "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

lower = {2}; upper = {3}; p1 = {4}; tag = '{5}'

set xlabel 'Coin Bias (+1)'
set xrange [lower:upper]
set xtics (lower, upper, p1)
set format x '%0.4f'

set ylabel 'Score'
set yrange [-0.05:0.8]
set format y '%4.2f'

set key top left
set key reverse Left

set linetype 1 linewidth 1 linecolor '#ff0000'
set linetype 2 pointtype 2 linecolor '#000000'

plot $data0 using 1:2 with lines title sprintf('Score %s', tag),\
     $data1 using 1:2:3 with labels offset 0,1 point linetype 2 title 'Optimum'
"

let renderScores path scores (p1, p2, score) tag =

    let xvals = scores |> Array.map (fun (p1, p2, s) -> p1)
    let lower = Array.min xvals
    let upper = Array.max xvals

    let data0 =
        scores
        |> Array.map (fun (p1, p2, s) -> sprintf "%O %O" p1 s)
        |> String.concat "\n"

    let data1 = sprintf "%O %O %0.8f" p1 score score

    render path plotScores [| data0; data1; lower; upper; p1; tag |]
