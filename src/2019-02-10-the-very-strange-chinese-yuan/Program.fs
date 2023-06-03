module Program

open System
open System.Text

//-------------------------------------------------------------------------------------------------

Console.OutputEncoding <- new UTF8Encoding()

let path filename = Common.Chart.outputPath filename

//-------------------------------------------------------------------------------------------------

let sigmas = 4.0

//-------------------------------------------------------------------------------------------------

type OutputFlag =
    | PriceLin
    | ProbsLin
    | ProbsLog

let output flag (dataset : Data.Dataset) =
    let descriptor = dataset.Descriptor
    let prices = dataset.GetPrices() |> Map.toArray |> Array.map snd
    let logprices = prices |> Array.map log
    let differences = logprices |> Compute.differences
    let criteriaN = differences |> Compute.fitDistributionN
    let criteriaL = differences |> Compute.fitDistributionL
    let wide = criteriaN |> snd |> (*) sigmas
    let histogram = differences |> Compute.histogram wide
    let dataPrice = (descriptor, prices)
    let dataProbs = (descriptor, histogram, sigmas, criteriaN, criteriaL)
    match flag with
    | PriceLin -> dataPrice |> Chart.renderPriceLin (path descriptor + "-price-lin.svg")
    | ProbsLin -> dataProbs |> Chart.renderProbsLin (path descriptor + "-probs-lin.svg")
    | ProbsLog -> dataProbs |> Chart.renderProbsLog (path descriptor + "-probs-log.svg")

//-------------------------------------------------------------------------------------------------

output PriceLin <| Data.ForexIntraday("USDCNY")
output ProbsLin <| Data.ForexIntraday("USDCNY")
output ProbsLog <| Data.ForexIntraday("USDCNY")

output PriceLin <| Data.ForexIntraday("USDCNH")
output ProbsLin <| Data.ForexIntraday("USDCNH")
output ProbsLog <| Data.ForexIntraday("USDCNH")

output PriceLin <| Data.ForexIntraday("EURCNH")
output ProbsLin <| Data.ForexIntraday("EURCNH")
output ProbsLog <| Data.ForexIntraday("EURCNH")

//-------------------------------------------------------------------------------------------------

let pricesEURUSD = Data.ForexIntraday("EURUSD").GetPrices()
let pricesEURCNH = Data.ForexIntraday("EURCNH").GetPrices()
let prices = Compute.syntheticPair pricesEURUSD pricesEURCNH

output PriceLin <| Data.PairSynthetic("USDCNH", prices)
output ProbsLin <| Data.PairSynthetic("USDCNH", prices)
output ProbsLog <| Data.PairSynthetic("USDCNH", prices)

//-------------------------------------------------------------------------------------------------

output ProbsLin <| Data.ForexDaily("USDCNY")
output ProbsLog <| Data.ForexDaily("USDCNY")

output ProbsLin <| Data.ForexDaily("USDCNH")
output ProbsLog <| Data.ForexDaily("USDCNH")
