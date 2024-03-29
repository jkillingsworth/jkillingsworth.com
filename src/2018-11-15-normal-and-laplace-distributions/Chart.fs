﻿module Chart

open Common.Chart

//-------------------------------------------------------------------------------------------------

let private plotLikelihood = baseplotRegular + "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

lower = {2}; upper = {3}; n = {4}

stats $data1 using 1:2 nooutput prefix 'data1'
yMax = data1_max_y

set xlabel 'Location (μ)'

set ylabel 'Likelihood'
set yrange [lower:upper]
set ytics (' Max' yMax)

set key top left
set key reverse Left

set linetype 1 linewidth 2 linecolor rgb baseBlue
set linetype 2 pointtype 7 linecolor rgb baseBlue

etitle = 'Likelihood (n is even)'
otitle = 'Likelihood (n is odd) '
if ((n % 2) == 0) {{ eotitle = etitle }} else {{ eotitle = otitle }}

plot $data0 using 1:2 with lines title eotitle,\
     $data1 using 1:2:xtic(3) with points notitle
"

let renderLikelihood path (lower, upper) items =

    let n = items |> Array.filter (fun (x, y, p) -> p) |> Array.length
    let m = (n / 2) + (n % 2)

    let xtic i =
        let offset = i - m + 1
        if (offset = 0) then "x_m" else sprintf "x_{{m%+i}}" offset

    let data0 =
        items
        |> Array.map (fun (x, y, p) -> sprintf "%O %O" x y)
        |> String.concat "\n"

    let data1 =
        items
        |> Array.filter (fun (x, y, p) -> p)
        |> Array.mapi (fun i (x, y, p) -> sprintf "%O %O %s" x y (xtic i))
        |> String.concat "\n"

    render path plotLikelihood [| data0; data1; lower; upper; n |]

//-------------------------------------------------------------------------------------------------

let private plotDistributions = baseplotRegular + "
$data0 << EOD
{0}
EOD

style = {1}

set xlabel 'x'
set xtics 1
set xtics add ('0' 0)
set format x '%+0.0f'

if (style == 1) {{
    set ylabel 'Probability Density'
    set yrange [0:0.55]
    set ytics 0.05
    set format y '%4.2f'
}}

if (style == 2) {{
    set ylabel 'Probability Density'
    set yrange [0.00001:1]
    set format y '10^{{%+T}}'
    set logscale y 10
}}

set key top left
set key reverse Left

set linetype 1 linewidth 2 linecolor rgb (richRojo + 0x40000000)
set linetype 2 linewidth 2 linecolor rgb (richBlue + 0x40000000)

plot $data0 using 1:2 with lines title 'Normal',\
     $data0 using 1:3 with lines title 'Laplace'
"

let private renderDistributions style path items =

    let data0 =
        items
        |> Array.map (fun (x, n, l) -> sprintf "%O %O %O" x n l)
        |> String.concat "\n"

    render path plotDistributions [| data0; style |]

let renderDistributionsLin path items = renderDistributions 1 path items
let renderDistributionsLog path items = renderDistributions 2 path items
