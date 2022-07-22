module Chart

open Common.Chart

//-------------------------------------------------------------------------------------------------

let private plotPmfunc = baseplotRegular + "
$data0 << EOD
{0}
EOD

n = {1}; upper = {2}

set xlabel 'Possible Outcome'
set xrange [-n-2:+n+2]
if (n % 2 == 0) {{ set xtics ('0' 0) }} else {{ set xtics () }}
if (n <= 10) {{
    set for [i=+n:+1:-2] xtics add (sprintf('%+i', i) i)
    set for [i=-n:-1:+2] xtics add (sprintf('%+i', i) i)
}} else {{
    set xtics add -n, n, +n
    set format x '%+0.0f'
}}

set ylabel 'Probability'
set yrange [0:upper]
set format y '%4.2f'

set key top left
set key reverse Left

set linetype 1 linewidth 1 linecolor rgb liteGray
set style fill solid border linecolor rgb parWhite

plot $data0 using 1:2 with boxes title 'Probability Mass'
"

let renderPmfunc path pmfunc upper =

    let n = (pmfunc |> Array.length) - 1

    let data0 =
        pmfunc
        |> Array.mapi (fun i x -> sprintf "%O %O" (2 * i - n) x)
        |> String.concat "\n"

    render path plotPmfunc [| data0; n; upper |]

//-------------------------------------------------------------------------------------------------

let private plotBiases = baseplotRegular + "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

n = {2}; degree = {3}

set xlabel 'State'
set xrange [-n:+n]
set xtics ('0' 0)
if (n <= 10) {{
    set for [i=+1:+n-1:+1] xtics add (sprintf('%+i', i) i)
    set for [i=-1:-n+1:-1] xtics add (sprintf('%+i', i) i)
}} else {{
    set xtics add -(n - 1), (n - 1), +(n - 1)
    set format x '%+0.0f'
}}

set ylabel 'Probability of Heads'
set yrange [0:1]
set ytics 0.10
set format y '%4.2f'

set key top left
set key reverse Left

set linetype 1 linewidth 1 linecolor rgb liteLeaf
set linetype 2 pointtype 7 linecolor rgb baseBlue
set linetype 3 linewidth 2 linecolor rgb baseBlue
set linetype 4 linewidth 1 linecolor rgb liteBlue
set style fill solid border linecolor rgb parWhite

if (n <= 10) {{
    xa = n + 1
    xb = n + 1 + degree
    plot $data0 using 1:2 with boxes title 'Coin Bias',\
         $data0 using 1:2:(sprintf('p_%i', $1)) every ::xa::xb with labels offset -0.5,-1.0 point linetype 2 textcolor rgb darkLeaf notitle,\
         $data1 using 1:2 with lines notitle
}}

if (n > 10) {{
    plot $data0 using 1:(1 <= $1 && $1 <= (degree + 1) ? 0 : $2) with boxes title 'Coin Bias',\
         $data0 using 1:(1 <= $1 && $1 <= (degree + 1) ? $2 : 0) with boxes notitle linetype 4,\
         $data1 using 1:2 with lines notitle
}}
"

let renderBiases path biases points degree =

    let n = (biases |> Array.length) / 2

    let data0 =
        biases
        |> Array.mapi (fun i x -> sprintf "%O %O" (i - n) x)
        |> String.concat "\n"

    let data1 =
        points
        |> Array.map (fun (x, y) -> sprintf "%O %O" x y)
        |> String.concat "\n"

    render path plotBiases [| data0; data1; n; degree |]
