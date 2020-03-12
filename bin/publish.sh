#!/bin/bash

set -e

#--------------------------------------------------------------------------------------------------

basedir=$(realpath "${0}" | xargs -0 dirname | xargs -0 dirname)

cd "${basedir}"

JEKYLL_ENV=production ./bin/jekyll build

cd _site

git update-ref -d HEAD
git add -A
git commit -m "Publish"
git push origin gh-pages -f
