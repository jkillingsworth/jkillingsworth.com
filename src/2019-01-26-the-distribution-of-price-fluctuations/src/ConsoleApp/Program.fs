module Program

open System

//-------------------------------------------------------------------------------------------------

let sigmas = 4.0

let path filename = "../../../" + filename

//-------------------------------------------------------------------------------------------------

type OutputFlag =
    | Price
    | Diffs
    | Probs
    | Stats

let output flag (dataset : Data.Dataset) =
    let descriptor = dataset.Descriptor
    let prices = dataset.GetPrices()
    let logprices = prices |> Array.map log
    let differences = logprices |> Compute.differences
    let criteriaN = differences |> Compute.fitDistributionN
    let criteriaL = differences |> Compute.fitDistributionL
    let wide = criteriaN |> snd |> (*) sigmas
    let histogram = differences |> Compute.histogram wide
    let dataPrice = (descriptor, logprices)
    let dataDiffs = (descriptor, differences)
    let dataProbs = (descriptor, histogram, sigmas, criteriaN, criteriaL)
    match flag with
    | Price -> dataPrice |> Chart.renderPrice (path descriptor + "-price.svg")
    | Diffs -> dataDiffs |> Chart.renderDiffs (path descriptor + "-diffs.svg")
    | Probs -> dataProbs |> Chart.renderProbs (path descriptor + "-probs.svg")
    | Stats ->
        printfn "----------------------------------------"
        printfn "%s" dataset.Descriptor
        printfn "µN = %+0.3e" (fst criteriaN)
        printfn "σN = %+0.3e" (snd criteriaN)
        printfn "µL = %+0.3e" (fst criteriaL)
        printfn "bL = %+0.3e" (snd criteriaL)
        printfn ""

//-------------------------------------------------------------------------------------------------

output Price <| Data.StocksDaily("SPY")
output Diffs <| Data.StocksDaily("SPY")
output Probs <| Data.StocksDaily("SPY")
output Stats <| Data.StocksDaily("SPY")

//-------------------------------------------------------------------------------------------------

output Probs <| Data.StocksDaily("DIA")
output Probs <| Data.StocksDaily("EEM")
output Probs <| Data.StocksDaily("GLD")
output Probs <| Data.StocksDaily("HYG")
output Probs <| Data.StocksDaily("LQD")
output Probs <| Data.StocksDaily("TLT")
output Probs <| Data.StocksDaily("UNG")
output Probs <| Data.StocksDaily("USO")

//-------------------------------------------------------------------------------------------------

output Probs <| Data.StocksDaily("AMZN")
output Probs <| Data.StocksDaily("AZO")
output Probs <| Data.StocksDaily("BLK")
output Probs <| Data.StocksDaily("CAT")
output Probs <| Data.StocksDaily("CMG")
output Probs <| Data.StocksDaily("FDX")
output Probs <| Data.StocksDaily("GM")
output Probs <| Data.StocksDaily("GOOG")
output Probs <| Data.StocksDaily("GWW")
output Probs <| Data.StocksDaily("HUM")
output Probs <| Data.StocksDaily("NFLX")
output Probs <| Data.StocksDaily("TSLA")
output Probs <| Data.StocksDaily("TWLO")
output Probs <| Data.StocksDaily("ULTA")
output Probs <| Data.StocksDaily("UNH")

//-------------------------------------------------------------------------------------------------

output Probs <| Data.StocksIntraday("AMZN")
output Probs <| Data.StocksIntraday("AZO")
output Probs <| Data.StocksIntraday("BLK")
output Probs <| Data.StocksIntraday("CAT")
output Probs <| Data.StocksIntraday("CMG")
output Probs <| Data.StocksIntraday("FDX")
output Probs <| Data.StocksIntraday("GM")
output Probs <| Data.StocksIntraday("GOOG")
output Probs <| Data.StocksIntraday("GWW")
output Probs <| Data.StocksIntraday("HUM")
output Probs <| Data.StocksIntraday("NFLX")
output Probs <| Data.StocksIntraday("TSLA")
output Probs <| Data.StocksIntraday("TWLO")
output Probs <| Data.StocksIntraday("ULTA")
output Probs <| Data.StocksIntraday("UNH")

//-------------------------------------------------------------------------------------------------

output Price <| Data.StocksDaily("VIX")
output Diffs <| Data.StocksDaily("VIX")
output Probs <| Data.StocksDaily("VIX")
output Probs <| Data.StocksIntraday("VIX")

//-------------------------------------------------------------------------------------------------

output Price <| Data.ForexDaily("EURUSD")
output Diffs <| Data.ForexDaily("EURUSD")
output Probs <| Data.ForexDaily("EURUSD")

//-------------------------------------------------------------------------------------------------

output Probs <| Data.ForexDaily("USDJPY")
output Probs <| Data.ForexDaily("USDMXN")
output Probs <| Data.ForexDaily("USDRUB")
output Probs <| Data.ForexDaily("USDTRY")
output Probs <| Data.ForexDaily("USDZAR")
output Probs <| Data.ForexDaily("EURNOK")
output Probs <| Data.ForexDaily("EURSEK")
output Probs <| Data.ForexDaily("EURTRY")

//-------------------------------------------------------------------------------------------------

output Probs <| Data.ForexIntraday("USDJPY")
output Probs <| Data.ForexIntraday("USDMXN")
output Probs <| Data.ForexIntraday("USDRUB")
output Probs <| Data.ForexIntraday("USDTRY")
output Probs <| Data.ForexIntraday("USDZAR")
output Probs <| Data.ForexIntraday("EURNOK")
output Probs <| Data.ForexIntraday("EURSEK")
output Probs <| Data.ForexIntraday("EURTRY")

//-------------------------------------------------------------------------------------------------

output Probs <| Data.CryptoDaily("BTC")
output Probs <| Data.CryptoDaily("ETH")
output Probs <| Data.CryptoDaily("XRP")
