#!/bin/bash

jobname=latextosvg
stdout=stdout.log
stderr=stderr.log

tempdir=$(mktemp -d)
pushd $tempdir > /dev/null

cat > $jobname.tex
ignore=$(latex --halt-on-error --interaction=nonstopmode $jobname.tex > $stdout 2> >(tee -a $stderr >&2))
output=$(dvisvgm $jobname.dvi --no-fonts --exact --zoom=1.333 --verb=3 --stdout 2> >(tee -a $stderr >&2))
echo $output

popd > /dev/null
rm -rf $tempdir
