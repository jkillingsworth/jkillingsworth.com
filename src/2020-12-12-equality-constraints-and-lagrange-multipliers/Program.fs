module Program

open System
open System.Text

//-------------------------------------------------------------------------------------------------

Console.OutputEncoding <- new UTF8Encoding()

let path filename = Common.Chart.outputPath filename

//-------------------------------------------------------------------------------------------------

let density = (20, 20)
let samples = 100

//-------------------------------------------------------------------------------------------------

let sfname = function
    | Compute.Sa -> "A"
    | Compute.Sb -> "B"

let pspair (x : float[]) = (x.[0], x.[1])

let printValues n = function
    | i when i < (n - 1)
        -> printfn "p%i:         % 0.10f" (i + 1)
    | i -> printfn "λ%i:         %+0.10f" (i + 2 - n)

//-------------------------------------------------------------------------------------------------

let executeMinimize minimize desc n sf cs tag xs =

    let pmfunc = Compute.pmfExponent n 0.5
    let scoringfunc = Compute.scoringfunc n sf
    let constraints = Compute.constraints n cs pmfunc
    let m = constraints.Length

    let costfunc =
        (scoringfunc, constraints)
        |> Compute.lagrange
        |> Compute.gradient n m
        |> Compute.costfunc

    let final, count, trace = minimize n m costfunc xs
    let meetsQualifications = Compute.isConstraintQualificationMet n constraints final

    if (n = 3 && Option.isSome trace) then
        let name = sprintf "trace-%s-%s" (sfname sf) tag
        let ps = trace |> Option.get |> Array.map pspair
        let λs = final |> Array.skip (n - 1)
        let heatmap = Compute.heatmap density costfunc λs
        let plateau = Compute.plateau samples pmfunc
        Chart.renderHeatmapTraces (path name + "-heatmap.svg") heatmap plateau ps samples tag

    printfn "----------------------------------------"
    printfn "method-%s-n%i-%A-%A-%s" desc n sf cs tag
    Array.iteri (printValues n) final
    printfn "iterations: %A" <| count
    printfn "qualified:  %A" <| meetsQualifications

let executeRootfind rootfind desc n sf cs tag xs =

    let pmfunc = Compute.pmfExponent n 0.5
    let scoringfunc = Compute.scoringfunc n sf
    let constraints = Compute.constraints n cs pmfunc
    let m = constraints.Length

    let lagrange =
        (scoringfunc, constraints)
        |> Compute.lagrange

    let final = rootfind n m lagrange xs

    printfn "----------------------------------------"
    printfn "method-%s-n%i-%A-%A-%s" desc n sf cs tag
    Array.iteri (printValues n) final

let executeGD = executeMinimize Compute.minimizeGD "GD"
let execute01 = executeMinimize Compute.minimize01 "01"
let execute02 = executeMinimize Compute.minimize02 "02"
let execute03 = executeMinimize Compute.minimize03 "03"
let executeRF = executeRootfind Compute.rootfinder "RF"

//-------------------------------------------------------------------------------------------------

executeGD 3 Compute.Sa Compute.Inclusive "1" [| 0.35; 0.20; 0.0; 0.0 |]
executeGD 3 Compute.Sa Compute.Inclusive "2" [| 0.65; 0.80; 0.0; 0.0 |]
executeGD 3 Compute.Sa Compute.Inclusive "3" [| 0.20; 0.35; 0.0; 0.0 |]
executeGD 3 Compute.Sa Compute.Inclusive "4" [| 0.80; 0.20; 0.0; 0.0 |]

executeGD 3 Compute.Sb Compute.Inclusive "1" [| 0.35; 0.20; 0.0; 0.0 |]
executeGD 3 Compute.Sb Compute.Inclusive "2" [| 0.65; 0.80; 0.0; 0.0 |]
executeGD 3 Compute.Sb Compute.Inclusive "3" [| 0.20; 0.35; 0.0; 0.0 |]
executeGD 3 Compute.Sb Compute.Inclusive "4" [| 0.80; 0.20; 0.0; 0.0 |]

executeGD 4 Compute.Sa Compute.Inclusive "0" [| 0.5; 0.5; 0.5; 0.0; 0.0; 0.0 |]
executeGD 4 Compute.Sb Compute.Inclusive "0" [| 0.5; 0.5; 0.5; 0.0; 0.0; 0.0 |]

//-------------------------------------------------------------------------------------------------

executeGD 3 Compute.Sa Compute.Exclusive "0" [| 0.5; 0.5; 0.0 |]
execute01 3 Compute.Sa Compute.Exclusive "0" [| 0.5; 0.5; 0.0 |]
execute02 3 Compute.Sa Compute.Exclusive "0" [| 0.5; 0.5; 0.0 |]
execute03 3 Compute.Sa Compute.Exclusive "0" [| 0.5; 0.5; 0.0 |]

executeGD 3 Compute.Sb Compute.Exclusive "0" [| 0.5; 0.5; 0.0 |]
execute01 3 Compute.Sb Compute.Exclusive "0" [| 0.5; 0.5; 0.0 |]
execute02 3 Compute.Sb Compute.Exclusive "0" [| 0.5; 0.5; 0.0 |]
execute03 3 Compute.Sb Compute.Exclusive "0" [| 0.5; 0.5; 0.0 |]

executeGD 4 Compute.Sa Compute.Exclusive "0" [| 0.5; 0.5; 0.5; 0.0; 0.0 |]
execute01 4 Compute.Sa Compute.Exclusive "0" [| 0.5; 0.5; 0.5; 0.0; 0.0 |]
execute02 4 Compute.Sa Compute.Exclusive "0" [| 0.5; 0.5; 0.5; 0.0; 0.0 |]
execute03 4 Compute.Sa Compute.Exclusive "0" [| 0.5; 0.5; 0.5; 0.0; 0.0 |]

executeGD 4 Compute.Sb Compute.Exclusive "0" [| 0.5; 0.5; 0.5; 0.0; 0.0 |]
execute01 4 Compute.Sb Compute.Exclusive "0" [| 0.5; 0.5; 0.5; 0.0; 0.0 |]
execute02 4 Compute.Sb Compute.Exclusive "0" [| 0.5; 0.5; 0.5; 0.0; 0.0 |]
execute03 4 Compute.Sb Compute.Exclusive "0" [| 0.5; 0.5; 0.5; 0.0; 0.0 |]

//-------------------------------------------------------------------------------------------------

executeRF 3 Compute.Sa Compute.Exclusive "0" [| 0.5; 0.5; 0.0 |]
executeRF 3 Compute.Sb Compute.Exclusive "0" [| 0.5; 0.5; 0.0 |]
executeRF 4 Compute.Sa Compute.Exclusive "0" [| 0.5; 0.5; 0.5; 0.0; 0.0 |]
executeRF 4 Compute.Sb Compute.Exclusive "0" [| 0.5; 0.5; 0.5; 0.0; 0.0 |]

//-------------------------------------------------------------------------------------------------

let stepsize = Compute.stepsize 100000

Chart.renderStepsize (path "stepsize.svg") stepsize samples

let pmfunc3 = Compute.pmfExponent 3 0.5
let pmfunc4 = Compute.pmfExponent 4 0.5

Chart.renderPmfunc (path "target-pmfunc-3.svg") pmfunc3
Chart.renderPmfunc (path "target-pmfunc-4.svg") pmfunc4
