FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ExampleApp/ExampleApp.csproj", "ExampleApp/"]
RUN dotnet restore "ExampleApp/ExampleApp.csproj"
COPY . .
WORKDIR "/src/ExampleApp"
RUN dotnet build "ExampleApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ExampleApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV DSTACK_SIMULATOR_ENDPOINT="http://host.docker.internal:8090"
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "ExampleApp.dll"]
