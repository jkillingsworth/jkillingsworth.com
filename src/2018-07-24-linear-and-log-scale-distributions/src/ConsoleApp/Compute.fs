module Compute

open MathNet.Numerics.Distributions
open MathNet.Numerics.Random

//-------------------------------------------------------------------------------------------------

type Flip =
    | Heads
    | Tails

//-------------------------------------------------------------------------------------------------

let private mapping = function
    | 0 -> Heads
    | _ -> Tails

let private increment (heads, tails) = function
    | Heads -> (heads + 1, tails)
    | Tails -> (heads, tails + 1)

let private generateFlips (seed : int) =
    seed
    |> Random.mersenneTwisterSeed
    |> Sample.discreteUniformSeq 0 1
    |> Seq.map mapping

let private generatePair n seed =
    seed
    |> generateFlips
    |> Seq.take n
    |> Seq.fold increment (0, 0)

let generateSamplesStochastic n count =
    generatePair n
    |> Array.init count
    |> Array.sort
    |> Array.groupBy id
    |> Array.map (fun (pair, items) -> pair, Array.length items)
    |> Array.map (fun (pair, tally) -> pair, float tally / float count)

//-------------------------------------------------------------------------------------------------

let private iterate n k =
    [| 1 .. k |]
    |> Array.map (fun i -> float (n + 1 - i) / float i)
    |> Array.reduce (*)

let private choose n = function
    | 0 -> 1.0
    | k -> iterate n k

let private probability n k =
    let p = 0.5
    let q = 1.0 - p
    (choose n k) * (p ** float k) * (q ** float (n - k))

let generateSamplesExhaustive n =
    [| 0 .. n |]
    |> Array.map (fun i -> (i, n - i), probability n i)

//-------------------------------------------------------------------------------------------------

let private rewardFixedConstantAdd b v0 = function
    | Heads -> +b * v0
    | Tails -> -b * v0

let private rewardFixedFractionAdd b = function
    | Heads -> 1.0 + b
    | Tails -> 1.0 - b

let private rewardFixedFractionMul b = function
    | Heads -> 1.0 / (1.0 - b)
    | Tails -> 1.0 - b

let private aggregationFixedConstantAdd b v0 (heads, tails) =
    let reward = rewardFixedConstantAdd b v0
    let W = float heads
    let L = float tails
    v0 + (W * reward Heads) + (L * reward Tails)

let private aggregationFixedFractionAdd b v0 (heads, tails) =
    let reward = rewardFixedFractionAdd b
    let W = float heads
    let L = float tails
    v0 * (reward Heads ** W) * (reward Tails ** L)

let private aggregationFixedFractionMul b v0 (heads, tails) =
    let reward = rewardFixedFractionMul b
    let W = float heads
    let L = float tails
    v0 * (reward Heads ** W) * (reward Tails ** L)

//-------------------------------------------------------------------------------------------------

let private apply aggregation =
    let mapping (pair, count) = (aggregation pair, count)
    Array.map mapping

let applyFixedConstantAdd v0 b = apply <| aggregationFixedConstantAdd b v0
let applyFixedFractionAdd v0 b = apply <| aggregationFixedFractionAdd b v0
let applyFixedFractionMul v0 b = apply <| aggregationFixedFractionMul b v0

//-------------------------------------------------------------------------------------------------

let private roundOff (value : float) =
    System.Math.Round(value, 3)

let private figurePercent operator v0 samples =
    samples
    |> Array.filter (fun x -> operator (x |> fst |> roundOff) v0)
    |> Array.map snd
    |> Array.sum
    |> roundOff

let percentProfitLossBreakeven v0 samples =
    let gt = figurePercent (>) v0 samples
    let lt = figurePercent (<) v0 samples
    let eq = figurePercent (=) v0 samples
    (gt, lt, eq)
