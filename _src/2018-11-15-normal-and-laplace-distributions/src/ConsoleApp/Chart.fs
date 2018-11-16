module Chart

open System.Diagnostics
open System.IO

//-------------------------------------------------------------------------------------------------

let private plotLikelihood = "
set terminal svg size 720 405 background 'white' font 'Consolas, Monaco, monospace'
set encoding utf8
set output '{0}'

$data << EOD
{1}
EOD

set xlabel 'Location ({{/symbol m}})'
set xtics scale 0.01, 0.01
set xtics ({2})

set ylabel 'Likelihood'
set ytics scale 0.01, 0.01
set yrange [{3}:{4}]
set ytics (' Max' {5})

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set key box linecolor '#808080' samplen 1
set key top left reverse Left
set key width -2

set linetype 1 linewidth 2 linecolor '#ff0000'
set linetype 2 pointtype 7 linecolor '#ff0000' pointsize 0.75

etitle = 'Likelihood (n is even)'
otitle = 'Likelihood (n is odd) '
if ({6} == 0) {{ eotitle = etitle }} else {{ eotitle = otitle }}

plot '$data' using 1:2 with lines title eotitle,\
     '$data' using 1:($3 != 0 ? $2 : 1/0) with points notitle
"

//-------------------------------------------------------------------------------------------------

let private plotDistributionsLin = "
set terminal svg size 720 405 background 'white' font 'Consolas, Monaco, monospace'
set encoding utf8
set output '{0}'

$data << EOD
{1}
EOD

set xlabel 'x'
set xtics scale 0.01, 0.01
set xtics 1

set ylabel 'Probability Density'
set ytics scale 0.01, 0.01
set ytics 0.05
set yrange [0:0.55]
set format y '%.2f'

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 2 linecolor '#0000ff'
set linetype 2 linewidth 2 linecolor '#ff0000'

plot '$data' using 1:2 with lines title 'Normal',\
     '$data' using 1:3 with lines title 'Laplace'
"

//-------------------------------------------------------------------------------------------------

let private plotDistributionsLog = "
set terminal svg size 720 405 background 'white' font 'Consolas, Monaco, monospace'
set encoding utf8
set output '{0}'

$data << EOD
{1}
EOD

set xlabel 'x'
set xtics scale 0.01, 0.01
set xtics 1

set ylabel 'Probability Density'
set ytics scale 0.01, 0.01
set yrange [0.00001:1]
set format y ' 10^{{%+L}}'
set logscale y 10

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set key box linecolor '#808080' samplen 1
set key top left reverse Left

set linetype 1 linewidth 2 linecolor '#0000ff'
set linetype 2 linewidth 2 linecolor '#ff0000'

plot '$data' using 1:2 with lines title 'Normal',\
     '$data' using 1:3 with lines title 'Laplace'
"

//-------------------------------------------------------------------------------------------------

let private render plot path (args : obj[]) =

    let file = Path.GetFullPath(path)
    use proc = new Process()
    proc.StartInfo.FileName <- "gnuplot.exe"
    proc.StartInfo.UseShellExecute <- false
    proc.StartInfo.RedirectStandardInput <- true
    proc.Start() |> ignore
    proc.StandardInput.Write(plot, args |> Array.append [| file |])
    proc.StandardInput.Flush()

let renderLikelihood path (lower, upper) data =

    let points = data |> Array.filter (fun (_, _, p) -> p)
    let ymax = points |> Array.map (fun (_, y, _) -> y) |> Array.max
    let n = points |> Array.length
    let m = (n / 2) + (n % 2)

    let xlabel i =
        let offset = i - m + 1
        if (offset = 0) then "x_m" else sprintf "x_{{m%+i}}" offset

    let xrange =
        points
        |> Array.mapi (fun i (x, _, _) -> sprintf "'%s' %i" (xlabel i) x)
        |> Array.reduce (fun l r -> l + ", " + r)

    let mapping (x, y, p) =
        sprintf "%i %f %i" x y (if p then 1 else 0)

    let data =
        data
        |> Array.map mapping
        |> String.concat "\n"

    render plotLikelihood path [| data; xrange; lower; upper; ymax; (n % 2) |]

let renderDistributionsLin path data =

    let data =
        data
        |> Array.map (fun (x, n, l) -> sprintf "%f %f %f" x n l)
        |> String.concat "\n"

    render plotDistributionsLin path [| data |]

let renderDistributionsLog path data =

    let data =
        data
        |> Array.map (fun (x, n, l) -> sprintf "%f %f %f" x n l)
        |> String.concat "\n"

    render plotDistributionsLog path [| data |]
