module Program

open System
open System.Text

//-------------------------------------------------------------------------------------------------

Console.OutputEncoding <- new UTF8Encoding()

let path filename = Common.Chart.outputPath filename

//-------------------------------------------------------------------------------------------------

let runExample1 () =

    let fs = Compute.pMass
    let hs = Compute.hFirstDifference

    let xs = Compute.generateSeriesFromPmfunc fs 20
    let ys = Compute.convolutionSeries xs hs

    Chart.renderSeries (path "example-1-series-xs.svg") xs 1
    Chart.renderSeries (path "example-1-series-ys.svg") ys 2

//-------------------------------------------------------------------------------------------------

let runExample2 () =

    let fs = Compute.pMass
    let gs = Compute.convolutionPmfunc fs fs

    Chart.renderPmfunc (path "example-2-pmfunc-fs.svg") fs 1
    Chart.renderPmfunc (path "example-2-pmfunc-gs.svg") gs 2

//-------------------------------------------------------------------------------------------------

let runExample3 () =

    let pRange = (+0.0, +0.5)
    let xRange = (-5.0, +5.0)

    let μ = 0.0
    let σ = 1.0

    let f = Compute.pDensityN μ σ
    let g = Compute.convolution f f

    let fs = Compute.ps xRange f
    let gs = Compute.ps xRange g

    Chart.renderPdfunc (path "example-3-pdfunc-fs.svg") pRange fs 1
    Chart.renderPdfunc (path "example-3-pdfunc-gs.svg") pRange gs 2

//-------------------------------------------------------------------------------------------------

let runExample4 () =

    let pRange = (+0.0, +0.5)
    let xRange = (-5.0, +5.0)

    let μ = 0.0
    let b = 1.0

    let f = Compute.pDensityL μ b
    let g = Compute.convolution f f

    let fs = Compute.ps xRange f
    let gs = Compute.ps xRange g

    Chart.renderPdfunc (path "example-4-pdfunc-fs.svg") pRange fs 1
    Chart.renderPdfunc (path "example-4-pdfunc-gs.svg") pRange gs 2

//-------------------------------------------------------------------------------------------------

let runExample5 () =

    let pRange = (+0.0, +0.5)
    let xRange = (-5.0, +5.0)

    let μ = 0.0
    let b = 2.0

    let G = Compute.fourierTransformDensityL μ b

    let f = Compute.deconvolution G
    let g = Compute.pDensityL μ b

    let fs = Compute.ps xRange f
    let gs = Compute.ps xRange g

    Chart.renderPdfunc (path "example-5-pdfunc-fs.svg") pRange fs 1
    Chart.renderPdfunc (path "example-5-pdfunc-gs.svg") pRange gs 2

//-------------------------------------------------------------------------------------------------

runExample1 ()
runExample2 ()
runExample3 ()
runExample4 ()
runExample5 ()
