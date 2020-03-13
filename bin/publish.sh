#!/bin/bash

set -e

#--------------------------------------------------------------------------------------------------

basedir=$(dirname "${0}" | xargs -I% realpath %/..)

cd "${basedir}"

rm -rf _site/*
rm -rf _site/.git

JEKYLL_ENV=production ./bin/jekyll build

cd _site

github_repo=https://github.com/jkillingsworth/jkillingsworth.com.git

git init
git remote add origin ${github_repo}
git config core.safecrlf false
git checkout -b gh-pages
git add -A
git commit -m "Publish"
git push origin gh-pages -f
