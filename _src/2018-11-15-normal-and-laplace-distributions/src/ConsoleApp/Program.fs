module Program

//-------------------------------------------------------------------------------------------------

let µ = 0.0
let σ = 1.0
let b = 1.0

let likelihoodE = Compute.logLikelihoodLaplace 0 100 b [| 0; 20; 40; 60; 80; 100 |]
let likelihoodO = Compute.logLikelihoodLaplace 0 100 b [| 0; 25; 50; 75; 100 |]

let range = [| -5.0 .. 0.1 .. +5.0 |]
let densityN = range |> Array.map (Compute.densityN 0.0 1.0)
let densityL = range |> Array.map (Compute.densityL 0.0 1.0)
let distributions = Array.zip3 range densityN densityL

let path filename = "../../../" + filename

likelihoodE |> Chart.renderLikelihood (path "log-likelihood-laplace-e.svg") (-330, -130)
likelihoodO |> Chart.renderLikelihood (path "log-likelihood-laplace-o.svg") (-300, -100)

distributions |> Chart.renderDistributionsLin (path "distributions-lin.svg")
distributions |> Chart.renderDistributionsLog (path "distributions-log.svg")
