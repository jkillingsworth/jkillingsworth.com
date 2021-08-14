module Compute

open System.Runtime.Intrinsics.X86
open Microsoft.FSharp.NativeInterop
open MathNet.Numerics

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

let private polynomial0 (args : float[]) =
    let p1 = args.[0]
    let a = (+1.0 * p1)
    let f x = a
    f

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
    | 0 -> polynomial0
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

let private stepsizes =
    [ 0.1
      0.01
      0.001
      0.0001
      0.00001 ]

let estimateBiases n pmfunc degree value =

    let getError degree = calculateSumSquaredError n pmfunc degree

    let rec loop degree (args, steps) =

        let step = List.head steps

        let nextstep x = function
            | 1 -> x + step
            | 2 -> x - step
            | _ -> x

        let populate i =
            args
            |> Array.mapi (fun j x -> nextstep x ((i / (pown 3 j)) % 3))
            |> Array.map boundary

        let m = pown 3 (args |> Array.length)
        let proposed = Array.init m populate

        let nexterror = proposed |> Array.map (getError degree)
        let i = [| 0 .. m - 1 |] |> Array.minBy (fun i -> nexterror.[i])
        let next = proposed.[i]

        match (i, steps) with
        | 0, _::[] -> None
        | 0, _::xs -> loop degree (args, xs)
        | _, steps -> Some (next, (next, steps))

    let getBasis degree trace =
        let final = Array.last trace
        let order = Array.length final - 1
        let basis = Array.init (degree + 1) ((+) 1 >> float >> polynomial order final)
        basis

    let folder trace degree =
        Array.append trace <| Array.unfold (loop degree) (getBasis degree trace, stepsizes)

    let start = [| value |]
    let trace = [| start |]
    let trace = [| 0 .. degree |] |> Array.fold folder trace

    let count = Array.length trace - 1
    let final = getBasis degree trace
    let error = getError degree final

    (trace, count, final, error)

//-------------------------------------------------------------------------------------------------

let evaluatePmfunc n biases =
    biases
    |> reshapeBiasesToPs n
    |> evaluations n
    |> reshapeRsToPmfunc n

//-------------------------------------------------------------------------------------------------

let private flopsBaselineMethod n = ((3L * pown n 2) + (11L * n)) / 2L
let private flopsEnhancedMethod n = ((3L * pown n 2) + ( 9L * n) + (10L * (n / 2L))) / 4L

let private flops n =
    let n = int64 n
    let bm = flopsBaselineMethod n
    let em = flopsEnhancedMethod n
    (bm, em)

let flopCounts n = Array.init (n + 1) flops
