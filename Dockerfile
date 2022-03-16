FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal AS base
WORKDIR /app
EXPOSE 9090
EXPOSE 8080

ENV ASPNETCORE_URLS=http://+:9090
ENV ConnectionString="Server=tcp:localhost;Database=ZhetistikDb;User Id=hbuser;Password=hbuser1029"
ENV MailPassword="rustcohle2022*"
ENV JwtKey="eyJhbGciOiJIUzI1NiJ9.eyJSb2xlIjoiQWRtaW4iLCJJc3N1ZXIiOiJJc3N1ZXIiLCJVc2VybmFtZSI6IkphdmFJblVzZSIsImV4cCI6MTY0NjQ4Mzk1MCwiaWF0IjoxNjQ2NDgzOTUwfQ.3fD1MslLNvdiN5Q3Vy-RSmO6AxeoIEZE7f2qA1VP6x8"


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
