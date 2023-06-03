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

let pDensityL (μ, b) x =

    (1.0 / (2.0 * b)) * exp (- (abs (x - μ)) / b)

let pDensityG (μ, α, β) x =

    (β / (2.0 * α * SpecialFunctions.Gamma (1.0 / β))) * exp (- ((abs (x - μ) / α) ** β))

//-------------------------------------------------------------------------------------------------

let fitDistributionN (values : float[]) =

    let n = values |> Array.length
    let μ = values |> Statistics.Mean
    let σ = values |> Array.map (fun x -> (x - μ) ** 2.0) |> Array.sum
    let σ = sqrt (σ / float n)

    (μ, σ)

let fitDistributionL (values : float[]) =

    let n = values |> Array.length
    let μ = values |> Statistics.Median
    let b = values |> Array.map (fun x -> abs (x - μ)) |> Array.sum
    let b = (b / float n)

    (μ, b)

let fitDistributionG (values : float[]) =

    let (μN, σN) = fitDistributionN values

    let μ0 = μN
    let α0 = σN * sqrt 2.0
    let β0 = 2.0

    let f μ α β =
        values
        |> Array.map (pDensityG (μ, α, β))
        |> Array.map log
        |> Array.sum
        |> (~-)

    FindMinimum.OfFunction(f, μ0, α0, β0).ToTuple()
