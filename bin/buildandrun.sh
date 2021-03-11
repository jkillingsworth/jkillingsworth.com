#!/bin/bash

set -e

#--------------------------------------------------------------------------------------------------

basedir=$(dirname "${0}" | xargs -I% realpath %/..)

cd "${basedir}"

#--------------------------------------------------------------------------------------------------

run_console_app()
{
    pushd "${dir}"/build/bin/Release/net5.0
    ./ConsoleApp
    popd
}

for dir in ./src/20[0-9][0-9]-[0-9][0-9]-[0-9][0-9]-*; do
    rm -rf "${dir}"/build
    dotnet build -c Release "${dir}"/Workspace.sln
    run_console_app > /dev/null
done
