#!/usr/bin/env bash

sdir="$( cd "$( dirname "${BASH_SOURCE[0]}" )" >/dev/null 2>&1 && pwd )"
cd $sdir

configuration="Release"

declare -a RID=(
    "win-x64"
    "win-x86"
    "win-arm"
    "win-arm64"

    "win7-x64"
    "win7-x86"

    "win81-x64"
    "win81-x86"
    "win81-arm"

    "win10-x64"
    "win10-x86"
    "win10-arm"
    "win10-arm64"

    "linux-x64"
    "linux-musl-x64"
    "linux-arm"
    "linux-arm64"

    "osx-x64"
    "osx.10.10-x64"
    "osx.10.11-x64"
    "osx.10.12-x64"
    "osx.10.13-x64"
    "osx.10.14-x64"
)

for rt in "${RID[@]}"
do
    dotnet publish --configuration "$configuration" --runtime $rt -p:PublishSingleFile=true
done
