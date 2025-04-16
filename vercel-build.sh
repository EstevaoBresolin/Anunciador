#!/bin/bash

# Instala ferramentas se necessário
dotnet workload install wasm-tools

# Restaura pacotes e publica o app Blazor
dotnet restore
dotnet publish -c Release -o output

# Copia os arquivos publicados para a pasta esperada pela Vercel
cp -r output/wwwroot ./wwwroot
