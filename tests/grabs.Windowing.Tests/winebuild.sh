#!/bin/bash

clear
dotnet build -r win-x64 || exit 1
clear
pushd bin/Debug/net9.0/win-x64
WINEDEBUG=-all wine grabs.Windowing.Tests.exe
popd
