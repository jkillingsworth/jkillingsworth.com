module Chart

open System
open Common.Chart

//-------------------------------------------------------------------------------------------------

let private makeTitle (descriptor : string) =
    let items = descriptor.Split("-")
    let period (item : string) = Char.ToUpper(item.[0]).ToString() + item.Substring(1)
    let symbol (item : string) = if (item.Length = 6) then item.Insert(3, "/") else item
    sprintf "%-18s" <| sprintf "%s (%s)" (symbol items.[2]) (period items.[1])

//-------------------------------------------------------------------------------------------------

let private plotPrice = baseplotRegular + "
$data0 << EOD
{0}
EOD

title = '{1}'

set xlabel 'Time (Days)'

set ylabel 'Market Price (Log Values)'
set format y '%5.2f'

set key top left
set key reverse Left

set linetype 1 linewidth 1 linecolor '#808080'

plot $data0 using 1:2 with lines title sprintf('%s', title)
"

let renderPrice path data =

    let descriptor, items = data
    let title = makeTitle descriptor

    let data0 =
        items
        |> Array.mapi (fun i x -> sprintf "%O %O" i x)
        |> String.concat "\n"

    render path plotPrice [| data0; title |]

//-------------------------------------------------------------------------------------------------

let private plotDiffs = baseplotRegular + "
$data0 << EOD
{0}
EOD

title = '{1}'

set xlabel 'Time (Days)'

set ylabel 'Price Differences (Log Values)'
set ytics add ('0.00' 0)
set format y '%+5.2f'

set key top left
set key reverse Left

set linetype 1 linewidth 1 linecolor '#808080'

plot $data0 using 1:2 with impulses title sprintf('%s', title)
"

let renderDiffs path data =

    let descriptor, items = data
    let title = makeTitle descriptor

    let data0 =
        items
        |> Array.mapi (fun i x -> sprintf "%O %O" i x)
        |> String.concat "\n"

    render path plotDiffs [| data0; title |]

//-------------------------------------------------------------------------------------------------

let private plotProbs = baseplotRegular + "
$data0 << EOD
{0}
EOD

title = '{1}'; sigmas = {2}; μN = {3}; σN = {4}; μL = {5}; bL = {6}

set xlabel gprintf('Price Differences (Log Values), σ = %0.3te%04T', σN)
set xrange [-(sigmas * σN):+(sigmas * σN)]
set xtics (0)
set for [i=+1:+sigmas:+1] xtics add (sprintf('%+iσ', i) i * σN)
set for [i=-1:-sigmas:-1] xtics add (sprintf('%+iσ', i) i * σN)

set ylabel 'Density'
set format y '%5.0f'

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

let renderProbs path data =

    let descriptor, histogram, sigmas, (μN, σN), (μL, bL) = data
    let title = makeTitle descriptor

    let data0 =
        histogram
        |> Array.map (fun (center, amount) -> sprintf "%O %O" center amount)
        |> String.concat "\n"

    render path plotProbs [| data0; title; sigmas; μN; σN; μL; bL |]
