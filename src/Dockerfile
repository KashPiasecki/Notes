FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Notes.Api/Notes.Api.csproj", "Notes.Api/"]
COPY ["src/Notes.Infrastructure/Notes.Infrastructure.csproj", "Notes.Infrastructure/"]
COPY ["src/Notes.Domain/Notes.Domain.csproj", "Notes.Domain/"]
COPY ["src/Notes.Application/Notes.Application.csproj", "Notes.Application/"]
RUN dotnet restore "src/Notes.Api/Notes.Api.csproj"
COPY . .
WORKDIR "src/Notes.Api"
RUN dotnet build "src/Notes.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/Notes.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notes.Api.dll"]
