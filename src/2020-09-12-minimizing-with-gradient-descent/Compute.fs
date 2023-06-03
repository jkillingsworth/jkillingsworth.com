module Compute

open MathNet.Numerics
open MathNet.Symbolics
open MathNet.Symbolics.Operators

//-------------------------------------------------------------------------------------------------

let private symmetrical n k =
    min k (n - k) |> float

let private pmf n f =
    let arr = Array.init (n + 1) f
    let sum = Array.sum arr
    arr |> Array.map (fun x -> x / sum)

let pmfBinomial n =
    let f k = Combinatorics.Combinations(n, k)
    pmf n f

let pmfTriangle n =
    let f k = symmetrical n k |> fun i -> i + 1.0
    pmf n f

let pmfExponent n x =
    let f k = symmetrical n k |> fun i -> 1.0 / (x ** i)
    pmf n f

//-------------------------------------------------------------------------------------------------

let private r1 = symbol "r1"
let private r3 = symbol "r3"
let private p1 = symbol "p1"
let private p2 = symbol "p2"
let private f1 = r1 - 0.5 * (1 - p1 * p2)
let private f2 = r3 - 0.5 * (p1 * p2)
let private cf = (f1 ** 2) + (f2 ** 2)
let private g1 = cf |> Calculus.differentiate p1
let private g2 = cf |> Calculus.differentiate p2

let private getSymbols (pmf : float[]) (ps : float[]) =

    let r1 = pmf.[2]
    let r3 = pmf.[3]
    let p1 = ps.[0]
    let p2 = ps.[1]

    let symbols = Map.empty
    let symbols = symbols |> Map.add "r1" (FloatingPoint.Real r1)
    let symbols = symbols |> Map.add "r3" (FloatingPoint.Real r3)
    let symbols = symbols |> Map.add "p1" (FloatingPoint.Real p1)
    let symbols = symbols |> Map.add "p2" (FloatingPoint.Real p2)

    symbols

let costfunc pmf ps =
    let symbols = getSymbols pmf ps
    let result = Evaluate.evaluate symbols cf
    result.RealValue

let gradient pmf ps =
    let symbols = getSymbols pmf ps
    let x1 = Evaluate.evaluate symbols g1
    let x2 = Evaluate.evaluate symbols g2
    let result = [| x1; x2 |]
    result |> Array.map (fun x -> x.RealValue)

//-------------------------------------------------------------------------------------------------

let heatmap (pmf : float[]) density =

    let densityX = fst density
    let densityY = snd density

    let f i j =
        let p1 = float i / float densityX
        let p2 = float j / float densityY
        costfunc pmf [| p1; p2 |]

    Array2D.init (densityX + 1) (densityY + 1) f

let plateau (pmf : float[]) samples =

    let r1 = pmf.[2]

    let p1Lower = 1.0 - 2.0 * r1
    let p1Upper = 1.0

    let calc p1 = (p1, (1.0 - 2.0 * r1) / p1)
    let point i = calc <| p1Lower + (float i / float samples) * (p1Upper - p1Lower)

    Array.init (samples + 1) point

//-------------------------------------------------------------------------------------------------

let private adjust gradient ps =

    let step = 0.0001

    let gs = gradient ps
    let magnitude = gs |> Array.sumBy (fun x -> x ** 2.0) |> (fun x -> x ** 0.5)
    if (magnitude < step) then
        None
    else
        let gamma = step / magnitude
        let psNew = Array.zip ps gs |> Array.map (fun (p, g) -> p - gamma * g)
        Some (psNew, psNew)

let estimate gradient (ps : float[]) =

    let psTrace = Array.append [| ps |] <| Array.unfold (adjust gradient) ps
    let psFinal = Array.last psTrace

    (psFinal, psTrace)
