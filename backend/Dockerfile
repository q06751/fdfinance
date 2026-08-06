FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore src/FdFinance.Api/FdFinance.Api.csproj
RUN dotnet publish src/FdFinance.Api/FdFinance.Api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://0.0.0.0:5050
EXPOSE 5050
ENTRYPOINT ["dotnet", "FdFinance.Api.dll"]
