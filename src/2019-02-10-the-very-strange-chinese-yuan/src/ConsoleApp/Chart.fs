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

let private plotPriceLin = "
$data << EOD
{0}
EOD

set xlabel 'Time (Minutes)'
set xtics scale 0.01, 0.01

set ylabel 'Market Price'
set ytics scale 0.01, 0.01
set format y '%1.3f'

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linecolor '#808080'

plot '$data' using 1:2 with lines title '{1}'
"

//-------------------------------------------------------------------------------------------------

let private plotProbsLin = "
$data << EOD
{0}
EOD

set xlabel 'Price Differences (Log Values), σ = {5:e3}'
set xtics scale 0.01, 0.01
set xrange [-{2}:+{2}]
set xtics ({3})

set ylabel 'Density'
set ytics scale 0.01, 0.01
set format y '%5.0f'

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key title '{1}' left width 7

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

//-------------------------------------------------------------------------------------------------

let private plotProbsLog = "
$data << EOD
{0}
EOD

set xlabel 'Price Differences (Log Values), σ = {5:e3}'
set xtics scale 0.01, 0.01
set xrange [-{2}:+{2}]
set xtics ({3})

set ylabel 'Density'
set yrange [1:]
set ytics scale 0.01, 0.01
set format y '  10^{{%+L}}'
set logscale y

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key title '{1}' left width 7

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

//-------------------------------------------------------------------------------------------------


let private makeLabel (descriptor : string) =
    let items = descriptor.Split("-")
    let period (item : string) = Char.ToUpper(item.[0]).ToString() + item.Substring(1)
    let symbol (item : string) = if (item.Length = 6) then item.Insert(3, "/") else item
    sprintf "%-19s" <| sprintf "%s (%s)" (symbol items.[2]) (period items.[1])

//-------------------------------------------------------------------------------------------------

let renderPriceLin path data =

    let descriptor, data = data
    let label = makeLabel descriptor

    let data =
        data
        |> Array.mapi (fun i x -> sprintf "%i %e" i x)
        |> String.concat "\n"

    render path plotPriceLin [| data; label |]

//-------------------------------------------------------------------------------------------------

let private renderProbs plot path data =

    let descriptor, histogram, sigmas, (µN : float, σN : float), (µL : float, bL: float) = data
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

    render path plot [| data; label; xwide; xrange; µN; σN; µL; bL |]

let renderProbsLin = renderProbs plotProbsLin
let renderProbsLog = renderProbs plotProbsLog
