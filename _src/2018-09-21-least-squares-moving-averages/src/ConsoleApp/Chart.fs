module Chart

open System.Diagnostics
open System.IO

//-------------------------------------------------------------------------------------------------

let private plotFull = "
set terminal svg size 720 405 background 'white' font 'Consolas, Monaco, monospace'
set encoding utf8
set output '{0}'

$data << EOD
{1}
EOD

set xlabel 'Time in Days'
set xtics scale 0.01, 0.01
set xtics 200
set xrange [0:2000]

set ylabel 'Price per Share'
set ytics scale 0.01, 0.01
set ytics {2}, {4}
set yrange [{2}:{3}]
set format y '%g'

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key title '{5} (Full)'

set linetype 1 linecolor '#00808080'
set linetype 2 linecolor '#00ff0000'
set linetype 3 linecolor '#800000ff'

plot '$data' using 1:2 with lines title 'Market Price',\
     '$data' using 1:3 with lines title 'Moving Average',\
     '$data' using 1:4 with lines title 'Fitted Line'
"

//-------------------------------------------------------------------------------------------------

let private plotZoom = "
set terminal svg size 720 405 background 'white' font 'Consolas, Monaco, monospace'
set encoding utf8
set output '{0}'

$data << EOD
{1}
EOD

set xlabel 'Time in Days'
set xtics scale 0.01, 0.01
set xtics 200
set xrange [1800:2000]

set ylabel 'Price per Share'
set ytics scale 0.01, 0.01
set ytics {2}, {4}
set yrange [{2}:{3}]
set format y '%g'

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key title '{5} (Zoom)'

set linetype 1 linecolor '#00808080'
set linetype 2 linecolor '#80ff0000'
set linetype 3 linecolor '#000000ff'

plot '$data' using 1:2 with lines title 'Market Price',\
     '$data' using 1:3 with lines title 'Moving Average',\
     '$data' using 1:4 with lines title 'Fitted Line'
"

//-------------------------------------------------------------------------------------------------

let private render plot path axis (ticker : string) data =

    let lower, upper, step = axis : (int * int * int)

    let formatOption = function Some x -> sprintf "%f" x | None -> "''"
    let formatData i (market, moving, fitted) =
        sprintf "%i %f %s %s" i market (formatOption moving) (formatOption fitted)

    let data =
        data
        |> Array.mapi formatData
        |> String.concat "\n"

    let file = Path.GetFullPath(path)
    use proc = new Process()
    proc.StartInfo.FileName <- "gnuplot.exe"
    proc.StartInfo.UseShellExecute <- false
    proc.StartInfo.RedirectStandardInput <- true
    proc.Start() |> ignore
    proc.StandardInput.Write(plot, file, data, lower, upper, step, ticker)
    proc.StandardInput.Flush()

let renderFull = render plotFull
let renderZoom = render plotZoom
