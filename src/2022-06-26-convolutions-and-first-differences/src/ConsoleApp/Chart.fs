module Chart

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

stats $data0 using 1 nooutput prefix 'data0'
xmax = data0_max

set arrow nohead linestyle 2 from (0-0.2),0 to (xmax+0.2),0

set xlabel 'Time'
set xrange [0-0.2:xmax+0.2]
set xtics 1

set ylabel 'Value'
set yrange [-5:+5]
set ytics 1
set ytics add ('0' 0)
set format y '%+4.0f'

set key top left
set key reverse Left

if (style == 1) {{ set linetype 1 linecolor '#4080ff' }}
if (style == 2) {{ set linetype 1 linecolor '#ff4040' }}
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

stats $data0 using 1 nooutput prefix 'data0'
xmin = data0_min
xmax = data0_max

set xlabel 'Possible Outcome'
set xrange [xmin-1:xmax+1]
set xtics xmin, 1, xmax
set xtics add ('0' 0)
set format x '%+0.0f'

set ylabel 'Probability'
set yrange [0:0.5]
set ytics 0.05
set format y '%4.2f'

set key top left
set key reverse Left

if (style == 1) {{
    set linetype 1 linewidth 1 linecolor '#a0c0ff'
    colorPercent = '#2060e0'
}}

if (style == 2) {{
    set linetype 1 linewidth 1 linecolor '#ffa0a0'
    colorPercent = '#e02020'
}}

plot $data0 using 1:2 with boxes fill solid border linecolor '#ffffff' title 'Probability Mass',\
     $data0 using 1:(0.02):3 with labels notitle textcolor rgb colorPercent
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

set ylabel 'Probability'
set yrange [yLower:yUpper]
set ytics 0.05
set format y '%4.2f'

set key top left
set key reverse Left

if (style == 1) {{
    set linetype 1 linewidth 2 linecolor '#4080ff'
    set linetype 2 linewidth 2 linecolor '#4080ff'
}}

if (style == 2) {{
    set linetype 1 linewidth 2 linecolor '#ff4040'
    set linetype 2 linewidth 2 linecolor '#ff4040'
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
