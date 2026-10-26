# ---- build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Florarium.slnx ./
COPY src/Florarium.Domain/Florarium.Domain.csproj src/Florarium.Domain/
COPY src/Florarium.Data/Florarium.Data.csproj     src/Florarium.Data/
COPY src/Florarium.Api/Florarium.Api.csproj       src/Florarium.Api/

RUN dotnet restore Florarium.slnx

COPY . .
RUN dotnet publish Florarium.slnx \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false

# ---- runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Development
EXPOSE 8080

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Florarium.Api.dll"]