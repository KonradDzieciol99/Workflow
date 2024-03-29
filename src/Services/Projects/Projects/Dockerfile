#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Services/Projects/Projects/Projects.csproj", "src/Services/Projects/Projects/"]
COPY ["src/Integration/Logging/Logging.csproj", "src/Integration/Logging/"]
COPY ["src/Services/Projects/Projects.Application/Projects.Application.csproj", "src/Services/Projects/Projects.Application/"]
COPY ["src/Integration/MessageBus/MessageBus.csproj", "src/Integration/MessageBus/"]
COPY ["src/Services/Projects/Projects.Domain/Projects.Domain.csproj", "src/Services/Projects/Projects.Domain/"]
COPY ["src/Services/Projects/Projects.Infrastructure/Projects.Infrastructure.csproj", "src/Services/Projects/Projects.Infrastructure/"]
RUN dotnet restore "src/Services/Projects/Projects/Projects.csproj"
COPY . .
WORKDIR "/src/src/Services/Projects/Projects"
RUN dotnet build "Projects.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Projects.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Projects.dll"]