module Program

open System
open System.Text

//-------------------------------------------------------------------------------------------------

Console.OutputEncoding <- new UTF8Encoding()

let path filename = Common.Chart.outputPath filename

//-------------------------------------------------------------------------------------------------

let n = 10
let density = (20, 20)
let samples = 100

//-------------------------------------------------------------------------------------------------

let runEstimate tag pmfunc degree start =

    let name = $"estimate-{tag}"

    let trace, count, final, error = Compute.estimateBiases n pmfunc degree start

    let fn = Compute.polynomial degree final

    let pointsFitted = Compute.polynomialPoints n samples fn
    let biasesFitted = Compute.biases n fn
    let pmfuncFitted = Compute.evaluatePmfunc n biasesFitted

    Chart.renderPmfunc (path $"{name}-pmfunc-fitted.svg") pmfuncFitted
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

let pmfunc = Compute.pmfExponent n 0.5

Chart.renderPmfunc (path "target-pmfunc.svg") pmfunc

runEstimate "0" pmfunc 1 [| 0.50; 0.50 |]
runEstimate "1" pmfunc 1 [| 0.90; 0.90 |]
runEstimate "2" pmfunc 1 [| 0.50; 0.95 |]
runEstimate "3" pmfunc 1 [| 0.25; 0.05 |]
runEstimate "4" pmfunc 1 [| 0.90; 0.40 |]
runEstimate "5" pmfunc 2 [| 0.50; 0.50; 0.50 |]
runEstimate "6" pmfunc 3 [| 0.50; 0.50; 0.50; 0.50 |]
