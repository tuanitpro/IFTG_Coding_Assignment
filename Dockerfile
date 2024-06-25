FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything
COPY . ./

# Build and publish a release
RUN dotnet restore

RUN dotnet publish SettlementBookingSystem/SettlementBookingSystem.csproj -c Release -o out --no-restore -p:UseAppHost=false

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "SettlementBookingSystem.dll"]
EXPOSE 5001