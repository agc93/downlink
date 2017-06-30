FROM microsoft/dotnet:2.0-runtime

WORKDIR /app
COPY ./dist/publish/Downlink .
ENTRYPOINT ["dotnet", "Downlink.dll"]
