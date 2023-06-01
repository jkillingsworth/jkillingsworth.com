module Data

open System
open System.IO
open System.Net.Http

//-------------------------------------------------------------------------------------------------

let private apikeytxt = "../../../../../private/apikey.txt"
let private cachepath = "../../../../../data/"

//-------------------------------------------------------------------------------------------------

let private urlFormat = "https://api.tiingo.com/tiingo/daily/{1}/prices?startDate=1900-01-01&format=csv&token={0}"

//-------------------------------------------------------------------------------------------------

let private httpClient = new HttpClient()

let private fetchData symbol =
    let apikey = File.ReadAllText(apikeytxt).Trim()
    let url = String.Format(urlFormat, apikey, symbol)
    httpClient.GetStringAsync(url)
    |> Async.AwaitTask
    |> Async.RunSynchronously

let private parseData final (data : string) =
    data.Split("\n", StringSplitOptions.RemoveEmptyEntries)
    |> Array.skip 1
    |> Array.map (fun x -> x.Split(","))
    |> Array.map (fun x -> x.[0], x.[6])
    |> Array.map (fun x -> DateTime.Parse(fst x), Double.Parse(snd x))
    |> Array.where (fun x -> (fst x) <= final)

let private writeData filepath data =
    let mapDate item = (fst item : DateTime).ToString("yyyy-MM-dd")
    let mapping item = sprintf "%s, %15.8e" (mapDate item) (snd item)
    let lines = data |> Array.map mapping
    File.WriteAllLines(filepath, lines)

let private cacheData filepath final symbol =
    if (Directory.Exists(cachepath) = false) then
        Directory.CreateDirectory(cachepath) |> ignore
    if (File.Exists(filepath) = false) then
        symbol
        |> fetchData
        |> parseData final
        |> writeData filepath

let getPrices final symbol =
    let filepath = Path.Combine(cachepath, symbol + ".txt")
    cacheData filepath final symbol
    File.ReadAllLines(filepath)
    |> Array.map (fun x -> x.Split(","))
    |> Array.map (fun x -> x.[1])
    |> Array.map (fun x -> Double.Parse(x))
