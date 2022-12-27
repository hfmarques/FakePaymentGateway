FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["FakePaymentGateway.Api/FakePaymentGateway.Api.csproj", "FakePaymentGateway.Api/"]
COPY ["FakePaymentGateway.Adapters/FakePaymentGateway.Adapters.csproj", "FakePaymentGateway.Adapters/"]
COPY ["FakePaymentGateway.Core/FakePaymentGateway.Core.csproj", "FakePaymentGateway.Core/"]
RUN dotnet restore "FakePaymentGateway.Api/FakePaymentGateway.Api.csproj"
COPY . .
WORKDIR "/src/FakePaymentGateway.Api"
RUN dotnet build "FakePaymentGateway.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "FakePaymentGateway.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "FakePaymentGateway.Api.dll"]
