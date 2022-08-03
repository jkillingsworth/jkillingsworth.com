module Chart

open Common.Chart

//-------------------------------------------------------------------------------------------------

let private plotTimeXs = baseplotRegular + "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

set xlabel 'Time (Seconds)'
set xrange [-0.01:+1]
set xtics 0.25
set xzeroaxis linestyle 2
set mxtics 2
set format x '%0.2f'

set ylabel 'Value'
set yrange [-2:+2]
set ytics add ('0.0' 0)
set format y '%+4.1f'

set key top left
set key reverse Left
set key title 'Time Domain'

set linetype 1 linewidth 1 linecolor '#a0a0a0'
set linetype 2 linewidth 2 linecolor rgb baseRojo
set linetype 3 pointtype 7 linecolor rgb baseRojo

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

let private plotFreqYs = baseplotRegular + "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

n = {2}; style = '{3}'

set xlabel 'Frequency (Hz)'
set xrange [-(0.01*n):+n]
set xtics 0, 1, n
set xzeroaxis linestyle 2

if (style eq 're') {{
    set ylabel 'Real'
    set yrange [-1.25:+1.25]
    set ytics 0.5
    set ytics add ('0.0' 0)
    set mytics 2
    set format y '%+4.1f'
}}

if (style eq 'im') {{
    set ylabel 'Imaginary'
    set yrange [-1.25:+1.25]
    set ytics 0.5
    set ytics add ('0.0' 0)
    set mytics 2
    set format y '%+4.1f'
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

set key top left
set key reverse Left
set key title 'Frequency Domain'

set linetype 1 linewidth 1 linecolor '#a0a0a0'

if (style eq 're') {{
    set linetype 2 linewidth 2 linecolor rgb baseBlue
    set linetype 3 pointtype 7 linecolor rgb baseBlue
}}

if (style eq 'im') {{
    set linetype 2 linewidth 2 linecolor rgb baseGold
    set linetype 3 pointtype 7 linecolor rgb baseGold
}}

if (style eq 'ma') {{
    set linetype 2 linewidth 2 linecolor rgb basePurp
    set linetype 3 pointtype 7 linecolor rgb basePurp
}}

if (style eq 'ph') {{
    set linetype 2 linewidth 2 linecolor rgb baseMint
    set linetype 3 pointtype 7 linecolor rgb baseMint
}}

plot $data0 using 1:2 with lines notitle,\
     $data1 using 1:2 with impulses notitle,\
     $data1 using 1:2 with points notitle
"

let renderFreqYs path unitYs freqYs mapping style =

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
