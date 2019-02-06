module Chart

open System
open System.Diagnostics
open System.IO
open System.Text

//-------------------------------------------------------------------------------------------------

let private preamble = "
set terminal svg size 720 405 font 'Consolas, Monaco, monospace'
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

let private plotFull = "
$data << EOD
{0}
EOD

set xlabel 'Time in Days'
set xtics scale 0.01, 0.01
set xtics 200
set xrange [0:2000]

set ylabel 'Price per Share'
set ytics scale 0.01, 0.01
set ytics {1}, {3}
set yrange [{1}:{2}]
set format y '%g'

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key title '{4} (Full)'

set linetype 1 linecolor '#00808080'
set linetype 2 linecolor '#00ff0000'
set linetype 3 linecolor '#800000ff'

plot '$data' using 1:2 with lines title 'Market Price',\
     '$data' using 1:3 with lines title 'Moving Average',\
     '$data' using 1:4 with lines title 'Fitted Line'
"

//-------------------------------------------------------------------------------------------------

let private plotZoom = "
$data << EOD
{0}
EOD

set xlabel 'Time in Days'
set xtics scale 0.01, 0.01
set xtics 200
set xrange [1800:2000]

set ylabel 'Price per Share'
set ytics scale 0.01, 0.01
set ytics {1}, {3}
set yrange [{1}:{2}]
set format y '%g'

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key title '{4} (Zoom)'

set linetype 1 linecolor '#00808080'
set linetype 2 linecolor '#80ff0000'
set linetype 3 linecolor '#000000ff'

plot '$data' using 1:2 with lines title 'Market Price',\
     '$data' using 1:3 with lines title 'Moving Average',\
     '$data' using 1:4 with lines title 'Fitted Line'
"

//-------------------------------------------------------------------------------------------------

let private renderChart plot path axis (ticker : string) data =

    let lower, upper, step = axis : (int * int * int)

    let formatOption = function Some x -> sprintf "%e" x | None -> "''"
    let formatData i (market, moving, fitted) =
        sprintf "%i %e %s %s" i market (formatOption moving) (formatOption fitted)

    let data =
        data
        |> Array.mapi formatData
        |> String.concat "\n"

    render path plot [| data; lower; upper; step; ticker |]

let renderFull = renderChart plotFull
let renderZoom = renderChart plotZoom
