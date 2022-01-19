#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TaigadevDiscordBot/TaigadevDiscordBot.csproj", "TaigadevDiscordBot/"]
COPY ["TaigadevDiscordBot.Core/TaigadevDiscordBot.Core.csproj", "TaigadevDiscordBot.Core/"]
COPY ["TaigadevDiscordBot.App/TaigadevDiscordBot.App.csproj", "TaigadevDiscordBot.App/"]
RUN dotnet restore "TaigadevDiscordBot/TaigadevDiscordBot.csproj"
COPY . .
WORKDIR "/src/TaigadevDiscordBot"
RUN dotnet build "TaigadevDiscordBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TaigadevDiscordBot.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TaigadevDiscordBot.dll"]