module Program

open System
open System.Text

//-------------------------------------------------------------------------------------------------

Console.OutputEncoding <- new UTF8Encoding()

let path filename = Common.Chart.outputPath filename

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

let pmfunc10 = Compute.pmfExponent 10 0.5
Chart.renderPmfunc (path "target-pmfunc-n-10.svg") pmfunc10 0.4
runEstimate 10 "1" pmfunc10 0.4 2 0.50
runEstimate 10 "2" pmfunc10 0.4 6 0.50

let pmfunc50 = Compute.pmfExponent 50 0.7
Chart.renderPmfunc (path "target-pmfunc-n-50.svg") pmfunc50 0.2
runEstimate 50 "0" pmfunc50 0.2 3 0.50
