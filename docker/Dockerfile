FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

COPY . .

RUN dotnet restore src/API/API.csproj
RUN dotnet publish src/API/API.csproj -c Release -o /out 

FROM mcr.microsoft.com/dotnet/aspnet:10.0

# ffmpeg
RUN apt-get update && \
    apt-get install -y ffmpeg && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/*

WORKDIR /app

COPY --from=build /out .

ENTRYPOINT ["dotnet", "API.dll"]