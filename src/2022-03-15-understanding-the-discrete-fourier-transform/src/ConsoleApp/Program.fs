module Program

open System
open System.Text

//-------------------------------------------------------------------------------------------------

Console.OutputEncoding <- new UTF8Encoding()

let path filename = "../../../" + filename

//-------------------------------------------------------------------------------------------------

let resolution = 500
let sampleRate = 8

let unitsX = Array.init (resolution + 1) (fun i -> (float i / float resolution))
let unitsY = Array.init (resolution + 1) (fun i -> (float i / float resolution) * float sampleRate)
let timesX = Array.init sampleRate (fun n -> float n / float sampleRate)
let freqsY = Array.init sampleRate (fun k -> float k)

let printX mapping n (t, x) = printfn "t%i = %0.4f, x%i = %+0.4f" n t n (mapping x)
let printY mapping k (f, y) = printfn "f%i = %0.4f, y%i = %+0.4f" k f k (mapping y)

//-------------------------------------------------------------------------------------------------

let runArctanSignedZero () =

    let format x tw =
        if Double.IsNegative(x) then fprintf tw "%0.4f" x else fprintf tw "%+0.4f" x

    let execute a b =
        let φ = Math.Atan2(b, a)
        printfn "a = %t, b = %t, φ = %t" (format a) (format b) (format φ)

    printfn "----------------------------------------"
    printfn "arctan-signed-zero"
    execute +0.0 +0.0
    execute +0.0 -0.0
    execute -0.0 +0.0
    execute -0.0 -0.0

//-------------------------------------------------------------------------------------------------

let runExample1 () =

    let func = Compute.funcExample1
    let name = "example-1"

    let unitXs = unitsX |> Array.map (fun t -> t, func t)
    let timeXs = timesX |> Array.map (fun t -> t, func t)
    let unitYs = unitsY |> Array.map (fun f -> f, Compute.fourierForward timeXs f)
    let freqYs = freqsY |> Array.map (fun f -> f, Compute.fourierForward timeXs f)

    Chart.renderTimeXs (path $"{name}-time-xs.svg") unitXs timeXs
    Chart.renderFreqYs (path $"{name}-freq-ys-re.svg") unitYs freqYs Compute.re "re"
    Chart.renderFreqYs (path $"{name}-freq-ys-im.svg") unitYs freqYs Compute.im "im"
    Chart.renderFreqYs (path $"{name}-freq-ys-ma.svg") unitYs freqYs Compute.ma "ma"
    Chart.renderFreqYs (path $"{name}-freq-ys-ph.svg") unitYs freqYs Compute.ph "ph"

    let unitXsRepro = unitsX |> Array.map (fun t -> t, Compute.fourierInverse freqYs t)
    let timeXsRepro = timesX |> Array.map (fun t -> t, Compute.fourierInverse freqYs t)

    Chart.renderTimeXs (path $"{name}-time-xs-repro.svg") unitXsRepro timeXsRepro

    printfn "----------------------------------------"
    printfn $"{name}-time-xs"
    timeXs |> Array.iteri (printX id)

    printfn "----------------------------------------"
    printfn $"{name}-freq-ys-re"
    freqYs |> Array.iteri (printY Compute.re)

    printfn "----------------------------------------"
    printfn $"{name}-freq-ys-im"
    freqYs |> Array.iteri (printY Compute.im)

    printfn "----------------------------------------"
    printfn $"{name}-freq-ys-ma"
    freqYs |> Array.iteri (printY Compute.ma)

    printfn "----------------------------------------"
    printfn $"{name}-freq-ys-ph"
    freqYs |> Array.iteri (printY Compute.ph)

//-------------------------------------------------------------------------------------------------

let runExample2 () =

    let func = Compute.funcExample2
    let name = "example-2"

    let timeXs = timesX |> Array.map (fun t -> t, func t)
    let unitYs = unitsY |> Array.map (fun f -> f, Compute.fourierForward timeXs f)
    let freqYs = freqsY |> Array.map (fun f -> f, Compute.fourierForward timeXs f)

    let unitYsLower = Compute.halfLower unitYs
    let freqYsLower = Compute.halfLower freqYs
    let unitXsLower = unitsX |> Array.map (fun t -> t, Compute.fourierInverse freqYsLower t)
    let timeXsLower = timesX |> Array.map (fun t -> t, Compute.fourierInverse freqYsLower t)

    Chart.renderFreqYs (path $"{name}-freq-ys-lower-ma.svg") unitYsLower freqYsLower Compute.ma "ma"
    Chart.renderFreqYs (path $"{name}-freq-ys-lower-ph.svg") unitYsLower freqYsLower Compute.ph "ph"
    Chart.renderTimeXs (path $"{name}-time-xs-lower.svg") unitXsLower timeXsLower

    let unitYsUpper = Compute.halfUpper unitYs
    let freqYsUpper = Compute.halfUpper freqYs
    let unitXsUpper = unitsX |> Array.map (fun t -> t, Compute.fourierInverse freqYsUpper t)
    let timeXsUpper = timesX |> Array.map (fun t -> t, Compute.fourierInverse freqYsUpper t)

    Chart.renderFreqYs (path $"{name}-freq-ys-upper-ma.svg") unitYsUpper freqYsUpper Compute.ma "ma"
    Chart.renderFreqYs (path $"{name}-freq-ys-upper-ph.svg") unitYsUpper freqYsUpper Compute.ph "ph"
    Chart.renderTimeXs (path $"{name}-time-xs-upper.svg") unitXsUpper timeXsUpper

//-------------------------------------------------------------------------------------------------

