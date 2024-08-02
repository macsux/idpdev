FROM mcr.microsoft.com/dotnet/aspnet AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk AS build
WORKDIR /src
COPY ["IdpDev.csproj", "/src"]
RUN dotnet restore
COPY . .
RUN dotnet publish "IdpDev.csproj" -c Release -o /app

#FROM build AS publish
#RUN dotnet publish "IdpDev.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_ENVIRONMENT=Development
CMD ["dotnet", "IdpDev.dll"]

EXPOSE 8080
