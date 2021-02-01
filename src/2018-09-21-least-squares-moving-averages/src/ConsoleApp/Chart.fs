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

let private plotPrice = "
$data << EOD
{0}
EOD

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics
set xtics scale 0.01, 0.01
set ytics scale 0.01, 0.01

set xlabel 'Time (Days)'
if ({5} == 1) {{ set xrange [0:2000] }}
if ({5} == 2) {{ set xrange [1800:2000] }}
set xtics 200

set ylabel 'Price per Share'
set yrange [{1}:{2}]
set ytics {1}, {3}
set format y '%g'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
if ({5} == 1) {{ set key title '{4} (Full)' left }}
if ({5} == 2) {{ set key title '{4} (Zoom)' left }}

if ({5} == 1) {{
    set linetype 1 linecolor '#00808080'
    set linetype 2 linecolor '#00ff0000'
    set linetype 3 linecolor '#800000ff'
}}

if ({5} == 2) {{
    set linetype 1 linecolor '#00808080'
    set linetype 2 linecolor '#80ff0000'
    set linetype 3 linecolor '#000000ff'
}}

plot '$data' using 1:2 with lines title 'Market Price',\
     '$data' using 1:3 with lines title 'Moving Average',\
     '$data' using 1:4 with lines title 'Fitted Line'
"

let private renderPrice scope path axis (ticker : string) data =

    let lower, upper, step = axis : (int * int * int)

    let formatOption = function Some x -> sprintf "%e" x | None -> "''"
    let formatData i (market, moving, fitted) =
        sprintf "%i %e %s %s" i market (formatOption moving) (formatOption fitted)

    let data =
        data
        |> Array.mapi formatData
        |> String.concat "\n"

    render path plotPrice [| data; lower; upper; step; ticker; scope |]

let renderPriceFull = renderPrice 1
let renderPriceZoom = renderPrice 2
