module Program

//-------------------------------------------------------------------------------------------------

let seed = 0
let density = (20, 20)
let samples = 100

let path filename = "../../../" + filename

//-------------------------------------------------------------------------------------------------

let pmfunc = Compute.pmfExponent 0.5

Chart.renderPmfunc (path "target-pmfunc.svg") pmfunc

let heatmap = Compute.heatmap pmfunc density
let plateau = Compute.plateau pmfunc samples
let scoresA = plateau |> Array.map Compute.scoreA
let scoresB = plateau |> Array.map Compute.scoreB

Chart.renderSurface (path "surface-1.svg") heatmap 1
Chart.renderSurface (path "surface-2.svg") heatmap 2
Chart.renderHeatmap (path "heatmap-plateau.svg") heatmap plateau

//-------------------------------------------------------------------------------------------------

let executeTrace tag pmfunc biases category select =

    let name = "trace-" + category + "-" + tag

    let biasesFinal, traces = Compute.estimate select pmfunc biases
    let pmfuncFinal, tosses = Compute.evaluate biasesFinal

    Chart.renderBiases (path name + "-begin-biases.svg") biases
    Chart.renderBiases (path name + "-final-biases.svg") biasesFinal
    Chart.renderPmfunc (path name + "-final-pmfunc.svg") pmfuncFinal
    Chart.renderTosses (path name + "-final-tosses.svg") tosses
    Chart.renderHeatmapTraces (path name + "-heatmap.svg") heatmap plateau traces samples tag

    printfn "----------------------------------------"
    printfn "%s" name
    printfn "cycles: %A" (traces.Length - 1)

//-------------------------------------------------------------------------------------------------

let executeScore tag scoref =

    let name = "score-" + tag

    let scores = Compute.scoresOverall scoref plateau
    let coords = Compute.coordsOptimum scoref pmfunc
    let biases = Compute.biasesOptimum coords

    let heatmapS = Compute.heatmapScores density scoref
    let heatmapC = heatmap

    Chart.renderScores (path name + "-scores-overall.svg") scores coords tag
    Chart.renderBiases (path name + "-biases-optimum.svg") biases
    Chart.renderHeatmapScores (path name + "-heatmap-S.svg") heatmapS scores coords 1
    Chart.renderHeatmapScores (path name + "-heatmap-C.svg") heatmapC scores coords 2

//-------------------------------------------------------------------------------------------------

let biases1 = Compute.biasesSlope 0.80
let biases2 = Compute.biasesSlope 0.20
let biases3 = Compute.biasesExact 0.20 0.35
let biases4 = Compute.biasesExact 0.80 0.20

executeTrace "1" pmfunc biases1 "calculated" <| Compute.selectCalculated
executeTrace "2" pmfunc biases2 "calculated" <| Compute.selectCalculated
executeTrace "3" pmfunc biases3 "calculated" <| Compute.selectCalculated
executeTrace "4" pmfunc biases4 "calculated" <| Compute.selectCalculated

executeTrace "1" pmfunc biases1 "stochastic" <| Compute.selectStochastic seed
executeTrace "2" pmfunc biases2 "stochastic" <| Compute.selectStochastic seed
executeTrace "3" pmfunc biases3 "stochastic" <| Compute.selectStochastic seed
executeTrace "4" pmfunc biases4 "stochastic" <| Compute.selectStochastic seed

executeScore "A" <| Compute.scoreA
executeScore "B" <| Compute.scoreB
