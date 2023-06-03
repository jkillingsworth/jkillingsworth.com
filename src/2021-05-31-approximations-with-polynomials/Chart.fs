module Chart

open Common.Chart

//-------------------------------------------------------------------------------------------------

let private matrix (items : float[,]) =

    let densityX = (items |> Array2D.length1) - 1
    let densityY = (items |> Array2D.length2) - 1

    let createLine j =
        { 0 .. densityX }
        |> Seq.map (fun i -> items.[i, j])
        |> Seq.map (sprintf "%O")
        |> String.concat "\t"

    let combinedRows =
        { 0 .. densityY }
        |> Seq.map createLine
        |> String.concat "\n"

    combinedRows

let private downsample samples items =

    let count = (items |> Array.length) - 1

    let compute i =
        let ratio = float i / float samples
        let value = ratio * float count
        let index = value |> round |> int
        items.[index]

    if (samples < count) then
        compute |> Array.init (samples + 1)
    else
        items

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
set yrange [0:0.4]
set format y '%4.2f'

set key top left
set key reverse Left

set linetype 1 linewidth 1 linecolor rgb liteGray
set style fill solid border linecolor rgb parWhite

plot $data0 using 1:2 with boxes title 'Probability Mass'
"

let renderPmfunc path pmfunc =

    let n = (pmfunc |> Array.length) - 1

    let data0 =
        pmfunc
        |> Array.mapi (fun i x -> sprintf "%O %O" (2 * i - n) x)
        |> String.concat "\n"

    render path plotPmfunc [| data0; n |]

//-------------------------------------------------------------------------------------------------

let private plotBiases = baseplotRegular + "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

n = {2}; style = {3}

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
set linetype 2 pointtype 7 linecolor rgb baseBlue
set linetype 3 linewidth 2 linecolor rgb baseBlue
set style fill solid border linecolor rgb parWhite

if (style == 0) {{
    plot $data0 using 1:2 with boxes title 'Coin Bias'
}}

if (style == 1) {{
    x1 = n + 1
    x2 = n + 2
    plot $data0 using 1:2 with boxes title 'Coin Bias',\
         $data0 using 1:2:(sprintf('p_%i', $1)) every ::x1::x2 with labels offset -0.5,-1.0 point linetype 2 textcolor rgb darkMint notitle,\
         $data1 using 1:2 with lines notitle
}}

if (style == 2) {{
    x1 = n + 1
    x3 = n + 3
    plot $data0 using 1:2 with boxes title 'Coin Bias',\
         $data0 using 1:2:(sprintf('p_%i', $1)) every ::x1::x3 with labels offset -0.5,-1.0 point linetype 2 textcolor rgb darkMint notitle,\
         $data1 using 1:2 with lines notitle
}}

if (style == 3) {{
    x1 = n + 1
    x4 = n + 4
    plot $data0 using 1:2 with boxes title 'Coin Bias',\
         $data0 using 1:2:(sprintf('p_%i', $1)) every ::x1::x4 with labels offset -0.5,-1.0 point linetype 2 textcolor rgb darkMint notitle,\
         $data1 using 1:2 with lines notitle
}}
"

let renderBiases path biases points style =

    let n = (biases |> Array.length) / 2

    let data0 =
        biases
        |> Array.mapi (fun i x -> sprintf "%O %O" (i - n) x)
        |> String.concat "\n"

    let data1 =
        points
        |> Array.map (fun (x, y) -> sprintf "%O %O" x y)
        |> String.concat "\n"

    render path plotBiases [| data0; data1; n; style |]

//-------------------------------------------------------------------------------------------------

let private plotSurface = baseplotSurface + "
$data0 << EOD
{0}
EOD

stats $data0 using 1:2 matrix nooutput prefix 'data0'
densityX = data0_size_x - 1
densityY = data0_size_y - 1

set xlabel 'Coin Bias (+1)'
set xrange [0:1]
set xtics 0.2
set format x '%0.1f'

set ylabel 'Coin Bias (+2)'
set yrange [0:1]
set ytics 0.2
set format y '%0.1f'

set format z '%4.2f'

set cblabel offset 1 'Error'
set format cb '%4.2f'

set key top left
set key reverse Left

set pm3d
set view 30,30,1,1.8

set palette defined\
(\
0.00 pcolorDef(deepPurp),\
0.40 pcolorDef(fadePurp),\
0.75 pcolorDef(liteRust),\
1.00 pcolorDef(highRust)\
)

set linetype 1 linewidth 1 linecolor rgb (pcolorGet(0.35) + 0x80000000)

splot $data0 using ($1/densityX):($2/densityY):3 matrix with lines title 'Surface Plot'
"

let renderSurface path heatmap =

    let data0 = matrix heatmap

    render path plotSurface [| data0 |]

//-------------------------------------------------------------------------------------------------

let private plotHeatmap = baseplotHeatmap + "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

$data2 << EOD
{2}
EOD

$data3 << EOD
{3}
EOD

tag = '{4}'

stats $data0 using 1:2 matrix nooutput prefix 'data0'
densityX = data0_size_x - 1
densityY = data0_size_y - 1

set xlabel 'Coin Bias (+1)'
set xrange [0:1]
set xtics 0.2
set format x '%0.2f'

set ylabel 'Coin Bias (+2)'
set yrange [0:1]
set ytics 0.2
set format y '%4.2f'

set cblabel offset 1 'Error'
set format cb '%4.2f'

set key top left
set key reverse Left
set key textcolor rgb parWhite

set palette defined\
(\
0.00 pcolorDef(deepPurp),\
0.40 pcolorDef(fadePurp),\
0.75 pcolorDef(liteRust),\
1.00 pcolorDef(highRust)\
)

set linetype 1 linewidth 2 linecolor rgb parWhite
set linetype 2 pointtype 6 linecolor rgb parWhite
set linetype 3 pointtype 7 linecolor rgb parWhite

plot $data0 using ($1/densityX):($2/densityY):3 matrix with image pixels notitle,\
     $data1 using 1:2 with lines title sprintf('Trace %s', tag),\
     $data2 using 1:2 with points linetype 2 title 'Start',\
     $data3 using 1:2 with points linetype 3 title 'Finish'
"

let renderHeatmap path heatmap trace samples tag =

    let data0 = matrix heatmap

    let data1 =
        trace
        |> downsample samples
        |> Array.map (fun (x, y) -> sprintf "%O %O" x y)
        |> String.concat "\n"

    let data2 = sprintf "%O %O" <|| (trace |> Array.head)
    let data3 = sprintf "%O %O" <|| (trace |> Array.last)

    render path plotHeatmap [| data0; data1; data2; data3; tag |]
