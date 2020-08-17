module Compute

open System
open MathNet.Numerics
open MathNet.Numerics.Distributions
open MathNet.Numerics.Random

//-------------------------------------------------------------------------------------------------

let private makePmfunc r1 r3 =
    [|
        r3
        r1
        r1
        r3
    |]

let pmfBinomial =
    let denominator = (2.0 * 3.0) + (2.0 * 1.0)
    let r1 = 3.0 / denominator
    let r3 = 1.0 / denominator
    makePmfunc r1 r3

let pmfTriangle =
    let denominator = (2.0 * 2.0) + (2.0 * 1.0)
    let r1 = 2.0 / denominator
    let r3 = 1.0 / denominator
    makePmfunc r1 r3

let pmfExponent x =
    let denominator = 2.0 * (x ** 0.0) + 2.0 * (x ** 1.0)
    let r1 = (x ** 0.0) / denominator
    let r3 = (x ** 1.0) / denominator
    makePmfunc r1 r3

//-------------------------------------------------------------------------------------------------

let private makeBiases p1 p2 =
    [|
        nan
        1.0 - p2
        1.0 - p1
        0.5
        p1
        p2
        nan
    |]

let biasesSlope x =
    let step = (1.0 - x - x) / 4.0
    let p1 = 0.5 + (1.0 * step)
    let p2 = 0.5 + (2.0 * step)
    makeBiases p1 p2

let biasesExact p1 p2 =
    makeBiases p1 p2

//-------------------------------------------------------------------------------------------------

let private sequences =
    [|
        "TTT"
        "TTH"
        "THT"
        "HTT"
        "THH"
        "HTH"
        "HHT"
        "HHH"
    |]

let private sorter s =
    let heads = s |> Seq.filter (fun x -> x = 'H') |> Seq.length
    let flips = s |> Seq.rev |> Seq.toArray |> String
    (heads, flips)

let private makeTosses p1 p2 =

    let ps = [| 0.5; p1; p2 |]

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

let private r1Calculation (p : float[]) =
    0.0
    + (p.[0] * (1.0 - p.[1]) * p.[0]) * 2.0
    + (p.[0] * p.[1] * (1.0 - p.[2]))

let private r3Calculation (p : float[]) =
    0.0
    + (p.[0] * p.[1] * p.[2])

let private cost fs rs ps =

    let squaredError i =
        let rCalculation = fs |> Array.item i
        let rDestination = rs |> Array.item i
        let error = rDestination - rCalculation ps
        error * error

    { 0 .. 1 } |> Seq.map squaredError |> Seq.sum

let private p2Derived (pmf : float[]) p1 =
    let r1 = pmf.[2]
    (1.0 - 2.0 * r1) / p1

//-------------------------------------------------------------------------------------------------

let private bundle (pmf : float[]) (biases : float[]) =

    let f1 = r1Calculation
    let f3 = r3Calculation

    let r1 = pmf.[2]
    let r3 = pmf.[3]

    let p1 = biases.[4]
    let p2 = biases.[5]

    let fs = [| f1; f3 |]
    let rs = [| r1; r3 |]
    let ps = [| 0.5; p1; p2 |]

    (fs, rs, ps)

//-------------------------------------------------------------------------------------------------

let heatmap (pmf : float[]) density =

    let densityX = fst density
    let densityY = snd density

    let f i j =
        let p1 = float i / float densityX
        let p2 = float j / float densityY
        cost <||| bundle pmf (makeBiases p1 p2)

    Array2D.init (densityX + 1) (densityY + 1) f


let plateau (pmf : float[]) samples =

    let r1 = pmf.[2]

    let p1Lower = 1.0 - 2.0 * r1
    let p1Upper = 1.0

    let calc p1 = (p1, p2Derived pmf p1)
    let point i = calc <| p1Lower + (float i / float samples) * (p1Upper - p1Lower)

    Array.init (samples + 1) point

//-------------------------------------------------------------------------------------------------

let evaluate (biases : float[]) =

    let p1 = biases.[4]
    let p2 = biases.[5]
    let ps = [| 0.5; p1; p2 |]

    let r1 = r1Calculation ps
    let r3 = r3Calculation ps

    let pmfunc = makePmfunc r1 r3
    let tosses = makeTosses p1 p2

    (pmfunc, tosses)

//-------------------------------------------------------------------------------------------------

let private adjust select fs rs ps =

    let step = 0.0001

    let psProposed = Array.zeroCreate 5

    psProposed.[0] <- ps

    for i = 1 to 2 do
        let j = i
        let psAdjusted = ps |> Array.copy
        psAdjusted.[j] <- ps.[j] + step
        psProposed.[i] <- psAdjusted

    for i = 3 to 4 do
        let j = i - 2
        let psAdjusted = ps |> Array.copy
        psAdjusted.[j] <- ps.[j] - step
        psProposed.[i] <- psAdjusted

    let costs = psProposed |> Array.map (cost fs rs)
    let index = { 0 .. 4 } |> Seq.minBy (fun i -> costs.[i])
    if (index = 0) then None
    else
        let ps = psProposed.[select costs index]
        let p1 = ps.[1]
        let p2 = ps.[2]
        Some ((p1, p2), ps)

let estimate select pmf biases =

    let (fs, rs, ps) = bundle pmf biases

    let psBegin = (ps.[1], ps.[2])
    let psTrace = Array.append [| psBegin |] <| Array.unfold (adjust select fs rs) ps
    let psFinal = Array.last psTrace
    let p1 = fst psFinal
    let p2 = snd psFinal

    (makeBiases p1 p2, psTrace)

let selectCalculated costs index =

    index

let selectStochastic seed =

    let random = Random.SystemRandomSource(seed, false)

    let select costs index =
        let basis = costs |> Array.item 0
        let diffs = costs |> Array.map (fun x -> basis - x) |> Array.map (max 0.0)
        let total = diffs |> Array.sum
        let range = diffs |> Array.scan (+) 0.0 |> Array.skip 1
        let r = Sample.continuousUniform 0.0 total random
        range |> Array.findIndex (fun x -> r < x)
    select

//-------------------------------------------------------------------------------------------------

let scoreA (p1, p2) = (p1 - 0.5) ** 2.0 + (p2 - 0.5) ** 2.0
let scoreB (p1, p2) = (p1 - 0.5) ** 2.0 + (p2 - p1) ** 2.0

let scoresOverall scoref plateau =
    let f (p1, p2) = (p1, p2, scoref (p1, p2))
    plateau |> Array.map f

let coordsOptimum scoref pmf =
    let f p1 = scoref (p1, p2Derived pmf p1)
    let f = Func<float, float>(f)
    let p1 = FindMinimum.OfScalarFunction(f, 0.5)
    let p2 = p2Derived pmf p1
    let score = scoref (p1, p2)
    (p1, p2, score)

let biasesOptimum (p1, p2, _) =
    makeBiases p1 p2

let heatmapScores density scoref =

    let densityX = fst density
    let densityY = snd density

    let f i j =
        let p1 = float i / float densityX
        let p2 = float j / float densityY
        scoref (p1, p2)

    Array2D.init (densityX + 1) (densityY + 1) f
