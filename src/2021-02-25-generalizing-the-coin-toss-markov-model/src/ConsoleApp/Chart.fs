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

let private percent x =
    if Double.IsNaN(x) then "" else x.ToString("0.00%;-0.00%;0.00%")

//-------------------------------------------------------------------------------------------------

let private plotPmfunc = "
$data0 << EOD
{0}
EOD

n = {1}

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics
set xtics scale 0.01, 0.01
set ytics scale 0.01, 0.01

set xlabel 'Possible Outcome'
set xrange [-n-2:+n+2]
if (n % 2 == 0) {{ set xtics (0) }} else {{ set xtics () }}
set for [i=+n:+1:-2] xtics add (sprintf('%+i', i) i)
set for [i=-n:-1:+2] xtics add (sprintf('%+i', i) i)

set ylabel 'Probability'
set yrange [0:(n <= 4) ? 0.6 : 0.4]
set format y '%0.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 1 linecolor '#808080'
set style fill solid border linecolor '#ffffff'

if (n <= 4) {{
    plot $data0 using 1:2 with boxes title 'Probability Mass',\
         $data0 using 1:(0.024):3 with labels notitle textcolor '#ffffff'
}} else {{
    plot $data0 using 1:2 with boxes title 'Probability Mass'
}}
"

let renderPmfunc path pmfunc =

    let n = (pmfunc |> Array.length) - 1

    let data0 =
        pmfunc
        |> Array.mapi (fun i x -> sprintf "%O %O %s" (2 * i - n) x (percent x))
        |> String.concat "\n"

    render path plotPmfunc [| data0; n |]

//-------------------------------------------------------------------------------------------------

let private plotBiases = "
$data0 << EOD
{0}
EOD

n = {1}

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics
set xtics scale 0.01, 0.01
set ytics scale 0.01, 0.01

set xlabel 'State'
set xrange [-n:+n]
set xtics (0)
set for [i=+1:+n-1:+1] xtics add (sprintf('%+i', i) i)
set for [i=-1:-n+1:-1] xtics add (sprintf('%+i', i) i)

set ylabel 'Probability of Heads'
set yrange [0:1]
set ytics 0.10
set format y '%0.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 1 linecolor '#80a080'
set style fill solid border linecolor '#ffffff'

if (n <= 4) {{
    plot $data0 using 1:2 with boxes title 'Coin Bias',\
         $data0 using 1:($3 == '0.00%' ? 0.04 : 1/0):3 with labels notitle textcolor '#607860',\
         $data0 using 1:($3 != '0.00%' ? 0.04 : 1/0):3 with labels notitle textcolor '#ffffff'
}} else {{
    plot $data0 using 1:2 with boxes title 'Coin Bias'
}}
"

let renderBiases path biases =

    let n = (biases |> Array.length) / 2

    let data0 =
        biases
        |> Array.mapi (fun i x -> sprintf "%O %O %s" (i - n) x (percent x))
        |> String.concat "\n"

    render path plotBiases [| data0; n |]
