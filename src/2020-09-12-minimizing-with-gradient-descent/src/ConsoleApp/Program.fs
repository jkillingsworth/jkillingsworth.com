module Program

//-------------------------------------------------------------------------------------------------

let density = (20, 20)
let samples = 100

let path filename = "../../../" + filename

//-------------------------------------------------------------------------------------------------

let pmfunc = Compute.pmfExponent 3 0.5
let heatmap = Compute.heatmap pmfunc density
let plateau = Compute.plateau pmfunc samples
let costfunc = Compute.costfunc pmfunc
let gradient = Compute.gradient pmfunc

//-------------------------------------------------------------------------------------------------

let executeTrace tag psStart =

    let name = "trace-" + tag

    let psFinal, psTrace = Compute.estimate gradient psStart
    let traces = psTrace |> Array.map (fun x -> x.[0], x.[1])
    Chart.renderHeatmapTraces (path name + "-heatmap.svg") heatmap plateau traces samples tag

    printfn "----------------------------------------"
    printfn "%s" name
    printfn "cycles: %A" (psTrace.Length - 1)
    printfn "p1:     %0.4f" psFinal.[0]
    printfn "p2:     %0.4f" psFinal.[1]
    printfn "cost:   %0.8f" (costfunc psFinal)

//-------------------------------------------------------------------------------------------------

Chart.renderPmfunc (path "target-pmfunc.svg") pmfunc

executeTrace "1" [| 0.35; 0.20 |]
executeTrace "2" [| 0.65; 0.80 |]
executeTrace "3" [| 0.20; 0.35 |]
executeTrace "4" [| 0.80; 0.20 |]
