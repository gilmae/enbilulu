FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# copy everything else and build app
COPY nuget.config .
COPY Enbilulu.Server/. ./Enbilulu.Server/
COPY libEnbilulu/. ./libEnbilulu/
RUN dotnet restore libEnbilulu
RUN dotnet publish libEnbilulu -c Release -o out
RUN dotnet restore Enbilulu.Server
WORKDIR /app
RUN dotnet publish Enbilulu.Server -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "Enbilulu.Server.dll"]