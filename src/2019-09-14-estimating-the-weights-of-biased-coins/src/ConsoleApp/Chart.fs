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

set xlabel 'Possible Outcome'
set xrange [-n-2:+n+2]
if (n % 2 == 0) {{ set xtics (0) }} else {{ set xtics () }}
set for [i=+n:+1:-2] xtics add (sprintf('%+i', i) i)
set for [i=-n:-1:+2] xtics add (sprintf('%+i', i) i)

set ylabel 'Probability'
set yrange [0:0.6]
set format y '%0.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 1 linecolor '#808080'
set style fill solid border linecolor '#ffffff'

plot $data0 using 1:2 with boxes title 'Probability Mass',\
     $data0 using 1:(0.024):3 with labels notitle textcolor '#ffffff'
"

let renderPmfunc path pmfunc =

    let n = 4

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

plot $data0 using 1:2 with boxes title 'Coin Bias',\
     $data0 using 1:($3 == '0.00%' ? 0.04 : 1/0):3 with labels notitle textcolor '#607860',\
     $data0 using 1:($3 != '0.00%' ? 0.04 : 1/0):3 with labels notitle textcolor '#ffffff'
"

let renderBiases path biases =

    let n = 4

    let data0 =
        biases
        |> Array.mapi (fun i x -> sprintf "%O %O %s" (i - n) x (percent x))
        |> String.concat "\n"

    render path plotBiases [| data0; n |]

//-------------------------------------------------------------------------------------------------

let private plotTosses = "
$data0 << EOD
{0}
EOD

n = {1}

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics

set xlabel 'Sequence'
set xrange [-1:(2**n)]

set ylabel 'Probability'
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

plot $data0 using 1:($3 == 0 ? $2 : 0):xtic(4) with boxes title '0 Heads, 4 Tails',\
     $data0 using 1:($3 == 1 ? $2 : 0):xtic(4) with boxes title '1 Heads, 3 Tails',\
     $data0 using 1:($3 == 2 ? $2 : 0):xtic(4) with boxes title '2 Heads, 2 Tails',\
     $data0 using 1:($3 == 3 ? $2 : 0):xtic(4) with boxes title '3 Heads, 1 Tails',\
     $data0 using 1:($3 == 4 ? $2 : 0):xtic(4) with boxes title '4 Heads, 0 Tails',\
     $data0 using 1:($5 == '0.00%' && $3 == 0 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor '#80b0e0',\
     $data0 using 1:($5 == '0.00%' && $3 == 1 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor '#8098b0',\
     $data0 using 1:($5 == '0.00%' && $3 == 2 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor '#808080',\
     $data0 using 1:($5 == '0.00%' && $3 == 3 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor '#b08080',\
     $data0 using 1:($5 == '0.00%' && $3 == 4 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor '#e08080',\
     $data0 using 1:($5 != '0.00%' ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor '#ffffff'
"

let renderTosses path tosses =

    let n = 4

    let color s =
        s
        |> Seq.filter (fun x -> x = 'H')
        |> Seq.length

    let data0 =
        tosses
        |> Array.rev
        |> Array.mapi (fun i (s, x) -> sprintf "%O %O %O %s %s" i x (color s) s (percent x))
        |> String.concat "\n"

    render path plotTosses [| data0; n |]
