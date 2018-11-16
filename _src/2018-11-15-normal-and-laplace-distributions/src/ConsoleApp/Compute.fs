module Compute

open System

//-------------------------------------------------------------------------------------------------

let logLikelihoodLaplace start stop b xs =

    let f mu =
        let n = xs |> Array.length |> float
        let s = xs |> Array.sumBy (fun x -> abs (x - mu))
        let y = -n * log (2.0 * b) - (1.0 / b) * (float s)
        mu, y, (xs |> Array.contains mu)

    [| start .. stop |] |> Array.map f

//-------------------------------------------------------------------------------------------------

let densityN mu sigma x =

    (1.0 / (sigma * sqrt (2.0 * Math.PI))) * exp (- ((x - mu) ** 2.0) / (2.0 * sigma ** 2.0))

let densityL mu b x =

    (1.0 / (2.0 * b)) * exp (- (abs (x - mu)) / b)
