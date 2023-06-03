module Compute

open MathNet.Numerics.Statistics

//-------------------------------------------------------------------------------------------------

let differences logprices =

    let count = logprices |> Array.length
    let initialize i = (logprices.[i + 1]) - (logprices.[i])
    Array.init<float> (count - 1) initialize

let histogram wide differences =

    let bins = 50 + 1
    let histogram = Histogram(differences, bins, -wide, +wide)

    let mapping i =
        let bucket = histogram.[i]
        let center = (bucket.LowerBound + bucket.UpperBound) / 2.0
        let amount = (bucket.Count / histogram.DataCount) / (wide * 2.0 / (float bins))
        (center, amount)

    Array.init bins mapping

//-------------------------------------------------------------------------------------------------

let fitDistributionN (diffs : float[]) =

    let n = diffs |> Array.length
    let μ = diffs |> Statistics.Mean
    let σ = diffs |> Array.map (fun x -> (x - μ) ** 2.0) |> Array.sum
    let σ = sqrt (σ / float n)

    (μ, σ)

let fitDistributionL (diffs : float[]) =

    let n = diffs |> Array.length
    let μ = diffs |> Statistics.Median
    let b = diffs |> Array.map (fun x -> abs (x - μ)) |> Array.sum
    let b = (b / float n)

    (μ, b)

//-------------------------------------------------------------------------------------------------

let syntheticPair pricesX pricesY =
    let f x y = (y / x) : float
    let keysX = pricesX |> Map.toArray |> Array.map fst |> Set.ofArray
    let keysY = pricesY |> Map.toArray |> Array.map fst |> Set.ofArray
    let keys = Set.intersect keysX keysY |> Set.toArray
    let mapping key = (key, f pricesX.[key] pricesY.[key])
    keys |> Array.map mapping|> Map.ofArray
