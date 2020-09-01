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
     '$data' using 1:($5 == '0.00%' && $3 == 0 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor '#80b0e0',\
     '$data' using 1:($5 == '0.00%' && $3 == 1 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor '#8098b0',\
     '$data' using 1:($5 == '0.00%' && $3 == 2 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor '#808080',\
     '$data' using 1:($5 == '0.00%' && $3 == 3 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor '#b08080',\
     '$data' using 1:($5 == '0.00%' && $3 == 4 ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor '#e08080',\
     '$data' using 1:($5 != '0.00%' ? 0.006 : 1/0):5 with labels notitle left rotate by 90 textcolor '#ffffff'
"

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

//-------------------------------------------------------------------------------------------------

let private plotClimb = "
$hilleast << EOD
3.0 0.375 Start
2.0 2.375 Finish
EOD

$hillwest << EOD
1.0 0.375 Start
2.0 2.375 Finish
EOD

$plateast << EOD
3.0 0.375 Start
2.5 1.375 Finish
EOD

$platwest << EOD
1.0 0.375 Start
1.5 1.375 Finish
EOD

set samples 150

hill(x) = (x > 1.0 && x < 3.0) ? 1 + cos(pi * x) : 0
plat(x) = (x > 1.5 && x < 2.5) ? 1 : hill(x)

if ({0} == 1) {{ land(x) = hill(x); peak = 2.25 }}
if ({0} == 2) {{ land(x) = plat(x); peak = 1.25 }}

landprofile(x) = land(x) + 0.25 + ((rand(0) - 0.5) * 0.04)
pathprofile(x) = land(x) + 0.25 + 0.125

set xlabel 'Location'
set xtics scale 0.01, 0.01
set xrange [0.5:3.5]
set xtics ('West' 0.8, 'East' 3.2)

set ylabel 'Elevation'
set yrange [0:2.75]
set ytics ('Base' 0.25 0, 'Peak' peak)

set linetype 1 linewidth 0 linecolor '#ff000000'
set linetype 2 linewidth 0 linecolor '#e0e0e0'
set linetype 3 linewidth 2 linecolor '#808080'
set linetype 4 linewidth 2 linecolor '#ff0000' dashtype 2

seed = -1

if ({0} == 1 && {1} == 1) {{
    plot rand(seed) linetype 1 notitle, landprofile(x) linetype 2 notitle with filledcurve y=0,\
         rand(seed) linetype 1 notitle, landprofile(x) linetype 3 notitle,\
         [3:2.0] pathprofile(x) notitle linetype 4,\
         $hilleast with labels notitle point pointtype 7 offset 0,1
}}

if ({0} == 1 && {1} == 2) {{
    plot rand(seed) linetype 1 notitle, landprofile(x) linetype 2 notitle with filledcurve y=0,\
         rand(seed) linetype 1 notitle, landprofile(x) linetype 3 notitle,\
         [1:2.0] pathprofile(x) notitle linetype 4,\
         $hillwest with labels notitle point pointtype 7 offset 0,1
}}

if ({0} == 2 && {1} == 1) {{
    plot rand(seed) linetype 1 notitle, landprofile(x) linetype 2 notitle with filledcurve y=0,\
         rand(seed) linetype 1 notitle, landprofile(x) linetype 3 notitle,\
         [3:2.5] pathprofile(x) notitle linetype 4,\
         $plateast with labels notitle point pointtype 7 offset 0,1
}}

if ({0} == 2 && {1} == 2) {{
    plot rand(seed) linetype 1 notitle, landprofile(x) linetype 2 notitle with filledcurve y=0,\
         rand(seed) linetype 1 notitle, landprofile(x) linetype 3 notitle,\
         [1:1.5] pathprofile(x) notitle linetype 4,\
         $platwest with labels notitle point pointtype 7 offset 0,1
}}
"

let private renderClimb landform eastwest path =

    render path plotClimb [| landform; eastwest |]

let renderClimbHillEast = renderClimb 1 1
let renderClimbHillWest = renderClimb 1 2
let renderClimbPlatEast = renderClimb 2 1
let renderClimbPlatWest = renderClimb 2 2

//-------------------------------------------------------------------------------------------------

let private plotScores = "
$scores << EOD
{0}
EOD

$optima << EOD
{1} {2} {2:f8}
EOD

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set xlabel 'Coin Bias (+1)'
set xtics scale 0.01, 0.01
set xrange [{3}:{4}]
set xtics ({3}, {4}, {1})
set format x '%0.4f'

set ylabel 'Score'
set ytics scale 0.01, 0.01
set yrange [0:0.8]
set format y '%0.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 1 linecolor '#ff0000'

plot '$scores' using 1:2 with lines title 'Score {5}',\
     '$optima' with labels offset 0,1 point pointtype 2 title 'Optimum'
"

let renderScores path scores (p1, score) (lower, upper) tag =

    let scores =
        scores
        |> Array.map (fun (x, y) -> sprintf "%e %e" x y)
        |> String.concat "\n"

    render path plotScores [| scores; p1; score; lower; upper; tag |]
