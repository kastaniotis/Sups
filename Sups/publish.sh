#!/bin/bash
dotnet publish -o ./public -p:DebugType=none
cd public
tar -czvf Sups.tar.gz ./Sups
