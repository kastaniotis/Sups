#!/bin/bash
dotnet publish -r linux-x64 -o ./public/x64 -p:DebugType=none -c Release -p:PublishTrimmed=true -p:EnableCompressionSingleFile=true --self-contained -p:PublishAot=true
cd public/x64
tar -czvf sups.tar.gz ./sups
