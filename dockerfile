FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env

RUN mkdir /App

WORKDIR /App

COPY . ./

RUN dotnet restore --verbosity detailed ./selenium-shenanigans.csproj

RUN dotnet build ./selenium-shenanigans.csproj --no-restore --output out --configuration Release

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine

WORKDIR /App

COPY --from=build-env /App/out .

ENTRYPOINT [ "dotnet", "selenium-shenanigans.dll" ]