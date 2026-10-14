# Fase 1: compilar la aplicación
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar proyecto y restaurar dependencias
COPY ProductosTest/ProductosTest.csproj ProductosTest/
RUN dotnet restore ProductosTest/ProductosTest.csproj

# Copiar código fuente y compilar
COPY ProductosTest/ ProductosTest/
RUN dotnet publish ProductosTest/ProductosTest.csproj -c Release -o /app

# Fase 2: crear la imagen de runtime (ligera)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

EXPOSE 8080
ENTRYPOINT ["dotnet", "ProductosTest.dll"]
