FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /src
COPY ["WebCore/WebCore.csproj", "WebCore/"]
COPY ["RoadFlow.Business/RoadFlow.Business.csproj", "RoadFlow.Business/"]
COPY ["RoadFlow.Model/RoadFlow.Model.csproj", "RoadFlow.Model/"]
COPY ["RoadFlow.Utility/RoadFlow.Utility.csproj", "RoadFlow.Utility/"]
COPY ["RoadFlow.Data/RoadFlow.Data.csproj", "RoadFlow.Data/"]
COPY ["RoadFlow.Mapper/RoadFlow.Mapper.csproj", "RoadFlow.Mapper/"]
COPY ["LogUtility/LogUtility.csproj", "LogUtility/"]
COPY ["SecurityUtility/SecurityUtility.csproj", "SecurityUtility/"]
RUN dotnet restore "WebCore/WebCore.csproj"
COPY . .
WORKDIR "/src/WebCore"
RUN dotnet build "WebCore.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebCore.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebCore.dll"]