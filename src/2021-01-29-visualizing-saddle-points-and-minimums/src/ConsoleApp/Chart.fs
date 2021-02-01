module Chart

open System
open System.Diagnostics
open System.IO
open System.Text

//-------------------------------------------------------------------------------------------------

let private preamble = "
set terminal svg size 720 405 font 'Consolas, Menlo, monospace'
set encoding utf8
set output '{0}'
"

let private render path template args =

    let preamble = String.Format(preamble, Path.GetFullPath(path))
    let template = String.Format(template, args)
    let plot = preamble + template
    use proc = new Process()
    proc.StartInfo.FileName <- "gnuplot.exe"
    proc.StartInfo.UseShellExecute <- false
    proc.StartInfo.RedirectStandardInput <- true
    proc.StartInfo.StandardInputEncoding <- new UTF8Encoding()
    proc.Start() |> ignore
    proc.StandardInput.Write(plot)
    proc.StandardInput.Flush()

//-------------------------------------------------------------------------------------------------

let private matrix (data : float[,]) densityX densityY =

    let createLine j =
        { 0 .. densityX }
        |> Seq.map (fun i -> data.[i, j])
        |> Seq.map (sprintf "%e")
        |> Seq.reduce (sprintf "%s %s")

    let combinedRows =
        { 0 .. densityY }
        |> Seq.map createLine
        |> Seq.reduce (sprintf "%s\n%s")

    combinedRows

//-------------------------------------------------------------------------------------------------

let private plotHeatmap = "
$heatmap << EOD
{0}
EOD

$optimum << EOD
0.5 0.0
EOD

$slice << EOD
{5}
EOD

set border linewidth 1.2
set xtics scale 0.01, 0.01
set ytics scale 0.01, 0.01

set xlabel 'Coin Bias (p)'
set xrange [0.0:1.0]
set xtics 0.0, 0.2
set format x '%0.1f'

set ylabel 'Lagrange Multiplier (λ)'
set yrange [-0.5:+0.5]
set ytics -0.5, 0.2
set format y '%+0.1f'

set cblabel offset 1 'Value'
set cbrange [{3}:{4}]
set cbtics add ('\u00A00.0' 0)
set format cb '%+0.1f'

set key box linecolor '#808080' textcolor '#ffffff' samplen 1
set key top left reverse Left

set linetype 4 linewidth 2 linecolor '#ffffff' dashtype 3

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

n = {7}

if ({6} == 0) {{
    plot '$heatmap' using ($1/{1}):($2/{2} - 0.5):3 matrix with image pixels notitle,\
         '$optimum' with labels point pointtype 7 linecolor '#ffffff' title 'Optimum'
}}

if ({6} == 1) {{
    plot '$heatmap' using ($1/{1}):($2/{2} - 0.5):3 matrix with image pixels notitle,\
         '$optimum' with labels point pointtype 7 linecolor '#ffffff' title 'Optimum',\
         '$slice' using 1:2:('A') every ::0::0 with labels offset +1.5,+1.3 nopoint textcolor '#ffffff' notitle,\
         '$slice' using 1:2:('B') every ::n::n with labels offset -1.5,-1.3 nopoint textcolor '#ffffff' notitle,\
         '$slice' using 1:2 with lines title 'Slice {6}'
}}

if ({6} == 2) {{
    plot '$heatmap' using ($1/{1}):($2/{2} - 0.5):3 matrix with image pixels notitle,\
         '$optimum' with labels point pointtype 7 linecolor '#ffffff' title 'Optimum',\
         '$slice' using 1:2:('A') every ::0::0 with labels offset -1.0,-0.7 nopoint textcolor '#ffffff' notitle,\
         '$slice' using 1:2:('B') every ::n::n with labels offset +1.0,+0.7 nopoint textcolor '#ffffff' notitle,\
         '$slice' using 1:2 with lines title 'Slice {6}'
}}
"

let renderHeatmap path heatmap (lower, upper) tag slice =

    let densityX = (heatmap |> Array2D.length1) - 1
    let densityY = (heatmap |> Array2D.length2) - 1
    let heatmap = matrix heatmap densityX densityY

    let samples = (slice |> Array.length) - 1

    let slice =
        slice
        |> Array.map (fun (p, λ, v) -> sprintf "%e %e %e" p λ v)
        |> String.concat "\n"

    render path plotHeatmap [| heatmap; densityX; densityY; lower; upper; slice; tag; samples |]

//-------------------------------------------------------------------------------------------------

let private plotSurface = "
$heatmap << EOD
{0}
EOD

set border linewidth 1.0
set xtics scale 0.01, 0.01
set ytics scale 0.01, 0.01

set xlabel 'Coin Bias (p)'
set xrange [0.0:1.0]
set xtics 0.0, 0.2
set format x '%0.1f'

set ylabel 'Lagrange Multiplier (λ)'
set yrange [-0.5:+0.5]
set ytics -0.5, 0.2
set format y '%+0.1f'

set zrange [{3}:{4}]
set ztics add ('\u00A00.0' 0)
set format z '%+0.1f'

set cblabel offset 1 'Value'
set cbrange [{3}:{4}]
set cbtics add ('\u00A00.0' 0)
set format cb '%+0.1f'

set pm3d
set grid
if ({5} == 1) {{ set view 30,30,1,1.8 }}
if ({5} == 2) {{ set view 30,60,1,1.8 }}
if ({5} == 3) {{ set view 60,60,1,1.2 }}

set key box linecolor '#808080' samplen 1 width -1
set key top left reverse Left

set linetype 1 linecolor '#812581'

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

splot '$heatmap' using ($1/{1}):($2/{2} - 0.5):3 matrix with lines title 'Surface Plot'
"

let renderSurface path data (lower, upper) style =

    let densityX = (data |> Array2D.length1) - 1
    let densityY = (data |> Array2D.length2) - 1
    let heatmap = matrix data densityX densityY

    render path plotSurface [| heatmap; densityX; densityY; lower; upper; style |]

//-------------------------------------------------------------------------------------------------

let private plotProfile = "
$optimum << EOD
0.5 0.0
EOD

$slice << EOD
{2}
EOD

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics
set xtics scale 0.01, 0.01
set ytics scale 0.01, 0.01

set xlabel 'Profile (p, λ)'
set xrange [0:1]
set xtics 0, 1
set xtics add ('A' 0, 'B' 1)
set mxtics 5

set ylabel 'Value'
set yrange [{0}:{1}]
set ytics add ('\u00A00.0' 0)
set format y '%+0.1f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 2 linewidth 2 linecolor '#b5367a'

plot '$optimum' with labels point pointtype 7 linecolor '#b5367a' title 'Optimum',\
     '$slice' using ($0 / {4}):3 with lines title 'Slice {3}'
"

let renderProfile path samples (lower, upper) tag slice =

    let slice =
        slice
        |> Array.map (fun (p, λ, value) -> sprintf "%e %e %e" p λ value)
        |> String.concat "\n"

    render path plotProfile [| lower; upper; slice; tag; samples |]
