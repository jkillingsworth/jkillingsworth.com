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

let private makeLabel (descriptor : string) =
    let items = descriptor.Split("-")
    let period (item : string) = Char.ToUpper(item.[0]).ToString() + item.Substring(1)
    let symbol (item : string) = if (item.Length = 6) then item.Insert(3, "/") else item
    sprintf "%-15s" <| sprintf "%s (%s)" (symbol items.[2]) (period items.[1])

//-------------------------------------------------------------------------------------------------

let private plotPrice = "
$data << EOD
{0}
EOD

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics
set xtics scale 0.01, 0.01
set ytics scale 0.01, 0.01

set xlabel 'Time (Days)'

set ylabel 'Market Price (Log Values)'
set format y '% 1.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 1 linecolor '#808080'

plot '$data' using 1:2 with lines title '{1}'
"

let renderPrice path data =

    let descriptor, data = data
    let label = makeLabel descriptor

    let data =
        data
        |> Array.mapi (fun i x -> sprintf "%i %e" i x)
        |> String.concat "\n"

    render path plotPrice [| data; label |]

//-------------------------------------------------------------------------------------------------

let private plotDiffs = "
$data << EOD
{0}
EOD

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics
set xtics scale 0.01, 0.01
set ytics scale 0.01, 0.01

set xlabel 'Time (Days)'

set ylabel 'Price Differences (Log Values)'
set format y '% 1.2f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 1 linecolor '#808080'

plot '$data' using 1:2 with impulses title '{1}'
"

let renderDiffs path data =

    let descriptor, data = data
    let label = makeLabel descriptor

    let data =
        data
        |> Array.mapi (fun i x -> sprintf "%i %e" i x)
        |> String.concat "\n"

    render path plotDiffs [| data; label |]

//-------------------------------------------------------------------------------------------------

let private plotProbs = "
$data << EOD
{0}
EOD

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics
set xtics scale 0.01, 0.01
set ytics scale 0.01, 0.01

set xlabel 'Price Differences (Log Values), σ = {5:e3}'
set xrange [-{2}:+{2}]
set xtics ({3})

set ylabel 'Density'
set format y '%5.0f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key title '{1}' left width 6

set linetype 1 linewidth 1 linecolor '#c0c0c0'
set linetype 2 linewidth 2 linecolor '#400000ff'
set linetype 3 linewidth 2 linecolor '#40ff0000'
set style fill solid border linecolor '#808080'

set samples 1000

distributionN(x,µ,σ) = (1 / (σ * ((2 * pi) ** 0.5))) * exp(-0.5 * ((x - µ) / σ) ** 2)
distributionL(x,µ,b) = (1 / (2 * b)) * exp(-abs(x - µ) / b)

plot '$data' using 1:2 with boxes title 'Histogram',\
     distributionN(x, {4}, {5}) title 'Normal',\
     distributionL(x, {6}, {7}) title 'Laplace'
"

let renderProbs path data =

    let descriptor, histogram, sigmas, (µN, σN), (µL, bL) = data
    let label = makeLabel descriptor
    let xwide = sigmas * σN : float
    let n = int sigmas

    let xlabel = function
        | 0 -> "0"
        | i -> sprintf "'%+iσ' %e" i (float i * σN)

    let xrange =
        [| -n .. +n |]
        |> Array.map xlabel
        |> Array.reduce (fun l r -> l + ", " + r)

    let data =
        histogram
        |> Array.map (fun (center, amount) -> sprintf "%e %e" center amount)
        |> String.concat "\n"

    render path plotProbs [| data; label; xwide; xrange; µN; σN; µL; bL |]
