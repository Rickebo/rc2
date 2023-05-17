FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["rc2/rc2.csproj", "rc2/"]
RUN dotnet restore "rc2/rc2.csproj"
COPY . .
WORKDIR "/src/rc2"
RUN dotnet build "rc2.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "rc2.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY rc2/nginx.conf /etc/nginx/nginx.conf
