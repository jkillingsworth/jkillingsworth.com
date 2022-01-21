﻿module Chart

open System
open System.Diagnostics
open System.IO
open System.Text

//-------------------------------------------------------------------------------------------------

let private preamble = "
set terminal svg size 720 405 font 'Consolas, Menlo, monospace'
set encoding utf8
set output '{0}'

set xtics scale 0, 0.0001
set ytics scale 0, 0.0001
"

let private terminal = "
exit
"

let private render path template args =

    let preamble = String.Format(preamble, Path.GetFullPath(path))
    let template = String.Format(template, args)
    let plot = preamble + template + terminal
    use proc = new Process()
    proc.StartInfo.FileName <- Environment.GetEnvironmentVariable("GNUPLOT_EXE")
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

let private plotBankroll = "
$data0 << EOD
{0}
EOD

style = {1}; lower = {2}; upper = {3}

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics

set xlabel 'Number of Plays'
set xrange [0:200]
set xtics 50
set mxtics 2

if (style == 1) {{
    set ylabel 'Dollars'
    set yrange [lower:upper]
    set ytics 100
    set mytics 5
}}

if (style == 2) {{
    set ylabel 'Dollars'
    set yrange [lower:upper]
    set ytics 10
    set mytics 9
    set logscale y 10
}}

set key box linecolor '#808080' samplen 1
set key top right noreverse Right

set linetype 1 linewidth 1 linecolor '#ff0000'

plot $data0 using 1:2 with lines title 'Gambler''s Bankroll'
"

let private renderBankroll style path lower upper items =

    let data0 =
        items
        |> Array.mapi (fun i x -> sprintf "%O %O" i x)
        |> String.concat "\n"

    render path plotBankroll [| data0; style; lower; upper |]

let renderBankrollLin lower upper items = renderBankroll 1 lower upper items
let renderBankrollLog lower upper items = renderBankroll 2 lower upper items
