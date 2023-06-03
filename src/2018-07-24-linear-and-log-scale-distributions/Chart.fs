module Chart

open System
open Common.Chart

//-------------------------------------------------------------------------------------------------

let private plotDistribution = baseplotRegular + "
$data0 << EOD
{0}
EOD

style = {1}

if (style == 1) {{
    set xlabel 'Possible Outcome'
    set xrange [-900:1100]
    set xtics -900, 200
    set mxtics 2
}}

if (style == 2) {{
    set xlabel 'Possible Outcome'
    set xrange [0.001:10000000]
    set xtics 0.001, 10
    set xtics add (0.001, 0.01, 0.1, 1, 10, 100, '1k' 1000, '10k' 10000, '100k' 100000, '1M' 1000000, '10M' 10000000)
    set logscale x 10
}}

set ylabel 'Probability'
set format y '%4.2f'

set key top right
set key noreverse Right

set linetype 1 linewidth 6 linecolor rgb baseMint
set linetype 2 linewidth 6 linecolor rgb baseRojo
set linetype 3 linewidth 6 linecolor rgb baseGray

plot $data0 using ($3 == 1 ? $1 : 1/0):2 with impulses title 'Profit',\
     $data0 using ($3 == 2 ? $1 : 1/0):2 with impulses title 'Loss',\
     $data0 using ($3 == 3 ? $1 : 1/0):2 with impulses title 'Breakeven'
"

let private renderDistribution style path items =

    let round (x : float) = Math.Round(x, 3)

    let color = function
        | x when round (fst x) > 100.0 -> 1
        | x when round (fst x) < 100.0 -> 2
        | _ -> 3

    let data0 =
        items
        |> Array.map (fun x -> sprintf "%O %O %O" (fst x) (snd x) (color x))
        |> String.concat "\n"

    render path plotDistribution [| data0; style |]

let renderDistributionLin path items = renderDistribution 1 path items
let renderDistributionLog path items = renderDistribution 2 path items
