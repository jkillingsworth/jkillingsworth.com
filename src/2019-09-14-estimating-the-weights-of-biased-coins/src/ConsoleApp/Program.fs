module Program

open System
open System.Text

//-------------------------------------------------------------------------------------------------

Console.OutputEncoding <- new UTF8Encoding()

let path filename = Common.Chart.outputPath filename

//-------------------------------------------------------------------------------------------------

let seed = 0
let iterations = 1000000

//-------------------------------------------------------------------------------------------------

let simulate name biases =

    let result = biases |> Compute.simulate seed iterations
    let pmfunc = result |> fst
    let tosses = result |> snd

    biases |> Chart.renderBiases (path name + "-biases.svg")
    pmfunc |> Chart.renderPmfunc (path name + "-pmfunc-simulated.svg")
    tosses |> Chart.renderTosses (path name + "-tosses-simulated.svg")

let evaluate name biases =

    let result = biases |> Compute.evaluate
    let pmfunc = result |> fst
    let tosses = result |> snd

    biases |> Chart.renderBiases (path name + "-biases.svg")
    pmfunc |> Chart.renderPmfunc (path name + "-pmfunc-evaluated.svg")
    tosses |> Chart.renderTosses (path name + "-tosses-evaluated.svg")

let estimate name pmfunc =

    let biasesStart = Compute.biasesEqual

    let biasesLower = pmfunc |> Compute.biasesLower
    let biasesUpper = pmfunc |> Compute.biasesUpper
    let biasesFinal = pmfunc |> Compute.estimate seed iterations biasesStart

    pmfunc |> Chart.renderPmfunc (path name + "-pmfunc.svg")
    biasesLower |> Chart.renderBiases (path name + "-biases-lower.svg")
    biasesUpper |> Chart.renderBiases (path name + "-biases-upper.svg")
    biasesFinal |> Chart.renderBiases (path name + "-biases-estimated.svg")

//-------------------------------------------------------------------------------------------------

simulate "binomial-equal" <| Compute.biasesEqual
evaluate "binomial-equal" <| Compute.biasesEqual
evaluate "binomial-lower" <| Compute.biasesLower Compute.pmfBinomial
evaluate "binomial-upper" <| Compute.biasesUpper Compute.pmfBinomial
estimate "triangle-learn" <| Compute.pmfTriangle
estimate "exponential-05" <| Compute.pmfExponent 0.5
estimate "exponential-04" <| Compute.pmfExponent 0.4
estimate "exponential-03" <| Compute.pmfExponent 0.3
