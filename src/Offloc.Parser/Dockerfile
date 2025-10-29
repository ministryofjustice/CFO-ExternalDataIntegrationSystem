#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Offloc.Parser/Offloc.Parser.csproj", "src/Offloc.Parser/"]
COPY ["src/Messaging/Messaging.csproj", "src/Messaging/"]

RUN dotnet restore "src/Offloc.Parser/Offloc.Parser.csproj"
COPY . .
WORKDIR "/src/src/Offloc.Parser"
RUN dotnet build "Offloc.Parser.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Offloc.Parser.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Offloc.Parser.dll"]