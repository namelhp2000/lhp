FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["DockerWeb/DockerWeb.csproj", "DockerWeb/"]
RUN dotnet restore "DockerWeb/DockerWeb.csproj"
COPY . .
WORKDIR "/src/DockerWeb"
RUN dotnet build "DockerWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DockerWeb.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockerWeb.dll"]