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

n = {1}; style = {2}

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
set linetype 3 pointtype 7 linecolor rgb baseBlue
set linetype 4 linewidth 2 linecolor rgb baseBlue
set style fill solid border linecolor rgb parWhite

if (style == 0) {{
    plot $data0 using 1:2 with boxes title 'Coin Bias'
}}

if (style == 1) {{
    s0 = n
    sa = n + n - 1
    plot $data0 using 1:2 with boxes title 'Coin Bias',\
         $data0 using 1:(p0=$2):(1/0) every ::s0::s0 with labels offset -0.5,-1.0 point linetype 2 notitle,\
         $data0 using 1:(pa=$2):('a') every ::sa::sa with labels offset -0.5,-1.0 point linetype 3 textcolor rgb darkMint title sprintf('a = %0.4f', pa),\
         $data0 using 1:2 every ::s0::sa with lines notitle
}}

if (style == 2) {{
    sa = n + 1
    sb = n + n - 1
    plot $data0 using 1:2 with boxes title 'Coin Bias',\
         $data0 using 1:(pa=$2):('a') every ::sa::sa with labels offset -0.5,-1.0 point linetype 2 textcolor rgb darkMint title sprintf('a = %0.4f', pa),\
         $data0 using 1:(pb=$2):('b') every ::sb::sb with labels offset -0.5,-1.0 point linetype 3 textcolor rgb darkMint title sprintf('b = %0.4f', pb),\
         $data0 using 1:2 every ::sa::sb with lines notitle
}}
"

let renderBiases path biases style =

    let n = (biases |> Array.length) / 2

    let data0 =
        biases
        |> Array.mapi (fun i x -> sprintf "%O %O" (i - n) x)
        |> String.concat "\n"

    render path plotBiases [| data0; n; style |]

//-------------------------------------------------------------------------------------------------

let private plotSurface = baseplotSurface + "
$data0 << EOD
{0}
EOD

n = {1}

stats $data0 using 1:2 matrix nooutput prefix 'data0'
densityX = data0_size_x - 1
densityY = data0_size_y - 1

set xlabel 'Coin Bias (+1)'
set xrange [0:1]
set xtics 0.2
set format x '%0.1f'

set ylabel sprintf('Coin Bias (+%i)', n - 1)
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
0 '#440154',\
1 '#472c7a',\
2 '#3b518b',\
3 '#2c718e',\
4 '#21908d',\
5 '#27ad81',\
6 '#5cc863',\
7 '#aadc32',\
8 '#fde725' \
)

set linetype 1 linewidth 1 linecolor '#4021908d'

splot $data0 using ($1/densityX):($2/densityY):3 matrix with lines title 'Surface Plot'
"

let renderSurface path heatmap n =

    let data0 = matrix heatmap

    render path plotSurface [| data0; n |]

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

n = {4}; tag = '{5}'

stats $data0 using 1:2 matrix nooutput prefix 'data0'
densityX = data0_size_x - 1
densityY = data0_size_y - 1

set xlabel 'Coin Bias (+1)'
set xrange [0:1]
set xtics 0.2
set format x '%0.2f'

set ylabel sprintf('Coin Bias (+%i)', n - 1)
set yrange [0:1]
set ytics 0.2
set format y '%4.2f'

set cblabel offset 1 'Error'
set format cb '%4.2f'

set key top left
set key reverse Left
set key textcolor '#ffffff'

set palette defined\
(\
0 '#440154',\
1 '#472c7a',\
2 '#3b518b',\
3 '#2c718e',\
4 '#21908d',\
5 '#27ad81',\
6 '#5cc863',\
7 '#aadc32',\
8 '#fde725' \
)

set linetype 1 linewidth 2 linecolor '#ffffff'
set linetype 2 pointtype 6 linecolor '#ffffff'
set linetype 3 pointtype 7 linecolor '#ffffff'

plot $data0 using ($1/densityX):($2/densityY):3 matrix with image pixels notitle,\
     $data1 using 1:2 with lines title sprintf('Trace %s', tag),\
     $data2 using 1:2 with points linetype 2 title 'Start',\
     $data3 using 1:2 with points linetype 3 title 'Finish'
"

let renderHeatmap path heatmap n trace samples tag =

    let data0 = matrix heatmap

    let data1 =
        trace
        |> downsample samples
        |> Array.map (fun (x, y) -> sprintf "%O %O" x y)
        |> String.concat "\n"

    let data2 = sprintf "%O %O" <|| (trace |> Array.head)
    let data3 = sprintf "%O %O" <|| (trace |> Array.last)

    render path plotHeatmap [| data0; data1; data2; data3; n; tag |]
