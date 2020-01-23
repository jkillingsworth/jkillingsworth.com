#!/bin/bash

if [ ! -d _site ]; then
    echo "You need to run this from the project root directory."
    exit 1
fi

find ./_assets -type f -name fig-*-latex-*.svg -delete
