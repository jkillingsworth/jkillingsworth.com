module Chart

open System
open Common.Chart

//-------------------------------------------------------------------------------------------------

let private percent x =
    if Double.IsNaN(x) then "" else x.ToString("0.00%;-0.00%;0.00%")

//-------------------------------------------------------------------------------------------------

let private plotPmfunc = baseplotRegular + "
$data0 << EOD
{0}
EOD

n = {1}

set xlabel 'Possible Outcome'
set xrange [-n-2:+n+2]
if (n % 2 == 0) {{ set xtics (0) }} else {{ set xtics () }}
set for [i=+n:+1:-2] xtics add (sprintf('%+i', i) i)
set for [i=-n:-1:+2] xtics add (sprintf('%+i', i) i)

set ylabel 'Probability'
set yrange [0:0.6]
set format y '%4.2f'

set key top left
set key reverse Left

set linetype 1 linewidth 1 linecolor rgb liteGray
set style fill solid border linecolor rgb parWhite

plot $data0 using 1:2 with boxes title 'Probability Mass',\
     $data0 using 1:(0.024):3 with labels notitle textcolor rgb darkGray
"

let renderPmfunc path pmfunc =

    let n = 4

    let data0 =
        pmfunc
        |> Array.mapi (fun i x -> sprintf "%O %O %s" (2 * i - n) x (percent x))
        |> String.concat "\n"

    render path plotPmfunc [| data0; n |]

//-------------------------------------------------------------------------------------------------

let private plotBiases = baseplotRegular + "
$data0 << EOD
{0}
EOD

n = {1}

set xlabel 'State'
set xrange [-n:+n]
set xtics (0)
set for [i=+1:+n-1:+1] xtics add (sprintf('%+i', i) i)
set for [i=-1:-n+1:-1] xtics add (sprintf('%+i', i) i)

set ylabel 'Probability of Heads'
set yrange [0:1]
set ytics 0.10
set format y '%4.2f'

set key top left
set key reverse Left

set linetype 1 linewidth 1 linecolor rgb liteLeaf
set style fill solid border linecolor rgb parWhite

plot $data0 using 1:2 with boxes title 'Coin Bias',\
     $data0 using 1:(0.04):3 with labels notitle textcolor rgb darkLeaf
"

let renderBiases path biases =

    let n = 4

    let data0 =
        biases
        |> Array.mapi (fun i x -> sprintf "%O %O %s" (i - n) x (percent x))
        |> String.concat "\n"

    render path plotBiases [| data0; n |]

//-------------------------------------------------------------------------------------------------

let private plotTosses = baseplotRegular + "
$data0 << EOD
{0}
EOD

n = {1}; liteBlueGray = {2}; liteRojoGray = {3}; darkBlueGray = {4}; darkRojoGray = {5}

set xlabel 'Sequence'
set xrange [-1:(2**n)]

set ylabel 'Probability'
set yrange [0:0.20]
set format y '%4.2f'

set key top left
set key reverse Left

set linetype 1 linewidth 1 linecolor rgb liteBlue
set linetype 2 linewidth 1 linecolor rgb liteBlueGray
set linetype 3 linewidth 1 linecolor rgb liteGray
set linetype 4 linewidth 1 linecolor rgb liteRojoGray
set linetype 5 linewidth 1 linecolor rgb liteRojo
set style fill solid border linecolor rgb parWhite

plot $data0 using 1:($3 == 0 ? $2 : 0):xtic(4) with boxes title '0 Heads, 4 Tails',\
     $data0 using 1:($3 == 1 ? $2 : 0):xtic(4) with boxes title '1 Heads, 3 Tails',\
     $data0 using 1:($3 == 2 ? $2 : 0):xtic(4) with boxes title '2 Heads, 2 Tails',\
     $data0 using 1:($3 == 3 ? $2 : 0):xtic(4) with boxes title '3 Heads, 1 Tails',\
     $data0 using 1:($3 == 4 ? $2 : 0):xtic(4) with boxes title '4 Heads, 0 Tails',\
     $data0 using 1:($3 == 0 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor rgb darkBlue,\
     $data0 using 1:($3 == 1 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor rgb darkBlueGray,\
     $data0 using 1:($3 == 2 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor rgb darkGray,\
     $data0 using 1:($3 == 3 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor rgb darkRojoGray,\
     $data0 using 1:($3 == 4 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor rgb darkRojo
"

let renderTosses path tosses =

    let n = 4

    let color s =
        s
        |> Seq.filter (fun x -> x = 'H')
        |> Seq.length

    let data0 =
        tosses
        |> Array.rev
        |> Array.mapi (fun i (s, x) -> sprintf "%O %O %O %s %s" i x (color s) s (percent x))
        |> String.concat "\n"

    let liteBlueGray = liteBlue |> mix 0.5 liteGray |> rgbToInt
    let liteRojoGray = liteRojo |> mix 0.5 liteGray |> rgbToInt
    let darkBlueGray = darkBlue |> mix 0.5 darkGray |> rgbToInt
    let darkRojoGray = darkRojo |> mix 0.5 darkGray |> rgbToInt

    render path plotTosses [| data0; n; liteBlueGray; liteRojoGray; darkBlueGray; darkRojoGray |]
