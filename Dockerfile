FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Loyiha faylini ko'chirish
COPY ["Console.App1.csproj", "."]
RUN dotnet restore "./Console.App1.csproj"

# Qolgan barcha fayllarni ko'chirish
COPY . .
RUN dotnet build "Console.App1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Console.App1.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Render botni o'chirib qo'ymasligi uchun portni simulyatsiya qilamiz
ENV PORT=10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "Console.App1.dll"]
