module Chart

open System.Diagnostics
open System.IO

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
set xrange [0:5]

set ylabel 'Probability Density'
set ytics scale 0.01, 0.01
set ytics 0.05
set yrange [0:0.75]
set format y '%.2f'

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set key box linecolor '#808080' samplen 1
set key top right reverse Left

set linetype 1 linewidth 2 linecolor '#0000ff'
set linetype 2 linewidth 2 linecolor '#ff0000'

plot '$data' using 1:2 with lines title 'Log-Normal',\
     '$data' using 1:3 with lines title 'Log-Laplace'
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
set xtics 0.1
set xrange [0.01:100]
set logscale x 10

set ylabel 'Probability Density'
set ytics scale 0.01, 0.01
set ytics 0.05
set yrange [0:0.75]
set format y '%.2f'

set grid xtics ytics mxtics mytics
set grid linestyle 1 linecolor '#e6e6e6'

set key box linecolor '#808080' samplen 1
set key top right reverse Left

set linetype 1 linewidth 2 linecolor '#0000ff'
set linetype 2 linewidth 2 linecolor '#ff0000'

plot '$data' using 1:2 with lines title 'Log-Normal',\
     '$data' using 1:3 with lines title 'Log-Laplace'
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
