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

lower = {1}; upper = {2}; mode = {3}; style = {4}

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
set ztics add ('0.0' 0)
set format z '%+4.1f'

set cblabel offset 1 'Value'
set cbrange [lower:upper]
set cbtics add ('\u00a00.0' 0)
set format cb '%+4.1f'

set key top left
set key reverse Left

set pm3d
if (style == 1) {{ set view 30,30,1,1.8 }}
if (style == 2) {{ set view 30,60,1,1.8 }}
if (style == 3) {{ set view 60,60,1,1.2 }}

if (mode == 1) {{
    set palette defined\
    (\
    0.00 pcolorDef(parBlack),\
    0.60 pcolorDef(fadePurp),\
    1.00 pcolorDef(highGray)\
    )
}}

if (mode == 2) {{
    set palette defined\
    (\
    0.00 pcolorDef(parBlack),\
    0.60 pcolorDef(fadePurp),\
    1.00 pcolorDef(highBlue)\
    )
}}

set linetype 1 linewidth 1 linecolor rgb (pcolorGet(0.50) + 0x80000000)

splot $data0 using ($1/densityX):($2/densityY - 0.5):3 matrix with lines title 'Surface Plot'
"

let renderSurface path heatmap (lower, upper) mode style =

    let data0 = matrix heatmap

    render path plotSurface [| data0; lower; upper; mode; style |]

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

lower = {3}; upper = {4}; mode = {5}; style = {6}

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
set format y '%+4.1f'

set cblabel offset 1 'Value'
set cbrange [lower:upper]
set cbtics add ('\u00a00.0' 0)
set format cb '%+4.1f'

set key top left
set key reverse Left
set key textcolor rgb parWhite

if (mode == 1) {{
    set palette defined\
    (\
    0.00 pcolorDef(parBlack),\
    0.60 pcolorDef(fadePurp),\
    1.00 pcolorDef(highGray)\
    )
}}

if (mode == 2) {{
    set palette defined\
    (\
    0.00 pcolorDef(parBlack),\
    0.60 pcolorDef(fadePurp),\
    1.00 pcolorDef(highBlue)\
    )
}}

set linetype 1 pointtype 7 linecolor rgb parWhite
set linetype 2 linewidth 2 linecolor rgb parWhite dashtype 3

if (style == 0) {{
    plot $data0 using ($1/densityX):($2/densityY - 0.5):3 matrix with image pixels notitle,\
         $data1 using 1:2 with points title 'Optimum'
}}

if (style == 1) {{
    plot $data0 using ($1/densityX):($2/densityY - 0.5):3 matrix with image pixels notitle,\
         $data1 using 1:2 with points title 'Optimum',\
         $data2 using 1:2 with lines title sprintf('Slice %i', style),\
         $data2 using 1:2:('A') every ::a::a with labels offset +1.5,+1.3 textcolor rgb parWhite notitle,\
         $data2 using 1:2:('B') every ::b::b with labels offset -1.5,-1.3 textcolor rgb parWhite notitle
}}

if (style == 2) {{
    plot $data0 using ($1/densityX):($2/densityY - 0.5):3 matrix with image pixels notitle,\
         $data1 using 1:2 with points title 'Optimum',\
         $data2 using 1:2 with lines title sprintf('Slice %i', style),\
         $data2 using 1:2:('A') every ::a::a with labels offset -1.0,-0.7 textcolor rgb parWhite notitle,\
         $data2 using 1:2:('B') every ::b::b with labels offset +1.0,+0.7 textcolor rgb parWhite notitle
}}
"

let renderHeatmap path heatmap (lower, upper) mode style optimum slice =

    let data0 = matrix heatmap

    let data1 = sprintf "%O %O" (fst optimum) (snd optimum)

    let data2 =
        slice
        |> Array.map (fun (p, λ, v) -> sprintf "%O %O" p λ)
        |> String.concat "\n"

    render path plotHeatmap [| data0; data1; data2; lower; upper; mode; style |]

//-------------------------------------------------------------------------------------------------

let private plotProfile = baseplotRegular + "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

lower = {2}; upper = {3}; mode = {4}; style = {5}

set xlabel 'Profile (p, λ)'
set xrange [0:1]
set xtics 0, 1
set xtics add ('A' 0, 'B' 1)
set xzeroaxis linestyle 2
set mxtics 5

set ylabel 'Value'
set yrange [lower:upper]
set ytics add ('0.0' 0)
set format y '%+4.1f'

set cblabel offset 1 'Value'
set cbrange [lower:upper]
set cbtics add ('\u00a00.0' 0)
set format cb '%+4.1f'

set key top left
set key reverse Left

if (mode == 1) {{
    set palette defined\
    (\
    0.00 pcolorDef(parBlack),\
    0.60 pcolorDef(fadePurp),\
    1.00 pcolorDef(highGray)\
    )
}}

if (mode == 2) {{
    set palette defined\
    (\
    0.00 pcolorDef(parBlack),\
    0.60 pcolorDef(fadePurp),\
    1.00 pcolorDef(highBlue)\
    )
}}

set linetype 1 pointtype 7 linecolor rgb darkPurp
set linetype 2 linewidth 2 linecolor rgb darkPurp dashtype 3
set linetype 3 linewidth 2 linecolor palette z

plot $data0 using 1:2 with points linetype 1 title 'Optimum',\
     $data1 using 1:2 with lines title sprintf('Slice %i', style),\
     $data1 using 1:(1/0):2 with lines notitle
"

let renderProfile path samples (lower, upper) mode style optimum slice =

    let data0 = sprintf "%O %O" (fst optimum) (snd optimum)

    let data1 =
        slice
        |> Array.mapi (fun i (p, λ, v) -> sprintf "%O %O" (float i / float samples) v)
        |> String.concat "\n"

    render path plotProfile [| data0; data1; lower; upper; mode; style |]
