#!/bin/bash

JEKYLL_ENV=production jekyll build

pushd _site
git update-ref -d HEAD
git add -A
git commit -m "Publish"
git push origin gh-pages -f
popd
