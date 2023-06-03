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

let pDensityC (μ, λ) x =

    1.0 / (π * λ * (1.0 + ((x - μ) / λ) ** 2.0))

let pDensityS (μ, γ, α) x =

    let integrand t = (1.0 / π) * (cos ((μ - x) * t)) * (exp (- ((γ * t) ** α)))

    Integrate.GaussKronrod(integrand, 0.0, infinity)

//-------------------------------------------------------------------------------------------------

let fitDistributionN (values : float[]) =

    let n = values |> Array.length
    let μ = values |> Statistics.Mean
    let σ = values |> Array.map (fun x -> (x - μ) ** 2.0) |> Array.sum
    let σ = sqrt (σ / float n)

    (μ, σ)

let fitDistributionC (values : float[]) =

    let (μN, σN) = fitDistributionN values

    let μ0 = μN
    let λ0 = σN / (sqrt 2.0)

    let f μ λ =
        values
        |> Array.map (pDensityC (μ, λ))
        |> Array.map log
        |> Array.sum
        |> (~-)

    FindMinimum.OfFunction(f, μ0, λ0).ToTuple()

let fitDistributionS (values : float[]) =

    let (μN, σN) = fitDistributionN values

    let μ0 = μN
    let γ0 = σN / (sqrt 2.0)
    let α0 = 2.0

    let f μ γ α =
        values
        |> Array.Parallel.map (pDensityS (μ, γ, α))
        |> Array.Parallel.map log
        |> Array.sum
        |> (~-)

    let f μ γ α =
        match α with
        | α when α <= 0.0 -> infinity
        | α when α >= 2.0 -> infinity
        | _ -> f μ γ α

    FindMinimum.OfFunction(f, μ0, γ0, α0).ToTuple()
