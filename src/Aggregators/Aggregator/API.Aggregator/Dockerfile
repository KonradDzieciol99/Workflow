#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/Aggregators/Aggregator/API.Aggregator/API.Aggregator.csproj", "src/Aggregators/Aggregator/API.Aggregator/"]
COPY ["src/Integration/HttpMessage/HttpMessage.csproj", "src/Integration/HttpMessage/"]
COPY ["src/Integration/Logging/Logging.csproj", "src/Integration/Logging/"]
RUN dotnet restore "src/Aggregators/Aggregator/API.Aggregator/API.Aggregator.csproj"
COPY . .
WORKDIR "/src/src/Aggregators/Aggregator/API.Aggregator"
RUN dotnet build "API.Aggregator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "API.Aggregator.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "API.Aggregator.dll"]