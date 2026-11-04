FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore ProductosMicroService/ProductosMicroService.csproj
RUN dotnet publish ProductosMicroService/ProductosMicroService.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5102
ENTRYPOINT ["dotnet", "ProductosMicroService.dll"]
