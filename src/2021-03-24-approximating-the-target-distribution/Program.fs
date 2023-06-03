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

let runEvaluate tag style biases =

    let name = $"evaluate-{tag}"

    let pmfunc = Compute.evaluatePmfunc n biases

    Chart.renderBiases (path $"{name}-biases.svg") biases style
    Chart.renderPmfunc (path $"{name}-pmfunc.svg") pmfunc

//-------------------------------------------------------------------------------------------------

let executeTrace name pmfunc heatmap starts i =

    let trace, count, final, error = Compute.estimateBiases n pmfunc (starts i)

    Chart.renderHeatmap (path $"{name}-trace-{i}.svg") heatmap n trace samples i

    printfn "----------------------------------------"
    printfn "trace-%i" i
    printfn "iterations: %A" count
    printfn "final a: %0.4f" (fst final)
    printfn "final b: %0.4f" (snd final)
    printfn "error:   %0.8f" error

    final

let executeTraces name pmfunc heatmap =

    let starts = function
        | 1 -> (0.8, 0.8)
        | 2 -> (0.8, 0.2)
        | 3 -> (0.2, 0.2)
        | _ -> (0.5, 0.5)

    Array.init 4 (executeTrace name pmfunc heatmap starts)

let runEstimate tag pmfunc =

    let name = $"estimate-{tag}"

    printfn "----------------------------------------"
    printfn "%s" name

    let heatmap = Compute.heatmap n density pmfunc
    let finals = executeTraces name pmfunc heatmap
    let biasesFitted = Compute.biasesType2 n <|| finals.[0]
    let pmfuncFitted = Compute.evaluatePmfunc n biasesFitted
    
    Chart.renderPmfunc (path $"{name}-pmfunc-target.svg") pmfunc
    Chart.renderPmfunc (path $"{name}-pmfunc-fitted.svg") pmfuncFitted
    Chart.renderBiases (path $"{name}-biases-fitted.svg") biasesFitted 2
    Chart.renderSurface (path $"{name}-surface.svg") heatmap n

//-------------------------------------------------------------------------------------------------

runEvaluate "1" 0 <| Compute.biasesEqual n
runEvaluate "2" 1 <| Compute.biasesType1 n 0.85
runEvaluate "3" 1 <| Compute.biasesType1 n 0.15
runEvaluate "4" 2 <| Compute.biasesType2 n 0.40 0.80
runEvaluate "5" 2 <| Compute.biasesType2 n 0.60 0.20

runEstimate "1" <| Compute.pmfBinomial n
runEstimate "2" <| Compute.pmfTriangle n
runEstimate "3" <| Compute.pmfExponent n 0.5
