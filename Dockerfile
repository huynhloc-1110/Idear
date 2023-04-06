# https://hub.docker.com/_/microsoft-dotnet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /source
COPY *.sln .
COPY Idear/. ./Idear/
WORKDIR /source/Idear
RUN dotnet publish -c release -o /app

COPY Idear/Idear.db /source/Idear/bin/release/net6.0/

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app ./
ENTRYPOINT ["dotnet", "Idear.dll"]
