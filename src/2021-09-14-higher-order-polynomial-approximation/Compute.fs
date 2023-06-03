module Compute

open System
open System.Runtime.Intrinsics.X86
open Microsoft.FSharp.NativeInterop
open MathNet.Numerics
open MathNet.Numerics.LinearAlgebra
open MathNet.Numerics.Optimization

#nowarn "9"

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

let polynomial degree =

    let m = degree + 1

    let init row col = (float (row + 1)) ** (float col)

    let matrix =
        init
        |> DenseMatrix.init m m
        |> Matrix.inverse

    let matrix = matrix.ToRowMajorArray()

    let init args row =
        args
        |> Array.mapi (fun i pi -> pi * matrix.[(row * m) + i])
        |> Array.sum

    let func args x =
        init args
        |> Array.init m
        |> Array.mapi (fun i a -> a * pown x i)
        |> Array.sum

    func

let polynomialPoints n samples f =

    let init i =
        let x = float (i * n) / float samples
        let y = f x
        (x, y)

    Array.init (samples + 1) init

//-------------------------------------------------------------------------------------------------

let private evaluations n (ps : float[]) =

    let simd = 2
    let pads = 2
    let wide = ((n / 2) + 1) + pads + (simd - 1)

    let blocks = NativePtr.stackalloc<float> (wide * 6)

    for i = 0 to ((wide * 6) - 1) do
        NativePtr.set blocks i 0.0

    let rBuffer0 = NativePtr.add blocks ((wide * 0) + 1)
    let rBuffer1 = NativePtr.add blocks ((wide * 1) + 1)
    let uInitial = NativePtr.add blocks ((wide * 2) + 1)
    let vInitial = NativePtr.add blocks ((wide * 3) + 1)
    let xInitial = NativePtr.add blocks ((wide * 4) + 1)
    let yInitial = NativePtr.add blocks ((wide * 5) + 1)

    for i = 0 to (n - 1) / 2 do
        NativePtr.set uInitial i (0.0 + ps.[2 * i])
        NativePtr.set vInitial i (1.0 - ps.[2 * i])

    for i = 0 to (n - 2) / 2 do
        NativePtr.set xInitial i (0.0 + ps.[2 * i + 1])
        NativePtr.set yInitial i (1.0 - ps.[2 * i + 1])

    NativePtr.set rBuffer0 0 1.0

    let uHeads = NativePtr.add uInitial 0
    let vTails = NativePtr.add vInitial +1
    let xHeads = NativePtr.add xInitial -1
    let yTails = NativePtr.add yInitial 0

    let mutable rLookup = rBuffer0
    let mutable rOutput = rBuffer1

    for k = 1 to n do

        let rHeads = if ((k % 2) = 0) then NativePtr.add rLookup -1 else rLookup
        let rTails = if ((k % 2) = 1) then NativePtr.add rLookup +1 else rLookup
        let pHeads = if ((k % 2) = 1) then uHeads else xHeads
        let qTails = if ((k % 2) = 1) then vTails else yTails

        for h = 0 to k / simd / 2 do
            let i = (h * simd)

            let vrHeads = Sse2.LoadVector128(NativePtr.add rHeads i)
            let vrTails = Sse2.LoadVector128(NativePtr.add rTails i)
            let vpHeads = Sse2.LoadVector128(NativePtr.add pHeads i)
            let vqTails = Sse2.LoadVector128(NativePtr.add qTails i)

            let vh = Sse2.Multiply(vrHeads, vpHeads)
            let vt = Sse2.Multiply(vrTails, vqTails)
            let vr = Sse2.Add(vh, vt)

            Sse2.Store(NativePtr.add rOutput i, vr)

        if (k % 2) = 0 then
            NativePtr.set rOutput 0 ((NativePtr.get rOutput 0) * 2.0)

        let swap = rLookup
        rLookup <- rOutput
        rOutput <- swap

    let rsInit i = NativePtr.get rLookup i
    Array.init ((n / 2) + 1) rsInit

//-------------------------------------------------------------------------------------------------

let private calculateSumSquaredError n pmfunc polynomial =
    let rsTarget = reshapePmfuncToRs n pmfunc
    let evaluate = evaluations n
    let execute args =
        let fn = polynomial args
        let ps = biases n fn |> reshapeBiasesToPs n
        let rs = evaluate ps
        rs
        |> Array.zip rsTarget
        |> Array.map (fun (x, y) -> (x - y) ** 2.0)
        |> Array.sum
    execute

//-------------------------------------------------------------------------------------------------

let estimateBiases n pmfunc degree value =

    let start = [| value |]

    let increment degree start =

        let calculate = calculateSumSquaredError n pmfunc (polynomial degree)
        let objective = ObjectiveFunction.Value(Func<_, _>(Vector.toArray >> calculate))
        let minimizer = NelderMeadSimplex(1e-5, Int32.MaxValue)
        let resultant = minimizer.FindMinimum(objective, DenseVector.ofArray start)

        let count = resultant.Iterations
        let final = resultant.FunctionInfoAtMinimum.Point |> Vector.toArray
        let error = resultant.FunctionInfoAtMinimum.Value

        (count, final, error)

    let folder (total, start, _) degree =
        let order = Array.length start - 1
        let basis = Array.init (degree + 1) ((+) 1 >> float >> polynomial order start)
        let count, final, error = increment degree basis
        (total + count, final, error)

    [| 0 .. degree |] |> Array.fold folder (0, start, nan)

//-------------------------------------------------------------------------------------------------

let evaluatePmfunc n biases =
    biases
    |> reshapeBiasesToPs n
    |> evaluations n
    |> reshapeRsToPmfunc n
