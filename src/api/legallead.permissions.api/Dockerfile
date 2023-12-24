#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=https://+:443
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["api/legallead.permissions.api/nuget.config", "api/legallead.permissions.api/"]
COPY ["nuget.config", "."]
COPY ["api/legallead.permissions.api/legallead.permissions.api.csproj", "api/legallead.permissions.api/"]
RUN dotnet restore "./api/legallead.permissions.api/./legallead.permissions.api.csproj"
COPY . .
WORKDIR "/src/api/legallead.permissions.api"
RUN dotnet build "./legallead.permissions.api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./legallead.permissions.api.csproj" -r linux-x64 -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "legallead.permissions.api.dll"]