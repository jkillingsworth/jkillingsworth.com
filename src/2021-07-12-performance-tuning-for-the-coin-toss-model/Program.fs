module Program

open System
open System.Text

//-------------------------------------------------------------------------------------------------

Console.OutputEncoding <- new UTF8Encoding()

let path filename = Common.Const.runoutput + filename

//-------------------------------------------------------------------------------------------------

let density = (20, 20)
let samples = 100

//-------------------------------------------------------------------------------------------------

let runEstimate n tag pmfunc upper degree start =

    let name = $"estimate-n-{n}-{tag}"

    let trace, count, final, error = Compute.estimateBiases n pmfunc degree start

    let fn = Compute.polynomial degree final

    let pointsFitted = Compute.polynomialPoints n samples fn
    let biasesFitted = Compute.biases n fn
    let pmfuncFitted = Compute.evaluatePmfunc n biasesFitted

    Chart.renderPmfunc (path $"{name}-pmfunc-fitted.svg") pmfuncFitted upper
    Chart.renderBiases (path $"{name}-biases-fitted.svg") biasesFitted pointsFitted degree

    if (degree = 1) then
        let heatmap = Compute.heatmap n density pmfunc degree
        let trace = trace |> Array.map (fun x -> x.[0], x.[1])
        Chart.renderSurface (path $"{name}-surface.svg") heatmap
        Chart.renderHeatmap (path $"{name}-heatmap.svg") heatmap trace samples tag

    printfn "----------------------------------------"
    printfn "%s" name
    count |> printfn "count: %A"
    error |> printfn "error: %0.8f"
    final |> Array.iteri (fun i x -> printfn "p%i:    %0.4f" (i + 1) x)

//-------------------------------------------------------------------------------------------------

let pmfunc10 = Compute.pmfExponent 10 0.5
let pmfunc20 = Compute.pmfExponent 20 0.6
let pmfunc50 = Compute.pmfExponent 50 0.7

Chart.renderPmfunc (path "target-pmfunc-n-10.svg") pmfunc10 0.4
Chart.renderPmfunc (path "target-pmfunc-n-20.svg") pmfunc20 0.3
Chart.renderPmfunc (path "target-pmfunc-n-50.svg") pmfunc50 0.2

runEstimate 10 "0" pmfunc10 0.4 1 [| 0.50; 0.50 |]
runEstimate 10 "1" pmfunc10 0.4 1 [| 0.90; 0.90 |]
runEstimate 10 "2" pmfunc10 0.4 1 [| 0.50; 0.95 |]
runEstimate 10 "3" pmfunc10 0.4 1 [| 0.25; 0.05 |]
runEstimate 10 "4" pmfunc10 0.4 1 [| 0.90; 0.40 |]
runEstimate 20 "5" pmfunc20 0.3 2 [| 0.50; 0.50; 0.50 |]
runEstimate 50 "6" pmfunc50 0.2 3 [| 0.50; 0.50; 0.50; 0.50 |]

//-------------------------------------------------------------------------------------------------

let n = 4

let counts = Compute.flopCounts 50

let (ra, la, om) = counts.[n]

printfn "----------------------------------------"
printfn "flop-counts-n-%i" n
printfn "ra:    %i" ra
printfn "la:    %i" la
printfn "om:    %i" om

Chart.renderFlopCounts (path "flop-counts.svg") counts
