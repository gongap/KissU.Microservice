# See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
# You can build it with the following CLI command:
# docker build -t kissustage:linux-latest .

FROM mcr.microsoft.com/dotnet/aspnet:5.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

# It's important to keep lines from here down to "COPY . ." identical in all Dockerfiles
# to take advantage of Docker's build cache, to speed up local container builds
COPY "KissU.Microservice.sln" "KissU.Microservice.sln"

COPY "modules/account/src/KissU.Modules.Account.Application.Contracts/KissU.Modules.Account.Application.Contracts.csproj" "modules/account/src/KissU.Modules.Account.Application.Contracts/KissU.Modules.Account.Application.Contracts.csproj"
COPY "modules/account/src/KissU.Modules.Account.Application/KissU.Modules.Account.Application.csproj" "modules/account/src/KissU.Modules.Account.Application/KissU.Modules.Account.Application.csproj"
COPY "modules/account/src/KissU.Modules.Account.Service.Contracts/KissU.Modules.Account.Service.Contracts.csproj" "modules/account/src/KissU.Modules.Account.Service.Contracts/KissU.Modules.Account.Service.Contracts.csproj"
COPY "modules/account/src/KissU.Modules.Account.Service/KissU.Modules.Account.Service.csproj" "modules/account/src/KissU.Modules.Account.Service/KissU.Modules.Account.Service.csproj"
COPY "modules/common/src/KissU.Modules.Common.Service.Contracts/KissU.Modules.Common.Service.Contracts.csproj" "modules/common/src/KissU.Modules.Common.Service.Contracts/KissU.Modules.Common.Service.Contracts.csproj"
COPY "modules/common/src/KissU.Modules.Common.Service/KissU.Modules.Common.Service.csproj" "modules/common/src/KissU.Modules.Common.Service/KissU.Modules.Common.Service.csproj"
COPY "modules/identity/src/KissU.Modules.Identity.Application.Contracts/KissU.Modules.Identity.Application.Contracts.csproj" "modules/identity/src/KissU.Modules.Identity.Application.Contracts/KissU.Modules.Identity.Application.Contracts.csproj"
COPY "modules/identity/src/KissU.Modules.Identity.Application/KissU.Modules.Identity.Application.csproj" "modules/identity/src/KissU.Modules.Identity.Application/KissU.Modules.Identity.Application.csproj"
COPY "modules/identity/src/KissU.Modules.Identity.DbMigrator/KissU.Modules.Identity.DbMigrator.csproj" "modules/identity/src/KissU.Modules.Identity.DbMigrator/KissU.Modules.Identity.DbMigrator.csproj"
COPY "modules/identity/src/KissU.Modules.Identity.Service.Contracts/KissU.Modules.Identity.Service.Contracts.csproj" "modules/identity/src/KissU.Modules.Identity.Service.Contracts/KissU.Modules.Identity.Service.Contracts.csproj"
COPY "modules/identity/src/KissU.Modules.Identity.Service/KissU.Modules.Identity.Service.csproj" "modules/identity/src/KissU.Modules.Identity.Service/KissU.Modules.Identity.Service.csproj"
COPY "services/KissU.Services.Client/KissU.Services.Client.csproj" "services/KissU.Services.Client/KissU.Services.Client.csproj"
COPY "services/KissU.Services.Host/KissU.Services.Host.csproj" "services/KissU.Services.Host/KissU.Services.Host.csproj"
COPY "services/KissU.Services.Stage/KissU.Services.Stage.csproj" "services/KissU.Services.Stage/KissU.Services.Stage.csproj"

COPY "docker-compose.dcproj" "docker-compose.dcproj"

COPY "NuGet.config" "NuGet.config"

RUN dotnet restore "KissU.Microservice.sln"

COPY . .
WORKDIR /src/services/KissU.Services.Stage
RUN dotnet publish --no-restore -c Release -o /app

FROM build AS publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "KissU.Services.Stage.dll"]