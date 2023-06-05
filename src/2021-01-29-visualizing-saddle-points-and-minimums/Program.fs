module Program

open System
open System.Text

//-------------------------------------------------------------------------------------------------

Console.OutputEncoding <- new UTF8Encoding()

let path filename = Common.Const.runoutput + filename

//-------------------------------------------------------------------------------------------------

let density = (20, 20)
let samples = 100

//-------------------------------------------------------------------------------------------------

let execute name func range mode =

    let optimum = (0.5, 0.0)

    let heatmap = Compute.heatmap density func

    let slice1 = Compute.slice samples func (0.0, 1.0) (-0.3, +0.3)
    let slice2 = Compute.slice samples func (0.3, 0.7) (+0.5, -0.5)

    Chart.renderHeatmap (path name + "-heatmap-0.svg") heatmap range mode 0 optimum Array.empty

    Chart.renderSurface (path name + "-surface-1.svg") heatmap range mode 1
    Chart.renderSurface (path name + "-surface-2.svg") heatmap range mode 2
    Chart.renderSurface (path name + "-surface-3.svg") heatmap range mode 3

    Chart.renderHeatmap (path name + "-heatmap-1.svg") heatmap range mode 1 optimum slice1
    Chart.renderProfile (path name + "-profile-1.svg") samples range mode 1 optimum slice1

    Chart.renderHeatmap (path name + "-heatmap-2.svg") heatmap range mode 2 optimum slice2
    Chart.renderProfile (path name + "-profile-2.svg") samples range mode 2 optimum slice2

//-------------------------------------------------------------------------------------------------

execute "lagrange" Compute.lagrange (-0.1, +0.5) 1
execute "costfunc" Compute.costfunc (-0.5, +2.5) 2
