# Dockerfile para microsserviços FastTech Foods
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["AuthService/AuthService.csproj", "AuthService/"]
COPY ["MenuService/MenuService.csproj", "MenuService/"]
COPY ["KitchenService/KitchenService.csproj", "KitchenService/"]
COPY ["OrderService/OrderService.csproj", "OrderService/"]

# Restaurar dependências
RUN dotnet restore "AuthService/AuthService.csproj"
RUN dotnet restore "MenuService/MenuService.csproj"
RUN dotnet restore "KitchenService/KitchenService.csproj"
RUN dotnet restore "OrderService/OrderService.csproj"

# Copiar código fonte
COPY . .

# Build dos projetos
RUN dotnet build "AuthService/AuthService.csproj" -c Release -o /app/build
RUN dotnet build "MenuService/MenuService.csproj" -c Release -o /app/build
RUN dotnet build "KitchenService/KitchenService.csproj" -c Release -o /app/build
RUN dotnet build "OrderService/OrderService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthService/AuthService.csproj" -c Release -o /app/publish
RUN dotnet publish "MenuService/MenuService.csproj" -c Release -o /app/publish
RUN dotnet publish "KitchenService/KitchenService.csproj" -c Release -o /app/publish
RUN dotnet publish "OrderService/OrderService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AuthService.dll"] 