#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/ASPNetUserService.API/ASPNetUserService.API.csproj", "src/ASPNetUserService.API/"]
COPY ["src/ASPNetUserService.Infrastructure/ASPNetUserService.Infrastructure.csproj", "src/ASPNetUserService.Infrastructure/"]
COPY ["src/ASPNetUserService.Domain/ASPNetUserService.Domain.csproj", "src/ASPNetUserService.Domain/"]
RUN dotnet restore "src/ASPNetUserService.API/ASPNetUserService.API.csproj"
COPY . .
WORKDIR "/src/src/ASPNetUserService.API"
RUN dotnet build "ASPNetUserService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ASPNetUserService.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ASPNetUserService.API.dll"]