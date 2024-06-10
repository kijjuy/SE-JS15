FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

#Build
COPY . ./

RUN dotnet restore

RUN dotnet publish -c Debug -o out

#Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .

ENTRYPOINT ["dotnet", "dbms-mvc.dll"]
