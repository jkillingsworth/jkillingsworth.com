module Data

open System
open System.Collections.Generic
open System.IO
open System.Net.Http
open Chiron

//-------------------------------------------------------------------------------------------------

let private apikeytxt = "./private/apikey.txt"
let private cachepath = "./data/"

//-------------------------------------------------------------------------------------------------

type private Quote = { Time : DateTime; Close : string }

[<AbstractClass>]
type Dataset() =

    abstract member Descriptor : string
    abstract member GetPrices : unit -> Map<DateTime, float>

//-------------------------------------------------------------------------------------------------

let private error () = failwith "Json error."

let private toString = function
    | String x -> x
    | _ -> error ()

let private toClose key = function
    | Object x -> x.[key] |> toString
    | _ -> error ()

let private toQuote key (item : KeyValuePair<string, Json>) =
    let time = DateTime.Parse(item.Key)
    let close = toClose key item.Value
    { Time = time; Close = close }

let private toQuotes key = function
    | Object x -> x |> Seq.map (toQuote key) |> Seq.toArray
    | _ -> error ()

let private toSeries key = function
    | Object x -> x.[key]
    | _ -> error ()

//-------------------------------------------------------------------------------------------------

let private httpClient = new HttpClient()

let private fetchData (url : Lazy<string>) =
    httpClient.GetStringAsync(url.Value)
    |> Async.AwaitTask
    |> Async.RunSynchronously

let private parseData keySeries keyQuotes data =
    data
    |> Parsing.Json.parse
    |> toSeries keySeries
    |> toQuotes keyQuotes

let private writeData filepath data =
    let predicate quote = quote.Time.TimeOfDay <> TimeSpan.Zero
    let intraday = data |> Array.exists predicate
    let timeFormat = if intraday then "yyyy-MM-dd HH:mm:ss" else "yyyy-MM-dd"
    let mapTime quote = quote.Time.ToString(timeFormat)
    let mapping quote = sprintf "%s, %s" (mapTime quote) quote.Close
    let lines = data |> Array.map mapping
    File.WriteAllLines(filepath, lines)

let private cacheData url keySeries keyQuotes cachefile =
    if (Directory.Exists(cachepath) = false) then
        Directory.CreateDirectory(cachepath) |> ignore
    if (File.Exists(cachefile) = false) then
        url
        |> fetchData
        |> parseData keySeries keyQuotes
        |> writeData cachefile
    cachefile

let private readCache cachefile =
    File.ReadAllLines(cachefile)
    |> Array.map (fun x -> x.Split(","))
    |> Array.map (fun x -> DateTime.Parse(x.[0]), float x.[1])
    |> Map.ofArray

let private getPrices descriptor url keySeries keyQuotes =
    Path.Combine(cachepath, descriptor + ".txt")
    |> cacheData url keySeries keyQuotes
    |> readCache

//-------------------------------------------------------------------------------------------------

type StocksDaily(symbol) =

    inherit Dataset()

    let urlFormat = "https://www.alphavantage.co/query?function=TIME_SERIES_DAILY_ADJUSTED&symbol={1}&outputsize=full&apikey={0}"
    let keySeries = "Time Series (Daily)"
    let keyQuotes = "5. adjusted close"

    let createUrl (symbol : string) =
        let apikey = File.ReadAllText(apikeytxt).Trim()
        String.Format(urlFormat, apikey, symbol)

    override val Descriptor =
        symbol |> (+) "stocks-daily-"

    override this.GetPrices() =
        let url = lazy createUrl symbol
        getPrices this.Descriptor url keySeries keyQuotes

//-------------------------------------------------------------------------------------------------

type StocksIntraday(symbol) =

    inherit Dataset()

    let urlFormat = "https://www.alphavantage.co/query?function=TIME_SERIES_INTRADAY&interval=1min&symbol={1}&outputsize=full&apikey={0}"
    let keySeries = "Time Series (1min)"
    let keyQuotes = "4. close"

    let createUrl (symbol : string) =
        let apikey = File.ReadAllText(apikeytxt).Trim()
        String.Format(urlFormat, apikey, symbol)

    override val Descriptor =
        symbol |> (+) "stocks-intraday-"

    override this.GetPrices() =
        let url = lazy createUrl symbol
        getPrices this.Descriptor url keySeries keyQuotes

//-------------------------------------------------------------------------------------------------

type ForexDaily(symbol) =

    inherit Dataset()

    let urlFormat = "https://www.alphavantage.co/query?function=FX_DAILY&from_symbol={1}&to_symbol={2}&outputsize=full&apikey={0}"
    let keySeries = "Time Series FX (Daily)"
    let keyQuotes = "4. close"

    let createUrl (symbol : string) =
        let apikey = File.ReadAllText(apikeytxt).Trim()
        let symbolBase = symbol.Substring(0, 3)
        let symbolQuot = symbol.Substring(3, 3)
        String.Format(urlFormat, apikey, symbolBase, symbolQuot)

    override val Descriptor =
        symbol |> (+) "forex-daily-"

    override this.GetPrices() =
        let url = lazy createUrl symbol
        getPrices this.Descriptor url keySeries keyQuotes

//-------------------------------------------------------------------------------------------------

type ForexIntraday(symbol) =

    inherit Dataset()

    let urlFormat = "https://www.alphavantage.co/query?function=FX_INTRADAY&interval=1min&from_symbol={1}&to_symbol={2}&outputsize=full&apikey={0}"
    let keySeries = "Time Series FX (1min)"
    let keyQuotes = "4. close"

    let createUrl (symbol : string) =
        let apikey = File.ReadAllText(apikeytxt).Trim()
        let symbolBase = symbol.Substring(0, 3)
        let symbolQuot = symbol.Substring(3, 3)
        String.Format(urlFormat, apikey, symbolBase, symbolQuot)

    override val Descriptor =
        symbol |> (+) "forex-intraday-"

    override this.GetPrices() =
        let url = lazy createUrl symbol
        getPrices this.Descriptor url keySeries keyQuotes

//-------------------------------------------------------------------------------------------------

type CryptoDaily(symbol) =

    inherit Dataset()

    let urlFormat = "https://www.alphavantage.co/query?function=DIGITAL_CURRENCY_DAILY&symbol={1}&market=USD&outputsize=full&apikey={0}"
    let keySeries = "Time Series (Digital Currency Daily)"
    let keyQuotes = "4a. close (USD)"

    let createUrl (symbol : string) =
        let apikey = File.ReadAllText(apikeytxt).Trim()
        String.Format(urlFormat, apikey, symbol)

    override val Descriptor =
        symbol |> (+) "crypto-daily-"

    override this.GetPrices() =
        let url = lazy createUrl symbol
        getPrices this.Descriptor url keySeries keyQuotes
