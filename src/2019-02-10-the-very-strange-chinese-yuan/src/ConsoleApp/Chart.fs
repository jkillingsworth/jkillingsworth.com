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

let private makeTitle (descriptor : string) =
    let items = descriptor.Split("-")
    let period (item : string) = Char.ToUpper(item.[0]).ToString() + item.Substring(1)
    let symbol (item : string) = if (item.Length = 6) then item.Insert(3, "/") else item
    sprintf "%-19s" <| sprintf "%s (%s)" (symbol items.[2]) (period items.[1])

//-------------------------------------------------------------------------------------------------

let private plotPriceLin = "
$data0 << EOD
{0}
EOD

title = '{1}'

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics

set xlabel 'Time (Minutes)'

set ylabel 'Market Price'
set format y '%5.3f'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 1 linecolor '#808080'

plot $data0 using 1:2 with lines title sprintf('%s', title)
"

let renderPriceLin path data =

    let descriptor, items = data
    let title = makeTitle descriptor

    let data0 =
        items
        |> Array.mapi (fun i x -> sprintf "%O %O" i x)
        |> String.concat "\n"

    render path plotPriceLin [| data0; title |]

//-------------------------------------------------------------------------------------------------

let private plotProbs = "
$data0 << EOD
{0}
EOD

title = '{1}'; style = {2}; sigmas = {3}; µN = {4}; σN = {5}; µL = {6}; bL = {7}

set border linewidth 1.2
set grid linestyle 1 linecolor '#e6e6e6'
set grid xtics mxtics
set grid ytics mytics

set xlabel gprintf('Price Differences (Log Values), σ = %0.3te%04T', σN)
set xrange [-(sigmas * σN):+(sigmas * σN)]
set xtics(0)
set for [i=+1:+sigmas:+1] xtics add (sprintf('%+iσ', i) i * σN)
set for [i=-1:-sigmas:-1] xtics add (sprintf('%+iσ', i) i * σN)

if (style == 1) {{
    set ylabel 'Density'
    set format y '%5.0f'
}}

if (style == 2) {{
    set ylabel 'Density'
    set yrange [1:]
    set format y '  10^{{%+T}}'
    set logscale y 10
}}

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

let renderProbsLin path data = renderProbs 1 path data
let renderProbsLog path data = renderProbs 2 path data
