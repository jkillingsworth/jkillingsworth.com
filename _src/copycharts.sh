#!/bin/bash

copychart () {
    postname=$1
    prefixno=$2
    filename=$3
    cp ./$postname/build/$filename ../_assets/$postname/$prefixno-$filename
}

post="2018-04-23-fixed-fractions-and-fair-games"

copychart $post fig-06 constant-add-lin-sim.svg
copychart $post fig-07 constant-add-lin-avg.svg
copychart $post fig-08 constant-add-lin-med.svg
copychart $post fig-15 fraction-add-lin-sim.svg
copychart $post fig-16 fraction-add-log-sim.svg
copychart $post fig-17 fraction-add-lin-avg.svg
copychart $post fig-18 fraction-add-lin-med.svg
copychart $post fig-19 fraction-add-log-med.svg

post="2018-07-24-linear-and-log-scale-distributions"

copychart $post fig-01 stochastic-constant-add-lin.svg
copychart $post fig-07 exhaustive-constant-add-lin.svg
copychart $post fig-10 exhaustive-fraction-add-lin.svg
copychart $post fig-11 exhaustive-fraction-add-log.svg
copychart $post fig-17 exhaustive-fraction-mul-lin.svg
copychart $post fig-18 exhaustive-fraction-mul-log.svg

post="2018-09-21-least-squares-moving-averages"

copychart $post fig-03 simple-MSFT-full.svg
copychart $post fig-04 simple-MSFT-zoom.svg
copychart $post fig-05 simple-WYNN-full.svg
copychart $post fig-06 simple-WYNN-zoom.svg
copychart $post fig-07 simple-HEAR-full.svg
copychart $post fig-08 simple-HEAR-zoom.svg
copychart $post fig-11 lsrlin-MSFT-full.svg
copychart $post fig-12 lsrlin-MSFT-zoom.svg
copychart $post fig-13 lsrlin-WYNN-full.svg
copychart $post fig-14 lsrlin-WYNN-zoom.svg
copychart $post fig-15 lsrlin-HEAR-full.svg
copychart $post fig-16 lsrlin-HEAR-zoom.svg
copychart $post fig-19 lsrpol-MSFT-full.svg
copychart $post fig-20 lsrpol-MSFT-zoom.svg
copychart $post fig-21 lsrpol-WYNN-full.svg
copychart $post fig-22 lsrpol-WYNN-zoom.svg
copychart $post fig-23 lsrpol-HEAR-full.svg
copychart $post fig-24 lsrpol-HEAR-zoom.svg
copychart $post fig-31 lsrexp-MSFT-full.svg
copychart $post fig-32 lsrexp-MSFT-zoom.svg
copychart $post fig-33 lsrexp-WYNN-full.svg
copychart $post fig-34 lsrexp-WYNN-zoom.svg
copychart $post fig-35 lsrexp-HEAR-full.svg
copychart $post fig-36 lsrexp-HEAR-zoom.svg
