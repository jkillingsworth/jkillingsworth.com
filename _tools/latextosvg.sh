#!/bin/bash

set -e

if [ -z "${1}" ]; then
    echo "Usage: ${0} TEXFILE [SVGFILE]"
    exit 1
fi

tempdir=$(mktemp -d)
texfile=$(realpath "${1}")
svgfile=output.svg
jobname=latextosvg

if [ -n "${2}" ]; then svgfile="${2}"; fi

pushd ${tempdir} > /dev/null

cp "${texfile}" ${jobname}.tex
latex --halt-on-error --interaction=nonstopmode ${jobname}.tex
dvisvgm ${jobname}.dvi --no-fonts --exact --zoom=1.333333 --precision=6 --verbosity=7

popd > /dev/null

cp ${tempdir}/${jobname}.svg "${svgfile}"
rm ${tempdir} -rf
