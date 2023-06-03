module Compute

open System

//-------------------------------------------------------------------------------------------------

let densityN μ σ x =

    (1.0 / (x * σ * sqrt (2.0 * Math.PI))) * exp (- ((log x - μ) ** 2.0) / (2.0 * σ ** 2.0))

let densityL μ b x =

    (1.0 / (2.0 * b * x)) * exp (- (abs (log x - μ)) / b)
