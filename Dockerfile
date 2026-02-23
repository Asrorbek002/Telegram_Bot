# QURILISH BOSQICHI (Build)
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Loyiha faylini ko'chirish
COPY ["Console.App1.csproj", "."]
RUN dotnet restore "./Console.App1.csproj"

# Qolgan barcha fayllarni ko'chirish
COPY . .
RUN dotnet build "Console.App1.csproj" -c Release -o /app/build

# NASHR QILISH (Publish)
FROM build AS publish
RUN dotnet publish "Console.App1.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ISHGA TUSHIRISH BOSQICHI (Final) - Aynan shu yerda 9.0 versiya qo'yildi
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Render botni o'chirib qo'ymasligi uchun port simulyatsiyasi
ENV PORT=10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "Console.App1.dll"]
