module Chart

open Common.Chart

//-------------------------------------------------------------------------------------------------

let private plotSimulations = baseplotRegular + "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

$data2 << EOD
{2}
EOD

set xlabel 'Number of Plays'

set ylabel 'Dollars'

set key top left
set key reverse Left

set linetype 1 linewidth 2 linecolor rgb baseRojo
set linetype 2 linewidth 2 linecolor rgb baseMint
set linetype 3 linewidth 2 linecolor rgb baseBlue

plot $data0 using 1:2 with lines title 'Simulation A',\
     $data1 using 1:2 with lines title 'Simulation B',\
     $data2 using 1:2 with lines title 'Simulation C'
"

let renderSimulations path vsA vsB vsC =

    let data0 =
        vsA
        |> Array.mapi (fun i x -> sprintf "%O %O" i x)
        |> String.concat "\n"

    let data1 =
        vsB
        |> Array.mapi (fun i x -> sprintf "%O %O" i x)
        |> String.concat "\n"

    let data2 =
        vsC
        |> Array.mapi (fun i x -> sprintf "%O %O" i x)
        |> String.concat "\n"

    render path plotSimulations [| data0; data1; data2 |]

//-------------------------------------------------------------------------------------------------

let private plotGs = baseplotRegular + "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

xLower = {2}; xUpper = {3}; yLower = {4}; yUpper = {5}

set xlabel 'Bet Size'
set xrange [xLower:xUpper]
set xtics add ('0.0' 0)
set xzeroaxis linestyle 2
set format x '%+0.1f'

set ylabel 'Growth Rate (Log Values)'
set yrange [yLower:yUpper]
set ytics add ('0.00' 0)
set yzeroaxis linestyle 2
set format y '%+5.2f'

set key top left
set key reverse Left

set linetype 1 linewidth 2 linecolor rgb basePurp
set linetype 2 pointtype 7 linecolor rgb basePurp

plot $data0 using 1:2 with lines title 'EV',\
     $data1 using 1:2 with points title 'Maximum'
"

let renderGs path gRange bs gs (b', g')=

    let data0 =
        gs
        |> Array.zip bs
        |> Array.map (fun (b, g) -> sprintf "%O %O" b g)
        |> String.concat "\n"

    let data1 = sprintf "%O %O" b' g'

    let xLower = bs |> Array.min
    let xUpper = bs |> Array.max
    let yLower = fst gRange
    let yUpper = snd gRange

    render path plotGs [| data0; data1; xLower; xUpper; yLower; yUpper |]

//-------------------------------------------------------------------------------------------------

let private plotPs = baseplotRegular + "
$data0 << EOD
{0}
EOD

xLower = {1}; xUpper = {2}; yLower = {3}; yUpper = {4}

set xlabel 'Price'
set xrange [xLower:xUpper]
set xtics 1

set ylabel 'Probability Density'
set yrange [yLower:yUpper]
set ytics 0.025
set format y '%5.3f'

set key top left
set key reverse Left

set linetype 1 linewidth 2 linecolor rgb baseBlue
set linetype 2 linewidth 2 linecolor rgb baseBlue

plot $data0 using 1:2 with lines title 'Log-Normal PDF',\
     $data0 using 1:2 with filledcurves y=0 fill solid 0.125 noborder notitle
"

let renderPs path pRange xs ps =

    let data0 =
        ps
        |> Array.zip xs
        |> Array.map (fun (x, p) -> sprintf "%O %O" x p)
        |> String.concat "\n"

    let xLower = xs |> Array.min
    let xUpper = xs |> Array.max
    let yLower = fst pRange
    let yUpper = snd pRange

    render path plotPs [| data0; xLower; xUpper; yLower; yUpper |]

//-------------------------------------------------------------------------------------------------

let private plotWs = baseplotRegular + "
$data0 << EOD
{0}
EOD

xLower = {1}; xUpper = {2}; yLower = {3}; yUpper = {4}

set xlabel 'Price'
set xrange [xLower:xUpper]
set xtics 1
set xzeroaxis linestyle 2

set ylabel 'Payoff'
set yrange [yLower:yUpper]
set ytics 0.05
set ytics add ('0.00' 0)
set mytics 2
set format y '%+5.2f'

set key top left
set key reverse Left

set linetype 1 linewidth 2 linecolor rgb baseMint
set linetype 2 linewidth 2 linecolor rgb baseRojo

