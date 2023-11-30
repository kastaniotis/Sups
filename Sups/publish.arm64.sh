#!/bin/bash
dotnet publish -r linux-arm64 -o ./public/arm64 -p:DebugType=none -c Release -p:PublishTrimmed=true -p:EnableCompressionSingleFile=true --self-contained -p:PublishAot=true
cd public/arm64
tar -czvf sups.tar.gz ./sups
