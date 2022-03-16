module Compute

open System
open MathNet.Numerics

//-------------------------------------------------------------------------------------------------

let private pi = Math.PI

//-------------------------------------------------------------------------------------------------

let funcExample1 t =
    sin (2.0 * pi * t)

let funcExample2 t =
    sin (2.0 * pi * t)

let funcExample3 t =
    let x1 = 1.00 * cos ((2.0 * pi) * (1.0 * t) + (-0.50 * pi))
    let x2 = 0.50 * cos ((2.0 * pi) * (2.0 * t) + (+0.75 * pi))
    let x3 = 0.75 * cos ((2.0 * pi) * (3.0 * t) + (-0.25 * pi))
    x1 + x2 + x3

let funcExample4 t =
    sin (2.0 * pi * t) + 0.5

let funcExample5 t =
    sin (2.0 * pi * t) - 0.5

let funcExample6 t =
    2.5 * t - 1.0

//-------------------------------------------------------------------------------------------------

let fourierForward timeXs (f : float) =

    let mapper (t : float, x : float) =
        let i = Complex.onei
        let ω = 2.0 * pi * f
        x * Complex.exp (-1.0 * i * ω * t)

    let result =
        timeXs
        |> Array.map mapper
        |> Array.reduce (+)

    result / (timeXs |> Array.length |> float)

let fourierInverse freqYs (t : float) =

    let mapper (f : float, y : complex) =
        let i = Complex.onei
        let ω = 2.0 * pi * f
        y * Complex.exp (+1.0 * i * ω * t)

    let result =
        freqYs
        |> Array.map mapper
        |> Array.reduce (+)

    result |> Complex.realPart

//-------------------------------------------------------------------------------------------------

let private coerceZero y = if Precision.AlmostEqual(y, Complex.zero) then Complex.zero else y

let re = coerceZero >> Complex.realPart
let im = coerceZero >> Complex.imagPart
let ma = coerceZero >> Complex.magnitude
let ph = coerceZero >> Complex.phase

//-------------------------------------------------------------------------------------------------

let private double (y : complex) = y * 2.0
let private naught (y : complex) = y * Complex.zero

let private halfFreqs freqYs lower upper =
    let N = Array.length freqYs
    let u = (N - 1) / 2
    let v = (N + 2) / 2
    let mapping y = function
        | k when 1 <= k && k <= u -> lower y
        | k when v <= k && k <= N -> upper y
        | _ -> y
    Array.mapi (fun k (f, y) -> f, mapping y k) freqYs

let halfLower freqYs =
    halfFreqs freqYs double naught

let halfUpper freqYs =
    halfFreqs freqYs naught double

//-------------------------------------------------------------------------------------------------

let isolateFreq kFreq =
    Array.mapi (fun k (f, y) -> if k = kFreq then (f, y) else (f, Complex.zero))
