module Program

//-------------------------------------------------------------------------------------------------

let µ = 0.0
let σ = 1.0
let b = 1.0

let range = [| 0.0 .. 0.01 .. +100.0 |]
let densityN = range |> Array.map (Compute.densityN µ σ)
let densityL = range |> Array.map (Compute.densityL µ b)
let distributions = Array.zip3 range densityN densityL

let path filename = "../../../" + filename

distributions |> Chart.renderDistributionsLin (path "distributions-lin.svg")
distributions |> Chart.renderDistributionsLog (path "distributions-log.svg")
