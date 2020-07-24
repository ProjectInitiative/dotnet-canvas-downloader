cd $PSScriptRoot

$RID = @(
    "win-x64",
    "win-x86",
    "win-arm",
    "win-arm64",

    "win7-x64",
    "win7-x86",

    "win81-x64",
    "win81-x86",
    "win81-arm",

    "win10-x64",
    "win10-x86",
    "win10-arm",
    "win10-arm64",

    "linux-x64",
    "linux-musl-x64",
    "linux-arm",
    "linux-arm64",

    "osx-x64",
    "osx.10.10-x64",
    "osx.10.11-x64",
    "osx.10.12-x64",
    "osx.10.13-x64",
    "osx.10.14-x64")

    Remove-Item -Recurse -Force "$PSScriptRoot\\bin"

For ($i=0; $i -lt $RID.Length; $i++) {
    New-Item -ItemType Directory -Force -Path "$PSScriptRoot\\bin\\artifacts\\$($RID[$i])"
    dotnet publish --configuration Release --runtime $RID[$i] -p:PublishSingleFile=true -p:TrimUnusedDependencies=true
    Remove-Item "$PSScriptRoot\\bin\\Release\\netcoreapp3.1\\$($RID[$i])\\publish\\canvas-downloader.pdb"
    Move-Item "$PSScriptRoot\\bin\\Release\\netcoreapp3.1\\$($RID[$i])\\publish\\*" "$PSScriptRoot\\bin\\artifacts\\$($RID[$i])"
    Compress-Archive -Path "$PSScriptRoot\\bin\\artifacts\\$($RID[$i])" -DestinationPath "$PSScriptRoot\\bin\\artifacts\\$($RID[$i]).zip"
}
