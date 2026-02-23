FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
# Bu yerda papka nomi olib tashlandi
COPY ["Console.App1.csproj", "."]
RUN dotnet restore "./Console.App1.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./Console.App1.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Console.App1.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --with-build-system=publish /app/publish .
ENTRYPOINT ["dotnet", "Console.App1.dll"]
