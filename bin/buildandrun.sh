#!/bin/bash

set -e

#--------------------------------------------------------------------------------------------------

basedir=$(dirname "${0}" | xargs -I% realpath %/..)

cd "${basedir}"

#--------------------------------------------------------------------------------------------------

run_console_app()
{
    pushd "${dir}"
    dotnet run -c Release --no-build
    popd
}

rm -rf ./src/Common/build/
dotnet build -c Release ./src/Common/Common.sln

for dir in ./src/20[0-9][0-9]-[0-9][0-9]-[0-9][0-9]-*; do
    rm -rf "${dir}"/build/
    dotnet build -c Release "${dir}"/ConsoleApp.sln
    run_console_app > /dev/null
done
