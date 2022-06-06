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

let private plotSimulations = "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

$data2 << EOD
{2}
EOD

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics
set xzeroaxis linestyle 1 linewidth 2 linecolor '#e6e6e6'
set yzeroaxis linestyle 1 linewidth 2 linecolor '#e6e6e6'

set xlabel 'Number of Plays'

set ylabel 'Dollars'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key opaque

set linetype 1 linewidth 2 linecolor '#ff4040'
set linetype 2 linewidth 2 linecolor '#40c040'
set linetype 3 linewidth 2 linecolor '#4080ff'

plot $data0 using 1:2 with lines title 'Simulation A',\
     $data1 using 1:2 with lines title 'Simulation B',\
     $data2 using 1:2 with lines title 'Simulation C'
"

let renderSimulations path vsA vsB vsC =

    let data0 =
        vsA
        |> Array.mapi (fun i x -> sprintf "%O %O" i x)
        |> String.concat "\n"

    let data1 =
        vsB
        |> Array.mapi (fun i x -> sprintf "%O %O" i x)
        |> String.concat "\n"

    let data2 =
        vsC
        |> Array.mapi (fun i x -> sprintf "%O %O" i x)
        |> String.concat "\n"

    render path plotSimulations [| data0; data1; data2 |]

//-------------------------------------------------------------------------------------------------

let private plotGs = "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

xLower = {2}; xUpper = {3}; yLower = {4}; yUpper = {5}

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics
set xzeroaxis linestyle 1 linewidth 2 linecolor '#e6e6e6'
set yzeroaxis linestyle 1 linewidth 2 linecolor '#e6e6e6'

set xlabel 'Bet Size'
set xrange [xLower:xUpper]
set xtics add ('0.0' 0)
set format x '%+4.1f'

set ylabel 'Growth Rate (Log Values)'
set yrange [yLower:yUpper]
set ytics add ('0.00' 0)
set format y '%+5.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key opaque

set linetype 1 linewidth 2 linecolor '#a040c0'
set linetype 2 pointtype 7 linecolor '#a040c0'

plot $data0 using 1:2 with lines title 'EV',\
     $data1 using 1:2 with points title 'Maximum'
"

let renderGs path gRange bs gs (b', g')=

    let data0 =
        gs
        |> Array.zip bs
        |> Array.map (fun (b, g) -> sprintf "%O %O" b g)
        |> String.concat "\n"

    let data1 = sprintf "%O %O" b' g'

    let xLower = bs |> Array.min
    let xUpper = bs |> Array.max
    let yLower = fst gRange
    let yUpper = snd gRange

    render path plotGs [| data0; data1; xLower; xUpper; yLower; yUpper |]

//-------------------------------------------------------------------------------------------------

let private plotPs = "
$data0 << EOD
{0}
EOD

xLower = {1}; xUpper = {2}; yLower = {3}; yUpper = {4}

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics
set xzeroaxis linestyle 1 linewidth 2 linecolor '#e6e6e6'
set yzeroaxis linestyle 1 linewidth 2 linecolor '#e6e6e6'

set xlabel 'Price'
set xrange [xLower:xUpper]
set xtics 1

set ylabel 'Probability Density'
set yrange [yLower:yUpper]
set ytics 0.025
set format y '%5.3f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key opaque
set key width -3

set linetype 1 linewidth 2 linecolor '#4080ff'
set linetype 2 linewidth 2 linecolor '#4080ff'

plot $data0 using 1:2 with lines title 'Log-Normal Distribution',\
     $data0 using 1:2 with filledcurves y=0 fill solid 0.125 noborder notitle
"

let renderPs path pRange xs ps =

    let data0 =
        ps
        |> Array.zip xs
        |> Array.map (fun (x, p) -> sprintf "%O %O" x p)
        |> String.concat "\n"

    let xLower = xs |> Array.min
    let xUpper = xs |> Array.max
    let yLower = fst pRange
    let yUpper = snd pRange

    render path plotPs [| data0; xLower; xUpper; yLower; yUpper |]

//-------------------------------------------------------------------------------------------------

let private plotWs = "
$data0 << EOD
{0}
EOD

xLower = {1}; xUpper = {2}; yLower = {3}; yUpper = {4}

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics
set xzeroaxis linestyle 1 linewidth 2 linecolor '#e6e6e6'
set yzeroaxis linestyle 1 linewidth 2 linecolor '#e6e6e6'

set xlabel 'Price'
set xrange [xLower:xUpper]
set xtics 1

set ylabel 'Payoff'
set yrange [yLower:yUpper]
set ytics 0.05
set ytics add ('0.00' 0)
set mytics 2
set format y '%+5.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key opaque

