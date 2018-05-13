module Compute

open MathNet.Numerics.Distributions
open MathNet.Numerics.Random
open MathNet.Numerics.Statistics

//-------------------------------------------------------------------------------------------------

type private Flip =
    | Heads
    | Tails

//-------------------------------------------------------------------------------------------------

let private mapping = function
    | 0 -> Heads
    | _ -> Tails

let private generateFlips (seed : int) =
    seed
    |> MersenneTwister
    |> Sample.discreteUniformSeq 0 1
    |> Seq.map mapping

let private rewardFixedConstantAdd b v0 = function
    | Heads -> +b * v0
    | Tails -> -b * v0

let private rewardFixedFractionAdd b = function
    | Heads -> 1.0 + b
    | Tails -> 1.0 - b

let private rewardFixedFractionMul b = function
    | Heads -> 1.0 / (1.0 - b)
    | Tails -> 1.0 - b

let private runSimulation n v0 operation reward seed =
    seed
    |> generateFlips
    |> Seq.scan (fun vn -> reward >> operation vn) (v0 : float)
    |> Seq.take (n + 1)
    |> Seq.toArray

let private runSimulationsBatch count n v0 operation reward =
    Array.init count <| runSimulation n v0 operation reward

let private aggregate f n (simulations : float[][]) =
    let initialization i = simulations |> Array.map (Array.item i) |> f
    Array.init (n + 1) initialization

//-------------------------------------------------------------------------------------------------

let runFixedConstantAdd count n v0 b =
    runSimulationsBatch count n v0 (+) <| rewardFixedConstantAdd b v0

let runFixedFractionAdd count n v0 b =
    runSimulationsBatch count n v0 (*) <| rewardFixedFractionAdd b

let runFixedFractionMul count n v0 b =
    runSimulationsBatch count n v0 (*) <| rewardFixedFractionMul b

let aggregateAvg =
    aggregate Statistics.Mean

let aggregateMed =
    aggregate Statistics.Median
