#!/bin/bash

set -e

#--------------------------------------------------------------------------------------------------

basedir=$(realpath "${0}" | xargs -0 dirname | xargs -0 dirname)

cd "${basedir}"

find ./_assets -type f -name fig-*-latex-*.svg -delete
