FROM mcr.microsoft.com/dotnet/sdk:8.0

RUN mkdir /App

WORKDIR /App

COPY . ./

RUN dotnet restore --verbosity detailed ./selenium-shenanigans-tests/selenium-shenanigans-tests.csproj

RUN dotnet build ./selenium-shenanigans-tests/selenium-shenanigans-tests.csproj

# ENV GRID_URI=http://selenium-hub:4444

RUN dotnet test ./selenium-shenanigans-tests/selenium-shenanigans-tests.csproj