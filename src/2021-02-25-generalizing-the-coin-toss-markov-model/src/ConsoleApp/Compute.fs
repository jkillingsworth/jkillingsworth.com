module Compute

open MathNet.Numerics
open MathNet.Numerics.LinearAlgebra
open MathNet.Symbolics
open MathNet.Symbolics.Operators

//-------------------------------------------------------------------------------------------------

let private reshapePmfToRs (pmf : float[]) =

    let n = pmf |> Array.length |> (+) -1
    let f = pmf |> Array.get
    Array.init ((n / 2) + 1) f |> Array.rev

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

let convertBiasesToXs n m λStart biases =
    let ps = reshapeBiasesToPs n biases
    let λs = Array.create m λStart
    Array.append (Array.skip 1 ps) λs

let convertXsToBiases n xs =
    let ps = xs |> Array.take (n - 1) |> Array.append [| 0.5 |]
    reshapePsToBiases n ps

let biasesEqual n =
    let ps = Array.create n 0.5
    reshapePsToBiases n ps

let biasesSlope n x =
    let step = (0.5 - x) / float (n - 1)
    let ps = Array.init n (fun i -> 0.5 + step * float i)
    reshapePsToBiases n ps

//-------------------------------------------------------------------------------------------------

type ScoringFunction =
    | Sa
    | Sb

let private scoring (ps : Expression[]) i = function
    | Sa -> ps.[i] - ps.[0]
    | Sb -> ps.[i] - ps.[i - 1]

let scoringfunc n sf =
    let initialize i = if (i = 0) then real 0.5 else symbol <| sprintf "p%i" i
    let ps = Array.init n initialize
    [| 1 .. (n - 1) |]
    |> Array.map (fun i -> scoring ps i sf)
    |> Array.map (fun x -> x ** 2)
    |> Array.sum

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
    Array.unfold f 0 |> Array.rev

let private createFunctions pmf (vector : Expression[]) =

    let rs = reshapePmfToRs pmf
    (rs, vector) ||> Array.map2 (fun r ex -> r - ex)

let constraints n pmf =
    n
    |> generateOutcome
    |> condenseOutcome
    |> createFunctions pmf

//-------------------------------------------------------------------------------------------------

let lagrange (scoringfunc, constraints) =

    let folder (i, acc) f =
        let i = i + 1
        let s = sprintf "λ%i" i
        let λ = symbol s
        (i, acc - (λ * f))

    constraints |> Array.fold folder (0, scoringfunc) |> snd

let gradient n m f =

    let derivative s i =
        let s = sprintf "%s%i" s (i + 1)
        let x = symbol s
        f |> Calculus.differentiate x

    let dps = derivative "p" |> Array.init (n - 1)
    let dλs = derivative "λ" |> Array.init m
    Array.append dps dλs

//-------------------------------------------------------------------------------------------------

let private evaluateExecution funcs xs =
    Array.map (fun f -> f xs) funcs

let private evaluate n m fs =

    let mapping s i = sprintf "%s%i" s (i + 1)
    let ps = (mapping "p") |> Array.init (n - 1)
    let λs = (mapping "λ") |> Array.init m
    let symbols = Array.append ps λs |> Array.map Symbol

    fs
    |> Array.map (Compile.expression symbols)
    |> evaluateExecution

//-------------------------------------------------------------------------------------------------

let private computeF fx xs =
    fx xs

let private computeJ jx xs =
    jx
    |> Array.map ((|>) xs)
    |> array2D

let private compileF n m fs =
    fs
    |> evaluate n m
    |> computeF

let private compileJ n m fs =
    fs
    |> Array.map (gradient n m)
    |> Array.map (evaluate n m)
    |> computeJ

//-------------------------------------------------------------------------------------------------

let private increment (gamma : float) f j xs =

    let xn = Vector.Build.DenseOfArray(xs)
    let fn = Vector.Build.DenseOfArray(xs |> f)
    let jn = Matrix.Build.DenseOfArray(xs |> j)

    Vector.toArray <| xn - gamma * jn.PseudoInverse() * fn

let rootfind gamma n m fs xs =

    let f = compileF n m fs
    let j = compileJ n m fs

    let error xs = f xs |> Array.sumBy (fun x -> x ** 2.0) |> (fun x -> x ** 0.5)
    let somevalue xs = Some (xs, xs)
    let generator xs =
        if (error xs < 1e-8) then
            None
        else
            somevalue <| increment gamma f j xs

    let trace = Array.append [| xs |] <| Array.unfold generator xs
    let final = Array.last trace
    let count = Array.length trace - 1

    (final, count)
