# Stage 1 : Base image avec configuration des variables d'environnement
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Configuration pour SQLite
ENV ASPNETCORE_ENVIRONMENT=Production
ENV CONNECTIONSTRINGS__DEFAULTCONNECTION="Data Source=/app/data/api.db;"

# Ports exposés et création du dossier de données
EXPOSE 8080
EXPOSE 8081
RUN mkdir -p /app/data && chmod -R 777 /app/data  # Permissions pour SQLite

# Stage 2 : Build de l'application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["OrthoHelperAPI/OrthoHelperAPI.csproj", "OrthoHelperAPI/"]
RUN dotnet restore "OrthoHelperAPI/OrthoHelperAPI.csproj"
COPY . .
WORKDIR "/src/OrthoHelperAPI"
RUN dotnet build "OrthoHelperAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 3 : Publication de l'application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OrthoHelperAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4 : Image finale avec persistance des données
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Volume pour la persistance de la base de données
VOLUME /app/data

ENTRYPOINT ["dotnet", "OrthoHelperAPI.dll"]