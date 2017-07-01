FROM microsoft/aspnetcore:2.0

WORKDIR /downlink
RUN mkdir /downlink/config
COPY ./dist/publish/Downlink .
VOLUME /downlink/config
ENTRYPOINT ["dotnet", "Downlink.dll"]
