From mcr.microsoft.com/dotnet/aspnet:7.0 AS Base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

From mcr.microsoft.com/dotnet/sdk:7.0 as build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["API/API.csproj", "API/"]
COPY ["Infrastructure/Infrastructure.csproj","Infrastructure/"]
COPY ["Entity/Entity.csproj","Entity/"]
RUN dotnet restore "API/API.csproj"

COPY . .

WORKDIR "/src/API"

RUN dotnet build "API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build as publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "API.csproj" -c $BUILD_CONFIGURATION -o /app/publish 

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT [ "dotnet","API.dll" ]
