pushd ..\Dlls
& .\pack.bat

if (-not (Test-Path ../../packages)) { mkdir ../../packages }
cp *.nupkg ../../packages
cp UniRx.SystemReactive.UnityWorkaround\UniRx.SystemReactive.UnityWorkaround\bin\Release\*.nupkg ..\..\packages
popd
