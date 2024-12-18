FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY  ../be/EdgeDevice/ ./
COPY  ../be/Contracts/ ./
RUN dotnet restore ./EdgeDevice.csproj

RUN dotnet publish ./EdgeDevice.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "EdgeDevice.dll"]
