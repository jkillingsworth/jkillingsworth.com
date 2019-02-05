module Chart

open System
open System.Diagnostics
open System.IO
open System.Text

//-------------------------------------------------------------------------------------------------

let private plotPrice = "
set terminal svg size 720 405 background 'white' font 'Consolas, Monaco, monospace'
set encoding utf8
set output '{0}'

$data << EOD
{1}
EOD

set xlabel 'Time (Days)'
set xtics scale 0.01, 0.01

set ylabel 'Market Price (Log Scale)'
set ytics scale 0.01, 0.01
set format y '% 1.2f'

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set key box linecolor '#808080' samplen 1 opaque
set key top left reverse Left

set linetype 1 linecolor '#808080'

plot '$data' using 1:2 with lines title '{2}'
"

//-------------------------------------------------------------------------------------------------

let private plotDiffs = "
set terminal svg size 720 405 background 'white' font 'Consolas, Monaco, monospace'
set encoding utf8
set output '{0}'

$data << EOD
{1}
EOD

set xlabel 'Time (Days)'
set xtics scale 0.01, 0.01

set ylabel 'Price Differences (Log Scale)'
set ytics scale 0.01, 0.01
set format y '% 1.2f'

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set key box linecolor '#808080' samplen 1 opaque
set key top left reverse Left

set linetype 1 linecolor '#808080'

plot '$data' using 1:2 with impulses title '{2}'
"

//-------------------------------------------------------------------------------------------------

let private plotProbs = "
set terminal svg size 720 405 background 'white' font 'Consolas, Monaco, monospace'
set encoding utf8
set output '{0}'

$data << EOD
{1}
EOD

set xlabel 'Price Differences (Log Scale), {{/symbol s}} = {6:e3}'
set xtics scale 0.01, 0.01
set xrange [-{3}:+{3}]
set xtics ({4})

set ylabel 'Density'
set ytics scale 0.01, 0.01
set format y '%5.0f'

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set key box linecolor '#808080' samplen 1 opaque
set key top left reverse Left
set key title '{2}' left width 6

set linetype 1 linewidth 1 linecolor '#c0c0c0'
set linetype 2 linewidth 2 linecolor '#400000ff'
set linetype 3 linewidth 2 linecolor '#40ff0000'

set style fill solid border linecolor '#808080'

set samples 1000
distributionN(x,µ,σ) = (1 / (σ * ((2 * pi) ** 0.5))) * exp(-0.5 * ((x - µ) / σ) ** 2)
distributionL(x,µ,b) = (1 / (2 * b)) * exp(-abs(x - µ) / b)

plot '$data' using 1:2 with boxes title 'Histogram',\
     distributionN(x, {5}, {6}) title 'Normal',\
     distributionL(x, {7}, {8}) title 'Laplace'
"

//-------------------------------------------------------------------------------------------------

let private render plot path (args : obj[]) =

    let file = Path.GetFullPath(path)
    use proc = new Process()
    proc.StartInfo.FileName <- "gnuplot.exe"
    proc.StartInfo.UseShellExecute <- false
    proc.StartInfo.RedirectStandardInput <- true
    proc.StartInfo.StandardInputEncoding <- new UTF8Encoding()
    proc.Start() |> ignore
    proc.StandardInput.Write(plot, args |> Array.append [| file |])
    proc.StandardInput.Flush()

let private makeLabel (descriptor : string) =
    let items = descriptor.Split("-")
    let period (item : string) = Char.ToUpper(item.[0]).ToString() + item.Substring(1)
    let symbol (item : string) = if (item.Length = 6) then item.Insert(3, "/") else item
    sprintf "%-15s" <| sprintf "%s (%s)" (symbol items.[2]) (period items.[1])

//-------------------------------------------------------------------------------------------------

let renderPrice path data =

    let descriptor, data = data
    let label = makeLabel descriptor

    let data =
        data
        |> Array.mapi (fun i x -> sprintf "%i %f" i x)
        |> String.concat "\n"

    render plotPrice path [| data; label |]

//-------------------------------------------------------------------------------------------------

let renderDiffs path data =

    let descriptor, data = data
    let label = makeLabel descriptor

    let data =
        data
        |> Array.mapi (fun i x -> sprintf "%i %f" i x)
        |> String.concat "\n"

    render plotDiffs path [| data; label |]

//-------------------------------------------------------------------------------------------------

let renderProbs path data =

    let descriptor, histogram, sigmas, (µN, σN), (µL, bL) = data
    let label = makeLabel descriptor
    let xwide = sigmas * σN : float
    let n = int sigmas

    let xlabel = function
        | 0 -> "0"
        | i -> sprintf "'%+i{{/symbol s}}' %f" i (float i * σN)

    let xrange =
        [| -n .. +n |]
        |> Array.map xlabel
        |> Array.reduce (fun l r -> l + ", " + r)

    let data =
        histogram
        |> Array.map (fun (center, amount) -> sprintf "%f %f" center amount)
        |> String.concat "\n"

    render plotProbs path [| data; label; xwide; xrange; µN; σN; µL; bL |]
