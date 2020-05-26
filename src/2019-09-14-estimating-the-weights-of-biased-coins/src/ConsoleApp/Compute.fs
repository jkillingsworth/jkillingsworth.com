module Compute

open System
open System.Collections.Generic
open MathNet.Numerics
open MathNet.Numerics.Distributions
open MathNet.Numerics.Random

//-------------------------------------------------------------------------------------------------

let private makePmfunc r0 r2 r4 =
    [|
        r4
        r2
        r0
        r2
        r4
    |]

let pmfBinomial =
    let denominator = 6.0 + (2.0 * 4.0) + (2.0 * 1.0)
    let r0 = 6.0 / denominator
    let r2 = 4.0 / denominator
    let r4 = 1.0 / denominator
    makePmfunc r0 r2 r4

let pmfTriangle =
    let denominator = 3.0 + (2.0 * 2.0) + (2.0 * 1.0)
    let r0 = 3.0 / denominator
    let r2 = 2.0 / denominator
    let r4 = 1.0 / denominator
    makePmfunc r0 r2 r4

let pmfExponent x =
    let denominator = (x ** 0.0) + 2.0 * (x ** 1.0) + 2.0 * (x ** 2.0)
    let r0 = (x ** 0.0) / denominator
    let r2 = (x ** 1.0) / denominator
    let r4 = (x ** 2.0) / denominator
    makePmfunc r0 r2 r4

//-------------------------------------------------------------------------------------------------

let private makeBiases p1 p2 p3 =
    [|
        nan
        1.0 - p3
        1.0 - p2
        1.0 - p1
        0.5
        p1
        p2
        p3
        nan
    |]

let private biasesLimit (pmf : float[]) p1Limit =
    let r0 = pmf.[2]
    let r4 = pmf.[0]
    let p1 = p1Limit r0 r4
    let p2 = (1.0 - p1 - r0) / (p1 * (1.0 - p1))
    let p3 = (2.0 * r4 * (1.0 - p1)) / (1.0 - p1 - r0)
    makeBiases p1 p2 p3

let biasesLower pmf =
    let p1Lower r0 r4 = 1.0 - (sqrt r0)
    biasesLimit pmf p1Lower

let biasesUpper pmf =
    let p1Upper r0 r4 = (2.0 * r4 - 1.0 + r0) / (2.0 * r4 - 1.0)
    biasesLimit pmf p1Upper

let biasesEqual =
    makeBiases 0.5 0.5 0.5

//-------------------------------------------------------------------------------------------------

let private sequences =
    [|
        "TTTT"
        "TTTH"
        "TTHT"
        "THTT"
        "HTTT"
        "TTHH"
        "THTH"
        "HTTH"
        "THHT"
        "HTHT"
        "HHTT"
        "THHH"
        "HTHH"
        "HHTH"
        "HHHT"
        "HHHH"
    |]

let private sorter s =
    let heads = s |> Seq.filter (fun x -> x = 'H') |> Seq.length
    let flips = s |> Seq.rev |> Seq.toArray |> String
    (heads, flips)

let private makeTosses p1 p2 p3 =

    let ps = [| 0.5; p1; p2; p3 |]

    let calculation (state, value) = function
        | 'H' when state = 0 -> state + 1, value * ps.[abs state]
        | 'H' when state > 0 -> state + 1, value * ps.[abs state]
        | 'H' when state < 0 -> state + 1, value * (1.0 - ps.[abs state])
        | 'T' when state = 0 -> state - 1, value * ps.[abs state]
        | 'T' when state < 0 -> state - 1, value * ps.[abs state]
        | 'T' when state > 0 -> state - 1, value * (1.0 - ps.[abs state])
        | _ -> failwith "Invalid flip."

    let calculate sequence =
        sequence, sequence |> Seq.fold calculation (0, 1.0) |> snd

    sequences
    |> Array.sortBy sorter
    |> Array.map calculate

//-------------------------------------------------------------------------------------------------

