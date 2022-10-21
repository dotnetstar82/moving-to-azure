# build "server" image
FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine as build

WORKDIR /src
COPY MovingToAzure.csproj .
RUN dotnet restore

COPY . .
RUN dotnet build -c Release --no-restore
RUN dotnet test -c Release --no-build
RUN dotnet publish -c Release --no-build -o /dist


# production runtime "server" image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine

# Because SqlClient doesn't like missing cultures https://github.com/dotnet/SqlClient/issues/220
RUN apk add --no-cache icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

ENV APPSETTINGS__BlobContainerName ""
ENV APPSETTINGS__CacheSeconds 60
ENV CONNECTIONSTRINGS__MSSQL ""
ENV CONNECTIONSTRINGS__Redis ""
ENV CONNECTIONSTRINGS__Blob ""
ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_URLS http://+:80
EXPOSE 80

WORKDIR /app
COPY --from=build /dist .

CMD ["dotnet", "MovingToAzure.dll"]
