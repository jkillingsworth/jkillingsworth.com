module Program

open System
open System.Text

//-------------------------------------------------------------------------------------------------

Console.OutputEncoding <- new UTF8Encoding()

let path filename = "../../../" + filename

//-------------------------------------------------------------------------------------------------

let μ = 0.0
let σ = 1.0
let b = 1.0

let range = [| 0.0 .. 0.01 .. +100.0 |]
let densityN = range |> Array.map (Compute.densityN μ σ)
let densityL = range |> Array.map (Compute.densityL μ b)
let distributions = Array.zip3 range densityN densityL

distributions |> Chart.renderDistributionsLin (path "distributions-lin.svg")
distributions |> Chart.renderDistributionsLog (path "distributions-log.svg")
