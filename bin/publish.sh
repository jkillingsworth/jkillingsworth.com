#!/bin/bash

set -e

#--------------------------------------------------------------------------------------------------

basedir=$(dirname "${0}" | xargs -I% realpath %/..)

cd "${basedir}"

JEKYLL_ENV=production ./bin/jekyll build

cd _site

git update-ref -d HEAD
git add -A
git commit -m "Publish"
git push origin gh-pages -f
