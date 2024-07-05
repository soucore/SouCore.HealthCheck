#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["examples/SampleWorker/SampleWorker.csproj", "examples/SampleWorker/"]
RUN dotnet restore "examples/SampleWorker/SampleWorker.csproj"
COPY . .
WORKDIR "/src/examples/SampleWorker"
RUN dotnet build "SampleWorker.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SampleWorker.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SampleWorker.dll"]