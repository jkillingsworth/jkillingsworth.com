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

let private plotDistributions = "
$data << EOD
{0}
EOD

set border linewidth 1.2

if ({1} == 1) {{
    set xlabel 'x'
    set xtics scale 0.01, 0.01
    set xtics 1
    set xrange [0:5]
}}

if ({1} == 2) {{
    set xlabel 'x'
    set xtics scale 0.01, 0.01
    set xtics 0.1
    set xrange [0.01:100]
    set logscale x 10
}}

set ylabel 'Probability Density'
set ytics scale 0.01, 0.01
set ytics 0.05
set yrange [0:0.75]
set format y '%.2f'

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set key box linecolor '#808080' samplen 1
set key top right reverse Left

set linetype 1 linewidth 2 linecolor '#0000ff'
set linetype 2 linewidth 2 linecolor '#ff0000'

plot '$data' using 1:2 with lines title 'Log-Normal',\
     '$data' using 1:3 with lines title 'Log-Laplace'
"

let private renderDistributions linlog path data =

    let data =
        data
        |> Array.map (fun (x, n, l) -> sprintf "%e %e %e" x n l)
        |> String.concat "\n"

    render path plotDistributions [| data; linlog |]

let renderDistributionsLin = renderDistributions 1
let renderDistributionsLog = renderDistributions 2
