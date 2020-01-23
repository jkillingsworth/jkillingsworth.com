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

let private plotLin = "
$data << EOD
{0}
EOD

set xlabel 'Number of Plays'
set xrange [0:200]
set xtics scale 0.01, 0.01
set xtics 50
set mxtics 2

set ylabel 'Dollars'
set yrange [{1}:{2}]
set ytics scale 0.01, 0.01
set ytics 100
set mytics 5

set grid xtics ytics mxtics mytics
set grid ls 1 lc '#e6e6e6'
set key box lc '#808080' samplen 1

plot '$data' with lines lc '#ff0000' title 'Gambler''s Bankroll'
"

//-------------------------------------------------------------------------------------------------

let private plotLog = "
$data << EOD
{0}
EOD

set xlabel 'Number of Plays'
set xrange [0:200]
set xtics scale 0.01, 0.01
set xtics 50
set mxtics 2

set ylabel 'Dollars'
set yrange [{1}:{2}]
set ytics scale 0.01, 0.01
set ytics 10
set mytics 9
set logscale y

set grid xtics ytics mxtics mytics
set grid ls 1 lc '#e6e6e6'
set key box lc '#808080' samplen 1

plot '$data' with lines lc '#ff0000' title 'Gambler''s Bankroll'
"

//-------------------------------------------------------------------------------------------------

let private renderChart plot path (lower : float) (upper : float) data =

    let data =
        data
        |> Array.mapi (fun i x -> sprintf "%i %e" i x)
        |> String.concat "\n"

    render path plot [| data; lower; upper |]

let renderLin = renderChart plotLin
let renderLog = renderChart plotLog
