FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://*80
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["McgTgBotNet/McgTgBotNet.csproj", "McgTgBotNet/"]
COPY ["ReportBot/ReportBot.csproj", "ReportBot/"]
COPY ["ReportBot.Hangfire/ReportBot.Hangfire.csproj", "ReportBot.Hangfire/"]
COPY ["ReportBot.Services/ReportBot.Services.csproj", "ReportBot.Services/"]
COPY ["ReportBot.Common/ReportBot.Common.csproj", "ReportBot.Common/"]
COPY ["ReportBot.DataBase/ReportBot.DataBase.csproj", "ReportBot.DataBase/"]
RUN dotnet restore "./ReportBot/ReportBot.csproj"
COPY . .
WORKDIR "/src/ReportBot"
RUN dotnet build "./ReportBot.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./ReportBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ReportBot.dll"]