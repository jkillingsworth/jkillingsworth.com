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

let private plotBiases = "
$data << EOD
{0}
EOD

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set xlabel 'State'
set xtics scale 0.01, 0.01
set xtics ({2})
set xrange [-{1}:+{1}]

set ylabel 'Probability of Heads'
set ytics scale 0.01, 0.01
set ytics 0.10
set yrange [0:1]
set format y '%0.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 1 linecolor '#80a080'
set style fill solid border linecolor '#ffffff'

plot '$data' using 1:2 with boxes title 'Coin Bias',\
     '$data' using 1:($3 == '0.00%' ? 1/0 : 0.04):3 with labels notitle center rotate by 0 textcolor '#ffffff',\
     '$data' using 1:($3 != '0.00%' ? 1/0 : 0.04):3 with labels notitle center rotate by 0 textcolor '#607860'
"

//-------------------------------------------------------------------------------------------------

let private plotPmfunc = "
$data << EOD
{0}
EOD

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set xlabel 'Possible Outcome'
set xtics scale 0.01, 0.01
set xtics ({2})
set xrange [-{1}-2:+{1}+2]

set ylabel 'Probability'
set ytics scale 0.01, 0.01
set yrange [0:0.6]
set format y '%0.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 1 linecolor '#808080'
set style fill solid border linecolor '#ffffff'

plot '$data' using 1:2 with boxes title 'Probability Mass',\
     '$data' using 1:(0.024):3 with labels notitle center rotate by 0 textcolor '#ffffff'
"

//-------------------------------------------------------------------------------------------------

let private plotTosses = "
$data << EOD
{0}
EOD

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set xlabel 'Sequence'
set xtics scale 0.01, 0.01
set xrange [-1:(2**{1})]

set ylabel 'Probability'
set ytics scale 0.01, 0.01
set yrange [0:0.20]
set format y '%0.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 1 linecolor '#80b0e0'
set linetype 2 linewidth 1 linecolor '#8098b0'
set linetype 3 linewidth 1 linecolor '#808080'
set linetype 4 linewidth 1 linecolor '#b08080'
set linetype 5 linewidth 1 linecolor '#e08080'
set style fill solid border linecolor '#ffffff'

plot '$data' using 1:($3 == 0 ? $4 : 0):xtic(2) with boxes title '0 Heads, 4 Tails',\
     '$data' using 1:($3 == 1 ? $4 : 0):xtic(2) with boxes title '1 Heads, 3 Tails',\
     '$data' using 1:($3 == 2 ? $4 : 0):xtic(2) with boxes title '2 Heads, 2 Tails',\
     '$data' using 1:($3 == 3 ? $4 : 0):xtic(2) with boxes title '3 Heads, 1 Tails',\
     '$data' using 1:($3 == 4 ? $4 : 0):xtic(2) with boxes title '4 Heads, 0 Tails',\
     '$data' using 1:(0.006):5 with labels notitle left rotate by 90 textcolor '#ffffff'
"

//-------------------------------------------------------------------------------------------------

let private percent x =
    if Double.IsNaN(x) then "" else sprintf "%.2f%%" (x * 100.0)

//-------------------------------------------------------------------------------------------------

let renderBiases path data =

    let n = 4

    let xtic = function
        | 0 -> "0"
        | i -> sprintf "'%+i' %i" i i

    let xtics =
        [| -(n - 1) .. 1 .. +(n - 1) |]
        |> Array.map xtic
        |> Array.reduce (fun l r -> l + ", " + r)

    let data =
        data
        |> Array.mapi (fun i x -> sprintf "%i %e %s" (i - n) x (percent x))
        |> String.concat "\n"

    render path plotBiases [| data; n; xtics |]

//-------------------------------------------------------------------------------------------------

let renderPmfunc path data =

    let n = 4

    let xtic = function
        | 0 -> "0"
        | i -> sprintf "'%+i' %i" i i

    let xtics =
        [| -n .. 2 .. +n |]
        |> Array.map xtic
        |> Array.reduce (fun l r -> l + ", " + r)

    let data =
        data
        |> Array.mapi (fun i x -> sprintf "%i %e %s" (2 * i - n) x (percent x))
        |> String.concat "\n"

    render path plotPmfunc [| data; n; xtics |]

//-------------------------------------------------------------------------------------------------

let renderTosses path data =

    let n = 4

    let color s =
        s
        |> Seq.filter (fun x -> x = 'H')
        |> Seq.length

    let data =
        data
        |> Array.mapi (fun i (s, r) -> sprintf "%i %s %i %e %s" i s (color s) r (percent r))
        |> String.concat "\n"

    render path plotTosses [| data; n |]