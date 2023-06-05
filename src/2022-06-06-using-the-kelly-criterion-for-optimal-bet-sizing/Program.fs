module Program

open System
open System.Text

//-------------------------------------------------------------------------------------------------

Console.OutputEncoding <- new UTF8Encoding()

let path filename = Common.Const.runoutput + filename

//-------------------------------------------------------------------------------------------------

let runSimulations () =

    let v0 = 100.0

    let n = 200
    let p = 0.6
    let q = 0.4
    let b = 0.1

    let vsA, hCountA, tCountA = Compute.runSimulation v0 n p q b 0
    let vsB, hCountB, tCountB = Compute.runSimulation v0 n p q b 1
    let vsC, hCountC, tCountC = Compute.runSimulation v0 n p q b 2

    Chart.renderSimulations (path "coin-toss-simulations.svg") vsA vsB vsC

    printfn "----------------------------------------"
    printfn "simulation-A"
    printfn "H:  %8i" hCountA
    printfn "T:  %8i" tCountA
    printfn "Vn: %8.2f" vsA.[n]

    printfn "----------------------------------------"
    printfn "simulation-B"
    printfn "H:  %8i" hCountB
    printfn "T:  %8i" tCountB
    printfn "Vn: %8.2f" vsB.[n]

    printfn "----------------------------------------"
    printfn "simulation-C"
    printfn "H:  %8i" hCountC
    printfn "T:  %8i" tCountC
    printfn "Vn: %8.2f" vsC.[n]

//-------------------------------------------------------------------------------------------------

let runExampleDiscrete i bRange gRange ps ws =

    let gb = Compute.gKellyDiscrete ps ws
    let bs = Compute.rangeNew bRange
    let gs = Compute.rangeMap bs gb
    let b' = Compute.bOptimal gb
    let g' = gb b'
    let f' = gb b' |> exp

    Chart.renderGs (path $"example-{i}-gs.svg") gRange bs gs (b', g')

    printfn "----------------------------------------"
    printfn $"example-{i}"
    printfn "b* = %+7.4f" b'
    printfn "g* = % 7.4f" g'
    printfn "f* = % 7.4f" f'

//-------------------------------------------------------------------------------------------------

let runExampleContinuous i bRange gRange xRange pRange wRange μ σ c r s =

    let p = Compute.pLogNormalDistribution μ σ
    let w = Compute.wStockWithBoundedExits c r s

    let gb = Compute.gKellyContinuous p w r s
    let bs = Compute.rangeNew bRange
    let gs = Compute.rangeMap bs gb
    let b' = Compute.bOptimal gb
    let g' = gb b'
    let f' = gb b' |> exp

    let xs = Compute.rangeNew xRange
    let ps = Compute.rangeMap xs p
    let ws = Compute.rangeMap xs w

    Chart.renderGs (path $"example-{i}-gs.svg") gRange bs gs (b', g')
    Chart.renderPs (path $"example-{i}-ps.svg") pRange xs ps
    Chart.renderWs (path $"example-{i}-ws.svg") wRange xs ws

    printfn "----------------------------------------"
    printfn $"example-{i}"
    printfn "μ  = % 7.4f = log(%7.4f)" μ (exp μ)
    printfn "σ  = % 7.4f = log(%7.4f)" σ (exp σ)
    printfn "b* = %+7.4f" b'
    printfn "g* = % 7.4f" g'
    printfn "f* = % 7.4f" f'

//-------------------------------------------------------------------------------------------------

let runExample1 () =

    let bRange = (-0.20, +0.50)
    let gRange = (-0.04, +0.04)

    let ps = [| 0.60; 0.40 |]
    let ws = [| +1.0; -1.0 |]

    runExampleDiscrete 1 bRange gRange ps ws

//-------------------------------------------------------------------------------------------------

let runExample2 () =

    let bRange = (-0.20, +0.50)
    let gRange = (-0.04, +0.04)

    let ps = [| 0.30; 0.70 |]
    let ws = [| +4.0; -1.0 |]

    runExampleDiscrete 2 bRange gRange ps ws

//-------------------------------------------------------------------------------------------------

let runExample3 () =

    let bRange = (-0.20, +0.50)
    let gRange = (-0.04, +0.04)

    let ps = [| 0.10; 0.10; 0.20; 0.30; 0.20; 0.10 |]
    let ws = [| +3.0; +2.0; +1.0; +0.0; -1.0; -2.0 |]

    runExampleDiscrete 3 bRange gRange ps ws

//-------------------------------------------------------------------------------------------------

let runExample4 () =

    let bRange = (-1.00, +6.00)
    let gRange = (-0.04, +0.04)

    let ps = [| 0.10; 0.10; 0.20; 0.30; 0.20; 0.10 |]
    let xs = [| 23.0; 22.0; 21.0; 20.0; 19.0; 18.0 |]

    let c = 20.0
    let ws = xs |> Array.map (fun x -> (x - c) / c)

    runExampleDiscrete 4 bRange gRange ps ws

//-------------------------------------------------------------------------------------------------

let runExample5 () =

    let bRange = (-1.00, +6.00)
    let gRange = (-0.04, +0.04)

    let xRange = (+15.0, +25.0)
    let pRange = (+0.00, +0.30)
    let wRange = (-0.20, +0.20)

    let μ = log 20.25
    let σ = log 1.072

    let c = 20.0
    let r = 17.0
    let s = 23.0

    runExampleContinuous 5 bRange gRange xRange pRange wRange μ σ c r s

//-------------------------------------------------------------------------------------------------

let runExample6 () =

    let bRange = (-6.00, +1.00)
    let gRange = (-0.04, +0.04)

    let xRange = (+15.0, +25.0)
    let pRange = (+0.00, +0.30)
    let wRange = (-0.20, +0.20)

    let μ = log 19.65
    let σ = log 1.076

    let c = 20.0
    let r = 17.0
    let s = 23.0

    runExampleContinuous 6 bRange gRange xRange pRange wRange μ σ c r s

//-------------------------------------------------------------------------------------------------

let runLimitations1 () =

    let bRange = (-4.00, +4.00)

    let c = 20.0

    let bs, rMins, sMaxs = Compute.limitations1 bRange c

    Chart.renderLimitations1 (path "limitations-1.svg") bs rMins sMaxs c

//-------------------------------------------------------------------------------------------------

let runLimitations2 () =

    let bRange = (-4.00, +4.00)

    let c = 20.0
    let ℓ = 0.50
    let m = 0.25

    let bs, rMins, sMaxs = Compute.limitations2 bRange c ℓ m

    Chart.renderLimitations2 (path "limitations-2.svg") bs rMins sMaxs c

//-------------------------------------------------------------------------------------------------

runSimulations ()
runExample1 ()
runExample2 ()
runExample3 ()
runExample4 ()
runExample5 ()
runExample6 ()
runLimitations1 ()
runLimitations2 ()
