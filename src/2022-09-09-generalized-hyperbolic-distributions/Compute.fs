module Compute

open System
open MathNet.Numerics
open MathNet.Numerics.Statistics

//-------------------------------------------------------------------------------------------------

let private π = Math.PI

//-------------------------------------------------------------------------------------------------

let differences values =

    let count = values |> Array.length
    let initialize i = (values.[i + 1]) - (values.[i])
    Array.init<float> (count - 1) initialize

let histogram wide values =

    let bins = 100 + 1
    let histogram = Histogram(values, bins, -wide, +wide)

    let mapping i =
        let bucket = histogram.[i]
        let center = (bucket.LowerBound + bucket.UpperBound) / 2.0
        let amount = (bucket.Count / histogram.DataCount) / (wide * 2.0 / (float bins))
        (center, amount)

    Array.init bins mapping

let ps wide pDensity =

    let samples = 1000
    let step = (wide * 2.0) / float samples

    let xs = [| -wide .. step .. +wide |]
    let ys = Array.map pDensity xs

    Array.zip xs ys

//-------------------------------------------------------------------------------------------------

let pDensityN (μ, σ) x =

    (1.0 / (σ * sqrt (2.0 * π))) * exp (- ((x - μ) ** 2.0) / (2.0 * σ ** 2.0))

let pDensityH (μ, δ, α, λ) x =

    let K v (z : float) = SpecialFunctions.BesselK(v, z)

    let θ = sqrt ((δ ** 2.0) + ((x - μ) ** 2.0))

    let part1 = sqrt (α / (2.0 * π))
    let part2 = (K (λ - 0.5) (α * θ)) * (θ ** (λ - 0.5))
    let part3 = (δ ** λ) * (K λ (δ * α))

    part1 * part2 / part3

//-------------------------------------------------------------------------------------------------

let fitDistributionN (values : float[]) =

    let n = values |> Array.length
    let μ = values |> Statistics.Mean
    let σ = values |> Array.map (fun x -> (x - μ) ** 2.0) |> Array.sum
    let σ = sqrt (σ / float n)

    (μ, σ)

let fitDistributionI (values : float[]) =

    let (μN, σN) = fitDistributionN values

    let μ = μN
    let δ = σN
    let α = 1.0 / δ
    let λ = -0.5

    (μ, δ, α, λ)

let fitDistributionH (values : float[]) =

    let (μI, δI, αI, λI) = fitDistributionI values

    let μ0 = μI
    let δ0 = δI
    let α0 = αI
    let λ0 = λI

    let f μ δ α λ =
        values
        |> Array.Parallel.map (pDensityH (μ, δ, α, λ))
        |> Array.Parallel.map log
        |> Array.sum
        |> (~-)

    let f μ δ α λ =
        match () with
        | _ when δ <= 0.0 -> infinity
        | _ when α <= 0.0 -> infinity
        | _ -> f μ δ α λ

    FindMinimum.OfFunction(f, μ0, δ0, α0, λ0).ToTuple()
