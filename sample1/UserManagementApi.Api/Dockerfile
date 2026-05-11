FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["UserManagementApi.Api/UserManagementApi.Api.csproj", "UserManagementApi.Api/"]
COPY ["UserManagementApi.Application/UserManagementApi.Application.csproj", "UserManagementApi.Application/"]
COPY ["UserManagementApi.Domain/UserManagementApi.Domain.csproj", "UserManagementApi.Domain/"]
COPY ["UserManagementApi.Infrastructure/UserManagementApi.Infrastructure.csproj", "UserManagementApi.Infrastructure/"]
RUN dotnet restore "UserManagementApi.Api/UserManagementApi.Api.csproj"
COPY . .
WORKDIR "/src/UserManagementApi.Api"
RUN dotnet build "UserManagementApi.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "UserManagementApi.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "UserManagementApi.Api.dll"]
