module Chart

open Common.Chart

//-------------------------------------------------------------------------------------------------

let private plotPrice = baseplotRegular + "
$data0 << EOD
{0}
EOD

lower = {1}; upper = {2}; step = {3}; ticker = '{4}'; style = {5}

set xlabel 'Time (Days)'
if (style == 1) {{ set xrange [0:2000] }}
if (style == 2) {{ set xrange [1800:2000] }}
set xtics 200

set ylabel 'Price per Share'
set yrange [lower:upper]
set ytics lower, step
set format y '%3.0f'

set key top left
set key reverse Left
if (style == 1) {{ set key title sprintf('%s (Full)', ticker) left }}
if (style == 2) {{ set key title sprintf('%s (Zoom)', ticker) left }}

if (style == 1) {{
    set linetype 1 linewidth 1 linecolor '#00808080'
    set linetype 2 linewidth 1 linecolor '#00ff0000'
    set linetype 3 linewidth 1 linecolor '#800000ff'
}}

if (style == 2) {{
    set linetype 1 linewidth 1 linecolor '#00808080'
    set linetype 2 linewidth 1 linecolor '#80ff0000'
    set linetype 3 linewidth 1 linecolor '#000000ff'
}}

plot $data0 using 1:2 with lines title 'Market Price',\
     $data0 using 1:3 with lines title 'Moving Average',\
     $data0 using 1:4 with lines title 'Fitted Line'
"

let private renderPrice style path axis ticker items =

    let lower, upper, step = axis

    let option x = Option.defaultValue nan x
    let format i (market, moving, fitted) =
        sprintf "%O %O %O %O" i market (option moving) (option fitted)

    let data0 =
        items
        |> Array.mapi format
        |> String.concat "\n"

    render path plotPrice [| data0; lower; upper; step; ticker; style |]

let renderPriceFull path axis ticker items = renderPrice 1 path axis ticker items
let renderPriceZoom path axis ticker items = renderPrice 2 path axis ticker items
