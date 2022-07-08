module Program

open System
open System.Text

//-------------------------------------------------------------------------------------------------

Console.OutputEncoding <- new UTF8Encoding()

let path filename = "../../../" + filename

//-------------------------------------------------------------------------------------------------

let final = DateTime(2022, 07, 07)
let sigmas = 4.0

//-------------------------------------------------------------------------------------------------

let runModelDistribution β =

    let μ = 0.0
    let α = 1.0

    let data = (μ, α, β) |> Compute.pDensityG |> Compute.ps 4.0

    Chart.renderShape (path $"shape-{β:F2}.svg") data β

//-------------------------------------------------------------------------------------------------

let runFitDistributions high symbol =

    let values =
        symbol
        |> Data.getPrices final
        |> Array.map log
        |> Compute.differences

    let argsN = Compute.fitDistributionN values
    let argsL = Compute.fitDistributionL values
    let argsG = Compute.fitDistributionG values

    let (μN, σN) = argsN
    let (μL, bL) = argsL
    let (μG, αG, βG) = argsG

    let wide = σN * sigmas
    let histogram = Compute.histogram wide values

    let dataN = argsN |> Compute.pDensityN |> Compute.ps wide
    let dataL = argsL |> Compute.pDensityL |> Compute.ps wide
    let dataG = argsG |> Compute.pDensityG |> Compute.ps wide

    Chart.renderFitted (path $"fitted-{symbol}-N.svg") histogram dataN sigmas σN high "N"
    Chart.renderFitted (path $"fitted-{symbol}-L.svg") histogram dataL sigmas σN high "L"
    Chart.renderFitted (path $"fitted-{symbol}-G.svg") histogram dataG sigmas σN high "G"

    printfn "----------------------------------------"
    printfn "%s" symbol
    printfn "μN = %+11.3e" μN
    printfn "σN = % 11.3e" σN
    printfn "μL = %+11.3e" μL
    printfn "bL = % 11.3e" bL
    printfn "μG = %+11.3e" μG
    printfn "αG = % 11.3e" αG
    printfn "βG = % 7.4f" βG

//-------------------------------------------------------------------------------------------------

runModelDistribution 0.50
runModelDistribution 0.75
runModelDistribution 1.00
runModelDistribution 1.50
runModelDistribution 2.00
runModelDistribution 4.00
runModelDistribution 8.00

runFitDistributions 40 "MSFT"
runFitDistributions 28 "BTCUSD"
runFitDistributions 24 "UNG"
