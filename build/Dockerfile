FROM microsoft/aspnetcore-build:2.0 AS build-env

WORKDIR /app
COPY ./src/ ./
WORKDIR /app/Downlink
RUN dotnet publish -c Release -o out

FROM microsoft/aspnetcore:2.0

WORKDIR /downlink
RUN mkdir /downlink/config
COPY --from=build-env /app/Downlink/out .
VOLUME /downlink/config
ENTRYPOINT ["dotnet", "Downlink.dll"]
