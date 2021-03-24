module Compute

open MathNet.Numerics
open MathNet.Symbolics
open MathNet.Symbolics.Operators

//-------------------------------------------------------------------------------------------------

let private reshapePmfuncToRs n pmfunc =
    pmfunc
    |> Array.skip ((n + 1) / 2)
    |> Array.take ((n / 2) + 1)

let private reshapeRsToPmfunc n (rs : float[]) =

    let init = function
        | i when i < (n / 2) -> rs.[-(2 * i - n) / 2]
        | i when i > (n / 2) -> rs.[+(2 * i - n) / 2]
        | _ -> rs.[0]

    Array.init (n + 1) init

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

let private reshapeBiasesToPs n biases =
    biases
    |> Array.skip n
    |> Array.take n

let private reshapePsToBiases n (ps : float[]) =

    let init = function
        | i when i % (2 * n) = 0 -> nan
        | i when i > n -> 0.0 + ps.[i - n]
        | i when i < n -> 1.0 - ps.[n - i]
        | _ -> 0.5

    Array.init ((2 * n) + 1) init

let biasesEqual n =
    let ps = Array.create n 0.5
    reshapePsToBiases n ps

let biasesType1 n a =
    let step = (a - 0.5) / float (n - 1)
    let ps = Array.init n (fun i -> 0.5 + step * float i)
    reshapePsToBiases n ps

let biasesType2 n a b =
    let step = (b - a) / float (n - 2)
    let ps = Array.init n (fun i -> if i = 0 then 0.5 else a + step * float (i - 1))
    reshapePsToBiases n ps

//-------------------------------------------------------------------------------------------------

let private createTransitionMatrix n =

    let length = (2 * n) + 1
    let matrix = Array2D.create length length zero

    let populatePos i k =
        let p = symbol ("p" + string k)
        matrix.[i, i + 1] <- 0 + p
        matrix.[i, i - 1] <- 1 - p

    let populateNeg i k =
        let p = symbol ("p" + string k)
        matrix.[i, i + 1] <- 1 - p
        matrix.[i, i - 1] <- 0 + p

    let populateMid i =
        matrix.[i, i + 1] <- real 0.5
        matrix.[i, i - 1] <- real 0.5

    for i = 0 to length - 1 do
        match (i - n) with
        | s when s = +n -> ()
        | s when s = -n -> ()
        | s when s > 0 -> populatePos i +s
        | s when s < 0 -> populateNeg i -s
        | _ -> populateMid i

    matrix

let private createInitiationVector n =

    let length = (2 * n) + 1
    let init i = number <| if (i = n) then 1 else 0
    Array.init length init

let private applyTransitions (vector : Expression[]) (matrix : Expression[,]) =

    let map j i _ = vector.[i] * matrix.[i, j]
    let execute j = vector |> Array.mapi (map j) |> Array.sum
    let dimension = vector |> Array.length
    execute |> Array.init dimension

let private generateOutcome n =

    let matrix = createTransitionMatrix n
    let vector = createInitiationVector n
    (n, matrix |> Array.create n |> Array.fold applyTransitions vector)

let private condenseOutcome (n, vector : Expression[]) =

    let f i = if (i > n) then None else Some (vector.[i], i + 2)
    (n, Array.unfold f 0 |> Array.rev)

let private createFunctions (n, vector : Expression[]) =

    let symbolMapping i = sprintf "p%i" (i + 1) |> Symbol
    let symbols = Array.init (n - 1) symbolMapping
    let combinedExecution fs xs = Array.map (fun f -> f xs) fs

    vector
    |> Array.map (Compile.expression symbols)
    |> combinedExecution

let private evaluations n =
    n
    |> generateOutcome
    |> condenseOutcome
    |> createFunctions

//-------------------------------------------------------------------------------------------------

let private calculateSumSquaredError n pmfunc =
    let rsTarget = reshapePmfuncToRs n pmfunc
    let evaluate = evaluations n
    let execute (a, b) =
        let ps = biasesType2 n a b |> reshapeBiasesToPs n
        let rs = evaluate ps
        rs
        |> Array.zip rsTarget
        |> Array.map (fun (x, y) -> (x - y) ** 2.0)
        |> Array.sum
    execute

//-------------------------------------------------------------------------------------------------

let heatmap n density pmfunc =

    let densityX = fst density
    let densityY = snd density
    let getError = calculateSumSquaredError n pmfunc

    let f i j =
        let a = float i / float densityX
        let b = float j / float densityY
        getError (a, b)

    Array2D.init (densityX + 1) (densityY + 1) f

//-------------------------------------------------------------------------------------------------

let estimateBiases n pmfunc (a, b) =

    let getError = calculateSumSquaredError n pmfunc

    let adjust (a, b) =

        let step = 0.0001

        let boundary = max 0.0 >> min 1.0
        let boundary (a, b) = (boundary a, boundary b)
        let proposed = Array.zeroCreate 5

        proposed.[0] <- (a, b)
        proposed.[1] <- boundary <| (a + step, b)
        proposed.[2] <- boundary <| (a - step, b)
        proposed.[3] <- boundary <| (a, b + step)
        proposed.[4] <- boundary <| (a, b - step)

        let error = proposed |> Array.map getError
        let i = [| 0 .. 4 |] |> Array.minBy (fun i -> error.[i])
        if (i = 0) then None
        else
            let next = proposed.[i]
            Some (next, next)

    let trace = Array.append [| (a, b) |] <| Array.unfold adjust (a, b)
    let count = Array.length trace - 1
    let final = Array.last trace
    let error = getError final

    (trace, count, final, error)

//-------------------------------------------------------------------------------------------------

let evaluatePmfunc n biases =
    biases
    |> reshapeBiasesToPs n
    |> evaluations n
    |> reshapeRsToPmfunc n
