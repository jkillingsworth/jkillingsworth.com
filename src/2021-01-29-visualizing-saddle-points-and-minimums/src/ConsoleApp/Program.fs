module Program

//-------------------------------------------------------------------------------------------------

let density = (20, 20)
let samples = 100

let path filename = "../../../" + filename

//-------------------------------------------------------------------------------------------------

let execute name func range =

    let optimum = (0.5, 0.0)

    let heatmap = Compute.heatmap density func

    let slice1 = Compute.slice samples func (0.0, 1.0) (-0.3, +0.3)
    let slice2 = Compute.slice samples func (0.3, 0.7) (+0.5, -0.5)

    Chart.renderHeatmap (path name + "-heatmap-0.svg") heatmap range 0 optimum Array.empty

    Chart.renderSurface (path name + "-surface-1.svg") heatmap range 1
    Chart.renderSurface (path name + "-surface-2.svg") heatmap range 2
    Chart.renderSurface (path name + "-surface-3.svg") heatmap range 3

    Chart.renderHeatmap (path name + "-heatmap-1.svg") heatmap range 1 optimum slice1
    Chart.renderProfile (path name + "-profile-1.svg") samples range 1 optimum slice1

    Chart.renderHeatmap (path name + "-heatmap-2.svg") heatmap range 2 optimum slice2
    Chart.renderProfile (path name + "-profile-2.svg") samples range 2 optimum slice2

//-------------------------------------------------------------------------------------------------

execute "lagrange" Compute.lagrange (-0.1, +0.5)
execute "costfunc" Compute.costfunc (-0.5, +2.5)
