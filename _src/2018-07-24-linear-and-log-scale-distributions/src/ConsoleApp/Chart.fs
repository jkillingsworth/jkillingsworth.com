﻿module Chart

open System.Diagnostics
open System.IO

//-------------------------------------------------------------------------------------------------

let private plotLin = "
set terminal svg size 720 405 background 'white' font 'Consolas, Monaco, monospace'
set encoding utf8
set output '{0}'

$data << EOD
{1}
EOD

set xlabel 'Possible Outcome'
set xrange [-900:1100]
set xtics scale 0.01, 0.01
set xtics -900, 200
set mxtics 2

set ylabel 'Probability'
set ytics scale 0.01, 0.01
set format y '%0.2f'

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'
set key box linecolor '#808080' samplen 1
set linetype 1 linewidth 5 linecolor '#00c000'
set linetype 2 linewidth 5 linecolor '#ff0000'
set linetype 3 linewidth 5 linecolor '#808080'

plot '$data' using (strcol(1) >= 100 ? $1 : 1/0):2 with impulses title 'Profit',\
     '$data' using (strcol(1) <= 100 ? $1 : 1/0):2 with impulses title 'Loss',\
     '$data' using (strcol(1) == 100 ? $1 : 1/0):2 with impulses title 'Breakeven'
"

//-------------------------------------------------------------------------------------------------

let private plotLog = "
set terminal svg size 720 405 background 'white' font 'Consolas, Monaco, monospace'
set encoding utf8
set output '{0}'

$data << EOD
{1}
EOD

set xlabel 'Possible Outcome'
set xrange [0.001:+10000000]
set xtics scale 0.01, 0.01
set xtics 0.001, 10
set xtics add (0.001,0.01,0.1,1,10,100,'1K' 1000,'10K' 10000,'100K' 100000,'1M' 1000000,'10M' 10000000)
set mxtics default
set logscale x

set ylabel 'Probability'
set ytics scale 0.01, 0.01
set format y '%0.2f'

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'
set key box linecolor '#808080' samplen 1
set linetype 1 linewidth 5 linecolor '#00c000'
set linetype 2 linewidth 5 linecolor '#ff0000'
set linetype 3 linewidth 5 linecolor '#808080'

plot '$data' using (strcol(1) >= 100 ? $1 : 1/0):2 with impulses title 'Profit',\
     '$data' using (strcol(1) <= 100 ? $1 : 1/0):2 with impulses title 'Loss',\
     '$data' using (strcol(1) == 100 ? $1 : 1/0):2 with impulses title 'Breakeven'
"

//-------------------------------------------------------------------------------------------------

let private render plot path data =

    let data =
        data
        |> Array.map (fun x -> sprintf "%3f %11f" (fst x) (snd x))
        |> String.concat "\n"

    let file = Path.GetFullPath(path)
    use proc = new Process()
    proc.StartInfo.FileName <- "gnuplot.exe"
    proc.StartInfo.UseShellExecute <- false
    proc.StartInfo.RedirectStandardInput <- true
    proc.Start() |> ignore
    proc.StandardInput.Write(plot, file, data)
    proc.StandardInput.Flush()

let renderLin = render plotLin
let renderLog = render plotLog