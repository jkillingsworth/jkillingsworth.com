module Program

open System
open System.Text

//-------------------------------------------------------------------------------------------------

Console.OutputEncoding <- new UTF8Encoding()

let path filename = Common.Chart.outputPath filename

//-------------------------------------------------------------------------------------------------

let final = DateTime(2018, 09, 21)
let count = 2000
let n = 200

let executeFull compute name ticker axis =

    let file = path (name + "-" + ticker + "-full.svg")

    ticker
    |> Data.getQuotes count final
    |> compute n
    |> Chart.renderPriceFull file axis ticker

let executeZoom compute name ticker axis =

    let file = path (name + "-" + ticker + "-zoom.svg")

    ticker
    |> Data.getQuotes count final
    |> compute n
    |> Chart.renderPriceZoom file axis ticker

let execute compute name =

    executeFull compute name "MSFT" ( 10, 120, 10)
    executeFull compute name "WYNN" ( 30, 250, 20)
    executeFull compute name "HEAR" (-10, 100, 10)

    executeZoom compute name "MSFT" ( 65, 120,  5)
    executeZoom compute name "WYNN" (120, 230, 10)
    executeZoom compute name "HEAR" (-10,  45,  5)

execute Compute.Simple.movingAverage "simple"
execute Compute.LsrLin.movingAverage "lsrlin"
execute Compute.LsrPol.movingAverage "lsrpol"
execute Compute.LsrExp.movingAverage "lsrexp"
