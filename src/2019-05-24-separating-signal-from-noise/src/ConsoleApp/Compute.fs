module Compute

open MathNet.Numerics.Statistics

//-------------------------------------------------------------------------------------------------

let private computeOutput computeCoeff computeValue n values =

    let count = Array.length values

    let initialize = function
        | k when k < (n - 1)
            -> None
        | k -> Some (computeValue k <| computeCoeff n values k)

    Array.init count initialize

let private computeCoeff n (values : float[]) k =

    let mutable sumx1 = 0.0
    let mutable sumx2 = 0.0
    let mutable sumy1 = 0.0
    let mutable sumxy = 0.0

    for i = k - n + 1 to k do
        let y = values.[i]
        let x = float i
        sumx1 <- sumx1 + x
        sumx2 <- sumx2 + x ** 2.0
        sumy1 <- sumy1 + y
        sumxy <- sumxy + y * x

    let a1 = (float n * sumxy - sumx1 * sumy1) / (float n * sumx2 - sumx1 * sumx1)
    let a0 = (sumy1 - a1 * sumx1) / float n

    (a0, a1)

let private computeValue x (a0, a1) = a0 + a1 * (float x)

let lsregs window values =

    computeOutput computeCoeff computeValue window values

//-------------------------------------------------------------------------------------------------

let dither (smooth : float option[]) (market : float[]) =

    let count = Array.length market

    let initialize i =
        match smooth.[i] with
        | Some item
            -> Some (market.[i] - item)
        | _ -> None

    Array.init count initialize

//-------------------------------------------------------------------------------------------------

let differences values =

    let count = values |> Array.length
    let initialize i = (values.[i + 1]) - (values.[i])
    Array.init<float> (count - 1) initialize

let histogram wide values =

    let bins = 50 + 1
    let histogram = Histogram(values, bins, -wide, +wide)

    let mapping i =
        let bucket = histogram.[i]
        let center = (bucket.LowerBound + bucket.UpperBound) / 2.0
        let amount = (bucket.Count / histogram.DataCount) / (wide * 2.0 / (float bins))
        (center, amount)

    Array.init bins mapping

//-------------------------------------------------------------------------------------------------

let fitDistributionN (values : float[]) =

    let n = values |> Array.length
    let µ = values |> Statistics.Mean
    let σ = values |> Array.map (fun x -> (x - µ) ** 2.0) |> Array.sum
    let σ = sqrt (σ / float n)

    (µ, σ)

let fitDistributionL (values : float[]) =

    let n = values |> Array.length
    let µ = values |> Statistics.Median
    let b = values |> Array.map (fun x -> abs (x - µ)) |> Array.sum
    let b = (b / float n)

    (µ, b)
