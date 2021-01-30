module Compute

//-------------------------------------------------------------------------------------------------

let lagrange p λ =

    ((p - 0.5) ** 2.0) - (λ * (0.5 - p))

let costfunc p λ =

    ((2.0 * p - 1.0 + λ) ** 2.0) + ((p - 0.5) ** 2.0)

//-------------------------------------------------------------------------------------------------

let heatmap density f =

    let densityX = fst density
    let densityY = snd density

    let f i j =
        let p = float i / float densityX
        let λ = float j / float densityY - 0.5
        f p λ

    Array2D.init (densityX + 1) (densityY + 1) f

//-------------------------------------------------------------------------------------------------

let slice samples f (p0, pN) (λ0, λN) =

    let calculation i =
        let p = p0 + (pN - p0) * (float i / float samples)
        let λ = λ0 + (λN - λ0) * (float i / float samples)
        (p, λ, f p λ)

    Array.init (samples + 1) calculation
