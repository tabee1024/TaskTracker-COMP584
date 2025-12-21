# ---------- Build Angular ----------
FROM node:20-bookworm AS ngbuild
WORKDIR /src/TaskTracker.Client

COPY TaskTracker.Client/package*.json ./
RUN npm ci

COPY TaskTracker.Client/ ./
RUN npx ng build --configuration production --project "TaskTracker.Client"

# ---------- Build .NET ----------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dotnetbuild
WORKDIR /src

COPY TaskTracker.Api/TaskTracker.Api.csproj TaskTracker.Api/
RUN dotnet restore TaskTracker.Api/TaskTracker.Api.csproj

COPY TaskTracker.Api/ TaskTracker.Api/

# Copy Angular output into API wwwroot (Angular outputs to dist/.../browser)
RUN rm -rf TaskTracker.Api/wwwroot/*
COPY --from=ngbuild /src/TaskTracker.Client/dist/TaskTracker.Client/browser/ TaskTracker.Api/wwwroot/

RUN dotnet publish TaskTracker.Api/TaskTracker.Api.csproj -c Release -o /app/publish

# ---------- Runtime ----------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=dotnetbuild /app/publish ./

# Render provides PORT env var
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

EXPOSE 10000

CMD ["dotnet", "TaskTracker.Api.dll"]
