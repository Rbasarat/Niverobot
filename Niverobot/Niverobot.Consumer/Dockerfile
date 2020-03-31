#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Niverobot.Consumer/Niverobot.Consumer.csproj", "Niverobot.Consumer/"]
COPY ["Niverobot.Domain/Niverobot.Domain.csproj", "Niverobot.Domain/"]
COPY ["Niverobot.Interfaces/Niverobot.Interfaces.csproj", "Niverobot.Interfaces/"]
COPY ["Niverobot.Services/Niverobot.Services.csproj", "Niverobot.Services/"]
RUN dotnet restore "Niverobot.Consumer/Niverobot.Consumer.csproj"
COPY . .
WORKDIR "/src/Niverobot.Consumer"
RUN dotnet build "Niverobot.Consumer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Niverobot.Consumer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Niverobot.Consumer.dll"]