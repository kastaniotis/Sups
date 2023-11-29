#!/bin/bash
dotnet publish -r linux-arm64 -o ./public/arm64 -p:DebugType=none
cd public/x64
tar -czvf sups.tar.gz ./sups
