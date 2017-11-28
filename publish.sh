#!/bin/bash

jekyll build
pushd _site
git update-ref -d HEAD
git add -A
git commit -m "Publish"
git push origin gh-pages -f
popd