let private r0Calculation (p : float[]) =
    0.0
    + (p.[0] * (1.0 - p.[1]) * p.[0] * (1.0 - p.[1])) * 4.0
    + (p.[0] * p.[1] * (1.0 - p.[2]) * (1.0 - p.[1])) * 2.0

let private r2Calculation (p : float[]) =
    0.0
    + (p.[0] * (1.0 - p.[1]) * p.[0] * p.[1]) * 2.0
    + (p.[0] * p.[1] * (1.0 - p.[2]) * p.[1])
    + (p.[0] * p.[1] * p.[2] * (1.0 - p.[3]))

let private r4Calculation (p : float[]) =
    0.0
    + (p.[0] * p.[1] * p.[2] * p.[3])

let evaluate (biases : float[]) =

    let p1 = biases.[5]
    let p2 = biases.[6]
    let p3 = biases.[7]
    let ps = [| 0.5; p1; p2; p3 |]

    let r0 = r0Calculation ps
    let r2 = r2Calculation ps
    let r4 = r4Calculation ps

    let pmfunc = makePmfunc r0 r2 r4
    let tosses = makeTosses p1 p2 p3

    (pmfunc, tosses)

//-------------------------------------------------------------------------------------------------

let private flipTheCoin (biases : float[]) state random =
    let value = Sample.continuousUniform 0.0 1.0 random
    if (value < biases.[state]) then true else false

let simulate iterations biases =

    let n = 4
    let random = Random.SystemRandomSource(false)
    let pmfunc = Array.zeroCreate<uint64> (n + 1)
    let tosses = Dictionary<string, uint64>()

    for sequence in sequences do tosses.[sequence] <- 0UL

    for _ in 1 .. iterations  do
        let mutable state = n
        let tracker = Array.zeroCreate<char> n
        for i = 0 to n - 1 do
            match flipTheCoin biases state random with
            | true  -> state <- state + 1; tracker.[i] <- 'H'
            | false -> state <- state - 1; tracker.[i] <- 'T'
        let endstate = state / 2
        let sequence = String(tracker)
        pmfunc.[endstate] <- 1UL + pmfunc.[endstate]
        tosses.[sequence] <- 1UL + tosses.[sequence]

    let pmfunc =
        pmfunc
        |> Array.map (fun x -> float x / float iterations)

    let tosses =
        tosses
        |> Seq.map (fun x -> x.Key, float x.Value / float iterations)
        |> Seq.sortBy (fun (s, _) -> sorter s)
        |> Seq.toArray

    (pmfunc, tosses)

//-------------------------------------------------------------------------------------------------

let private adjust random fs rs ps _ =

    let step = 0.00001

    let s = Sample.discreteUniform 0 1 random
    let i = Sample.discreteUniform 1 3 random
    let j = Sample.discreteUniform 0 2 random

    let psOriginal = ps
    let psProposed = ps |> Array.copy

    psProposed.[i] <- psProposed.[i] + (if s = 0 then +step else -step)

    let rCalculation = fs |> Array.item j
    let rDestination = rs |> Array.item j
    let diffOriginal = abs (rDestination - rCalculation psOriginal)
    let diffProposed = abs (rDestination - rCalculation psProposed)

    if (diffProposed < diffOriginal) then
        psProposed
    else
        psOriginal

let estimate iterations (biases : float[]) (pmf : float[]) =

    let random = Random.SystemRandomSource(false)

    let f0 = r0Calculation
    let f2 = r2Calculation
    let f4 = r4Calculation

    let r0 = pmf.[2]
    let r2 = pmf.[3]
    let r4 = pmf.[4]

    let p1 = biases.[5]
    let p2 = biases.[6]
    let p3 = biases.[7]

    let fs = [| f0; f2; f4 |]
    let rs = [| r0; r2; r4 |]
    let ps = [| 0.5; p1; p2; p3 |]

    let ps = seq { 1 .. iterations } |> Seq.fold (adjust random fs rs) ps

    let p1 = ps.[1]
    let p2 = ps.[2]
    let p3 = ps.[3]

    makeBiases p1 p2 p3
