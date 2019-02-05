module Chart

open System.Diagnostics
open System.IO
open System.Text

//-------------------------------------------------------------------------------------------------

let private plotLin = "
set terminal svg size 720 405 background 'white' font 'Consolas, Monaco, monospace'
set encoding utf8
set output '{0}'

$data << EOD
{1}
EOD

set xlabel 'Number of Plays'
set xrange [0:200]
set xtics scale 0.01, 0.01
set xtics 50
set mxtics 2

set ylabel 'Dollars'
set yrange [{2}:{3}]
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
set terminal svg size 720 405 background 'white' font 'Consolas, Monaco, monospace'
set encoding utf8
set output '{0}'

$data << EOD
{1}
EOD

set xlabel 'Number of Plays'
set xrange [0:200]
set xtics scale 0.01, 0.01
set xtics 50
set mxtics 2

set ylabel 'Dollars'
set yrange [{2}:{3}]
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

let private render plot path (lower : float) (upper : float) data =

    let data =
        data
        |> Array.mapi (fun i x -> sprintf "%i %e" i x)
        |> String.concat "\n"

    let file = Path.GetFullPath(path)
    use proc = new Process()
    proc.StartInfo.FileName <- "gnuplot.exe"
    proc.StartInfo.UseShellExecute <- false
    proc.StartInfo.RedirectStandardInput <- true
    proc.StartInfo.StandardInputEncoding <- new UTF8Encoding()
    proc.Start() |> ignore
    proc.StandardInput.Write(plot, file, data, lower, upper)
    proc.StandardInput.Flush()

let renderLin = render plotLin
let renderLog = render plotLog
