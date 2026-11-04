FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiar los csproj y restaurar ANTES de copiar el codigo (cache de capas)
COPY ProductosTest/ProductosTest.csproj ProductosTest/
COPY ProductosTest.Test/ProductosTest.Test.csproj ProductosTest.Test/
RUN dotnet restore ProductosTest/ProductosTest.csproj

COPY . .
RUN dotnet publish ProductosTest -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish .

# Puerto interno 8080: el compose publica 5110:8080 como el resto de la unidad
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "ProductosTest.dll"]
