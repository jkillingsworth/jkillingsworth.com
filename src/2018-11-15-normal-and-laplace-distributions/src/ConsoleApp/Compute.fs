module Compute

open System

//-------------------------------------------------------------------------------------------------

let logLikelihoodLaplace start stop b xs =

    let f μ =
        let n = xs |> Array.length |> float
        let s = xs |> Array.sumBy (fun x -> abs (x - μ))
        let y = -n * log (2.0 * b) - (1.0 / b) * (float s)
        (μ, y, (xs |> Array.contains μ))

    [| start .. stop |] |> Array.map f

//-------------------------------------------------------------------------------------------------

let densityN μ σ x =

    (1.0 / (σ * sqrt (2.0 * Math.PI))) * exp (- ((x - μ) ** 2.0) / (2.0 * σ ** 2.0))

let densityL μ b x =

    (1.0 / (2.0 * b)) * exp (- (abs (x - μ)) / b)
