module Program

open System
open System.Text

//-------------------------------------------------------------------------------------------------

Console.OutputEncoding <- new UTF8Encoding()

let path filename = "../../../" + filename

//-------------------------------------------------------------------------------------------------

let final = DateTime(2022, 09, 09)
let sigmas = 4.0

//-------------------------------------------------------------------------------------------------

let runModelByScale δ =

    let μ = 0.00
    let α = 1.00
    let λ = +1.00

    let data = (μ, δ, α, λ) |> Compute.pDensityH |> Compute.ps 4.0

    Chart.renderModel (path $"model-scale-{δ:F2}.svg") data δ α λ

let runModelByShape α =

    let μ = 0.00
    let δ = 1.00
    let λ = -0.50

    let data = (μ, δ, α, λ) |> Compute.pDensityH |> Compute.ps 4.0

    Chart.renderModel (path $"model-shape-{α:F2}.svg") data δ α λ

//-------------------------------------------------------------------------------------------------

let runFitDistributions high symbol =

    let values =
        symbol
        |> Data.getPrices final
        |> Array.map log
        |> Compute.differences

    let argsN = Compute.fitDistributionN values
    let argsI = Compute.fitDistributionI values
    let argsH = Compute.fitDistributionH values

    let (μN, σN) = argsN
    let (μI, δI, αI, λI) = argsI
    let (μH, δH, αH, λH) = argsH

    let wide = σN * sigmas
    let histogram = Compute.histogram wide values

    let dataN = argsN |> Compute.pDensityN |> Compute.ps wide
    let dataI = argsI |> Compute.pDensityH |> Compute.ps wide
    let dataH = argsH |> Compute.pDensityH |> Compute.ps wide

    Chart.renderFitted (path $"fitted-{symbol}-N.svg") histogram dataN sigmas σN high "N"
    Chart.renderFitted (path $"fitted-{symbol}-I.svg") histogram dataI sigmas σN high "I"
    Chart.renderFitted (path $"fitted-{symbol}-H.svg") histogram dataH sigmas σN high "H"

    printfn "----------------------------------------"
    printfn "%s" symbol
    printfn "μN = %+11.3e" μN
    printfn "σN = % 11.3e" σN
    printfn "μI = %+11.3e" μI
    printfn "δI = %+11.3e" δI
    printfn "αI = %+11.3e" αI
    printfn "λI = %+7.4f" λI
    printfn "μH = %+11.3e" μH
    printfn "δH = %+11.3e" δH
    printfn "αH = %+11.3e" αH
    printfn "λH = %+7.4f" λH

//-------------------------------------------------------------------------------------------------

runModelByScale 1.00
runModelByScale 0.33
runModelByScale 0.01

runModelByShape 1.00
runModelByShape 0.33
runModelByShape 0.01

runFitDistributions 40 "MSFT"
runFitDistributions 28 "BTCUSD"
runFitDistributions 24 "UNG"
