FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["src/Services/CustomerService/CustomerService.csproj", "src/Services/CustomerService/"]

RUN dotnet restore "src/Services/CustomerService/CustomerService.csproj"

COPY . .

WORKDIR "/src/src/Services/CustomerService"

RUN dotnet publish "CustomerService.csproj" -c Release -o /app/publish /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "CustomerService.dll"]