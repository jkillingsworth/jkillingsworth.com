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

let private plotDistribution = "
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
    set xlabel 'Possible Outcome'
    set xrange [-900:1100]
    set xtics -900, 200
    set mxtics 2
}}

if (style == 2) {{
    set xlabel 'Possible Outcome'
    set xrange [0.001:+10000000]
    set xtics 0.001, 10
    set xtics add (0.001, 0.01, 0.1, 1, 10, 100, '1K' 1000, '10K' 10000, '100K' 100000, '1M' 1000000, '10M' 10000000)
    set mxtics default
    set logscale x
}}

set ylabel 'Probability'
set format y '%0.2f'

set key box linecolor '#808080' samplen 1
set key top right noreverse Right

set linetype 1 linewidth 5 linecolor '#00c000'
set linetype 2 linewidth 5 linecolor '#ff0000'
set linetype 3 linewidth 5 linecolor '#808080'

plot $data0 using ($3 == 1 ? $1 : 1/0):2 with impulses title 'Profit',\
     $data0 using ($3 == 2 ? $1 : 1/0):2 with impulses title 'Loss',\
     $data0 using ($3 == 3 ? $1 : 1/0):2 with impulses title 'Breakeven'
"

let private renderDistribution style path items =

    let round (x : float) = Math.Round(x, 3)

    let color = function
        | x when round (fst x) > 100.0 -> 1
        | x when round (fst x) < 100.0 -> 2
        | _ -> 3

    let data0 =
        items
        |> Array.map (fun x -> sprintf "%O %O %O" (fst x) (snd x) (color x))
        |> String.concat "\n"

    render path plotDistribution [| data0; style |]

let renderDistributionLin path items = renderDistribution 1 path items
let renderDistributionLog path items = renderDistribution 2 path items
