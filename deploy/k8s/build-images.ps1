# Script para build das imagens Docker dos serviços FastTech Foods
# Execute este script como Administrador

Write-Host "=== FastTech Foods - Build de Imagens Docker ===" -ForegroundColor Green

# Verificar se Docker está rodando
if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    Write-Host "Docker não encontrado. Por favor, instale o Docker Desktop primeiro." -ForegroundColor Red
    exit 1
}

# Verificar se Docker está rodando
docker version
if ($LASTEXITCODE -ne 0) {
    Write-Host "Docker não está rodando. Por favor, inicie o Docker Desktop." -ForegroundColor Red
    exit 1
}

# Diretório raiz do projeto
$projectRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)

Write-Host "Diretório do projeto: $projectRoot" -ForegroundColor Yellow

# Build AuthService
Write-Host "`nBuildando AuthService..." -ForegroundColor Yellow
Set-Location "$projectRoot\AuthService"
docker build -t fasttech/authservice:latest -t gustavoomacedo/authservice:latest .

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro no build do AuthService" -ForegroundColor Red
    exit 1
}
Write-Host "Fazendo push da imagem gustavoomacedo/authservice:latest..." -ForegroundColor Yellow
docker push gustavoomacedo/authservice:latest

# Build KitchenService
Write-Host "`nBuildando KitchenService..." -ForegroundColor Yellow
Set-Location "$projectRoot\KitchenService"
docker build -t fasttech/kitchenservice:latest -t gustavoomacedo/kitchenservice:latest .

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro no build do KitchenService" -ForegroundColor Red
    exit 1
}
Write-Host "Fazendo push da imagem gustavoomacedo/kitchenservice:latest..." -ForegroundColor Yellow
docker push gustavoomacedo/kitchenservice:latest

# Build MenuService
Write-Host "`nBuildando MenuService..." -ForegroundColor Yellow
Set-Location "$projectRoot\MenuService"
docker build -t fasttech/menuservice:latest -t gustavoomacedo/menuservice:latest .

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro no build do MenuService" -ForegroundColor Red
    exit 1
}
Write-Host "Fazendo push da imagem gustavoomacedo/menuservice:latest..." -ForegroundColor Yellow
docker push gustavoomacedo/menuservice:latest

# Build OrderService
Write-Host "`nBuildando OrderService..." -ForegroundColor Yellow
Set-Location "$projectRoot\OrderService"
docker build -t fasttech/orderservice:latest -t gustavoomacedo/orderservice:latest .

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro no build do OrderService" -ForegroundColor Red
    exit 1
}
Write-Host "Fazendo push da imagem gustavoomacedo/orderservice:latest..." -ForegroundColor Yellow
docker push gustavoomacedo/orderservice:latest

# Voltar ao diretório original
Set-Location $PSScriptRoot

# Listar imagens criadas
Write-Host "`n=== Imagens Criadas ===" -ForegroundColor Green
docker images | Select-String "gustavoomacedo"

Write-Host "`n=== Build Concluído! ===" -ForegroundColor Green
Write-Host "Agora você pode executar o deployment com: .\deploy-windows.ps1" -ForegroundColor Yellow 