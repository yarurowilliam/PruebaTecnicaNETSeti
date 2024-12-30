FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["PruebaTecnicaNET/PruebaTecnicaNET.csproj", "PruebaTecnicaNET/"]
COPY ["PruebaTecnicaNETTests/PruebaTecnicaNETTests.csproj", "PruebaTecnicaNETTests/"]

RUN dotnet restore "PruebaTecnicaNET/PruebaTecnicaNET.csproj"
RUN dotnet restore "PruebaTecnicaNETTests/PruebaTecnicaNETTests.csproj"

COPY . .

FROM build AS testing
WORKDIR /src
RUN dotnet test "PruebaTecnicaNETTests/PruebaTecnicaNETTests.csproj" --logger "console;verbosity=detailed"

FROM build AS publish
WORKDIR "/src/PruebaTecnicaNET"
RUN dotnet publish "PruebaTecnicaNET.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Imagen final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PruebaTecnicaNET.dll"]