#!/bin/bash
dotnet publish -o ./public -p:DebugType=none
cd public
tar -czvf sups.tar.gz ./sups
