FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 9090
ENV ConnectionStrings:MagsConnectionMssql="Server=tcp:localhost; Database=ZhetistikDb;User ID=SA;Password=Effuone1990*;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
ENV DB_UserId="SA"
ENV ASPNETCORE_URLS=http://+:9090

# Creates a non-root user with an explicit UID and adds permission to access the /app folder
# For more info, please refer to https://aka.ms/vscode-docker-dotnet-configure-containers
RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src

COPY ["Zhetistik.Data/Zhetistik.Data.csproj", "Zhetistik.Data/"]
COPY ["Zhetistik.Core/Zhetistik.Core.csproj", "Zhetistik.Core/"]
COPY ["Zhetistik.Api/Zhetistik.Api.csproj", "Zhetistik.Api/"]
RUN dotnet restore "Zhetistik.Api/Zhetistik.Api.csproj"
COPY . .
WORKDIR "/src/Zhetistik.Data"
RUN dotnet build -c Release -o /app/build

WORKDIR "/src/Zhetistik.Core"
RUN dotnet build -c Release -o /app/build

WORKDIR "/src/Zhetistik.Api"
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Zhetistik.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Zhetistik.Api.dll"]