set linetype 1 linewidth 2 linecolor '#40c040'
set linetype 2 linewidth 2 linecolor '#ff4040'

plot $data0 using 1:($2 >= 0 ? $2 : 1/0) with lines title 'Positive',\
     $data0 using 1:($2 <= 0 ? $2 : 1/0) with lines title 'Negative'
"

let renderWs path wRange xs ws =

    let data0 =
        ws
        |> Array.zip xs
        |> Array.map (fun (x, w) -> sprintf "%O %O" x w)
        |> String.concat "\n"

    let xLower = xs |> Array.min
    let xUpper = xs |> Array.max
    let yLower = fst wRange
    let yUpper = snd wRange

    render path plotWs [| data0; xLower; xUpper; yLower; yUpper |]

//-------------------------------------------------------------------------------------------------

let private plotLimitations = "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

xLower = {2}; xUpper = {3}; c = {4}

stats $data0 using 1:3 nooutput prefix 'data0'
bMin = data0_min_x

stats $data1 using 1:3 nooutput prefix 'data1'
bMax = data1_max_x

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics
set xzeroaxis linestyle 1 linewidth 2 linecolor '#e6e6e6'
set yzeroaxis linestyle 1 linewidth 2 linecolor '#e6e6e6'

set xlabel 'Bet Size'
set xrange [xLower:xUpper]
set xtics 1
set xtics add ('0.0' 0)
set format x '%+4.1f'

set ylabel 'Price'
set yrange [0:2*c]
set ytics 5
set format y '%5.0f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key opaque

set linetype 1 linewidth 2 linecolor '#c0c0c0'
set linetype 2 linewidth 2 linecolor '#40c040'
set linetype 3 linewidth 2 linecolor '#ff4040'

plot $data0 using ($1 <= 0 ? $1 : 1/0):2 linetype 1 with lines notitle,\
     $data1 using ($1 >= 1 ? $1 : 1/0):2 linetype 1 with lines notitle,\
     $data0 using ($1 <= 0 ? $1 : 1/0):3 linetype 2 with lines notitle,\
     $data1 using ($1 >= 1 ? $1 : 1/0):3 linetype 3 with lines notitle,\
     $data0 using ($1 <= bMin || $1 >= bMax ? $1 : 1/0):2:4 linetype 1 with filledcurves fill solid 0.125 notitle,\
     $data1 using ($1 <= bMin || $1 >= bMax ? $1 : 1/0):2:4 linetype 1 with filledcurves fill solid 0.125 notitle,\
     $data0 using ($1 >= bMin && $1 <= bMax ? $1 : 1/0):2:3 linetype 1 with filledcurves fill solid 0.125 notitle,\
     $data1 using ($1 >= bMin && $1 <= bMax ? $1 : 1/0):2:3 linetype 1 with filledcurves fill solid 0.125 notitle,\
     $data0 using ($1 >= bMin && $1 <= bMax ? $1 : 1/0):3:4 linetype 2 with filledcurves fill solid 0.125 title 'Upper (s)',\
     $data1 using ($1 >= bMin && $1 <= bMax ? $1 : 1/0):3:4 linetype 3 with filledcurves fill solid 0.125 title 'Lower (r)'
"

let private renderLimitations path bs rMins0 sMaxs0 rMinsM sMaxsM c =

    let sMaxs0 = sMaxs0 |> Array.map (min 1e+6)
    let sMaxsM = sMaxsM |> Array.map (min 1e+6)

    let data0 =
        sMaxsM
        |> Array.zip3 bs sMaxs0
        |> Array.map (fun (b, s0, sM) -> sprintf "%O %O %O %O" b s0 sM c)
        |> String.concat "\n"

    let data1 =
        rMinsM
        |> Array.zip3 bs rMins0
        |> Array.map (fun (b, r0, rM) -> sprintf "%O %O %O %O" b r0 rM c)
        |> String.concat "\n"

    let xLower = bs |> Array.min
    let xUpper = bs |> Array.max

    render path plotLimitations [| data0; data1; xLower; xUpper; c |]

let renderLimitations1 path bs rMins sMaxs c =

    let sMaxs0 = sMaxs |> Array.map (fun _ -> nan)
    let rMins0 = rMins |> Array.map (fun _ -> nan)
    let sMaxsM = sMaxs
    let rMinsM = rMins

    renderLimitations path bs rMins0 sMaxs0 rMinsM sMaxsM c

let renderLimitations2 path bs rMins sMaxs c =

    let sMaxs0 = fst sMaxs
    let rMins0 = fst rMins
    let sMaxsM = snd sMaxs
    let rMinsM = snd rMins

    renderLimitations path bs rMins0 sMaxs0 rMinsM sMaxsM c
