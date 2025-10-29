#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["src/Directory.Packages.props", "src/"]
COPY ["src/DbInteractions/DbInteractions.csproj", "src/DbInteractions/"]
COPY ["src/FileStorage/FileStorage.csproj", "src/FileStorage/"]
COPY ["src/Messaging/Messaging.csproj", "src/Messaging/"]
RUN dotnet restore "src/DbInteractions/DbInteractions.csproj"
COPY . .
WORKDIR "/src/src/DbInteractions"
RUN dotnet build "DbInteractions.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DbInteractions.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DbInteractions.dll"]