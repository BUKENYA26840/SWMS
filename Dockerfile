FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY AMS_26840.csproj .
RUN dotnet restore "AMS_26840.csproj"
COPY Controllers/ ./Controllers/
COPY Data/ ./Data/
COPY DTOs/ ./DTOs/
COPY Middleware/ ./Middleware/
COPY Migrations/ ./Migrations/
COPY Models/ ./Models/
COPY Properties/ ./Properties/
COPY Services/ ./Services/
COPY wwwroot/ ./wwwroot/
COPY Program.cs .
COPY WeatherForecast.cs .
COPY appsettings.json .
COPY appsettings.Development.json .
RUN dotnet publish "AMS_26840.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AMS_26840.dll"]
