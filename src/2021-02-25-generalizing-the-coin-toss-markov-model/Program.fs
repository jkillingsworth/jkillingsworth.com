module Program

open System
open System.Text

//-------------------------------------------------------------------------------------------------

Console.OutputEncoding <- new UTF8Encoding()

let path filename = Common.Chart.outputPath filename

//-------------------------------------------------------------------------------------------------

let n = 10

//-------------------------------------------------------------------------------------------------

let printValues n = function
    | i when i < (n - 1)
        -> printfn "p%i: % 15.10f" (i + 1)
    | i -> printfn "λ%i: %+15.10f" (i + 2 - n)

//-------------------------------------------------------------------------------------------------

let execute tag sf gamma pmfunc biases =

    let scoringfunc = Compute.scoringfunc n sf
    let constraints = Compute.constraints n pmfunc
    let m = constraints.Length

    let λStart = 0.0
    let xs = Compute.convertBiasesToXs n m λStart biases

    let fs =
        (scoringfunc, constraints)
        |> Compute.lagrange
        |> Compute.gradient n m

    let final, count = Compute.rootfind gamma n m fs xs
    let biasesFinal = Compute.convertXsToBiases n final

    Chart.renderBiases (path "biases-final-" + tag + ".svg") biasesFinal

    printfn "----------------------------------------"
    printfn "method-NM-n%i-%A-%0.1f-%s" n sf gamma tag
    printfn "iterations: %A" <| count
    Array.iteri (printValues n) final

//-------------------------------------------------------------------------------------------------

let pmfuncExponent = Compute.pmfExponent n 0.5
let pmfuncTriangle = Compute.pmfTriangle n
let biasesSlope = Compute.biasesSlope n (0.5 / float n)
let biasesEqual = Compute.biasesEqual n

Chart.renderPmfunc (path "pmfunc-exponent.svg") pmfuncExponent
Chart.renderPmfunc (path "pmfunc-triangle.svg") pmfuncTriangle
Chart.renderBiases (path "biases-start-slope.svg") biasesSlope
Chart.renderBiases (path "biases-start-equal.svg") biasesEqual

execute "1" Compute.Sa 1.0 pmfuncExponent biasesSlope
execute "2" Compute.Sb 1.0 pmfuncExponent biasesSlope
execute "3" Compute.Sa 0.1 pmfuncTriangle biasesEqual
execute "4" Compute.Sb 0.1 pmfuncTriangle biasesEqual