plot $data0 using 1:($2 >= 0 ? $2 : 1/0) with lines title 'Positive',\
     $data0 using 1:($2 <= 0 ? $2 : 1/0) with lines title 'Negative'
"

let renderWs path wRange xs ws =

    let data0 =
        ws
        |> Array.zip xs
        |> Array.map (fun (x, w) -> sprintf "%O %O" x w)
        |> String.concat "\n"

    let xLower = xs |> Array.min
    let xUpper = xs |> Array.max
    let yLower = fst wRange
    let yUpper = snd wRange

    render path plotWs [| data0; xLower; xUpper; yLower; yUpper |]

//-------------------------------------------------------------------------------------------------

let private plotLimitations = baseplotRegular + "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

xLower = {2}; xUpper = {3}; c = {4}

stats $data0 using 1:3 nooutput prefix 'data0'
bMin = data0_min_x

stats $data1 using 1:3 nooutput prefix 'data1'
bMax = data1_max_x

set xlabel 'Bet Size'
set xrange [xLower:xUpper]
set xtics 1
set xtics add ('0.0' 0)
set format x '%+0.1f'

set ylabel 'Price'
set yrange [0:2*c]
set ytics 5
set yzeroaxis linestyle 2
set format y '%5.0f'

set key top left
set key reverse Left

set linetype 1 linewidth 2 linecolor rgb liteGray
set linetype 2 linewidth 2 linecolor rgb baseMint
set linetype 3 linewidth 2 linecolor rgb baseRojo

plot $data0 using ($1 <= 0 ? $1 : 1/0):2 linetype 1 with lines notitle,\
     $data1 using ($1 >= 1 ? $1 : 1/0):2 linetype 1 with lines notitle,\
     $data0 using ($1 <= 0 ? $1 : 1/0):3 linetype 2 with lines notitle,\
     $data1 using ($1 >= 1 ? $1 : 1/0):3 linetype 3 with lines notitle,\
     $data0 using ($1 <= bMin || $1 >= bMax ? $1 : 1/0):2:4 linetype 1 with filledcurves fill solid 0.125 notitle,\
     $data1 using ($1 <= bMin || $1 >= bMax ? $1 : 1/0):2:4 linetype 1 with filledcurves fill solid 0.125 notitle,\
     $data0 using ($1 >= bMin && $1 <= bMax ? $1 : 1/0):2:3 linetype 1 with filledcurves fill solid 0.125 notitle,\
     $data1 using ($1 >= bMin && $1 <= bMax ? $1 : 1/0):2:3 linetype 1 with filledcurves fill solid 0.125 notitle,\
     $data0 using ($1 >= bMin && $1 <= bMax ? $1 : 1/0):3:4 linetype 2 with filledcurves fill solid 0.125 title 'Upper (s)',\
     $data1 using ($1 >= bMin && $1 <= bMax ? $1 : 1/0):3:4 linetype 3 with filledcurves fill solid 0.125 title 'Lower (r)'
"

let private renderLimitations path bs rMins0 sMaxs0 rMinsM sMaxsM c =

    let sMaxs0 = sMaxs0 |> Array.map (min 1e+6)
    let sMaxsM = sMaxsM |> Array.map (min 1e+6)

    let data0 =
        sMaxsM
        |> Array.zip3 bs sMaxs0
        |> Array.map (fun (b, s0, sM) -> sprintf "%O %O %O %O" b s0 sM c)
        |> String.concat "\n"

    let data1 =
        rMinsM
        |> Array.zip3 bs rMins0
        |> Array.map (fun (b, r0, rM) -> sprintf "%O %O %O %O" b r0 rM c)
        |> String.concat "\n"

    let xLower = bs |> Array.min
    let xUpper = bs |> Array.max

    render path plotLimitations [| data0; data1; xLower; xUpper; c |]

let renderLimitations1 path bs rMins sMaxs c =

    let sMaxs0 = sMaxs |> Array.map (fun _ -> nan)
    let rMins0 = rMins |> Array.map (fun _ -> nan)
    let sMaxsM = sMaxs
    let rMinsM = rMins

    renderLimitations path bs rMins0 sMaxs0 rMinsM sMaxsM c

let renderLimitations2 path bs rMins sMaxs c =

    let sMaxs0 = fst sMaxs
    let rMins0 = fst rMins
    let sMaxsM = snd sMaxs
    let rMinsM = snd rMins

    renderLimitations path bs rMins0 sMaxs0 rMinsM sMaxsM c
