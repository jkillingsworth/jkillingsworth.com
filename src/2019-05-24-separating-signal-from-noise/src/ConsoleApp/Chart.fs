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
    proc.StartInfo.FileName <- "gnuplot.exe"
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

let private makeTitle (descriptor : string) =
    let items = descriptor.Split("-")
    let period (item : string) = Char.ToUpper(item.[0]).ToString() + item.Substring(1)
    let symbol (item : string) = if (item.Length = 6) then item.Insert(3, "/") else item
    sprintf "%-18s" <| sprintf "%s (%s)" (symbol items.[2]) (period items.[1])

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

$data1 << EOD
{1}
EOD

title = '{2}'; tunit = '{3}'; lower = {4}; upper = {5}

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics

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
     $data1 using 1:2 with lines title 'Smooth'
"

let renderPrice path data =

    let descriptor, market, smooth, (lower, upper) = data
    let title = makeTitle descriptor
    let tunit = makeTunit descriptor

    let formatMarket = id
    let formatSmooth = Option.defaultValue nan

    let data0 =
        market
        |> Array.mapi (fun i x -> sprintf "%O %O" i (formatMarket x))
        |> String.concat "\n"

    let data1 =
        smooth
        |> Array.mapi (fun i x -> sprintf "%O %O" i (formatSmooth x))
        |> String.concat "\n"

    render path plotPrice [| data0; data1; title; tunit; lower; upper |]

//-------------------------------------------------------------------------------------------------

let private plotNoise = "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

title = '{2}'; tunit = '{3}'

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics

set xlabel sprintf('Time (%s)', tunit)

set ylabel 'Noise'
set format y '%7.4f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key title sprintf('%s', title) left

set linetype 1 linewidth 1 linecolor '#808080'
set linetype 2 linewidth 1 linecolor '#ff0000'

plot $data0 using 1:2 with lines title 'Dither',\
     $data1 using 1:2 with lines notitle
"

let renderNoise path data =

    let descriptor, dither = data
    let title = makeTitle descriptor
    let tunit = makeTunit descriptor

    let zeroval _ _ = 0.0
    let formatValue = Option.defaultValue nan
    let formatZeros = Option.fold zeroval nan

    let data0 =
        dither
        |> Array.mapi (fun i x -> sprintf "%O %O" i (formatValue x))
        |> String.concat "\n"

    let data1 =
        dither
        |> Array.mapi (fun i x -> sprintf "%O %O" i (formatZeros x))
        |> String.concat "\n"

    render path plotNoise [| data0; data1; title; tunit |]

//-------------------------------------------------------------------------------------------------

let private plotProbs = "
$data0 << EOD
{0}
EOD

title = '{1}'; style = '{2}'; sigmas = {3}; µN = {4}; σN = {5}; µL = {6}; bL = {7}

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics

if (style == 1) {{ set xlabel gprintf('Market Price Differences, σ = %0.3te%04T', σN) }}
if (style == 2) {{ set xlabel gprintf('Smooth Price Differences, σ = %0.3te%04T', σN) }}
if (style == 3) {{ set xlabel gprintf('Dither Noise Differences, σ = %0.3te%04T', σN) }}
set xrange [-(sigmas * σN):+(sigmas * σN)]
set xtics(0)
set for [i=+1:+sigmas:+1] xtics add (sprintf('%+iσ', i) i * σN)
set for [i=-1:-sigmas:-1] xtics add (sprintf('%+iσ', i) i * σN)

set ylabel 'Density'
set format y '%7.0f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key title sprintf('%s', title) left

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

let private renderProbs style path data =

    let descriptor, histogram, sigmas, (µN, σN), (µL, bL) = data
    let title = makeTitle descriptor

    let data0 =
        histogram
        |> Array.map (fun (center, amount) -> sprintf "%O %O" center amount)
        |> String.concat "\n"

    render path plotProbs [| data0; title; style; sigmas; µN; σN; µL; bL |]

let renderProbsMarket path data = renderProbs 1 path data
let renderProbsSmooth path data = renderProbs 2 path data
let renderProbsDither path data = renderProbs 3 path data
