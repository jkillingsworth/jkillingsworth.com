module Compute

open System
open MathNet.Numerics
open MathNet.Numerics.Distributions
open MathNet.Numerics.Random

//-------------------------------------------------------------------------------------------------

let runSimulation v0 n p q b seed =

    let random = SystemRandomSource(seed, false)

    let folder (v, _, _) = function
        | 0 -> (v * (1.0 + b), 1, 0)
        | _ -> (v * (1.0 - b), 0, 1)

    let values =
        random
        |> Sample.categoricalSeq [| p; q |]
        |> Seq.take n
        |> Seq.scan folder (v0, 0, 0)
        |> Seq.toArray

    let vs, hs, ts = Array.unzip3 values

    let hCount = hs |> Array.sum
    let tCount = ts |> Array.sum

    (vs, hCount, tCount)

//-------------------------------------------------------------------------------------------------

let gKellyDiscrete ps ws b =

    Array.zip ps ws
    |> Array.map (fun (p, w) -> p * log (1.0 + b * w))
    |> Array.sum

let gKellyContinuous p w r s b =

    let integrand x = (p x) * log (1.0 + b * (w x))
    let limitA = 0.0
    let limitB = r
    let limitC = s
    let limitD = infinity
    let partAB = Integrate.GaussLegendre(integrand, limitA, limitB, 1024)
    let partBC = Integrate.GaussLegendre(integrand, limitB, limitC, 1024)
    let partCD = Integrate.GaussLegendre(integrand, limitC, limitD, 1024)
    partAB + partBC + partCD

let bOptimal gKelly =

    let fn = gKelly
    let fn = fn >> (~-)
    let fn = Func<float, float>(fn)
    FindMinimum.OfScalarFunction(fn, 0.0)

//-------------------------------------------------------------------------------------------------

let rangeNew (start, final) =
    let samples = 200
    let step = (final - start) / (float samples)
    Array.init (samples + 1) (fun i -> start + (float i) * step)

let rangeMap xs f =
    xs |> Array.map f

//-------------------------------------------------------------------------------------------------

let pLogNormalDistribution μ σ x =
    (1.0 / (x * σ * sqrt (2.0 * Math.PI))) * exp (- (pown (log x - μ) 2) / (2.0 * pown σ 2))

let wStockWithBoundedExits c r s : (float -> float) = function
    | x when x < r -> (r - c) / c
    | x when x > s -> (s - c) / c
    | x -> (x - c) / c

//-------------------------------------------------------------------------------------------------

let private sMax c ℓ m = function
    | b when b > +(1.0 / ℓ) -> nan
    | b when b < -(1.0 / ℓ) -> nan
    | b when b < 0.0 -> (c * (b - 1.0)) / (b * (1.0 + m))
    | _ -> +infinity

let private rMin c ℓ m = function
    | b when b > +(1.0 / ℓ) -> nan
    | b when b < -(1.0 / ℓ) -> nan
    | b when b > 1.0 -> (c * (b - 1.0)) / (b * (1.0 - m))
    | _ -> 0.0

let limitations1 bRange c =

    let bs =
        rangeNew bRange
        |> Array.append [| 1.0 |]
        |> Array.distinct
        |> Array.sort

    let sMaxs0 = bs |> Array.map (sMax c 0.0 0.0)
    let rMins0 = bs |> Array.map (rMin c 0.0 0.0)

    (bs, rMins0, sMaxs0)

let limitations2 bRange c ℓ m =

    let bMax = +(1.0 / ℓ)
    let bMin = -(1.0 / ℓ)

    let bs =
        rangeNew bRange
        |> Array.append [| 1.0; bMin; bMax  |]
        |> Array.distinct
        |> Array.sort

    let sMaxs0 = bs |> Array.map (sMax c 0.0 0.0)
    let rMins0 = bs |> Array.map (rMin c 0.0 0.0)
    let sMaxsM = bs |> Array.map (sMax c ℓ m)
    let rMinsM = bs |> Array.map (rMin c ℓ m)

    (bs, (rMins0, rMinsM), (sMaxs0, sMaxsM))
