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

set linetype 1 linewidth 1 linecolor rgb liteMint
set style fill solid border linecolor rgb parWhite

plot $data0 using 1:2 with boxes title 'Coin Bias',\
     $data0 using 1:(0.04):3 with labels notitle textcolor rgb darkMint
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

n = {1}

set xlabel 'Sequence'
set xrange [-1:(2**n)]

set ylabel 'Probability'
set yrange [0:0.20]
set format y '%4.2f'

set key top left
set key reverse Left

set palette defined\
(\
0.00 pcolorDef(liteBlue),\
0.50 pcolorDef(liteGray),\
1.00 pcolorDef(liteRojo)\
)

lite0 = pcolorGet(0.00)
lite1 = pcolorGet(0.25)
lite2 = pcolorGet(0.50)
lite3 = pcolorGet(0.75)
lite4 = pcolorGet(1.00)

set palette defined\
(\
0.00 pcolorDef(darkBlue),\
0.50 pcolorDef(darkGray),\
1.00 pcolorDef(darkRojo)\
)

dark0 = pcolorGet(0.00)
dark1 = pcolorGet(0.25)
dark2 = pcolorGet(0.50)
dark3 = pcolorGet(0.75)
dark4 = pcolorGet(1.00)

set linetype 1 linewidth 1 linecolor rgb lite0
set linetype 2 linewidth 1 linecolor rgb lite1
set linetype 3 linewidth 1 linecolor rgb lite2
set linetype 4 linewidth 1 linecolor rgb lite3
set linetype 5 linewidth 1 linecolor rgb lite4
set style fill solid border linecolor rgb parWhite

plot $data0 using 1:($3 == 0 ? $2 : 0):xtic(4) with boxes title '0 Heads, 4 Tails',\
     $data0 using 1:($3 == 1 ? $2 : 0):xtic(4) with boxes title '1 Heads, 3 Tails',\
     $data0 using 1:($3 == 2 ? $2 : 0):xtic(4) with boxes title '2 Heads, 2 Tails',\
     $data0 using 1:($3 == 3 ? $2 : 0):xtic(4) with boxes title '3 Heads, 1 Tails',\
     $data0 using 1:($3 == 4 ? $2 : 0):xtic(4) with boxes title '4 Heads, 0 Tails',\
     $data0 using 1:($3 == 0 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor rgb dark0,\
     $data0 using 1:($3 == 1 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor rgb dark1,\
     $data0 using 1:($3 == 2 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor rgb dark2,\
     $data0 using 1:($3 == 3 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor rgb dark3,\
     $data0 using 1:($3 == 4 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor rgb dark4
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

    render path plotTosses [| data0; n |]