let runExample3 () =

    let func = Compute.funcExample3
    let name = "example-3"

    let unitXs = unitsX |> Array.map (fun t -> t, func t)
    let timeXs = timesX |> Array.map (fun t -> t, func t)
    let unitYs = unitsY |> Array.map (fun f -> f, Compute.fourierForward timeXs f)
    let freqYs = freqsY |> Array.map (fun f -> f, Compute.fourierForward timeXs f)

    let unitYs = Compute.halfLower unitYs
    let freqYs = Compute.halfLower freqYs

    Chart.renderTimeXs (path $"{name}-time-xs.svg") unitXs timeXs
    Chart.renderFreqYs (path $"{name}-freq-ys-ma.svg") unitYs freqYs Compute.ma "ma"
    Chart.renderFreqYs (path $"{name}-freq-ys-ph.svg") unitYs freqYs Compute.ph "ph"

    let freqYs1 = freqYs |> Compute.isolateFreq 1
    let unitXs1 = unitsX |> Array.map (fun t -> t, Compute.fourierInverse freqYs1 t)
    let timeXs1 = timesX |> Array.map (fun t -> t, Compute.fourierInverse freqYs1 t)
    Chart.renderTimeXs (path $"{name}-time-xs-1.svg") unitXs1 timeXs1

    let freqYs2 = freqYs |> Compute.isolateFreq 2
    let unitXs2 = unitsX |> Array.map (fun t -> t, Compute.fourierInverse freqYs2 t)
    let timeXs2 = timesX |> Array.map (fun t -> t, Compute.fourierInverse freqYs2 t)
    Chart.renderTimeXs (path $"{name}-time-xs-2.svg") unitXs2 timeXs2

    let freqYs3 = freqYs |> Compute.isolateFreq 3
    let unitXs3 = unitsX |> Array.map (fun t -> t, Compute.fourierInverse freqYs3 t)
    let timeXs3 = timesX |> Array.map (fun t -> t, Compute.fourierInverse freqYs3 t)
    Chart.renderTimeXs (path $"{name}-time-xs-3.svg") unitXs3 timeXs3

    printfn "----------------------------------------"
    printfn $"{name}-time-xs"
    timeXs |> Array.iteri (printX id)

    printfn "----------------------------------------"
    printfn $"{name}-freq-ys-ma"
    freqYs |> Array.iteri (printY Compute.ma)

    printfn "----------------------------------------"
    printfn $"{name}-freq-ys-ph"
    freqYs |> Array.iteri (printY Compute.ph)

//-------------------------------------------------------------------------------------------------

let runExample4 () =

    let func = Compute.funcExample4
    let name = "example-4"

    let unitXs = unitsX |> Array.map (fun t -> t, func t)
    let timeXs = timesX |> Array.map (fun t -> t, func t)
    let unitYs = unitsY |> Array.map (fun f -> f, Compute.fourierForward timeXs f)
    let freqYs = freqsY |> Array.map (fun f -> f, Compute.fourierForward timeXs f)

    let unitYs = Compute.halfLower unitYs
    let freqYs = Compute.halfLower freqYs

    Chart.renderTimeXs (path $"{name}-time-xs.svg") unitXs timeXs
    Chart.renderFreqYs (path $"{name}-freq-ys-ma.svg") unitYs freqYs Compute.ma "ma"
    Chart.renderFreqYs (path $"{name}-freq-ys-ph.svg") unitYs freqYs Compute.ph "ph"

//-------------------------------------------------------------------------------------------------

let runExample5 () =

    let func = Compute.funcExample5
    let name = "example-5"

    let unitXs = unitsX |> Array.map (fun t -> t, func t)
    let timeXs = timesX |> Array.map (fun t -> t, func t)
    let unitYs = unitsY |> Array.map (fun f -> f, Compute.fourierForward timeXs f)
    let freqYs = freqsY |> Array.map (fun f -> f, Compute.fourierForward timeXs f)

    let unitYs = Compute.halfLower unitYs
    let freqYs = Compute.halfLower freqYs

    Chart.renderTimeXs (path $"{name}-time-xs.svg") unitXs timeXs
    Chart.renderFreqYs (path $"{name}-freq-ys-ma.svg") unitYs freqYs Compute.ma "ma"
    Chart.renderFreqYs (path $"{name}-freq-ys-ph.svg") unitYs freqYs Compute.ph "ph"

//-------------------------------------------------------------------------------------------------

let runExample6 () =

    let func = Compute.funcExample6
    let name = "example-6"

    let unitXs = unitsX |> Array.map (fun t -> t, func t)
    let timeXs = timesX |> Array.map (fun t -> t, func t)
    let unitYs = unitsY |> Array.map (fun f -> f, Compute.fourierForward timeXs f)
    let freqYs = freqsY |> Array.map (fun f -> f, Compute.fourierForward timeXs f)

    let unitYs = Compute.halfLower unitYs
    let freqYs = Compute.halfLower freqYs

    Chart.renderTimeXs (path $"{name}-time-xs.svg") unitXs timeXs
    Chart.renderFreqYs (path $"{name}-freq-ys-ma.svg") unitYs freqYs Compute.ma "ma"
    Chart.renderFreqYs (path $"{name}-freq-ys-ph.svg") unitYs freqYs Compute.ph "ph"

    let unitXsRepro = unitsX |> Array.map (fun t -> t, Compute.fourierInverse freqYs t)
    let timeXsRepro = timesX |> Array.map (fun t -> t, Compute.fourierInverse freqYs t)

    Chart.renderTimeXs (path $"{name}-time-xs-repro.svg") unitXsRepro timeXsRepro

//-------------------------------------------------------------------------------------------------

runArctanSignedZero ()

runExample1 ()
runExample2 ()
runExample3 ()
runExample4 ()
runExample5 ()
runExample6 ()
