#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["FortniteNotifier.Admin/FortniteNotifier.Admin.csproj", "FortniteNotifier.Admin/"]
COPY ["FortniteNotifier.Shared/FortniteNotifier.Shared.csproj", "FortniteNotifier.Shared/"]
RUN dotnet restore "FortniteNotifier.Admin/FortniteNotifier.Admin.csproj"
COPY . .
WORKDIR "/src/FortniteNotifier.Admin"
RUN dotnet build "FortniteNotifier.Admin.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FortniteNotifier.Admin.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FortniteNotifier.Admin.dll"]