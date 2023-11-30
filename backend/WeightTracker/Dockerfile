#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["WeightTracker.csproj", "."]
RUN dotnet restore "./WeightTracker.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "WeightTracker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WeightTracker.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WeightTracker.dll"]