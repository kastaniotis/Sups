#!/bin/bash
dotnet publish -r linux-arm -o ./public/arm32 -p:DebugType=none -c Release -p:PublishTrimmed=true -p:EnableCompressionSingleFile=true --self-contained -p:PublishSingleFile=true
cd public/arm32
tar -czvf sups.tar.gz ./sups
