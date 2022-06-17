#!/bin/bash

set -e

#--------------------------------------------------------------------------------------------------

basedir=$(dirname "${0}" | xargs -I% realpath %/..)

cd "${basedir}"

#--------------------------------------------------------------------------------------------------

run_console_app()
{
    pushd "${dir}"/build/bin-ConsoleApp/Release/net6.0
    ./ConsoleApp
    popd
}

rm -rf ./src/Common/build
dotnet build -c Release ./src/Common/Workspace.sln

for dir in ./src/20[0-9][0-9]-[0-9][0-9]-[0-9][0-9]-*; do
    rm -rf "${dir}"/build
    dotnet build -c Release "${dir}"/Workspace.sln
    run_console_app > /dev/null
done
