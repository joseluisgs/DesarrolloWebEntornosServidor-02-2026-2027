# Fase 1: compilar la aplicacion
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar proyecto y restaurar dependencias
COPY ProductosMongo/ProductosMongo.csproj ProductosMongo/
RUN dotnet restore ProductosMongo/ProductosMongo.csproj

# Copiar codigo fuente y compilar
COPY ProductosMongo/ ProductosMongo/
RUN dotnet publish ProductosMongo/ProductosMongo.csproj -c Release -o /app

# Fase 2: crear la imagen de runtime (ligera)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
ENTRYPOINT ["dotnet", "ProductosMongo.dll"]
