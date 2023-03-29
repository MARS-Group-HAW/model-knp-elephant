#!/usr/bin/env bash
output=KrugerNationalParkBase

rm -rf "${output}"
rm -rf ./${output}_*.zip

dotnet publish -c Release -r osx-x64 /p:DebugSymbols=false /p:DebugType=None \
  --self-contained true -o ${output}/${output}_MACOSX && \
  cp -r model_input/ ./${output}/${output}_MACOSX/model_input && \
  cp config.json ./${output}/${output}_MACOSX/
  
dotnet publish -c Release -r win-x64 /p:DebugSymbols=false /p:DebugType=None \
  --self-contained true -o ${output}/${output}_WINDOWS && \
  cp -r model_input/ ./${output}/${output}_WINDOWS/model_input && \
  cp config.json ./${output}/${output}_WINDOWS/
  
dotnet publish -c Release -r linux-x64 /p:DebugSymbols=false /p:DebugType=None \
  --self-contained true -o ${output}/${output}_LINUX && \
  cp -r model_input/ ./${output}/${output}_LINUX/model_input && \
  cp config.json ./${output}/${output}_LINUX/


zip -r ./${output}_WINDOWS.zip ./${output}/${output}_WINDOWS/
zip -r ./${output}_MACOSX.zip ./${output}/${output}_MACOSX/
zip -r ./${output}_LINUX.zip ./${output}/${output}_LINUX/