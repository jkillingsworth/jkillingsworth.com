#!/bin/bash

set -e

#--------------------------------------------------------------------------------------------------

basedir=$(dirname "${0}" | xargs -I% realpath %/..)

cd "${basedir}"

find ./_assets -type f -name fig-??-*.svg ! -name fig-??-latex-*.svg -delete
