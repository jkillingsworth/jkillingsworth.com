#!/bin/bash

set -e

#--------------------------------------------------------------------------------------------------

basedir=$(dirname "${0}" | xargs -I% realpath %/..)

cd "${basedir}"

#--------------------------------------------------------------------------------------------------

run_console_app()
{
    pushd "${dir}"/build/bin/Release/net7.0
    ./ConsoleApp
    popd
}

rm -rf ./src/Common/build/
dotnet build -c Release ./src/Common/Common.sln

for dir in ./src/20[0-9][0-9]-[0-9][0-9]-[0-9][0-9]-*; do
    rm -rf "${dir}"/build/
    dotnet build -c Release "${dir}"/ConsoleApp.sln
    run_console_app > /dev/null
done
