FROM microsoft/aspnetcore:2.0

WORKDIR /downlink
RUN mkdir /downlink/config
COPY ./publish/Downlink .
VOLUME /downlink/config
EXPOSE 80
ENTRYPOINT ["dotnet", "Downlink.dll"]
