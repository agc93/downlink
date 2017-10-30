FROM microsoft/aspnetcore-build:2.0 AS build-env

WORKDIR /app
COPY ./src/ ./
WORKDIR /app/Downlink.Host
RUN dotnet publish -c Release -o out

FROM microsoft/aspnetcore:2.0

WORKDIR /downlink
RUN mkdir /downlink/config
COPY --from=build-env /app/Downlink.Host/out .
COPY ./build/appsettings.json .
VOLUME /downlink/config
EXPOSE 80
ENTRYPOINT ["dotnet", "Downlink.Host.dll"]
