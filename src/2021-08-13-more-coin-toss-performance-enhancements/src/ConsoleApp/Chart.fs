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

let private plotPmfunc = "
$data0 << EOD
{0}
EOD

n = {1}; upper = {2}

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics

set xlabel 'Possible Outcome'
set xrange [-n-2:+n+2]
if (n % 2 == 0) {{ set xtics ('0' 0) }} else {{ set xtics () }}
if (n <= 10) {{
    set for [i=+n:+1:-2] xtics add (sprintf('%+i', i) i)
    set for [i=-n:-1:+2] xtics add (sprintf('%+i', i) i)
}} else {{
    set xtics add -n, n, +n
    set format x '%+0.0f'
}}

set ylabel 'Probability'
set yrange [0:upper]
set format y '%0.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 1 linecolor '#808080'
set style fill solid border linecolor '#ffffff'

plot $data0 using 1:2 with boxes title 'Probability Mass'
"

let renderPmfunc path pmfunc upper =

    let n = (pmfunc |> Array.length) - 1

    let data0 =
        pmfunc
        |> Array.mapi (fun i x -> sprintf "%O %O" (2 * i - n) x)
        |> String.concat "\n"

    render path plotPmfunc [| data0; n; upper |]

//-------------------------------------------------------------------------------------------------

let private plotBiases = "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

n = {2}; style = {3}

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics

set xlabel 'State'
set xrange [-n:+n]
set xtics ('0' 0)
if (n <= 10) {{
    set for [i=+1:+n-1:+1] xtics add (sprintf('%+i', i) i)
    set for [i=-1:-n+1:-1] xtics add (sprintf('%+i', i) i)
}} else {{
    set xtics add -(n - 1), (n - 1), +(n - 1)
    set format x '%+0.0f'
}}

set ylabel 'Probability of Heads'
set yrange [0:1]
set ytics 0.10
set format y '%0.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 1 linecolor '#80a080'
set linetype 2 pointtype 7 linecolor '#a060a0'
set linetype 3 linewidth 2 linecolor '#a060a0'
set linetype 4 linewidth 1 linecolor '#908090'
set style fill solid border linecolor '#ffffff'

if (style == 0 && n <= 10) {{
    x1 = n + 1
    plot $data0 using 1:2 with boxes title 'Coin Bias',\
         $data0 using 1:2:(sprintf('p_%i', $1)) every ::x1::x1 with labels offset -0.5,-1.0 point linetype 2 textcolor '#ffffff' notitle,\
         $data1 using 1:2 with lines notitle
}}

if (style == 1 && n <= 10) {{
    x1 = n + 1
    x2 = n + 2
    plot $data0 using 1:2 with boxes title 'Coin Bias',\
         $data0 using 1:2:(sprintf('p_%i', $1)) every ::x1::x2 with labels offset -0.5,-1.0 point linetype 2 textcolor '#ffffff' notitle,\
         $data1 using 1:2 with lines notitle
}}

if (style == 2 && n <= 10) {{
    x1 = n + 1
    x3 = n + 3
    plot $data0 using 1:2 with boxes title 'Coin Bias',\
         $data0 using 1:2:(sprintf('p_%i', $1)) every ::x1::x3 with labels offset -0.5,-1.0 point linetype 2 textcolor '#ffffff' notitle,\
         $data1 using 1:2 with lines notitle
}}

if (style == 3 && n <= 10) {{
    x1 = n + 1
    x4 = n + 4
    plot $data0 using 1:2 with boxes title 'Coin Bias',\
         $data0 using 1:2:(sprintf('p_%i', $1)) every ::x1::x4 with labels offset -0.5,-1.0 point linetype 2 textcolor '#ffffff' notitle,\
         $data1 using 1:2 with lines notitle
}}

if (style == 0 && n > 10) {{
    plot $data0 using 1:(1 <= $1 && $1 <= 1 ? 0 : $2) with boxes title 'Coin Bias',\
         $data0 using 1:(1 <= $1 && $1 <= 1 ? $2 : 0) with boxes notitle linetype 4,\
         $data1 using 1:2 with lines notitle
}}

if (style == 1 && n > 10) {{
    plot $data0 using 1:(1 <= $1 && $1 <= 2 ? 0 : $2) with boxes title 'Coin Bias',\
         $data0 using 1:(1 <= $1 && $1 <= 2 ? $2 : 0) with boxes notitle linetype 4,\
         $data1 using 1:2 with lines notitle
}}

if (style == 2 && n > 10) {{
    plot $data0 using 1:(1 <= $1 && $1 <= 3 ? 0 : $2) with boxes title 'Coin Bias',\
         $data0 using 1:(1 <= $1 && $1 <= 3 ? $2 : 0) with boxes notitle linetype 4,\
         $data1 using 1:2 with lines notitle
}}

if (style == 3 && n > 10) {{
    plot $data0 using 1:(1 <= $1 && $1 <= 4 ? 0 : $2) with boxes title 'Coin Bias',\
         $data0 using 1:(1 <= $1 && $1 <= 4 ? $2 : 0) with boxes notitle linetype 4,\
         $data1 using 1:2 with lines notitle
}}
"

let renderBiases path biases points style =

    let n = (biases |> Array.length) / 2

    let data0 =
        biases
        |> Array.mapi (fun i x -> sprintf "%O %O" (i - n) x)
        |> String.concat "\n"

    let data1 =
        points
        |> Array.map (fun (x, y) -> sprintf "%O %O" x y)
        |> String.concat "\n"

    render path plotBiases [| data0; data1; n; style |]

//-------------------------------------------------------------------------------------------------

let private plotFlopCounts = "
$data0 << EOD
{0}
EOD

n = {1}

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics

set xlabel 'Coin Tosses (n)'
set xtics 10
set xtics add (n)

set ylabel 'Floating Point Operations'
set ytics 10
set mytics 1
set logscale y 10
set format y '  10^{{%T}}'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 1.5 linecolor '#00c000'
set linetype 2 linewidth 1.5 linecolor '#d000d0'

plot $data0 using 1:2 with lines title 'Baseline Method',\
     $data0 using 1:3 with lines title 'Enhanced Method'
"

let renderFlopCounts path counts n =

    let data0 =
        counts
        |> Array.mapi (fun i (bm, em) -> sprintf "%O %O %O" i bm em)
        |> String.concat "\n"

    render path plotFlopCounts [| data0; n |]
