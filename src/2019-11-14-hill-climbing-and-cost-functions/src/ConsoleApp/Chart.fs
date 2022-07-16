module Chart

open System
open Common.Chart

//-------------------------------------------------------------------------------------------------

let private percent x =
    if Double.IsNaN(x) then "" else x.ToString("0.00%;-0.00%;0.00%")

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

    let n = 4

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

    let n = 4

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
set yrange [0:0.20]
set format y '%4.2f'

set key top left
set key reverse Left

set linetype 1 linewidth 1 linecolor '#80b0e0'
set linetype 2 linewidth 1 linecolor '#8098b0'
set linetype 3 linewidth 1 linecolor '#808080'
set linetype 4 linewidth 1 linecolor '#b08080'
set linetype 5 linewidth 1 linecolor '#e08080'
set style fill solid border linecolor '#ffffff'

plot $data0 using 1:($3 == 0 ? $2 : 0):xtic(4) with boxes title '0 Heads, 4 Tails',\
     $data0 using 1:($3 == 1 ? $2 : 0):xtic(4) with boxes title '1 Heads, 3 Tails',\
     $data0 using 1:($3 == 2 ? $2 : 0):xtic(4) with boxes title '2 Heads, 2 Tails',\
     $data0 using 1:($3 == 3 ? $2 : 0):xtic(4) with boxes title '3 Heads, 1 Tails',\
     $data0 using 1:($3 == 4 ? $2 : 0):xtic(4) with boxes title '4 Heads, 0 Tails',\
     $data0 using 1:($5 == '0.00%' && $3 == 0 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor '#80b0e0',\
     $data0 using 1:($5 == '0.00%' && $3 == 1 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor '#8098b0',\
     $data0 using 1:($5 == '0.00%' && $3 == 2 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor '#808080',\
     $data0 using 1:($5 == '0.00%' && $3 == 3 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor '#b08080',\
     $data0 using 1:($5 == '0.00%' && $3 == 4 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor '#e08080',\
     $data0 using 1:($5 != '0.00%' ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor '#ffffff'
"

let renderTosses path tosses =

    let n = 4

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

let private plotClimb = baseplotRegular + "
$hilleast << EOD
3.0 0.375 Start
2.0 2.375 Finish
EOD

$hillwest << EOD
1.0 0.375 Start
2.0 2.375 Finish
EOD

$plateast << EOD
3.0 0.375 Start
2.5 1.375 Finish
EOD

$platwest << EOD
1.0 0.375 Start
1.5 1.375 Finish
EOD

landform = {0}; eastwest = {1}

unset grid

set xlabel 'Location'
set xrange [0.5:3.5]
set xtics ('West' 0.8, 'East' 3.2)

set ylabel 'Elevation'
set yrange [0:2.75]
if (landform == 1) {{ set ytics ('Base' 0.25 0, 'Peak' 2.25) }}
if (landform == 2) {{ set ytics ('Base' 0.25 0, 'Peak' 1.25) }}
set ytics scale 1

unset key

set linetype 1 linewidth 0 linecolor '#ff000000'
set linetype 2 linewidth 0 linecolor rgb highGray
set linetype 3 linewidth 2 linecolor rgb baseGray
set linetype 4 linewidth 2 linecolor rgb baseRojo dashtype 2
set linetype 5 pointtype 7 linecolor rgb darkGray

set samples 150

hill(x) = (x > 1.0 && x < 3.0) ? 1 + cos(pi * x) : 0
plat(x) = (x > 1.5 && x < 2.5) ? 1 : hill(x)

if (landform == 1) {{ land(x) = hill(x) }}
if (landform == 2) {{ land(x) = plat(x) }}

landprofile(x) = land(x) + 0.25 + ((rand(0) - 0.5) * 0.04)
pathprofile(x) = land(x) + 0.25 + 0.125

seed = -1

if (landform == 1 && eastwest == 1) {{
    plot rand(seed) linetype 1 notitle, landprofile(x) linetype 2 notitle with filledcurves y=0,\
         rand(seed) linetype 1 notitle, landprofile(x) linetype 3 notitle,\
         [3:2.0] pathprofile(x) linetype 4 notitle,\
         $hilleast using 1:2:3 with labels notitle point linetype 5 offset 0,1
}}

if (landform == 1 && eastwest == 2) {{
    plot rand(seed) linetype 1 notitle, landprofile(x) linetype 2 notitle with filledcurves y=0,\
         rand(seed) linetype 1 notitle, landprofile(x) linetype 3 notitle,\
         [1:2.0] pathprofile(x) linetype 4 notitle,\
         $hillwest using 1:2:3 with labels notitle point linetype 5 offset 0,1
}}

if (landform == 2 && eastwest == 1) {{
    plot rand(seed) linetype 1 notitle, landprofile(x) linetype 2 notitle with filledcurves y=0,\
         rand(seed) linetype 1 notitle, landprofile(x) linetype 3 notitle,\
         [3:2.5] pathprofile(x) linetype 4 notitle,\
         $plateast using 1:2:3 with labels notitle point linetype 5 offset 0,1
}}

if (landform == 2 && eastwest == 2) {{
    plot rand(seed) linetype 1 notitle, landprofile(x) linetype 2 notitle with filledcurves y=0,\
         rand(seed) linetype 1 notitle, landprofile(x) linetype 3 notitle,\
         [1:1.5] pathprofile(x) linetype 4 notitle,\
         $platwest using 1:2:3 with labels notitle point linetype 5 offset 0,1
}}
"

let private renderClimb landform eastwest path =

    render path plotClimb [| landform; eastwest |]

let renderClimbHillEast path = renderClimb 1 1 path
let renderClimbHillWest path = renderClimb 1 2 path
let renderClimbPlatEast path = renderClimb 2 1 path
let renderClimbPlatWest path = renderClimb 2 2 path

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
set yrange [0:0.8]
set format y '%4.2f'

set key top left
set key reverse Left

set linetype 1 linewidth 1 linecolor rgb richRojo
set linetype 2 pointtype 2 linecolor '#000000'

plot $data0 using 1:2 with lines title sprintf('Score %s', tag),\
     $data1 using 1:2:3 with labels offset 0,1 point linetype 2 title 'Optimum'
"

let renderScores path scores (p1, score) (lower, upper) tag =

    let data0 =
        scores
        |> Array.map (fun (p1, s) -> sprintf "%O %O" p1 s)
        |> String.concat "\n"

    let data1 = sprintf "%O %O %0.8f" p1 score score

    render path plotScores [| data0; data1; lower; upper; p1; tag |]
