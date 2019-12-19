FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["RPedretti.Grpc.Server/RPedretti.Grpc.Server.csproj", "RPedretti.Grpc.Server/"]
COPY ["RPedretti.Grpc.Shared/RPedretti.Grpc.Shared.csproj", "RPedretti.Grpc.Shared/"]
COPY ["RPedretti.Grpc.DAL/RPedretti.Grpc.DAL.csproj", "RPedretti.Grpc.DAL/"]
RUN dotnet restore "RPedretti.Grpc.Server/RPedretti.Grpc.Server.csproj"
RUN dotnet restore "RPedretti.Grpc.Shared/RPedretti.Grpc.Shared.csproj"
COPY . .
WORKDIR "/src/RPedretti.Grpc.Server"
RUN dotnet build "RPedretti.Grpc.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RPedretti.Grpc.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RPedretti.Grpc.Server.dll"]
