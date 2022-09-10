﻿module Chart

open Common.Chart

//-------------------------------------------------------------------------------------------------

let private plotModel = baseplotRegular + "
$data0 << EOD
{0}
EOD

δ = {1}; α = {2}; λ = {3}

set xlabel 'Possible Outcome'
set xtics add ('0' 0)
set format x '%+0.0f'

set ylabel 'Probability Density'
set yrange [0:0.6]
set format y '%4.1f'

set key top left
set key reverse Left

set linetype 1 linewidth 2 linecolor rgb baseGold
set linetype 2 linewidth 2 linecolor rgb baseGold

plot $data0 using 1:2 with lines title sprintf('PDF (δ = %4.2f, α = %4.2f, λ = %+4.2f)', δ, α, λ),\
     $data0 using 1:2 with filledcurves y=0 fill solid 0.125 noborder notitle
"

let renderModel path pdf δ α λ =

    let data0 =
        pdf
        |> Array.map (fun (x, y) -> sprintf "%O %O" x y)
        |> String.concat "\n"

    render path plotModel [| data0; δ; α; λ |]

//-------------------------------------------------------------------------------------------------

let private plotFitted = baseplotRegular + "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

sigmas = {2}; σN = {3}; high = {4}; style = '{5}'

set xlabel gprintf('Price Differences (Log Values), σ = %0.3te%04T', σN)
set xrange [-(sigmas * σN):+(sigmas * σN)]
set xtics (0)
set for [i=+1:+sigmas:+1] xtics add (sprintf('%+iσ', i) i * σN)
set for [i=-1:-sigmas:-1] xtics add (sprintf('%+iσ', i) i * σN)

set ylabel 'Probability Density'
set yrange [0:high]
set ytics 0, (high / 8.0)
set format y '%4.1f'

set key top left
set key reverse Left

set linetype 1 linewidth 1 linecolor rgb highGray
if (style eq 'N') {{ set linetype 2 linewidth 2 linecolor rgb baseRojo }}
if (style eq 'I') {{ set linetype 2 linewidth 2 linecolor rgb baseBlue }}
if (style eq 'H') {{ set linetype 2 linewidth 2 linecolor rgb baseMint }}
set style fill solid border linecolor rgb liteGray

plot $data0 using 1:2 with boxes title 'Histogram',\
     $data1 using 1:2 with lines title sprintf('PDF (%s)', style)
"

let renderFitted path histogram pdf sigmas σN high style =

    let data0 =
        histogram
        |> Array.map (fun (center, amount) -> sprintf "%O %O" center amount)
        |> String.concat "\n"

    let data1 =
        pdf
        |> Array.map (fun (x, y) -> sprintf "%O %O" x y)
        |> String.concat "\n"

    render path plotFitted [| data0; data1; sigmas; σN; high; style |]
