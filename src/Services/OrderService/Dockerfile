FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["src/Services/OrderService/OrderService.csproj", "src/Services/OrderService/"]

RUN dotnet restore "src/Services/OrderService/OrderService.csproj"

COPY . .

WORKDIR "/src/src/Services/OrderService"

RUN dotnet publish "OrderService.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "OrderService.dll"]