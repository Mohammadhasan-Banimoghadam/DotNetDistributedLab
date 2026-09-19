FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["src/Services/PaymentService/PaymentService.csproj", "src/Services/PaymentService/"]

RUN dotnet restore "src/Services/PaymentService/PaymentService.csproj"

COPY . .

WORKDIR "/src/src/Services/PaymentService"

RUN dotnet publish "PaymentService.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "PaymentService.dll"]