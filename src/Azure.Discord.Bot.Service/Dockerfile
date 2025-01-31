# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Azure.Discord.Bot.Service/Azure.Discord.Bot.Service.csproj", "src/Azure.Discord.Bot.Service/"]
COPY ["src/Azure.Discord.Bot.DataAccess.Npgsql/Azure.Discord.Bot.DataAccess.Npgsql.csproj", "src/Azure.Discord.Bot.DataAccess.Npgsql/"]
COPY ["src/Azure.Discord.Bot.DataAccess/Azure.Discord.Bot.DataAccess.csproj", "src/Azure.Discord.Bot.DataAccess/"]
COPY ["src/Azure.Discord.Bot.Models/Azure.Discord.Bot.Models.csproj", "src/Azure.Discord.Bot.Models/"]
COPY ["src/Azure.Discord.Bot.DataAccess.Sqlite/Azure.Discord.Bot.DataAccess.Sqlite.csproj", "src/Azure.Discord.Bot.DataAccess.Sqlite/"]
RUN dotnet restore "./src/Azure.Discord.Bot.Service/Azure.Discord.Bot.Service.csproj"
COPY . .
WORKDIR "/src/src/Azure.Discord.Bot.Service"
RUN dotnet build "./Azure.Discord.Bot.Service.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Azure.Discord.Bot.Service.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Azure.Discord.Bot.Service.dll"]