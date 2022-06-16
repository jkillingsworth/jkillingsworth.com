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

//-------------------------------------------------------------------------------------------------

let private plotSurface = baseplotSurface + "
$data0 << EOD
{0}
EOD

lower = {1}; upper = {2}; style = {3}

stats $data0 using 1:2 matrix nooutput prefix 'data0'
densityX = data0_size_x - 1
densityY = data0_size_y - 1

set xlabel 'Coin Bias (p)'
set xrange [0.0:1.0]
set xtics 0.0, 0.2
set format x '%0.1f'

set ylabel 'Lagrange Multiplier (λ)'
set yrange [-0.5:+0.5]
set ytics -0.5, 0.2
set format y '%+0.1f'

set zrange [lower:upper]
set ztics add ('\u00A00.0' 0)
set format z '%+0.1f'

set cblabel offset 1 'Value'
set cbrange [lower:upper]
set cbtics add ('\u00A00.0' 0)
set format cb '%+0.1f'

set key top left
set key reverse Left
set key width -1

set pm3d
if (style == 1) {{ set view 30,30,1,1.8 }}
if (style == 2) {{ set view 30,60,1,1.8 }}
if (style == 3) {{ set view 60,60,1,1.2 }}

set palette defined\
(\
0 '#000004',\
1 '#1c1044',\
2 '#4f127b',\
3 '#812581',\
4 '#b5367a',\
5 '#e55964',\
6 '#fb8761',\
7 '#fec287',\
8 '#fbfdbf' \
)

set linetype 1 linewidth 1 linecolor '#812581'

splot $data0 using ($1/densityX):($2/densityY - 0.5):3 matrix with lines title 'Surface Plot'
"

let renderSurface path heatmap (lower, upper) style =

    let data0 = matrix heatmap

    render path plotSurface [| data0; lower; upper; style |]

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

lower = {3}; upper = {4}; style = {5}

stats $data0 using 1:2 matrix nooutput prefix 'data0'
densityX = data0_size_x - 1
densityY = data0_size_y - 1

if (style != 0) {{
    stats $data2 using 1:2 nooutput prefix 'data2'
    a = 0
    b = data2_records - 1
}}

set xlabel 'Coin Bias (p)'
set xrange [0.0:1.0]
set xtics 0.0, 0.2
set format x '%0.1f'

set ylabel 'Lagrange Multiplier (λ)'
set yrange [-0.5:+0.5]
set ytics -0.5, 0.2
set format y '%+0.1f'

set cblabel offset 1 'Value'
set cbrange [lower:upper]
set cbtics add ('\u00A00.0' 0)
set format cb '%+0.1f'

set key top left
set key reverse Left
set key textcolor '#ffffff'

set palette defined\
(\
0 '#000004',\
1 '#1c1044',\
2 '#4f127b',\
3 '#812581',\
4 '#b5367a',\
5 '#e55964',\
6 '#fb8761',\
7 '#fec287',\
8 '#fbfdbf' \
)

set linetype 1 pointtype 7 linecolor '#ffffff'
set linetype 2 linewidth 2 linecolor '#ffffff' dashtype 3

if (style == 0) {{
    plot $data0 using ($1/densityX):($2/densityY - 0.5):3 matrix with image pixels notitle,\
         $data1 using 1:2 with points title 'Optimum'
}}

if (style == 1) {{
    plot $data0 using ($1/densityX):($2/densityY - 0.5):3 matrix with image pixels notitle,\
         $data1 using 1:2 with points title 'Optimum',\
         $data2 using 1:2 with lines title sprintf('Slice %i', style),\
         $data2 using 1:2:('A') every ::a::a with labels offset +1.5,+1.3 textcolor '#ffffff' notitle,\
         $data2 using 1:2:('B') every ::b::b with labels offset -1.5,-1.3 textcolor '#ffffff' notitle
}}

if (style == 2) {{
    plot $data0 using ($1/densityX):($2/densityY - 0.5):3 matrix with image pixels notitle,\
         $data1 using 1:2 with points title 'Optimum',\
         $data2 using 1:2 with lines title sprintf('Slice %i', style),\
         $data2 using 1:2:('A') every ::a::a with labels offset -1.0,-0.7 textcolor '#ffffff' notitle,\
         $data2 using 1:2:('B') every ::b::b with labels offset +1.0,+0.7 textcolor '#ffffff' notitle
}}
"

let renderHeatmap path heatmap (lower, upper) style optimum slice =

    let data0 = matrix heatmap

    let data1 = sprintf "%O %O" (fst optimum) (snd optimum)

    let data2 =
        slice
        |> Array.map (fun (p, λ, v) -> sprintf "%O %O" p λ)
        |> String.concat "\n"

    render path plotHeatmap [| data0; data1; data2; lower; upper; style |]

//-------------------------------------------------------------------------------------------------

let private plotProfile = baseplotRegular + "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

lower = {2}; upper = {3}; style = {4}

set xlabel 'Profile (p, λ)'
set xrange [0:1]
set xtics 0, 1
set xtics add ('A' 0, 'B' 1)
set mxtics 5

set ylabel 'Value'
set yrange [lower:upper]
set ytics add ('\u00A00.0' 0)
set format y '%+0.1f'

set key top left
set key reverse Left

set linetype 1 pointtype 7 linecolor '#a000a0'
set linetype 2 linewidth 2 linecolor '#a000a0'

plot $data0 using 1:2 with points linetype 1 title 'Optimum',\
     $data1 using 1:2 with lines title sprintf('Slice %i', style)
"

let renderProfile path samples (lower, upper) style optimum slice =

    let data0 = sprintf "%O %O" (fst optimum) (snd optimum)

    let data1 =
        slice
        |> Array.mapi (fun i (p, λ, v) -> sprintf "%O %O" (float i / float samples) v)
        |> String.concat "\n"

    render path plotProfile [| data0; data1; lower; upper; style |]
