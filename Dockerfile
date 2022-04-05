FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 9090
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /src
COPY ["Zhetistik.Data/Zhetistik.Data.csproj", "Zhetistik.Data/"]
COPY ["Zhetistik.Core/Zhetistik.Core.csproj", "Zhetistik.Core/"]
COPY ["Zhetistik.Api/Zhetistik.Api.csproj", "Zhetistik.Api/"]

WORKDIR /src
RUN dotnet user-secrets set ConnectionStrings:ConnectionString
RUN dotnet restore "Zhetistik.Api/Zhetistik.Api.csproj"
COPY . .
WORKDIR "/src/Zhetistik.Api"
RUN dotnet build "Zhetistik.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Zhetistik.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Zhetistik.Api.dll"]
