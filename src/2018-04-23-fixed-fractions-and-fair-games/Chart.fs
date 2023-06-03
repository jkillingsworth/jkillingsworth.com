module Chart

open Common.Chart

//-------------------------------------------------------------------------------------------------

let private plotBankroll = baseplotRegular + "
$data0 << EOD
{0}
EOD

style = {1}; lower = {2}; upper = {3}

set xlabel 'Number of Plays'
set xrange [0:200]
set xtics 50
set mxtics 2

if (style == 1) {{
    set ylabel 'Dollars'
    set yrange [lower:upper]
    set ytics 100
    set mytics 5
}}

if (style == 2) {{
    set ylabel 'Dollars'
    set yrange [lower:upper]
    set ytics 10
    set logscale y 10
}}

set key top right
set key noreverse Right

set linetype 1 linewidth 1 linecolor rgb richRojo

plot $data0 using 1:2 with lines title 'Gambler''s Bankroll'
"

let private renderBankroll style path lower upper items =

    let data0 =
        items
        |> Array.mapi (fun i x -> sprintf "%O %O" i x)
        |> String.concat "\n"

    render path plotBankroll [| data0; style; lower; upper |]

let renderBankrollLin lower upper items = renderBankroll 1 lower upper items
let renderBankrollLog lower upper items = renderBankroll 2 lower upper items
