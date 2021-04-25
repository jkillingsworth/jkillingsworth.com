module Program

open System
open System.Text

//-------------------------------------------------------------------------------------------------

Console.OutputEncoding <- new UTF8Encoding()

let path filename = "../../../" + filename

//-------------------------------------------------------------------------------------------------

let window = 200
let sigmas = 4.0

//-------------------------------------------------------------------------------------------------

let outputResult render descriptor name logvalues =
    let differences = logvalues |> Compute.differences
    let criteriaN = differences |> Compute.fitDistributionN
    let criteriaL = differences |> Compute.fitDistributionL
    let wide = criteriaN |> snd |> (*) sigmas
    let histogram = differences |> Compute.histogram wide
    let dataProbs = (descriptor, histogram, sigmas, criteriaN, criteriaL)
    dataProbs |> render (path descriptor + "-probs-" + name + ".svg")
    printfn "----------------------------------------"
    printfn "%s %s" descriptor name
    printfn "µN = %+0.3e" (fst criteriaN)
    printfn "σN = %+0.3e" (snd criteriaN)
    printfn "µL = %+0.3e" (fst criteriaL)
    printfn "bL = %+0.3e" (snd criteriaL)
    printfn ""

let output priceRange (dataset : Data.Dataset) =
    let descriptor = dataset.Descriptor
    let market = dataset.GetPrices() |> Map.toArray |> Array.map (snd >> log)
    let smooth = market |> Compute.lsregs window
    let dither = market |> Compute.dither smooth
    let dataPrice = (descriptor, market, smooth, priceRange)
    let dataNoise = (descriptor, dither)
    dataPrice |> Chart.renderPrice (path descriptor + "-price.svg")
    dataNoise |> Chart.renderNoise (path descriptor + "-noise.svg")
    outputResult Chart.renderProbsMarket descriptor "market" (market)
    outputResult Chart.renderProbsSmooth descriptor "smooth" (smooth |> Array.choose id)
    outputResult Chart.renderProbsDither descriptor "dither" (dither |> Array.choose id)

//-------------------------------------------------------------------------------------------------

output (4.0000,  5.8000) <| Data.StocksDaily("SPY")
output (5.6580,  5.6720) <| Data.StocksIntraday("SPY")
output (4.3000,  5.1000) <| Data.ForexDaily("USDJPY")
output (1.9025,  1.9075) <| Data.ForexIntraday("USDCNH")
output (5.0000, 10.0000) <| Data.CryptoDaily("BTC")
