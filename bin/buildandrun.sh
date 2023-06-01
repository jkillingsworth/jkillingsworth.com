#!/bin/bash

set -e

#--------------------------------------------------------------------------------------------------

basedir=$(dirname "${0}" | xargs -I% realpath %/..)

cd "${basedir}"

#--------------------------------------------------------------------------------------------------

run_console_app()
{
    pushd "${dir}"/src/ConsoleApp/bin/Release/net7.0
    ./ConsoleApp
    popd
}

rm -rf ./src/Common/src/Common/bin
rm -rf ./src/Common/src/Common/obj
dotnet build -c Release ./src/Common/Workspace.sln

for dir in ./src/20[0-9][0-9]-[0-9][0-9]-[0-9][0-9]-*; do
    rm -rf "${dir}"/output
    rm -rf "${dir}"/src/ConsoleApp/bin
    rm -rf "${dir}"/src/ConsoleApp/obj
    dotnet build -c Release "${dir}"/Workspace.sln
    run_console_app > /dev/null
done
