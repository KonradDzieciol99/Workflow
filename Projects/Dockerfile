#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Projects/Projects.csproj", "Projects/"]
COPY ["Projects.Application/Projects.Application.csproj", "Projects.Application/"]
COPY ["MessageBus/MessageBus.csproj", "MessageBus/"]
COPY ["Projects.Domain/Projects.Domain.csproj", "Projects.Domain/"]
COPY ["Projects.Infrastructure/Projects.Infrastructure.csproj", "Projects.Infrastructure/"]
RUN dotnet restore "Projects/Projects.csproj"
COPY . .
WORKDIR "/src/Projects"
RUN dotnet build "Projects.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Projects.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Projects.dll"]