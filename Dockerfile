FROM mcr.microsoft.com/dotnet/aspnet AS base
WORKDIR /app
EXPOSE 1146
EXPOSE 44300

FROM mcr.microsoft.com/dotnet/sdk AS build
WORKDIR /src
COPY ["IdpDev.csproj", ""]
RUN dotnet restore
COPY . .
RUN dotnet build "IdpDev.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "IdpDev.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "IdpDev.dll"]
