#!/bin/bash
echo "Iniciando build Blazor WebAssembly para Vercel..."

dotnet workload install wasm-tools
dotnet restore
dotnet publish -c Release -o published

cp -r published/wwwroot/* .

echo "Build finalizado"
