module Compute

open System
open MathNet.Numerics
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.Optimization
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

let private adjustGD evaluateGradient (xs, i) =

    let step = min 0.0001 (1.0 / float (1 + i))
    let maglimit = 0.0001

    let gs = evaluateGradient xs
    let magnitude = gs |> Array.sumBy (fun x -> x ** 2.0) |> (fun x -> x ** 0.5)
    if (magnitude < maglimit) then
        None
    else
        let gamma = step / magnitude
        let xsNew = Array.zip xs gs |> Array.map (fun (x, g) -> x - gamma * g)
        let state = (xsNew, i + 1)
        Some (xsNew, state)

let minimizeGD n m costfunc xs =

    let generator =
        costfunc
        |> gradient n m
        |> evaluate n m
        |> adjustGD

    let trace = Array.append [| xs |] <| Array.unfold generator (xs, 0)
    let final = Array.last trace
    let count = Array.length trace - 1

    (final, count, Some trace)

//-------------------------------------------------------------------------------------------------

let private f n m costfunc xs =
    let fs = costfunc |> Array.singleton
    xs
    |> Vector.toArray
    |> evaluate n m fs
    |> Array.exactlyOne

let private g n m costfunc xs =
    let fs = costfunc |> gradient n m
    xs
    |> Vector.toArray
    |> evaluate n m fs
    |> DenseVector.ofArray

let private h n m costfunc xs =
    let fs = costfunc |> gradient n m |> Array.map (gradient n m)
    xs
    |> Vector.toArray
    |> (fun xs -> fs |> Array.map (fun fs -> evaluate n m fs xs))
    |> DenseMatrix.ofRowArrays

let private toFunc f =
    Func<'T, 'U>(f)

let private mapResultOutput (result : MinimizationResult) =
    let final = result.MinimizingPoint.ToArray()
    let count = result.Iterations
    let trace = None
    (final, count, trace)

//-------------------------------------------------------------------------------------------------

let minimize01 n m costfunc xs =

    let f = toFunc <| f n m costfunc

    let perturbation = DenseVector.create (n + m - 1) 1.0
    let objective = ObjectiveFunction.Value(f)
    let minimizer = Optimization.NelderMeadSimplex(1e-8, 1000)
    mapResultOutput <| minimizer.FindMinimum(objective, DenseVector.ofArray xs, perturbation)

let minimize02 n m costfunc xs =

    let f = toFunc <| f n m costfunc
    let g = toFunc <| g n m costfunc

    let objective = ObjectiveFunction.Gradient(f, g)
    let minimizer = Optimization.BfgsMinimizer(1e-8, 1e-8, 1e-8, 1000)
    mapResultOutput <| minimizer.FindMinimum(objective, DenseVector.ofArray xs)

let minimize03 n m costfunc xs =

    let f = toFunc <| f n m costfunc
    let g = toFunc <| g n m costfunc
    let h = toFunc <| h n m costfunc

    let objective = ObjectiveFunction.GradientHessian(f, g, h)
    let minimizer = Optimization.NewtonMinimizer(1e-8, 1000)
    mapResultOutput <| minimizer.FindMinimum(objective, DenseVector.ofArray xs)

//-------------------------------------------------------------------------------------------------

let rootfinder n m lagrange xs =

    let f =
        lagrange
        |> gradient n m
        |> evaluate n m
        |> toFunc

    RootFinding.Broyden.FindRoot(f, xs, 1e-8, 1000)

//-------------------------------------------------------------------------------------------------

let isConstraintQualificationMet n constraints final =

    let vectors =
        constraints
        |> Array.map (gradient n 0)
        |> Array.map (evaluate n 0)
        |> Array.map (fun f -> f final)

    let matrix = Matrix.Build.DenseOfColumnArrays(vectors)

    if (matrix.ColumnCount = 1) then
        let zero = matrix |> Matrix.forall (fun x -> x = 0.0)
        not zero
    else
        let rank = matrix |> Matrix.rank
        let cols = matrix |> Matrix.columnCount
        rank = cols

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

//-------------------------------------------------------------------------------------------------

let stepsize iterations =
    let step i = min 0.0001 (1.0 / float (1 + i))
    let step i = (i, step i)
    Array.init (iterations + 1) step
