$runtimeIdentifiers = @("linux-arm", "linux-arm64", "linux-bionic-arm64", "linux-musl-arm", "linux-musl-arm64", "linux-musl-x64", "linux-x64", "osx-arm64", "osx-x64", "win-arm64", "win-x64", "win-x86")
$config = "Release"
$path = "publish"

Get-ChildItem -Path $path | Remove-Item -Recurse -Force

foreach ($rid in $runtimeIdentifiers) {
	dotnet publish .\src\Void.Proxy\ -c $config -r $rid --output "$path\$rid" --self-contained true
	dotnet publish .\src\Void.Proxy\ -c $config -r $rid --output "$path\$rid-minimal" --self-contained false
}

Get-ChildItem -Path $path -Recurse | Where-Object {!$_.PSIsContainer -and $_.Extension -ne '.pdb'} | Move-Item -Destination $path -Force
Get-ChildItem -Path $path -Directory | Remove-Item -Recurse -Force