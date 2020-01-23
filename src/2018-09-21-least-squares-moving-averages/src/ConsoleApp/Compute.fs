module Compute

open MathNet.Numerics.LinearAlgebra

//-------------------------------------------------------------------------------------------------

let private computeOutput computeCoeff computeValue n prices =

    let count = Array.length prices
    let coeff = computeCoeff n prices (count - 1)

    let initMoving = function
        | k when k < (n - 1)
            -> None
        | k -> Some (computeValue k <| computeCoeff n prices k)

    let initFitted = function
        | k when k < (count - n)
            -> None
        | k -> Some (computeValue k <| coeff)

    let moving = initMoving |> Array.init count
    let fitted = initFitted |> Array.init count

    Array.zip3 prices moving fitted

//-------------------------------------------------------------------------------------------------

module Simple =

    let private computeValue x (a0) = a0

    let private computeCoeff n (prices : float[]) k =

        let mutable sum = 0.0
        for i = k - n + 1 to k do sum <- sum + prices.[i]
        sum / float n

    let movingAverage =
        computeOutput computeCoeff computeValue

//-------------------------------------------------------------------------------------------------

module LsrLin =

    let private computeValue x (a0, a1) = a0 + a1 * (float x)

    let private computeCoeff n (prices : float[]) k =

        let mutable sumx1 = 0.0
        let mutable sumx2 = 0.0
        let mutable sumy1 = 0.0
        let mutable sumxy = 0.0

        for i = k - n + 1 to k do
            let y = prices.[i]
            let x = float i
            sumx1 <- sumx1 + x
            sumx2 <- sumx2 + x ** 2.0
            sumy1 <- sumy1 + y
            sumxy <- sumxy + y * x

        let a1 = (float n * sumxy - sumx1 * sumy1) / (float n * sumx2 - sumx1 * sumx1)
        let a0 = (sumy1 - a1 * sumx1) / float n

        (a0, a1)

    let movingAverage =
        computeOutput computeCoeff computeValue

//-------------------------------------------------------------------------------------------------

module LsrPol =

    let private computeValue x (a0, a1, a2) = a0 + a1 * (float x) + a2 * ((float x) ** 2.0)

    let private computeCoeff n (prices : float[]) k =

        let mutable sumx1y0 = 0.0
        let mutable sumx2y0 = 0.0
        let mutable sumx3y0 = 0.0
        let mutable sumx4y0 = 0.0
        let mutable sumx0y1 = 0.0
        let mutable sumx1y1 = 0.0
        let mutable sumx2y1 = 0.0

        for i = k - n + 1 to k do
            let y = prices.[i]
            let x = float i
            sumx1y0 <- sumx1y0 + x
            sumx2y0 <- sumx2y0 + x ** 2.0
            sumx3y0 <- sumx3y0 + x ** 3.0
            sumx4y0 <- sumx4y0 + x ** 4.0
            sumx0y1 <- sumx0y1 + y
            sumx1y1 <- sumx1y1 + y * x
            sumx2y1 <- sumx2y1 + y * x ** 2.0

        let ma =
            matrix [ [ float n; sumx1y0; sumx2y0 ]
                     [ sumx1y0; sumx2y0; sumx3y0 ]
                     [ sumx2y0; sumx3y0; sumx4y0 ] ]

        let mb =
            matrix [ [ sumx0y1 ]
                     [ sumx1y1 ]
                     [ sumx2y1 ] ]

        let mx = ma.Inverse().Multiply(mb)

        let a0 = mx.[0, 0]
        let a1 = mx.[1, 0]
        let a2 = mx.[2, 0]

        (a0, a1, a2)

    let movingAverage =
        computeOutput computeCoeff computeValue

//-------------------------------------------------------------------------------------------------

module LsrExp =

    let private computeValue x (p, r) = p * (1.0 + r) ** (float x)

    let private computeCoeff n (prices : float[]) k =

        let mutable sumx1 = 0.0
        let mutable sumx2 = 0.0
        let mutable sumy1 = 0.0
        let mutable sumxy = 0.0

        for i = k - n + 1 to k do
            let y = log prices.[i]
            let x = float i
            sumx1 <- sumx1 + x
            sumx2 <- sumx2 + x ** 2.0
            sumy1 <- sumy1 + y
            sumxy <- sumxy + y * x

        let a1 = (float n * sumxy - sumx1 * sumy1) / (float n * sumx2 - sumx1 * sumx1)
        let a0 = (sumy1 - a1 * sumx1) / float n

        let p = exp a0
        let r = exp a1 - 1.0

        (p, r)

    let movingAverage =
        computeOutput computeCoeff computeValue
