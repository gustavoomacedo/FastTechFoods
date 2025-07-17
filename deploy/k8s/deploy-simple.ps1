# Script de Deployment Simples para FastTech Foods (sem Ingress)
# Execute este script como Administrador

Write-Host "=== FastTech Foods - Deployment Simples (sem Ingress) ===" -ForegroundColor Green

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

# Aplicar configuração simples
Write-Host "Aplicando configuração simples..." -ForegroundColor Yellow
kubectl apply -f simple-deployment.yaml

# Aguardar todos os pods estarem prontos
Write-Host "Aguardando todos os serviços estarem prontos..." -ForegroundColor Yellow
kubectl wait --for=condition=ready pod -l app=authservice -n fasttech-foods --timeout=300s
kubectl wait --for=condition=ready pod -l app=kitchenservice -n fasttech-foods --timeout=300s
kubectl wait --for=condition=ready pod -l app=menuservice -n fasttech-foods --timeout=300s
kubectl wait --for=condition=ready pod -l app=orderservice -n fasttech-foods --timeout=300s
kubectl wait --for=condition=ready pod -l app=mongodb -n fasttech-foods --timeout=300s

# Mostrar status dos pods
Write-Host "`n=== Status dos Pods ===" -ForegroundColor Green
kubectl get pods -n fasttech-foods

Write-Host "`n=== Status dos Serviços ===" -ForegroundColor Green
kubectl get services -n fasttech-foods

# Obter NodePorts
Write-Host "`n=== URLs de Acesso ===" -ForegroundColor Green
Write-Host "AuthService: http://localhost:30001" -ForegroundColor White
Write-Host "KitchenService: http://localhost:30002" -ForegroundColor White
Write-Host "MenuService: http://localhost:30003" -ForegroundColor White
Write-Host "OrderService: http://localhost:30004" -ForegroundColor White

Write-Host "`n=== Vantagens desta configuração ===" -ForegroundColor Green
Write-Host "✅ Mais simples de configurar" -ForegroundColor Green
Write-Host "✅ Não precisa de Ingress Controller" -ForegroundColor Green
Write-Host "✅ Acesso direto via localhost" -ForegroundColor Green
Write-Host "✅ Menos componentes para gerenciar" -ForegroundColor Green

Write-Host "`n=== Desvantagens ===" -ForegroundColor Yellow
Write-Host "❌ URLs não são amigáveis" -ForegroundColor Yellow
Write-Host "❌ Precisa lembrar das portas" -ForegroundColor Yellow
Write-Host "❌ Não tem roteamento baseado em hostname" -ForegroundColor Yellow
Write-Host "❌ Mais difícil de configurar SSL" -ForegroundColor Yellow

Write-Host "`n=== Deployment Simples Concluído! ===" -ForegroundColor Green 