#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Services/Tasks/Tasks/Tasks.csproj", "src/Services/Tasks/Tasks/"]
COPY ["src/Integration/Logging/Logging.csproj", "src/Integration/Logging/"]
COPY ["src/Integration/MessageBus/MessageBus.csproj", "src/Integration/MessageBus/"]
RUN dotnet restore "src/Services/Tasks/Tasks/Tasks.csproj"
COPY . .
WORKDIR "/src/src/Services/Tasks/Tasks"
RUN dotnet build "Tasks.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Tasks.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Tasks.dll"]