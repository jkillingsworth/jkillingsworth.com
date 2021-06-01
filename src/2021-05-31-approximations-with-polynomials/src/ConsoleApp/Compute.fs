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

let private boundary value =
    value
    |> max 0.0
    |> min 1.0

let biases n f =
    let init i = if i = 0 then 0.5 else boundary <| f (float i)
    let ps = Array.init n init
    reshapePsToBiases n ps

//-------------------------------------------------------------------------------------------------

let private polynomial1 (args : float[]) =
    let p1 = args.[0]
    let p2 = args.[1]
    let a = (-1.0 * p1) + (+1.0 * p2)
    let b = (+2.0 * p1) + (-1.0 * p2)
    let f x = (a * x) + b
    f

let private polynomial2 (args : float[]) =
    let p1 = args.[0]
    let p2 = args.[1]
    let p3 = args.[2]
    let a = (+0.5 * p1) + (-1.0 * p2) + (+0.5 * p3)
    let b = (-2.5 * p1) + (+4.0 * p2) + (-1.5 * p3)
    let c = (+3.0 * p1) + (-3.0 * p2) + (+1.0 * p3)
    let f x = (a * pown x 2) + (b * x) + c
    f

let private polynomial3 (args : float[]) =
    let p1 = args.[0]
    let p2 = args.[1]
    let p3 = args.[2]
    let p4 = args.[3]
    let a = (1.0 / 6.0) * (( -1.0 * p1) + ( +3.0 * p2) + ( -3.0 * p3) + ( +1.0 * p4))
    let b = (1.0 / 6.0) * (( +9.0 * p1) + (-24.0 * p2) + (+21.0 * p3) + ( -6.0 * p4))
    let c = (1.0 / 6.0) * ((-26.0 * p1) + (+57.0 * p2) + (-42.0 * p3) + (+11.0 * p4))
    let d = (1.0 / 6.0) * ((+24.0 * p1) + (-36.0 * p2) + (+24.0 * p3) + ( -6.0 * p4))
    let f x = (a * pown x 3) + (b * pown x 2) + (c * x) + d
    f

let polynomial = function
    | 1 -> polynomial1
    | 2 -> polynomial2
    | 3 -> polynomial3
    | n -> failwithf "Polynomial degree %i not implemented." n

let polynomialPoints n samples f =

    let init i =
        let x = float (i * n) / float samples
        let y = f x
        (x, y)

    Array.init (samples + 1) init

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

    let symbolMapping i = sprintf "p%i" i |> Symbol
    let symbols = Array.init n symbolMapping
    let combinedExecution fs ps = Array.map (fun f -> f ps) fs

    vector
    |> Array.map (Compile.expression symbols)
    |> combinedExecution

let private evaluations n =
    n
    |> generateOutcome
    |> condenseOutcome
    |> createFunctions

//-------------------------------------------------------------------------------------------------

let private calculateSumSquaredError n pmfunc degree =
    let rsTarget = reshapePmfuncToRs n pmfunc
    let evaluate = evaluations n
    let execute args =
        let fn = polynomial degree args
        let ps = biases n fn |> reshapeBiasesToPs n
        let rs = evaluate ps
        rs
        |> Array.zip rsTarget
        |> Array.map (fun (x, y) -> (x - y) ** 2.0)
        |> Array.sum
    execute

//-------------------------------------------------------------------------------------------------

let heatmap n density pmfunc degree =

    let densityX = fst density
    let densityY = snd density
    let getError = calculateSumSquaredError n pmfunc degree

    let f i j =
        let p1 = float i / float densityX
        let p2 = float j / float densityY
        getError [| p1; p2 |]

    Array2D.init (densityX + 1) (densityY + 1) f

//-------------------------------------------------------------------------------------------------

let estimateBiases n pmfunc degree start =

    let getError = calculateSumSquaredError n pmfunc degree

    let adjust args =

        let step = 0.00001

        let m = pown 3 (args |> Array.length)

        let nextstep x = function
            | 1 -> x + step
            | 2 -> x - step
            | _ -> x

        let populate i =
            args
            |> Array.mapi (fun j x -> nextstep x ((i / (pown 3 j)) % 3))
            |> Array.map boundary

        let proposed = Array.init m populate

        let nexterror = proposed |> Array.map getError
        let i = [| 0 .. m - 1 |] |> Array.minBy (fun i -> nexterror.[i])
        if (i = 0) then None
        else
            let next = proposed.[i]
            Some (next, next)

    let trace = Array.append [| start |] <| Array.unfold adjust start
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
