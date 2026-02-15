FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Cloud9.2.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish ./

ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

ENTRYPOINT ["dotnet", "Cloud9.2.dll"]
