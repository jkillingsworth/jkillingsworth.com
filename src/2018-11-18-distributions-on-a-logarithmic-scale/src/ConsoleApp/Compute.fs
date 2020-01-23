module Compute

open System

//-------------------------------------------------------------------------------------------------

let densityN µ σ x =

    (1.0 / (x * σ * sqrt (2.0 * Math.PI))) * exp (- ((log x - µ) ** 2.0) / (2.0 * σ ** 2.0))

let densityL µ b x =

    (1.0 / (2.0 * b * x)) * exp (- (abs (log x - µ)) / b)
