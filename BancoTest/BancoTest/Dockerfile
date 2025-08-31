# Consulte https://aka.ms/customizecontainer para aprender a personalizar su contenedor de depuración y cómo Visual Studio usa este Dockerfile para compilar sus imágenes para una depuración más rápida.

# Esta fase se usa cuando se ejecuta desde VS en modo rápido (valor predeterminado para la configuración de depuración)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base


RUN apt-get update && apt-get install -y tzdata \
    && ln -fs /usr/share/zoneinfo/America/Guayaquil /etc/localtime \
    && dpkg-reconfigure -f noninteractive tzdata

ENV TZ=America/Guayaquil

USER $APP_UID
WORKDIR /app
EXPOSE 8080


# Esta fase se usa para compilar el proyecto de servicio
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["BancoTest/BancoTest.csproj", "BancoTest/"]
COPY ["Banco.Application/Banco.Application.csproj", "Banco.Application/"]
COPY ["Banco.Domain/Banco.Domain.csproj", "Banco.Domain/"]
COPY ["Shared/Shared.csproj", "Shared/"]
COPY ["Banco.Infrastructure/Banco.Infrastructure.csproj", "Banco.Infrastructure/"]
RUN dotnet restore "./BancoTest/BancoTest.csproj"
COPY . .
WORKDIR "/src/BancoTest"
RUN dotnet build "./BancoTest.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Esta fase se usa para publicar el proyecto de servicio que se copiará en la fase final.
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./BancoTest.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Esta fase se usa en producción o cuando se ejecuta desde VS en modo normal (valor predeterminado cuando no se usa la configuración de depuración)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BancoTest.dll"]