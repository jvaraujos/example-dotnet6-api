#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Example.Api/Example.Api.csproj", "Example.Api/"]
COPY ["Example.Identity/Example.Identity.csproj", "Example.Identity/"]
COPY ["Example.Application/Example.Application.csproj", "Example.Application/"]
COPY ["Example.Domain/Example.Domain.csproj", "Example.Domain/"]
COPY ["Example.Persistence/Example.Persistence.csproj", "Example.Persistence/"]
RUN dotnet restore "Example.Api/Example.Api.csproj"
COPY . .
WORKDIR "/src/Example.Api"
RUN dotnet build "Example.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Example.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Example.Api.dll"]