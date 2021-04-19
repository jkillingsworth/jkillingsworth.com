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

let private terminal = "
exit
"

let private render path template args =

    let preamble = String.Format(preamble, Path.GetFullPath(path))
    let template = String.Format(template, args)
    let plot = preamble + template + terminal
    use proc = new Process()
    proc.StartInfo.FileName <- "gnuplot.exe"
    proc.StartInfo.UseShellExecute <- false
    proc.StartInfo.RedirectStandardInput <- true
    proc.StartInfo.RedirectStandardError <- true
    proc.StartInfo.StandardInputEncoding <- new UTF8Encoding()
    proc.StartInfo.StandardErrorEncoding <- new UTF8Encoding()
    proc.Start() |> ignore
    proc.StandardInput.Write(plot)
    proc.StandardInput.Flush()
    proc.WaitForExit()
    let stderr = proc.StandardError.ReadToEnd()
    Console.ForegroundColor <- ConsoleColor.Red
    Console.Error.Write(stderr)
    Console.ResetColor()

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

if (style == 1) {{
    set xlabel 'x'
    set xrange [0:5]
    set xtics 1
}}

if (style == 2) {{
    set xlabel 'x'
    set xrange [0.01:100]
    set xtics 0.1
    set logscale x 10
}}

set ylabel 'Probability Density'
set yrange [0:0.75]
set ytics 0.05
set format y '%0.2f'

set key box linecolor '#808080' samplen 1
set key top right reverse Left

set linetype 1 linewidth 2 linecolor '#0000ff'
set linetype 2 linewidth 2 linecolor '#ff0000'

plot $data0 using 1:2 with lines title 'Log-Normal',\
     $data0 using 1:3 with lines title 'Log-Laplace'
"

let private renderDistributions style path items =

    let data0 =
        items
        |> Array.map (fun (x, n, l) -> sprintf "%O %O %O" x n l)
        |> String.concat "\n"

    render path plotDistributions [| data0; style |]

let renderDistributionsLin path items = renderDistributions 1 path items
let renderDistributionsLog path items = renderDistributions 2 path items
