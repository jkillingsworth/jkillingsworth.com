module Program

//-------------------------------------------------------------------------------------------------

let iterations = 1000000

let path filename = "../../../" + filename

//-------------------------------------------------------------------------------------------------

let runEstimate name pmfunc biases =

    Chart.renderBiases (path name + "-begin-biases.svg") biases

    let biasesFinal, cycles = Compute.estimate iterations pmfunc biases
    let pmfuncFinal, tosses = Compute.evaluate biasesFinal

    Chart.renderBiases (path name + "-final-biases.svg") biasesFinal
    Chart.renderPmfunc (path name + "-final-pmfunc.svg") pmfuncFinal
    Chart.renderTosses (path name + "-final-tosses.svg") tosses

    let rcosts = Compute.revisionCosts pmfunc biases
    let scoreA = Compute.evaluateScore biasesFinal <| Compute.scoreA
    let scoreB = Compute.evaluateScore biasesFinal <| Compute.scoreB

    printfn "----------------------------------------"
    printfn "%s" name
    Array.iteri (printfn "cost %A: %0.8f") rcosts
    printfn "cycles: %A" cycles
    printfn "scoreA: %A" scoreA
    printfn "scoreB: %A" scoreB

//-------------------------------------------------------------------------------------------------

let runOptimize name pmfunc tag scoref =

    let bounds = Compute.bounds pmfunc
    let scores = Compute.scoresOverall pmfunc bounds scoref
    let coords = Compute.coordsOptimum pmfunc bounds scoref
    let biases = Compute.biasesOptimum pmfunc bounds scoref
    let pmfunc, tosses = Compute.evaluate biases

    Chart.renderScores (path name + "-scores.svg") scores coords bounds tag
    Chart.renderBiases (path name + "-biases.svg") biases
    Chart.renderPmfunc (path name + "-pmfunc.svg") pmfunc
    Chart.renderTosses (path name + "-tosses.svg") tosses

//-------------------------------------------------------------------------------------------------

let pmfunc = Compute.pmfExponent 0.5

Chart.renderPmfunc (path "target-pmfunc.svg") pmfunc

runEstimate "estimate-equal-05" pmfunc <| Compute.biasesEqual
runEstimate "estimate-slope-02" pmfunc <| Compute.biasesSlope 0.2

Chart.renderClimbHillEast (path "climb-hill-east.svg")
Chart.renderClimbHillWest (path "climb-hill-west.svg")
Chart.renderClimbPlatEast (path "climb-plat-east.svg")
Chart.renderClimbPlatWest (path "climb-plat-west.svg")

runOptimize "optimize-score-A" pmfunc "A" <| Compute.scoreA
runOptimize "optimize-score-B" pmfunc "B" <| Compute.scoreB
