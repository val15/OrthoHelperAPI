# Stage 1 : Build Angular
FROM node:20 AS angular-build
WORKDIR /app
# Copier le projet Angular depuis Frontend/ortho-helper-angular-app
COPY Frontend/ortho-helper-angular-app/ .
# Installer les dépendances et construire le projet Angular
# La commande de build génère les fichiers dans ../OrthoHelperAPI/wwwroot
RUN npm install && npm run build -- --configuration production --output-path=../OrthoHelperAPI/wwwroot

# Stage 2 : Base .NET runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
ENV ASPNETCORE_ENVIRONMENT=Production
ENV CONNECTIONSTRINGS__DEFAULTCONNECTION="Data Source=/app/data/api.db;"
EXPOSE 8080
EXPOSE 8081
RUN mkdir -p /app/data && chmod -R 777 /app/data

# Stage 3 : Build du projet .NET
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Copier le fichier projet .NET et restaurer les dépendances
COPY ["OrthoHelperAPI/OrthoHelperAPI.csproj", "OrthoHelperAPI/"]
RUN dotnet restore "OrthoHelperAPI/OrthoHelperAPI.csproj"
# Copier l'ensemble du code source (cela inclut le dossier OrthoHelperAPI avec wwwroot)
COPY . .
# Placer le contexte de travail dans le dossier du projet
WORKDIR /src/OrthoHelperAPI
# Copier le contenu généré par Angular depuis le stage angular-build dans wwwroot
COPY --from=angular-build /OrthoHelperAPI/wwwroot ./wwwroot
# Déplacer le contenu du dossier "browser" à la racine de wwwroot et supprimer le dossier "browser"
RUN mv wwwroot/browser/* wwwroot/ && rm -rf wwwroot/browser
# Construire le projet .NET
RUN dotnet build "OrthoHelperAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Stage 4 : Publication du projet .NET
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "OrthoHelperAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 5 : Image finale
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
VOLUME /app/data
ENTRYPOINT ["dotnet", "OrthoHelperAPI.dll"]
