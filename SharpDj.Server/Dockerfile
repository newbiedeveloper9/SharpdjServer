FROM mcr.microsoft.com/dotnet/runtime:5.0-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0-buster-slim AS build
WORKDIR /src
COPY ["SharpDj.Server/appsettings.json", "SharpDj.Server/"] 
COPY ["SharpDj.Server/SharpDj.Server.csproj", "SharpDj.Server/"]
COPY ["SharpDj.Common/SharpDj.Common.csproj", "SharpDj.Common/"]
COPY ["SharpDj.Infrastructure/SharpDj.Infrastructure.csproj", "SharpDj.Infrastructure/"]
COPY ["SharpDj.Domain/SharpDj.Domain.csproj", "SharpDj.Domain/"]
RUN dotnet restore "SharpDj.Server/SharpDj.Server.csproj"
COPY . .
WORKDIR "/src/SharpDj.Server"

RUN dotnet build "SharpDj.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SharpDj.Server.csproj" -c Release -o /app/publish

#COPY ./volumes/data  /var/opt/mssql/data

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

EXPOSE 5666

ENTRYPOINT ["dotnet", "SharpDj.Server.dll"]