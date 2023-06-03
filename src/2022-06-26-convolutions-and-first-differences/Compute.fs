module Compute

open System
open MathNet.Numerics
open MathNet.Numerics.Distributions
open MathNet.Numerics.Random

//-------------------------------------------------------------------------------------------------

let private i = Complex.onei
let private π = Math.PI

//-------------------------------------------------------------------------------------------------

let generateSeriesFromPmfunc pMass count =

    let random = SystemRandomSource(0, false)

    let xs = pMass |> Array.map fst
    let ps = pMass |> Array.map snd

    random
    |> Sample.categoricalSeq ps
    |> Seq.take (count + 1)
    |> Seq.mapi (fun t r -> t, float xs.[r])
    |> Seq.toArray

//-------------------------------------------------------------------------------------------------

let private createFunc fs t =
    fs
    |> Map.ofArray
    |> Map.tryFind t
    |> Option.defaultValue 0.0

let private convolve fs gs x =

    let lower = fs |> Array.map fst |> Array.min
    let upper = fs |> Array.map fst |> Array.max

    let f = createFunc fs
    let g = createFunc gs

    [| lower .. upper |]
    |> Array.map (fun k -> (f k) * (g (x - k)))
    |> Array.sum

let convolutionSeries xs hs =

    xs
    |> Array.skip 1
    |> Array.map fst
    |> Array.map (fun t -> t, convolve xs hs t)

let convolutionPmfunc fs gs =

    let lowerF = fs |> Array.map fst |> Array.min
    let upperF = fs |> Array.map fst |> Array.max
    let lowerG = gs |> Array.map fst |> Array.min
    let upperG = gs |> Array.map fst |> Array.max

    let lower = lowerF + lowerG
    let upper = upperF + upperG

    [| lower .. upper |]
    |> Array.map (fun x -> x, convolve fs gs x)

//-------------------------------------------------------------------------------------------------

let convolution f g x =

    let integrand τ = (f τ) * (g (x - τ))
    let lower = -infinity
    let upper = +infinity
    Integrate.GaussKronrod(integrand, lower, upper)

let deconvolution (G : float -> complex) (x : float) =

    let integrand ω = (1.0 / (2.0 * π)) * (exp (i * ω * x)) * (sqrt (G ω))
    let lower = -1000
    let upper = +1000
    ContourIntegrate.GaussKronrod(integrand, lower, upper).Real

let ps (lower, upper) mapping =

    let samples = 200
    let step = (upper - lower) / float samples

    [| lower .. step .. upper |]
    |> Array.map (fun x -> x, mapping x)

//-------------------------------------------------------------------------------------------------

let pMass =

    [| (-2, 0.1); (-1, 0.2); (0, 0.4); (+1, 0.2); (+2, 0.1) |]

let hFirstDifference =

    [| (0, +1.0); (1, -1.0) |]

let pDensityN μ σ x =

    (1.0 / (σ * sqrt (2.0 * π))) * exp (- ((x - μ) ** 2.0) / (2.0 * σ ** 2.0))

let pDensityL μ b x =

    (1.0 / (2.0 * b)) * exp (- (abs (x - μ)) / b)

let fourierTransformDensityL (μ : float) (b : float) (ω : float) =

    (exp (-i * μ * ω)) / (((b * ω) ** 2.0) + 1.0)
