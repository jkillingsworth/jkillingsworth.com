#!/bin/bash

set -e

#--------------------------------------------------------------------------------------------------

basedir=$(dirname "${0}" | xargs -I% realpath %/..)

cd "${basedir}"

#--------------------------------------------------------------------------------------------------

rm -rf _site/*
rm -rf _site/.git

git clone https://github.com/jkillingsworth/jkillingsworth.com.git _site -b gh-pages --depth 1

JEKYLL_ENV=production ./bin/jekyll build

cd _site

git config core.safecrlf false
git add -A
git commit -m "Publish"
git push origin gh-pages
