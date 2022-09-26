FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Notes.Api/Notes.Api.csproj", "Notes.Api/"]
COPY ["Notes.Infrastructure/Notes.Infrastructure.csproj", "Notes.Infrastructure/"]
COPY ["Notes.Domain/Notes.Domain.csproj", "Notes.Domain/"]
COPY ["Notes.Application/Notes.Application.csproj", "Notes.Application/"]
RUN dotnet restore "Notes.Api/Notes.Api.csproj"
COPY . .
WORKDIR "Notes.Api"
RUN dotnet build "Notes.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Notes.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Notes.Api.dll"]
