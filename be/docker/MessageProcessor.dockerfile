FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

COPY  ../be/MessageProcessor/ ./
RUN dotnet restore ./MessageProcessor.csproj

RUN dotnet publish ./MessageProcessor.csproj -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/out .

ENTRYPOINT ["dotnet", "MessageProcessor.dll"]
