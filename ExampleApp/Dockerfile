FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Phala.DStack.ExampleApp/Phala.DStack.ExampleApp.csproj", "Phala.DStack.ExampleApp/"]
RUN dotnet restore "Phala.DStack.ExampleApp/Phala.DStack.ExampleApp.csproj"
COPY . .
WORKDIR "/src/Phala.DStack.ExampleApp"
RUN dotnet build "Phala.DStack.ExampleApp.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Phala.DStack.ExampleApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV DSTACK_SIMULATOR_ENDPOINT="http://host.docker.internal:8090"
ENV ASPNETCORE_ENVIRONMENT=Development
ENTRYPOINT ["dotnet", "Phala.DStack.ExampleApp.dll"]
