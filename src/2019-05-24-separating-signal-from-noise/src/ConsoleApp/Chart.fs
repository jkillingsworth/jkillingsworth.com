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

let private makeTitle (descriptor : string) =
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
$data0 << EOD
{0}
EOD

title = '{1}'; tunit = '{2}'; lower = {3}; upper = {4}

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics
set xtics scale 0.01, 0.01
set ytics scale 0.01, 0.01

set xlabel sprintf('Time (%s)', tunit)

set ylabel 'Price (Log Values)'
set yrange [lower:upper]
set format y '%7.4f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key title sprintf('%s', title) left

set linetype 1 linewidth 1 linecolor '#808080'
set linetype 2 linewidth 1 linecolor '#ff0000'

plot $data0 using 1:2 with lines title 'Market',\
     $data0 using 1:3 with lines title 'Smooth'
"

let renderPrice path data =

    let descriptor, market, smooth, (lower, upper) = data
    let title = makeTitle descriptor
    let tunit = makeTunit descriptor
    let smooth = smooth |> Array.map (Option.defaultValue nan)

    let data0 =
        market
        |> Array.mapi (fun i _ -> sprintf "%i %e %e" i market.[i] smooth.[i])
        |> String.concat "\n"

    render path plotPrice [| data0; title; tunit; lower; upper |]

//-------------------------------------------------------------------------------------------------

let private plotNoise = "
$data0 << EOD
{0}
EOD

title = '{1}'; tunit = '{2}'

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics
set xtics scale 0.01, 0.01
set ytics scale 0.01, 0.01

set xlabel sprintf('Time (%s)', tunit)

set ylabel 'Noise'
set format y '%7.4f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key title sprintf('%s', title) left

set linetype 1 linewidth 1 linecolor '#808080'
set linetype 2 linewidth 1 linecolor '#ff0000'

plot $data0 using 1:2 with lines title 'Dither',\
     $data0 using 1:3 with lines notitle
"

let renderNoise path data =

    let descriptor, dither = data
    let title = makeTitle descriptor
    let tunit = makeTunit descriptor
    let dither = dither |> Array.map (Option.defaultValue nan)
    let baseln = dither |> Array.map (fun x -> if Double.IsNaN(x) then nan else 0.0)

    let data0 =
        dither
        |> Array.mapi (fun i _ -> sprintf "%i %e %e" i dither.[i] baseln.[i])
        |> String.concat "\n"

    render path plotNoise [| data0; title; tunit |]

//-------------------------------------------------------------------------------------------------

let private plotProbs = "
$data0 << EOD
{0}
EOD

title = '{1}'; label = '{2}'; sigmas = {3}; µN = {4}; σN = {5}; µL = {6}; bL = {7}

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics
set xtics scale 0.01, 0.01
set ytics scale 0.01, 0.01

set xlabel sprintf('%s, σ = %s', label, gprintf('%0.3te%04T', σN))
set xrange [-(sigmas * σN):+(sigmas * σN)]
set xtics(0)
set for [i=-sigmas:-1] xtics add (sprintf('%+iσ', i) sprintf('%e', i * σN))
set for [i=+1:+sigmas] xtics add (sprintf('%+iσ', i) sprintf('%e', i * σN))

set ylabel 'Density'
set format y '%7.0f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key title sprintf('%s', title) left width 6

set linetype 1 linewidth 1 linecolor '#c0c0c0'
set linetype 2 linewidth 2 linecolor '#400000ff'
set linetype 3 linewidth 2 linecolor '#40ff0000'
set style fill solid border linecolor '#808080'

set samples 1000

distributionN(x,µ,σ) = (1 / (σ * ((2 * pi) ** 0.5))) * exp(-0.5 * ((x - µ) / σ) ** 2)
distributionL(x,µ,b) = (1 / (2 * b)) * exp(-abs(x - µ) / b)

plot $data0 using 1:2 with boxes title 'Histogram',\
     distributionN(x, µN, σN) title 'Normal',\
     distributionL(x, µL, bL) title 'Laplace'
"

let private renderProbs label path data =

    let descriptor, histogram, sigmas, (µN, σN), (µL, bL) = data
    let title = makeTitle descriptor

    let data0 =
        histogram
        |> Array.map (fun (center, amount) -> sprintf "%e %e" center amount)
        |> String.concat "\n"

    render path plotProbs [| data0; title; label; sigmas; µN; σN; µL; bL |]

let renderProbsMarket path data = renderProbs "Market Price Differences" path data
let renderProbsSmooth path data = renderProbs "Smooth Price Differences" path data
let renderProbsDither path data = renderProbs "Dither Noise Differences" path data
