module Compute

open MathNet.Numerics
open MathNet.Numerics.LinearAlgebra
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

type ConstraintSetDependentEquations =
    | Inclusive
    | Exclusive

let private p1 = symbol "p1"
let private p2 = symbol "p2"
let private p3 = symbol "p3"

let private constraints3 constraintSet (pmf : float[]) =
    let r1 = pmf.[2]
    let r3 = pmf.[3]
    let f1 = r1 - 0.5 * (1 - p1 * p2)
    let f2 = r3 - 0.5 * (p1 * p2)
    match constraintSet with
    | Inclusive -> [| f1; f2 |]
    | Exclusive -> [| f1 |]

let private constraints4 constraintSet (pmf : float[]) =
    let r0 = pmf.[2]
    let r2 = pmf.[3]
    let r4 = pmf.[4]
    let f1 = r0 - (1 - p1) * (1 - p1 * p2)
    let f2 = r2 - 0.5 * (p1 + p1 * p2 - p1 ** 2 * p2 - p1 * p2 * p3)
    let f3 = r4 - 0.5 * (p1 * p2 * p3)
    match constraintSet with
    | Inclusive -> [| f1; f2; f3 |]
    | Exclusive -> [| f1; f3 |]

let constraints = function
    | 3 -> constraints3
    | 4 -> constraints4
    | n -> failwithf "No constraint functions defined for n = %i." n

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

let costfunc (fs : Expression[]) =
    fs
    |> Array.map (fun f -> f ** 2)
    |> Array.sum

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

let private increment f j xs =

    let xn = Vector.Build.DenseOfArray(xs)
    let fn = Vector.Build.DenseOfArray(xs |> f)
    let jn = Matrix.Build.DenseOfArray(xs |> j)

    Vector.toArray <| xn - jn.PseudoInverse() * fn

let rootfind n m fs xs =

    let f = compileF n m fs
    let j = compileJ n m fs

    let error xs = f xs |> Array.sumBy (fun x -> x ** 2.0) |> (fun x -> x ** 0.5)
    let somevalue xs = Some (xs, xs)
    let generator xs =
        if (error xs < 1e-8) then
            None
        else
            somevalue <| increment f j xs

    let trace = Array.append [| xs |] <| Array.unfold generator xs
    let final = Array.last trace
    let count = Array.length trace - 1

    (final, count, trace)

//-------------------------------------------------------------------------------------------------

let heatmap density costfunc λs =

    let densityX = fst density
    let densityY = snd density

    let n = 3
    let m = Array.length λs

    let f i j =
        let p1 = float i / float densityX
        let p2 = float j / float densityY
        let xs = Array.append [| p1; p2 |] λs
        let fs = Array.singleton costfunc
        xs |> evaluate n m fs |> Array.exactlyOne

    Array2D.init (densityX + 1) (densityY + 1) f

let plateau samples (pmf : float[]) =

    let r1 = pmf.[2]

    let p1Lower = 1.0 - 2.0 * r1
    let p1Upper = 1.0

    let calc p1 = (p1, (1.0 - 2.0 * r1) / p1)
    let point i = calc <| p1Lower + (float i / float samples) * (p1Upper - p1Lower)

    Array.init (samples + 1) point
