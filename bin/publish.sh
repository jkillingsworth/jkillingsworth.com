#!/bin/bash

if [ ! -d _site ]; then
    echo "You need to run this from the project root directory."
    exit 1
fi

JEKYLL_ENV=production ./bin/jekyll build

pushd _site
git update-ref -d HEAD
git add -A
git commit -m "Publish"
git push origin gh-pages -f
popd
