module Data

open System
open System.Collections.Generic
open System.IO
open System.Net.Http
open Chiron

//-------------------------------------------------------------------------------------------------

let private apikeytxt = "../../../../../private/apikey.txt"
let private cachepath = "../../../../../data/"

//-------------------------------------------------------------------------------------------------

let private urlFormat = "https://www.alphavantage.co/query?function=TIME_SERIES_DAILY_ADJUSTED&symbol={1}&outputsize=full&apikey={0}"

//-------------------------------------------------------------------------------------------------

type private Quote = { Date : DateTime; Close : double }

//-------------------------------------------------------------------------------------------------

let private error () = failwith "Json error."

let private toString = function
    | String x -> x
    | _ -> error ()

let private toAdjustedClose = function
    | Object x -> x.["5. adjusted close"] |> toString
    | _ -> error ()

let private toQuote (item : KeyValuePair<string, Json>) =
    let date = DateTime.Parse(item.Key)
    let close = Double.Parse(toAdjustedClose item.Value)
    { Date = date; Close = close }

let private toQuotes = function
    | Object x -> x |> Seq.map toQuote |> Seq.toArray
    | _ -> error ()

let private toSeries = function
    | Object x -> x.["Time Series (Daily)"]
    | _ -> error ()

//-------------------------------------------------------------------------------------------------

let private httpClient = new HttpClient()

let private fetchData ticker =
    let apikey = File.ReadAllText(apikeytxt).Trim()
    let url = String.Format(urlFormat, apikey, ticker)
    httpClient.GetStringAsync(url)
    |> Async.AwaitTask
    |> Async.RunSynchronously

let private parseData count final data =
    data
    |> Parsing.Json.parse
    |> toSeries
    |> toQuotes
    |> Array.where (fun x -> x.Date <= final)
    |> Array.rev
    |> Array.take count
    |> Array.rev

let private writeData filepath data =
    let mapDate quote = quote.Date.ToString("yyyy-MM-dd")
    let mapping quote = sprintf "%s, %8.4f" (mapDate quote) quote.Close
    let lines = data |> Array.map mapping
    File.WriteAllLines(filepath, lines)

let private cacheData count final ticker filepath =
    if (Directory.Exists(cachepath) = false) then
        Directory.CreateDirectory(cachepath) |> ignore
    if (File.Exists(filepath) = false) then
        ticker
        |> fetchData
        |> parseData count final
        |> writeData filepath

let getQuotes count final ticker =
    let filepath = Path.Combine(cachepath, ticker + ".txt")
    cacheData count final ticker filepath
    File.ReadAllLines(filepath)
    |> Array.map (fun x -> x.Split(","))
    |> Array.map (fun x -> x.[0], x.[1])
    |> Array.map (fun x -> DateTime.Parse(fst x), Double.Parse(snd x))
    |> Array.map (fun x -> { Date = fst x; Close = snd x })
    |> Array.map (fun x -> x.Close)
