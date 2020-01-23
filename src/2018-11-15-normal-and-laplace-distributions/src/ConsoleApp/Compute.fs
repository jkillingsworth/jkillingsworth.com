module Compute

open System

//-------------------------------------------------------------------------------------------------

let logLikelihoodLaplace start stop b xs =

    let f µ =
        let n = xs |> Array.length |> float
        let s = xs |> Array.sumBy (fun x -> abs (x - µ))
        let y = -n * log (2.0 * b) - (1.0 / b) * (float s)
        (µ, y, (xs |> Array.contains µ))

    [| start .. stop |] |> Array.map f

//-------------------------------------------------------------------------------------------------

let densityN µ σ x =

    (1.0 / (σ * sqrt (2.0 * Math.PI))) * exp (- ((x - µ) ** 2.0) / (2.0 * σ ** 2.0))

let densityL µ b x =

    (1.0 / (2.0 * b)) * exp (- (abs (x - µ)) / b)
