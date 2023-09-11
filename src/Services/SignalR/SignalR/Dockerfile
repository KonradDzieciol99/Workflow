#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Services/SignalR/SignalR/SignalR.csproj", "src/Services/SignalR/SignalR/"]
COPY ["src/Integration/Logging/Logging.csproj", "src/Integration/Logging/"]
COPY ["src/Integration/MessageBus/MessageBus.csproj", "src/Integration/MessageBus/"]
RUN dotnet restore "src/Services/SignalR/SignalR/SignalR.csproj"
COPY . .
WORKDIR "/src/src/Services/SignalR/SignalR"
RUN dotnet build "SignalR.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SignalR.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SignalR.dll"]