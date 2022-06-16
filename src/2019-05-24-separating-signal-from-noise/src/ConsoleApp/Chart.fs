module Chart

open System
open Common.Chart

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

let private plotPrice = baseplotRegular + "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

title = '{2}'; tunit = '{3}'; lower = {4}; upper = {5}

set xlabel sprintf('Time (%s)', tunit)

set ylabel 'Price (Log Values)'
set yrange [lower:upper]
set format y '%7.4f'

set key top left
set key reverse Left
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

let private plotNoise = baseplotRegular + "
$data0 << EOD
{0}
EOD

$data1 << EOD
{1}
EOD

title = '{2}'; tunit = '{3}'

set xlabel sprintf('Time (%s)', tunit)

set ylabel 'Noise'
set format y '%7.4f'

set key top left
set key reverse Left
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

let private plotProbs = baseplotRegular + "
$data0 << EOD
{0}
EOD

title = '{1}'; style = '{2}'; sigmas = {3}; μN = {4}; σN = {5}; μL = {6}; bL = {7}

if (style == 1) {{ set xlabel gprintf('Market Price Differences, σ = %0.3te%04T', σN) }}
if (style == 2) {{ set xlabel gprintf('Smooth Price Differences, σ = %0.3te%04T', σN) }}
if (style == 3) {{ set xlabel gprintf('Dither Noise Differences, σ = %0.3te%04T', σN) }}
set xrange [-(sigmas * σN):+(sigmas * σN)]
set xtics(0)
set for [i=+1:+sigmas:+1] xtics add (sprintf('%+iσ', i) i * σN)
set for [i=-1:-sigmas:-1] xtics add (sprintf('%+iσ', i) i * σN)

set ylabel 'Density'
set format y '%7.0f'

set key top left
set key reverse Left
set key title sprintf('%s', title) left

set linetype 1 linewidth 1 linecolor '#c0c0c0'
set linetype 2 linewidth 2 linecolor '#400000ff'
set linetype 3 linewidth 2 linecolor '#40ff0000'
set style fill solid border linecolor '#808080'

set samples 1000

distributionN(x,μ,σ) = (1 / (σ * ((2 * pi) ** 0.5))) * exp(-0.5 * ((x - μ) / σ) ** 2)
distributionL(x,μ,b) = (1 / (2 * b)) * exp(-abs(x - μ) / b)

plot $data0 using 1:2 with boxes title 'Histogram',\
     distributionN(x, μN, σN) title 'Normal',\
     distributionL(x, μL, bL) title 'Laplace'
"

let private renderProbs style path data =

    let descriptor, histogram, sigmas, (μN, σN), (μL, bL) = data
    let title = makeTitle descriptor

    let data0 =
        histogram
        |> Array.map (fun (center, amount) -> sprintf "%O %O" center amount)
        |> String.concat "\n"

    render path plotProbs [| data0; title; style; sigmas; μN; σN; μL; bL |]

let renderProbsMarket path data = renderProbs 1 path data
let renderProbsSmooth path data = renderProbs 2 path data
let renderProbsDither path data = renderProbs 3 path data
