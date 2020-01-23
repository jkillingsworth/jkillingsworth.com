module Compute

open System
open MathNet.Numerics
open MathNet.Symbolics

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

let biasesEqual =
    let p1 = 0.5
    let p2 = 0.5
    let p3 = 0.5
    makeBiases p1 p2 p3

let biasesSlope x =
    let step = (1.0 - x - x) / 6.0
    let p1 = 0.5 + (1.0 * step)
    let p2 = 0.5 + (2.0 * step)
    let p3 = 0.5 + (3.0 * step)
    makeBiases p1 p2 p3

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
        | 'H' -> state + 1, value * ps.[abs state]
        | 'T' -> state - 1, value * ps.[abs state]
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

let private bundle (pmf : float[]) (biases : float[]) =

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

    (fs, rs, ps)

let private revisions ps =

    let step = 0.0001

    let psProposed = Array.zeroCreate 7

    psProposed.[0] <- ps

    for i = 1 to 3 do
        let j = i
        let psAdjusted = ps |> Array.copy
        psAdjusted.[j] <- ps.[j] + step
        psProposed.[i] <- psAdjusted

    for i = 4 to 6 do
        let j = i - 3
        let psAdjusted = ps |> Array.copy
        psAdjusted.[j] <- ps.[j] - step
        psProposed.[i] <- psAdjusted

    psProposed

let private cost fs rs ps =

    let squaredError i =
        let rCalculation = fs |> Array.item i
        let rDestination = rs |> Array.item i
        let error = rDestination - rCalculation ps
        error * error

    { 0 .. 2 } |> Seq.map squaredError |> Seq.sum

let private adjust fs rs ps =

    let psProposed = revisions ps
    let costs = psProposed |> Array.map (cost fs rs)
    let index = { 0 .. 6 } |> Seq.minBy (fun i -> costs.[i])

    (psProposed.[index], index = 0)

let estimate iterations pmf biases =

    let (fs, rs, ps) = bundle pmf biases

    let mutable values = ps
    let mutable finish = false
    let mutable cycles = 0

    while (cycles < iterations) && (finish = false) do
       let result = adjust fs rs values
       values <- fst result
       finish <- snd result
       cycles <- cycles + 1

    let ps = values

    let p1 = ps.[1]
    let p2 = ps.[2]
    let p3 = ps.[3]

    (makeBiases p1 p2 p3, cycles)

let revisionCosts pmf biases =

    let (fs, rs, ps) = bundle pmf biases

    ps
    |> revisions
    |> Array.map (cost fs rs)

//-------------------------------------------------------------------------------------------------

module private Option =

    let value (op : Option<'T>) = op.Value

module private Expr =

    let r0 = Operators.symbol "r0"
    let r2 = Operators.symbol "r2"
    let r4 = Operators.symbol "r4"

    let p1 = Operators.symbol "p1"
    let p2 = Operators.symbol "p2"
    let p3 = Operators.symbol "p3"

    let p2Derived = (1 - p1 - r0) / (p1 * (1 - p1))
    let p3Derived = (2 * r4 * (1 - p1)) / (1 - p1 - r0)

    let scoreA = (p1 - 0.5) ** 2 + (p2 - 0.5) ** 2 + (p3 - 0.5) ** 2
    let scoreB = (p1 - 0.5) ** 2 + (p2 - p1) ** 2 + (p3 - p2) ** 2

let private getSymbol = function
    | Identifier s -> s | _ -> failwith "Error."

let private getName =
    getSymbol >> (fun (Symbol n) -> n)

let private substitutions (pmf : float[]) f =

    let r0 = pmf.[2]
    let r2 = pmf.[3]
    let r4 = pmf.[4]

    f
    |> Structure.substitute Expr.p2 Expr.p2Derived
    |> Structure.substitute Expr.p3 Expr.p3Derived
    |> Structure.substitute Expr.r0 (Operators.real r0)
    |> Structure.substitute Expr.r2 (Operators.real r2)
    |> Structure.substitute Expr.r4 (Operators.real r4)

let private buildFunction f =

    let dg = Compile.compileExpressionOrThrow f [ getSymbol Expr.p1 ]
    let fn = dg :?> Func<float, float>
    let fn x = fn.Invoke(x)
    fn

let private p1FindOptimum (pmf : float[]) (lower, upper) exScore =
    exScore
    |> substitutions pmf
    |> Calculus.differentiate Expr.p1
    |> buildFunction
    |> FindRoots.ofFunction lower upper
    |> Option.value

//-------------------------------------------------------------------------------------------------

let bounds (pmf : float[]) =

    let r0 = pmf.[2]
    let r4 = pmf.[4]

    let lower = 1.0 - (sqrt r0)
    let upper = (2.0 * r4 - 1.0 + r0) / (2.0 * r4 - 1.0)

    (lower, upper)

let scoresOverall (pmf : float[]) (lower, upper) exScore =

    let samples = 100
    let compute = exScore |> substitutions pmf |> buildFunction
    let calc p1 = (p1, compute p1)

    let init = function
        | i when i = samples
            -> calc <| upper
        | i -> calc <| lower + (float i / float samples) * (upper - lower)

    init |> Array.init (samples + 1)

let coordsOptimum (pmf : float[]) (lower, upper) exScore =

    let p1 = exScore |> p1FindOptimum pmf (lower, upper)
    let fn = exScore |> substitutions pmf |> buildFunction

    (p1, fn p1)

let biasesOptimum (pmf : float[]) (lower, upper) exScore =

    let p1 = exScore |> p1FindOptimum pmf (lower, upper)
    let p2 = Expr.p2 |> substitutions pmf |> buildFunction
    let p3 = Expr.p3 |> substitutions pmf |> buildFunction

    makeBiases p1 (p2 p1) (p3 p1)

//-------------------------------------------------------------------------------------------------

let evaluateScore (biases : float[]) exScore =

    let p1 = biases.[5]
    let p2 = biases.[6]
    let p3 = biases.[7]

    let mapping (symbol, value) =
        (getName symbol, FloatingPoint.Real value)

    let symbols =
        [ (Expr.p1, p1); (Expr.p2, p2); (Expr.p3, p3) ]
        |> List.map mapping
        |> Map.ofList

    let score = exScore |> Evaluate.evaluate symbols

    score.RealValue

//-------------------------------------------------------------------------------------------------

let scoreA = Expr.scoreA
let scoreB = Expr.scoreB
