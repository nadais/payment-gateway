FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore PaymentGateway.Api/PaymentGateway.Api.csproj
RUN dotnet publish PaymentGateway.Api/PaymentGateway.Api.csproj -o /out -c Release --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /out .
ENTRYPOINT ["dotnet", "PaymentGateway.Api.dll"]
