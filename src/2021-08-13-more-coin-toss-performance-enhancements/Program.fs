module Program

open System
open System.Text

//-------------------------------------------------------------------------------------------------

Console.OutputEncoding <- new UTF8Encoding()

let path filename = Common.Const.runoutput + filename

//-------------------------------------------------------------------------------------------------

let samples = 100

//-------------------------------------------------------------------------------------------------

let runEstimate n tag pmfunc upper degree start =

    let name = $"estimate-n-{n}-{tag}"

    let count, final, error = Compute.estimateBiases n pmfunc degree start

    let fn = Compute.polynomial degree final

    let pointsFitted = Compute.polynomialPoints n samples fn
    let biasesFitted = Compute.biases n fn
    let pmfuncFitted = Compute.evaluatePmfunc n biasesFitted

    Chart.renderPmfunc (path $"{name}-pmfunc-fitted.svg") pmfuncFitted upper
    Chart.renderBiases (path $"{name}-biases-fitted.svg") biasesFitted pointsFitted degree

    printfn "----------------------------------------"
    printfn "%s" name
    count |> printfn "count: %A"
    error |> printfn "error: %0.8f"
    final |> Array.iteri (fun i x -> printfn "p%i:    %0.4f" (i + 1) x)

//-------------------------------------------------------------------------------------------------

let pmfunc50 = Compute.pmfExponent 50 0.7

Chart.renderPmfunc (path "target-pmfunc-n-50.svg") pmfunc50 0.2

runEstimate 50 "0" pmfunc50 0.2 0 0.50
runEstimate 50 "1" pmfunc50 0.2 1 0.50
runEstimate 50 "2" pmfunc50 0.2 2 0.50
runEstimate 50 "3" pmfunc50 0.2 3 0.50

//-------------------------------------------------------------------------------------------------

let n = 4

let counts = Compute.flopCounts 50

let (bm, em) = counts.[n]

printfn "----------------------------------------"
printfn "flop-counts-n-%i" n
printfn "bm:    %i" bm
printfn "em:    %i" em

Chart.renderFlopCounts (path "flop-counts.svg") counts n
