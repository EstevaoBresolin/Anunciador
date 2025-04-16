#!/bin/bash

echo "⚙️ Iniciando build Blazor WASM..."

dotnet workload install wasm-tools
dotnet restore
dotnet publish -c Release -o output

# Copia o wwwroot para a pasta raiz do deploy
cp -r output/wwwroot/ wwwroot
