FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore AuthMicroService/AuthMicroService.csproj
RUN dotnet publish AuthMicroService/AuthMicroService.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 5101
ENTRYPOINT ["dotnet", "AuthMicroService.dll"]
