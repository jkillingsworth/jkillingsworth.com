module Program

//-------------------------------------------------------------------------------------------------

let v0 = 100.0
let b = 0.2
let n = 200
let count = 10000

let simsConstantAdd = Compute.runFixedConstantAdd count n v0 b
let simsFractionAdd = Compute.runFixedFractionAdd count n v0 b
let simsFractionMul = Compute.runFixedFractionMul count n v0 b

let seed = 1
let simConstantAdd = simsConstantAdd.[seed]
let simFractionAdd = simsFractionAdd.[seed]
let simFractionMul = simsFractionMul.[seed]

let avgConstantAdd = Compute.aggregateAvg n simsConstantAdd
let avgFractionAdd = Compute.aggregateAvg n simsFractionAdd
let avgFractionMul = Compute.aggregateAvg n simsFractionMul

let medConstantAdd = Compute.aggregateMed n simsConstantAdd
let medFractionAdd = Compute.aggregateMed n simsFractionAdd
let medFractionMul = Compute.aggregateMed n simsFractionMul

let path filename = "../../../" + filename

simConstantAdd |> Chart.renderLin (path "constant-add-lin-sim.svg") -100.0 300.0
avgConstantAdd |> Chart.renderLin (path "constant-add-lin-avg.svg") -100.0 300.0
medConstantAdd |> Chart.renderLin (path "constant-add-lin-med.svg") -100.0 300.0

simFractionAdd |> Chart.renderLin (path "fraction-add-lin-sim.svg") -100.0 300.0
avgFractionAdd |> Chart.renderLin (path "fraction-add-lin-avg.svg") -100.0 300.0
medFractionAdd |> Chart.renderLin (path "fraction-add-lin-med.svg") -100.0 300.0

simFractionAdd |> Chart.renderLog (path "fraction-add-log-sim.svg")    0.1 300.0
avgFractionAdd |> Chart.renderLog (path "fraction-add-log-avg.svg")    0.1 300.0
medFractionAdd |> Chart.renderLog (path "fraction-add-log-med.svg")    0.1 300.0

simFractionMul |> Chart.renderLin (path "fraction-mul-lin-sim.svg")    0.0 800.0
avgFractionMul |> Chart.renderLin (path "fraction-mul-lin-avg.svg")    0.0 800.0
medFractionMul |> Chart.renderLin (path "fraction-mul-lin-med.svg")    0.0 800.0

simFractionMul |> Chart.renderLog (path "fraction-mul-log-sim.svg")    1.0 900.0
avgFractionMul |> Chart.renderLog (path "fraction-mul-log-avg.svg")    1.0 900.0
medFractionMul |> Chart.renderLog (path "fraction-mul-log-med.svg")    1.0 900.0
