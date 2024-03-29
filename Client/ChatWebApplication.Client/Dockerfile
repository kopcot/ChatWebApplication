#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine3.19 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0-alpine3.19 AS restorer
ARG BUILD_CONFIGURATION=Release
ARG BUILD_PLATFORM=linux-arm64
WORKDIR /src
RUN mkdir /src/Client
RUN mkdir /src/Client/ChatWebApplication.Client
RUN mkdir /src/Share
RUN mkdir /src/Share/ChatWebApplication.Share
COPY ["./Client/ChatWebApplication.Client/ChatWebApplication.Client.csproj", "/src/Client/ChatWebApplication.Client"]
COPY ["./Share/ChatWebApplication.Share/ChatWebApplication.Share.csproj", "/src/Share/ChatWebApplication.Share"]
RUN \
    --mount=type=cache,id=nugethttpcache,sharing=locked,target=/root/.local/share/NuGet/http-cache \
    --mount=type=cache,id=nugetglobalpackages,sharing=locked,target=/root/.nuget/packages/ \
    --mount=type=cache,id=nugettemp,sharing=locked,target=/Temp/NuGet \
    --mount=type=cache,id=nugetplugins-cache,sharing=locked,target=/root/.local/share/NuGet/plugin-cache \
    dotnet restore "./Client/ChatWebApplication.Client/ChatWebApplication.Client.csproj" -r $BUILD_PLATFORM 

FROM restorer AS build
COPY . .
WORKDIR "/src/Client/ChatWebApplication.Client"
RUN \
    --mount=type=cache,id=nugethttpcache,sharing=locked,target=/root/.local/share/NuGet/http-cache \
    --mount=type=cache,id=nugetglobalpackages,sharing=locked,target=/root/.nuget/packages/ \
    --mount=type=cache,id=nugettemp,sharing=locked,target=/Temp/NuGet \
    --mount=type=cache,id=nugetplugins-cache,sharing=locked,target=/root/.local/share/NuGet/plugin-cache \ 
    dotnet build "./ChatWebApplication.Client.csproj" -c $BUILD_CONFIGURATION -o /app/build --runtime $BUILD_PLATFORM --self-contained false

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN \
    --mount=type=cache,id=nugethttpcache,sharing=locked,target=/root/.local/share/NuGet/http-cache \
    --mount=type=cache,id=nugetglobalpackages,sharing=locked,target=/root/.nuget/packages/ \
    --mount=type=cache,id=nugettemp,sharing=locked,target=/Temp/NuGet \
    --mount=type=cache,id=nugetplugins-cache,sharing=locked,target=/root/.local/share/NuGet/plugin-cache \ 
    dotnet publish "./ChatWebApplication.Client.csproj" -c $BUILD_CONFIGURATION -o /app/publish --runtime $BUILD_PLATFORM --self-contained false /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ChatWebApplication.Client.dll"]