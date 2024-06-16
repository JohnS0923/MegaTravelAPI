#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MegaTravelAPI/MegaTravelAPI.csproj", "MegaTravelAPI/"]
RUN dotnet restore "MegaTravelAPI/MegaTravelAPI.csproj"
COPY . .
WORKDIR "/src/MegaTravelAPI"
RUN dotnet build "MegaTravelAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MegaTravelAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MegaTravelAPI.dll"]