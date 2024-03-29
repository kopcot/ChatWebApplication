#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine-composite AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine3.19 AS restorer
ARG BUILD_CONFIGURATION=Release
ARG BUILD_PLATFORM=linux-arm64
WORKDIR /src
RUN mkdir /src/Server
RUN mkdir /src/Server/ChatWebApplication.Server
RUN mkdir /src/Share
RUN mkdir /src/Share/ChatWebApplication.Share
COPY ["./Server/ChatWebApplication.Server/ChatWebApplication.Server.csproj", "/src/Server/ChatWebApplication.Server"]
COPY ["./Share/ChatWebApplication.Share/ChatWebApplication.Share.csproj", "/src/Share/ChatWebApplication.Share"]
RUN \
    --mount=type=cache,id=nugethttpcache,sharing=locked,target=/root/.local/share/NuGet/http-cache \
    --mount=type=cache,id=nugetglobalpackages,sharing=locked,target=/root/.nuget/packages/ \
    --mount=type=cache,id=nugettemp,sharing=locked,target=/Temp/NuGet \
    --mount=type=cache,id=nugetplugins-cache,sharing=locked,target=/root/.local/share/NuGet/plugin-cache \ 
    dotnet restore "/src/Server/ChatWebApplication.Server/ChatWebApplication.Server.csproj" -r $BUILD_PLATFORM 
    
FROM restorer AS build
COPY . .
WORKDIR "/src/Server/ChatWebApplication.Server"
RUN \
    --mount=type=cache,id=nugethttpcache,sharing=locked,target=/root/.local/share/NuGet/http-cache \
    --mount=type=cache,id=nugetglobalpackages,sharing=locked,target=/root/.nuget/packages/ \
    --mount=type=cache,id=nugettemp,sharing=locked,target=/Temp/NuGet \
    --mount=type=cache,id=nugetplugins-cache,sharing=locked,target=/root/.local/share/NuGet/plugin-cache \ 
    dotnet build "./ChatWebApplication.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build --runtime $BUILD_PLATFORM --self-contained false

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN \
    --mount=type=cache,id=nugethttpcache,sharing=locked,target=/root/.local/share/NuGet/http-cache \
    --mount=type=cache,id=nugetglobalpackages,sharing=locked,target=/root/.nuget/packages/ \
    --mount=type=cache,id=nugettemp,sharing=locked,target=/Temp/NuGet \
    --mount=type=cache,id=nugetplugins-cache,sharing=locked,target=/root/.local/share/NuGet/plugin-cache \ 
    dotnet publish "./ChatWebApplication.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish --runtime $BUILD_PLATFORM --self-contained false /p:UseAppHost=false 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatWebApplication.Server.dll"]