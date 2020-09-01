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

let private makeTunit (descriptor : string) =
    let items = descriptor.Split("-")
    match items.[1] with
    | "intraday" -> "Minutes"
    | "daily" -> "Days"
    | _ -> failwith "Unrecognized time unit."

//-------------------------------------------------------------------------------------------------

let private plotPrice = "
$data << EOD
{0}
EOD

set xlabel 'Time ({2})'
set xtics scale 0.01, 0.01

set ylabel 'Price (Log Values)'
set ytics scale 0.01, 0.01
set yrange [{3}:{4}]
set format y '%7.4f'

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key title '{1}' left

set linetype 1 linecolor '#808080'
set linetype 2 linecolor '#ff0000'

plot '$data' using 1:2 with lines title 'Market',\
     '$data' using 1:3 with lines title 'Smooth'
"

let renderPrice path data =

    let descriptor, market, smooth, (priceLower, priceUpper) = data
    let label = makeLabel descriptor
    let tunit = makeTunit descriptor
    let smooth = smooth |> Array.map (Option.defaultValue nan)

    let data =
        market
        |> Array.mapi (fun i _ -> sprintf "%i %e %e" i market.[i] smooth.[i])
        |> String.concat "\n"

    render path plotPrice [| data; label; tunit; priceLower; priceUpper |]

//-------------------------------------------------------------------------------------------------

let private plotNoise = "
$data << EOD
{0}
EOD

set xlabel 'Time ({2})'
set xtics scale 0.01, 0.01

set ylabel 'Noise'
set ytics scale 0.01, 0.01
set format y '%7.4f'

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key title '{1}' left

set linetype 1 linecolor '#808080'
set linetype 2 linecolor '#ff0000'

plot '$data' using 1:2 with lines title 'Dither',\
     '$data' using 1:3 with lines notitle
"

let renderNoise path data =

    let descriptor, dither = data
    let label = makeLabel descriptor
    let tunit = makeTunit descriptor
    let dither = dither |> Array.map (Option.defaultValue nan)
    let baseln = dither |> Array.map (fun x -> if Double.IsNaN(x) then nan else 0.0)

    let data =
        dither
        |> Array.mapi (fun i _ -> sprintf "%i %e %e" i dither.[i] baseln.[i])
        |> String.concat "\n"

    render path plotNoise [| data; label; tunit |]

//-------------------------------------------------------------------------------------------------

let private plotProbs = "
$data << EOD
{0}
EOD

set xlabel '{2}, σ = {6:e3}'
set xtics scale 0.01, 0.01
set xrange [-{3}:+{3}]
set xtics ({4})

set ylabel 'Density'
set ytics scale 0.01, 0.01
set format y '%7.0f'

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

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
     distributionN(x, {5}, {6}) title 'Normal',\
     distributionL(x, {7}, {8}) title 'Laplace'
"

let private renderProbs xlabel path data =

    let descriptor, histogram, sigmas, (µN : float, σN : float), (µL : float, bL: float) = data
    let label = makeLabel descriptor
    let xwide = sigmas * σN : float
    let n = int sigmas

    let xtic = function
        | 0 -> "0"
        | i -> sprintf "'%+iσ' %e" i (float i * σN)

    let xtics =
        [| -n .. +n |]
        |> Array.map xtic
        |> Array.reduce (fun l r -> l + ", " + r)

    let data =
        histogram
        |> Array.map (fun (center, amount) -> sprintf "%e %e" center amount)
        |> String.concat "\n"

    render path plotProbs [| data; label; xlabel; xwide; xtics; µN; σN; µL; bL |]

let renderProbsMarket = renderProbs "Market Price Differences"
let renderProbsSmooth = renderProbs "Smooth Price Differences"
let renderProbsDither = renderProbs "Dither Noise Differences"
