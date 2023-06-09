FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MeowLib.WebApi/MeowLib.WebApi.csproj", "MeowLib.WebApi/"]
COPY ["MeowLib.Domain/MeowLib.Domain.csproj", "MeowLib.Domain/"]
COPY ["MeowLib.WebApi.DAL/MeowLib.WebApi.DAL.csproj", "MeowLib.WebApi.DAL/"]
COPY ["MeowLIb.WebApi.Services/MeowLIb.WebApi.Services.csproj", "MeowLIb.WebApi.Services/"]
RUN dotnet restore "MeowLib.WebApi/MeowLib.WebApi.csproj"
COPY . .
WORKDIR "/src/MeowLib.WebApi"
RUN dotnet build "MeowLib.WebApi.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MeowLib.WebApi.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MeowLib.WebApi.dll"]
