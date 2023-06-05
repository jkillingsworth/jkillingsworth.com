module Program

open System
open System.Text

//-------------------------------------------------------------------------------------------------

Console.OutputEncoding <- new UTF8Encoding()

let path filename = Common.Const.runoutput + filename

//-------------------------------------------------------------------------------------------------

let final = DateTime(2022, 08, 17)
let sigmas = 4.0

//-------------------------------------------------------------------------------------------------

let runModelShape α =

    let μ = 0.0
    let γ = 1.0

    let data = (μ, γ, α) |> Compute.pDensityS |> Compute.ps 4.0

    Chart.renderShape (path $"model-shape-{α:F2}.svg") data α

//-------------------------------------------------------------------------------------------------

let runFitDistributions high symbol =

    let values =
        symbol
        |> Data.getPrices final
        |> Array.map log
        |> Compute.differences

    let argsN = Compute.fitDistributionN values
    let argsC = Compute.fitDistributionC values
    let argsS = Compute.fitDistributionS values

    let (μN, σN) = argsN
    let (μC, λC) = argsC
    let (μS, γS, αS) = argsS

    let wide = σN * sigmas
    let histogram = Compute.histogram wide values

    let dataN = argsN |> Compute.pDensityN |> Compute.ps wide
    let dataC = argsC |> Compute.pDensityC |> Compute.ps wide
    let dataS = argsS |> Compute.pDensityS |> Compute.ps wide

    Chart.renderFitted (path $"fitted-{symbol}-N.svg") histogram dataN sigmas σN high "N"
    Chart.renderFitted (path $"fitted-{symbol}-C.svg") histogram dataC sigmas σN high "C"
    Chart.renderFitted (path $"fitted-{symbol}-S.svg") histogram dataS sigmas σN high "S"

    printfn "----------------------------------------"
    printfn "%s" symbol
    printfn "μN = %+11.3e" μN
    printfn "σN = % 11.3e" σN
    printfn "μC = %+11.3e" μC
    printfn "λC = % 11.3e" λC
    printfn "μS = %+11.3e" μS
    printfn "γS = % 11.3e" γS
    printfn "αS = % 7.4f" αS

//-------------------------------------------------------------------------------------------------

runModelShape 0.45
runModelShape 0.55
runModelShape 0.75
runModelShape 1.00
runModelShape 1.35
runModelShape 2.00

runFitDistributions 40 "MSFT"
runFitDistributions 28 "BTCUSD"
runFitDistributions 24 "UNG"
