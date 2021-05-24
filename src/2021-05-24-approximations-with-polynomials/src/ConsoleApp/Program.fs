module Program

open System
open System.Text

//-------------------------------------------------------------------------------------------------

Console.OutputEncoding <- new UTF8Encoding()

let path filename = "../../../" + filename

//-------------------------------------------------------------------------------------------------

let n = 10
let density = (20, 20)
let samples = 100

//-------------------------------------------------------------------------------------------------

let estimate tag pmfunc costfn degree start =

    let name = $"estimate-{tag}"

    let trace, count, final, error = Compute.estimateBiases n pmfunc costfn degree start

    let fn = Compute.polynomial degree final

    let pointsFitted = Compute.polynomialPoints n samples fn
    let biasesFitted = Compute.biases n fn
    let pmfuncFitted = Compute.evaluatePmfunc n biasesFitted

    Chart.renderPmfunc (path $"{name}-pmfunc-fitted.svg") pmfuncFitted
    Chart.renderBiases (path $"{name}-biases-fitted.svg") biasesFitted pointsFitted degree

    if (degree = 1) then
        let heatmap = Compute.heatmap n density pmfunc costfn degree
        let trace = trace |> Array.map (fun x -> x.[0], x.[1])
        Chart.renderSurface (path $"{name}-surface.svg") heatmap
        Chart.renderHeatmap (path $"{name}-heatmap.svg") heatmap trace samples tag

    printfn "----------------------------------------"
    printfn "%s" name
    count |> printfn "count: %A"
    error |> printfn "error: %0.8f"
    final |> Array.iteri (fun i x -> printfn "p%i:    %0.4f" (i + 1) x)

//-------------------------------------------------------------------------------------------------

let execute1 tag pmfunc costfn degree start =
    estimate tag pmfunc costfn degree start

let execute2 tag pmfunc costfn degree =
    estimate tag pmfunc costfn degree <| Compute.determineStart n pmfunc costfn degree

//-------------------------------------------------------------------------------------------------

let pmfunc = Compute.pmfExponent n 0.5
let costfn = Compute.costfnDiffSquared

Chart.renderPmfunc (path "target-pmfunc.svg") pmfunc

execute1 "0" pmfunc costfn 1 [| 0.50; 0.50 |]
execute1 "1" pmfunc costfn 1 [| 0.95; 0.85 |]
execute1 "2" pmfunc costfn 1 [| 0.10; 0.10 |]
execute1 "3" pmfunc costfn 1 [| 0.95; 0.25 |]
execute1 "4" pmfunc costfn 1 [| 0.55; 0.95 |]

execute2 "5" pmfunc costfn 1
execute2 "6" pmfunc costfn 2
execute2 "7" pmfunc costfn 3
