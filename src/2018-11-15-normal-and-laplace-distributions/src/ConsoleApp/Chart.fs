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

let private plotLikelihood = "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

lower = {2}; upper = {3}; ymax = {4}; n = {5}

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics
set xtics scale 0.01, 0.01
set ytics scale 0.01, 0.01

set xlabel 'Location (µ)'

set ylabel 'Likelihood'
set yrange [lower:upper]
set ytics (' Max' ymax)

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key width -2

set linetype 1 linewidth 2 linecolor '#ff0000'
set linetype 2 pointtype 7 linecolor '#ff0000' pointsize 0.75

etitle = 'Likelihood (n is even)'
otitle = 'Likelihood (n is odd) '
if ((n % 2) == 0) {{ eotitle = etitle }} else {{ eotitle = otitle }}

plot $data0 using 1:2 with lines title eotitle,\
     $data1 using 1:2:xtic(3) with points notitle
"

let renderLikelihood path (lower, upper) items =

    let points = items |> Array.filter (fun (_, _, p) -> p)
    let ymax = points |> Array.map (fun (_, y, _) -> y) |> Array.max
    let n = points |> Array.length
    let m = (n / 2) + (n % 2)

    let xtic i =
        let offset = i - m + 1
        if (offset = 0) then "x_m" else sprintf "x_{{m%+i}}" offset

    let data0 =
        items
        |> Array.map (fun (x, y, p) -> sprintf "%i %e" x y)
        |> String.concat "\n"

    let data1 =
        points
        |> Array.mapi (fun i (x, y, p) -> sprintf "%i %e %s" x y (xtic i))
        |> String.concat "\n"

    render path plotLikelihood [| data0; data1; lower; upper; ymax; n |]

//-------------------------------------------------------------------------------------------------

let private plotDistributions = "
$data0 << EOD
{0}
EOD

style = {1}

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics
set xtics scale 0.01, 0.01
set ytics scale 0.01, 0.01

set xlabel 'x'
set xtics 1

if (style == 1) {{
    set ylabel 'Probability Density'
    set yrange [0:0.55]
    set ytics 0.05
    set format y '%0.2f'
}}

if (style == 2) {{
    set ylabel 'Probability Density'
    set yrange [0.00001:1]
    set format y ' 10^{{%+L}}'
    set logscale y 10
}}

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 2 linecolor '#0000ff'
set linetype 2 linewidth 2 linecolor '#ff0000'

plot $data0 using 1:2 with lines title 'Normal',\
     $data0 using 1:3 with lines title 'Laplace'
"

let private renderDistributions style path items =

    let data0 =
        items
        |> Array.map (fun (x, n, l) -> sprintf "%e %e %e" x n l)
        |> String.concat "\n"

    render path plotDistributions [| data0; style |]

let renderDistributionsLin = renderDistributions 1
let renderDistributionsLog = renderDistributions 2
