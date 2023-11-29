#!/bin/bash
dotnet publish -r linux-x64 -o ./public/x64 -p:DebugType=none
cd public/x64
tar -czvf sups.tar.gz ./sups
