FROM mcr.microsoft.com/dotnet/sdk:5.0 AS builder
WORKDIR /source

COPY *.sln .
COPY Procfile .
COPY TaigadevDiscordBot/TaigadevDiscordBot.csproj ./TaigadevDiscordBot/
COPY TaigadevDiscordBot.App/TaigadevDiscordBot.App.csproj ./TaigadevDiscordBot.App/
COPY TaigadevDiscordBot.Core/TaigadevDiscordBot.Core.csproj ./TaigadevDiscordBot.Core/
RUN dotnet restore

COPY TaigadevDiscordBot/. ./TaigadevDiscordBot/
COPY TaigadevDiscordBot.App/. ./TaigadevDiscordBot.App/
COPY TaigadevDiscordBot.Core/. ./TaigadevDiscordBot.Core/
WORKDIR /source/TaigadevDiscordBot
RUN dotnet publish --output /app/ --configuration Release

FROM mcr.microsoft.com/dotnet/sdk:5.0
WORKDIR /app
COPY --from=builder /app ./
CMD ["dotnet", "TaigadevDiscordBot.dll"]




# FROM mcr.microsoft.com/dotnet/sdk:5.0 AS builder
# WORKDIR .


# COPY *.sln .
# COPY Procfile .
# COPY ./TaigadevDiscordBot/TaigadevDiscordBot.csproj ./TaigadevDiscordBot/
# COPY ./TaigadevDiscordBot.App/TaigadevDiscordBot.App.csproj ./TaigadevDiscordBot.App/
# COPY ./TaigadevDiscordBot.Core/TaigadevDiscordBot.Core.csproj ./TaigadevDiscordBot.Core/
# 
# RUN dotnet restore
# COPY . .
# 
# RUN dotnet publish --output /app/ --configuration Release
# 
# COPY --from=builder .
# CMD ["dotnet", "TaigadevDiscordBot.dll"]