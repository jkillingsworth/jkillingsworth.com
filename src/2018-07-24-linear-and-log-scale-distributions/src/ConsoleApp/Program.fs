module Program

//-------------------------------------------------------------------------------------------------

let v0 = 100.0
let b = 0.2
let n = 200
let count = 10000

let samplesStochastic = Compute.generateSamplesStochastic n count
let samplesStochasticConstantAdd = samplesStochastic |> Compute.applyFixedConstantAdd v0 b

let samplesExhaustive = Compute.generateSamplesExhaustive n
let samplesExhaustiveConstantAdd = samplesExhaustive |> Compute.applyFixedConstantAdd v0 b
let samplesExhaustiveFractionAdd = samplesExhaustive |> Compute.applyFixedFractionAdd v0 b
let samplesExhaustiveFractionMul = samplesExhaustive |> Compute.applyFixedFractionMul v0 b

let reportPercents (gt, lt, eq) = printfn "Profit loss breakeven: %0.3f %0.3f %0.3f" gt lt eq
samplesStochasticConstantAdd |> Compute.percentProfitLossBreakeven v0 |> reportPercents
samplesExhaustiveConstantAdd |> Compute.percentProfitLossBreakeven v0 |> reportPercents
samplesExhaustiveFractionAdd |> Compute.percentProfitLossBreakeven v0 |> reportPercents
samplesExhaustiveFractionMul |> Compute.percentProfitLossBreakeven v0 |> reportPercents

let path filename = "../../../" + filename

samplesStochasticConstantAdd |> Chart.renderDistributionLin (path "stochastic-constant-add-lin.svg")
samplesExhaustiveConstantAdd |> Chart.renderDistributionLin (path "exhaustive-constant-add-lin.svg")
samplesExhaustiveFractionAdd |> Chart.renderDistributionLin (path "exhaustive-fraction-add-lin.svg")
samplesExhaustiveFractionAdd |> Chart.renderDistributionLog (path "exhaustive-fraction-add-log.svg")
samplesExhaustiveFractionMul |> Chart.renderDistributionLin (path "exhaustive-fraction-mul-lin.svg")
samplesExhaustiveFractionMul |> Chart.renderDistributionLog (path "exhaustive-fraction-mul-log.svg")
