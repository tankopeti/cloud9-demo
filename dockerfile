# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY Cloud9.2.csproj ./
RUN dotnet restore "Cloud9.2.csproj"

COPY . ./
RUN dotnet publish "Cloud9.2.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

COPY --from=build /app/publish ./

EXPOSE 8080
ENTRYPOINT ["dotnet", "Cloud9.2.dll"]