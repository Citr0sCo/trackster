FROM node:22 AS angular-build

WORKDIR /web-gui
COPY ./ .

RUN npm install --legacy-peer-deps
RUN npm install -g @angular/cli@19.2.5
RUN npm run build

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /web-api/app
EXPOSE 82

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /web-api/src

COPY ["/api/Trackster.Api/Trackster.Api.csproj", "api/"]

RUN dotnet restore "api/Trackster.Api.csproj"

WORKDIR "/web-api/src/api"
COPY . .

RUN rm -rf /web-api/src/api/**/obj /web-api/src/api/**/bin

RUN dotnet build "Trackster.Api.csproj" -c Release -o /web-api/app/build

FROM build AS publish
RUN dotnet publish "Trackster.Api.csproj" -c Release -o /web-api/app/publish

FROM base AS final
WORKDIR /web-api/app

COPY --from=publish /web-api/app/publish .

COPY --from=angular-build /web-gui/dist/trackster/browser /web-api/app/wwwroot

CMD ["dotnet", "Trackster.Api.dll"]