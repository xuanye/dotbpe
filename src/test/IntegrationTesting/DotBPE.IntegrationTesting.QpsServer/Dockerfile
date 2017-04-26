FROM microsoft/dotnet:1.1.1-runtime
WORKDIR /qps_server
COPY ./bin/out ./
EXPOSE 6201/tcp
ENTRYPOINT ["dotnet", "DotBPE.IntegrationTesting.QpsServer.dll","--port","6201"]
