# Script de Limpeza para FastTech Foods no Kubernetes
# Execute este script como Administrador

Write-Host "=== FastTech Foods - Limpeza do Ambiente ===" -ForegroundColor Green

# Confirmar limpeza
$confirmation = Read-Host "Tem certeza que deseja remover todos os recursos do FastTech Foods? (s/N)"
if ($confirmation -ne "s" -and $confirmation -ne "S") {
    Write-Host "Operação cancelada." -ForegroundColor Yellow
    exit 0
}

# Verificar se kubectl está instalado
if (-not (Get-Command kubectl -ErrorAction SilentlyContinue)) {
    Write-Host "kubectl não encontrado. Por favor, instale o kubectl primeiro." -ForegroundColor Red
    exit 1
}

# Verificar se o cluster está acessível
Write-Host "Verificando conexão com o cluster..." -ForegroundColor Yellow
kubectl cluster-info

if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro ao conectar com o cluster Kubernetes. Verifique se o cluster está rodando." -ForegroundColor Red
    exit 1
}

# Remover deployments
Write-Host "Removendo deployments..." -ForegroundColor Yellow
kubectl delete deployment authservice -n fasttech-foods --ignore-not-found=true
kubectl delete deployment kitchenservice -n fasttech-foods --ignore-not-found=true
kubectl delete deployment menuservice -n fasttech-foods --ignore-not-found=true
kubectl delete deployment orderservice -n fasttech-foods --ignore-not-found=true
kubectl delete deployment mongodb -n fasttech-foods --ignore-not-found=true
kubectl delete deployment rabbitmq -n fasttech-foods --ignore-not-found=true

# Remover services
Write-Host "Removendo services..." -ForegroundColor Yellow
kubectl delete service authservice-service -n fasttech-foods --ignore-not-found=true
kubectl delete service kitchenservice-service -n fasttech-foods --ignore-not-found=true
kubectl delete service menuservice-service -n fasttech-foods --ignore-not-found=true
kubectl delete service orderservice-service -n fasttech-foods --ignore-not-found=true
kubectl delete service mongodb-service -n fasttech-foods --ignore-not-found=true
kubectl delete service rabbitmq -n fasttech-foods --ignore-not-found=true



# Remover secrets
Write-Host "Removendo secrets..." -ForegroundColor Yellow
kubectl delete secret mongodb-secret -n fasttech-foods --ignore-not-found=true
kubectl delete secret jwt-secret -n fasttech-foods --ignore-not-found=true

# Remover namespace fasttech-foods
Write-Host "Removendo namespace fasttech-foods..." -ForegroundColor Yellow
kubectl delete namespace fasttech-foods --ignore-not-found=true



# Aguardar remoção completa
Write-Host "Aguardando remoção completa dos recursos..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Verificar se ainda existem recursos
Write-Host "`n=== Verificação Final ===" -ForegroundColor Green
$namespaces = kubectl get namespaces --no-headers -o custom-columns=":metadata.name" | Select-String "fasttech-foods"
if ($namespaces) {
    Write-Host "Ainda existe o namespace: $namespaces" -ForegroundColor Yellow
    Write-Host "Execute manualmente: kubectl delete namespace fasttech-foods --force --grace-period=0" -ForegroundColor Yellow
} else {
    Write-Host "Todos os namespaces foram removidos com sucesso!" -ForegroundColor Green
}

# Limpar imagens Docker (opcional)
$cleanImages = Read-Host "Deseja também remover as imagens Docker do FastTech Foods? (s/N)"
if ($cleanImages -eq "s" -or $cleanImages -eq "S") {
    Write-Host "Removendo imagens Docker..." -ForegroundColor Yellow
    docker rmi fasttech/authservice:latest --force 2>$null
    docker rmi fasttech/kitchenservice:latest --force 2>$null
    docker rmi fasttech/menuservice:latest --force 2>$null
    docker rmi fasttech/orderservice:latest --force 2>$null
    docker rmi rabbitmq:3-management --force 2>$null
    docker rmi mongo:6.0 --force 2>$null
    Write-Host "Imagens Docker removidas!" -ForegroundColor Green
}

Write-Host "`n=== Limpeza Concluída! ===" -ForegroundColor Green
Write-Host "Todos os recursos do FastTech Foods foram removidos do cluster Kubernetes." -ForegroundColor Yellow 