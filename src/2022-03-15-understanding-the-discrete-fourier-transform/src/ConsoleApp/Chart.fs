module Chart

open Common.Chart

//-------------------------------------------------------------------------------------------------

let private plotTimeXs = "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics
set xzeroaxis linestyle 1 linewidth 2 linecolor '#e6e6e6'

set xlabel 'Time (Seconds)'
set xrange [-0.01:+1]
set xtics 0.25
set mxtics 2
set format x '%0.2f'

set ylabel 'Value'
set yrange [-2:+2]
set format y '%4.1f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key title 'Time Domain'

set linetype 1 linewidth 1 linecolor '#a0a0a0'
set linetype 2 linewidth 2 linecolor '#ff4040'
set linetype 3 pointtype 7 linecolor '#ff4040'

plot $data0 using 1:2 with lines notitle,\
     $data1 using 1:2 with impulses notitle,\
     $data1 using 1:2 with points notitle
"

let renderTimeXs path unitXs timeXs =

    let data0 =
        unitXs
        |> Array.map (fun x -> sprintf "%O %O" (fst x) (snd x))
        |> String.concat "\n"

    let data1 =
        timeXs
        |> Array.map (fun x -> sprintf "%O %O" (fst x) (snd x))
        |> String.concat "\n"

    render path plotTimeXs [| data0; data1 |]

//-------------------------------------------------------------------------------------------------

let private plotFreqYs = "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

n = {2}; style = '{3}'

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics
set xzeroaxis linestyle 1 linewidth 2 linecolor '#e6e6e6'

set xlabel 'Frequency (Hz)'
set xrange [-(0.01*n):+n]
set xtics 0, 1, n

if (style eq 're') {{
    set ylabel 'Real'
    set yrange [-1.25:+1.25]
    set ytics 0.5
    set mytics 2
    set format y '%4.1f'
}}

if (style eq 'im') {{
    set ylabel 'Imaginary'
    set yrange [-1.25:+1.25]
    set ytics 0.5
    set mytics 2
    set format y '%4.1f'
}}

if (style eq 'ma') {{
    set ylabel 'Magnitude'
    set yrange [-0.25:+2.25]
    set ytics 0.5
    set mytics 2
    set format y '%4.1f'
}}

if (style eq 'ph') {{
    set ylabel 'Phase'
    set yrange [-(pi * 1.25):+(pi * 1.25)]
    set ytics -pi, pi, +pi
    set ytics add ('-π' -pi, 0, '+π' +pi)
    set mytics 4
    set format y '%4.0f'
}}

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key title 'Frequency Domain'

set linetype 1 linewidth 1 linecolor '#a0a0a0'

if (style eq 're') {{
    set linetype 2 linewidth 2 linecolor '#4080ff'
    set linetype 3 pointtype 7 linecolor '#4080ff'
}}

if (style eq 'im') {{
    set linetype 2 linewidth 2 linecolor '#ffc040'
    set linetype 3 pointtype 7 linecolor '#ffc040'
}}

if (style eq 'ma') {{
    set linetype 2 linewidth 2 linecolor '#8040c0'
    set linetype 3 pointtype 7 linecolor '#8040c0'
}}

if (style eq 'ph') {{
    set linetype 2 linewidth 2 linecolor '#40c040'
    set linetype 3 pointtype 7 linecolor '#40c040'
}}

plot $data0 using 1:2 with lines notitle,\
     $data1 using 1:2 with impulses notitle,\
     $data1 using 1:2 with points notitle
"

let renderFreqYs path unitYs freqYs mapping (style : string) =

    let n = freqYs |> Array.length

    let data0 =
        unitYs
        |> Array.map (fun x -> sprintf "%O %O" (fst x) (mapping (snd x)))
        |> String.concat "\n"

    let data1 =
        freqYs
        |> Array.map (fun x -> sprintf "%O %O" (fst x) (mapping (snd x)))
        |> String.concat "\n"

    render path plotFreqYs [| data0; data1; n; style |]
