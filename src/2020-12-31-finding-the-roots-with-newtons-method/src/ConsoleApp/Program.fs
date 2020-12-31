module Program

open System

//-------------------------------------------------------------------------------------------------

let density = (20, 20)
let samples = 100

let path filename = "../../../" + filename

//-------------------------------------------------------------------------------------------------

Console.OutputEncoding <- Text.Encoding.UTF8

let sfname = function
    | Compute.Sa -> "A"
    | Compute.Sb -> "B"

let pspair (x : float[]) = (x.[0], x.[1])

let printValues n = function
    | i when i < (n - 1)
        -> printfn "p%i:         % .10f" (i + 1)
    | i -> printfn "λ%i:         %+.10f" (i + 2 - n)

//-------------------------------------------------------------------------------------------------

let executeNM n sf cs tag xs =

    let pmfunc = Compute.pmfExponent n 0.5
    let scoringfunc = Compute.scoringfunc n sf
    let constraints = Compute.constraints n cs pmfunc
    let m = constraints.Length

    let fs =
        (scoringfunc, constraints)
        |> Compute.lagrange
        |> Compute.gradient n m

    let costfunc =
        fs
        |> Compute.costfunc

    let final, count, trace = Compute.rootfind n m fs xs

    if (n = 3) then
        let name = sprintf "trace-%s-%s" (sfname sf) tag
        let ps = trace |> Array.map pspair
        let λs = final |> Array.skip (n - 1)
        let heatmap = Compute.heatmap density costfunc λs
        let plateau = Compute.plateau samples pmfunc
        Chart.renderHeatmapTraces (path name + "-heatmap.svg") heatmap plateau ps tag

    printfn "----------------------------------------"
    printfn "method-NM-n%i-%A-%A-%s" n sf cs tag
    Array.iteri (printValues n) final
    printfn "iterations: %A" <| count

//-------------------------------------------------------------------------------------------------

executeNM 3 Compute.Sa Compute.Inclusive "1" [| 0.35; 0.20; 0.0; 0.0 |]
executeNM 3 Compute.Sa Compute.Inclusive "2" [| 0.65; 0.80; 0.0; 0.0 |]
executeNM 3 Compute.Sa Compute.Inclusive "3" [| 0.20; 0.35; 0.0; 0.0 |]
executeNM 3 Compute.Sa Compute.Inclusive "4" [| 0.80; 0.20; 0.0; 0.0 |]

executeNM 3 Compute.Sb Compute.Inclusive "1" [| 0.35; 0.20; 0.0; 0.0 |]
executeNM 3 Compute.Sb Compute.Inclusive "2" [| 0.65; 0.80; 0.0; 0.0 |]
executeNM 3 Compute.Sb Compute.Inclusive "3" [| 0.20; 0.35; 0.0; 0.0 |]
executeNM 3 Compute.Sb Compute.Inclusive "4" [| 0.80; 0.20; 0.0; 0.0 |]

executeNM 4 Compute.Sa Compute.Inclusive "0" [| 0.5; 0.5; 0.5; 0.0; 0.0; 0.0 |]
executeNM 4 Compute.Sb Compute.Inclusive "0" [| 0.5; 0.5; 0.5; 0.0; 0.0; 0.0 |]

//-------------------------------------------------------------------------------------------------

executeNM 3 Compute.Sa Compute.Exclusive "0" [| 0.5; 0.5; 0.0 |]
executeNM 3 Compute.Sb Compute.Exclusive "0" [| 0.5; 0.5; 0.0 |]
executeNM 4 Compute.Sa Compute.Exclusive "0" [| 0.5; 0.5; 0.5; 0.0; 0.0 |]
executeNM 4 Compute.Sb Compute.Exclusive "0" [| 0.5; 0.5; 0.5; 0.0; 0.0 |]
