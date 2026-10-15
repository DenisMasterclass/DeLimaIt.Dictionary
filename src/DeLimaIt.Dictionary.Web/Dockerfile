FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY src/DeLimaIt.Dictionary.Web/DeLimaIt.Dictionary.Web.csproj src/DeLimaIt.Dictionary.Web/
RUN dotnet restore src/DeLimaIt.Dictionary.Web/DeLimaIt.Dictionary.Web.csproj
COPY . .
RUN dotnet publish src/DeLimaIt.Dictionary.Web/DeLimaIt.Dictionary.Web.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DeLimaIt.Dictionary.Web.dll"]
