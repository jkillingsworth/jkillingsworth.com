﻿module Chart

open System
open Common.Chart

//-------------------------------------------------------------------------------------------------

let private percent x =
    if Double.IsNaN(x) then "" else x.ToString("0%;-0%;0%")

//-------------------------------------------------------------------------------------------------

let private plotSeries = baseplotRegular + "
$data0 << EOD
{0}
EOD

style = {1}

stats $data0 using 1:2 nooutput prefix 'data0'
xMax = data0_max_x

set xlabel 'Time'
set xrange [0-0.2:xMax+0.2]
set xtics 1
set xzeroaxis linestyle 2

set ylabel 'Value'
set yrange [-5:+5]
set ytics 1
set ytics add ('0' 0)
set format y '%+4.0f'

set key top left
set key reverse Left

if (style == 1) {{ set linetype 1 linecolor rgb baseBlue }}
if (style == 2) {{ set linetype 1 linecolor rgb baseRojo }}
set linetype 1 linewidth 1 pointtype 7 pointinterval -1
set pointintervalbox 1.25

plot $data0 using 1:2 with linespoints title 'Time Series'
"

let renderSeries path values style =

    let data0 =
        values
        |> Array.map (fun (t, v) -> sprintf "%O %O" t v)
        |> String.concat "\n"

    render path plotSeries [| data0; style |]

//-------------------------------------------------------------------------------------------------

let private plotPmfunc = baseplotRegular + "
$data0 << EOD
{0}
EOD

style = {1}

stats $data0 using 1:2 nooutput prefix 'data0'
xMin = data0_min_x
xMax = data0_max_x

set xlabel 'Possible Outcome'
set xrange [xMin-1:xMax+1]
set xtics xMin, 1, xMax
set xtics add ('0' 0)
set format x '%+0.0f'

set ylabel 'Probability'
set yrange [0:0.5]
set ytics 0.05
set format y '%4.2f'

set key top left
set key reverse Left

if (style == 1) {{
    set linetype 1 linewidth 1 linecolor rgb liteBlue
    darkText = darkBlue
}}

if (style == 2) {{
    set linetype 1 linewidth 1 linecolor rgb liteRojo
    darkText = darkRojo
}}

set style fill solid border linecolor rgb parWhite

plot $data0 using 1:2 with boxes title 'Probability Mass',\
     $data0 using 1:(0.02):3 with labels notitle textcolor rgb darkText
"

let renderPmfunc path pmfunc style =

    let data0 =
        pmfunc
        |> Array.map (fun (x, p) -> sprintf "%O %O %s" x p (percent p))
        |> String.concat "\n"

    render path plotPmfunc [| data0; style |]

//-------------------------------------------------------------------------------------------------

let private plotPdfunc = baseplotRegular + "
$data0 << EOD
{0}
EOD

style = {1}; xLower = {2}; xUpper = {3}; yLower = {4}; yUpper = {5}

set xlabel 'Possible Outcome'
set xrange [xLower:xUpper]
set xtics 1
set xtics add ('0' 0)
set format x '%+0.0f'

set ylabel 'Probability Density'
set yrange [yLower:yUpper]
set ytics 0.05
set format y '%4.2f'

set key top left
set key reverse Left

if (style == 1) {{
    set linetype 1 linewidth 2 linecolor rgb baseBlue
    set linetype 2 linewidth 2 linecolor rgb baseBlue
}}

if (style == 2) {{
    set linetype 1 linewidth 2 linecolor rgb baseRojo
    set linetype 2 linewidth 2 linecolor rgb baseRojo
}}

plot $data0 using 1:2 with lines title 'Probability Density',\
     $data0 using 1:2 with filledcurves y=0 fill solid 0.125 noborder notitle
"

let renderPdfunc path pRange ps style =

    let data0 =
        ps
        |> Array.map (fun (x, p) -> sprintf "%O %O" x p)
        |> String.concat "\n"

    let xLower = ps |> Array.map fst |> Array.min
    let xUpper = ps |> Array.map fst |> Array.max
    let yLower = fst pRange
    let yUpper = snd pRange

    render path plotPdfunc [| data0; style; xLower; xUpper; yLower; yUpper |]
