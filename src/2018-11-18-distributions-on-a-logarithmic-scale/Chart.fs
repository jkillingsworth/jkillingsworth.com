﻿module Chart

open Common.Chart

//-------------------------------------------------------------------------------------------------

let private plotDistributions = baseplotRegular + "
$data0 << EOD
{0}
EOD

style = {1}

if (style == 1) {{
    set xlabel 'x'
    set xrange [0:5]
    set xtics 1
}}

if (style == 2) {{
    set xlabel 'x'
    set xrange [0.01:100]
    set xtics 0.1
    set logscale x 10
}}

set ylabel 'Probability Density'
set yrange [0:0.75]
set ytics 0.05
set format y '%4.2f'

set key top right
set key reverse Left

set linetype 1 linewidth 2 linecolor rgb (richRojo + 0x40000000)
set linetype 2 linewidth 2 linecolor rgb (richBlue + 0x40000000)

plot $data0 using 1:2 with lines title 'Log-Normal',\
     $data0 using 1:3 with lines title 'Log-Laplace'
"

let private renderDistributions style path items =

    let data0 =
        items
        |> Array.map (fun (x, n, l) -> sprintf "%O %O %O" x n l)
        |> String.concat "\n"

    render path plotDistributions [| data0; style |]

let renderDistributionsLin path items = renderDistributions 1 path items
let renderDistributionsLog path items = renderDistributions 2 path items
