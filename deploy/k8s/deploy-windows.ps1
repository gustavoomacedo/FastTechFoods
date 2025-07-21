# Script de Deployment para FastTech Foods no Kubernetes (Windows)
# Execute este script como Administrador

Write-Host "=== FastTech Foods - Kubernetes Deployment ===" -ForegroundColor Green

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

# Criar namespace
Write-Host "Criando namespace fasttech-foods..." -ForegroundColor Yellow
kubectl apply -f namespace.yaml

# Aplicar secrets
Write-Host "Aplicando secrets..." -ForegroundColor Yellow
kubectl apply -f secrets.yaml

# Deploy MongoDB
Write-Host "Deployando MongoDB..." -ForegroundColor Yellow
kubectl apply -f mongodb-deployment.yaml

# Deploy RabbitMQ
Write-Host "Deployando RabbitMQ..." -ForegroundColor Yellow
kubectl apply -f rabbitmq-deployment.yaml

# Aguardar MongoDB estar pronto
Write-Host "Aguardando MongoDB estar pronto..." -ForegroundColor Yellow
kubectl wait --for=condition=ready pod -l app=mongodb -n fasttech-foods --timeout=300s

# Aguardar RabbitMQ estar pronto
Write-Host "Aguardando RabbitMQ estar pronto..." -ForegroundColor Yellow
kubectl wait --for=condition=ready pod -l app=rabbitmq -n fasttech-foods --timeout=300s


# Deploy serviços
Write-Host "Deployando AuthService..." -ForegroundColor Yellow
kubectl apply -f authservice-deployment.yaml

Write-Host "Deployando KitchenService..." -ForegroundColor Yellow
kubectl apply -f kitchenservice-deployment.yaml

Write-Host "Deployando MenuService..." -ForegroundColor Yellow
kubectl apply -f menuservice-deployment.yaml

Write-Host "Deployando OrderService..." -ForegroundColor Yellow
kubectl apply -f orderservice-deployment.yaml

# Deploy Prometheus
Write-Host "Deployando Prometheus..." -ForegroundColor Yellow
kubectl apply -f prometheus-deployment.yaml

# Deploy Grafana
Write-Host "Deployando Grafana..." -ForegroundColor Yellow
kubectl apply -f grafana-deployment.yaml

# Deploy Zabbix
Write-Host "Deployando Zabbix..." -ForegroundColor Yellow
kubectl apply -f zabbix-deployment.yaml

# Deploy Zabbix Agents
Write-Host "Deployando Zabbix Agents..." -ForegroundColor Yellow
kubectl apply -f zabbix-agents.yaml

# Aguardar todos os pods estarem prontos
Write-Host "Aguardando todos os serviços estarem prontos..." -ForegroundColor Yellow
kubectl wait --for=condition=ready pod -l app=authservice -n fasttech-foods --timeout=300s
kubectl wait --for=condition=ready pod -l app=kitchenservice -n fasttech-foods --timeout=300s
kubectl wait --for=condition=ready pod -l app=menuservice -n fasttech-foods --timeout=300s
kubectl wait --for=condition=ready pod -l app=orderservice -n fasttech-foods --timeout=300s

# Aguardar Zabbix estar pronto
Write-Host "Aguardando Zabbix estar pronto..." -ForegroundColor Yellow
kubectl wait --for=condition=ready pod -l app=zabbix-server -n fasttech-foods --timeout=300s
kubectl wait --for=condition=ready pod -l app=zabbix-web -n fasttech-foods --timeout=300s

# Mostrar status dos pods
Write-Host "`n=== Status dos Pods ===" -ForegroundColor Green
kubectl get pods -n fasttech-foods

Write-Host "`n=== Status dos Serviços ===" -ForegroundColor Green
kubectl get services -n fasttech-foods

# Mostrar URLs de acesso
Write-Host "`n=== URLs de Acesso ===" -ForegroundColor Green
Write-Host "AuthService: http://localhost:30001" -ForegroundColor White
Write-Host "KitchenService: http://localhost:30002" -ForegroundColor White
Write-Host "MenuService: http://localhost:30003" -ForegroundColor White
Write-Host "OrderService: http://localhost:30004" -ForegroundColor White
Write-Host "RabbitMQ Management: http://localhost:31672 (usuário: guest, senha: guest)" -ForegroundColor White
Write-Host "Prometheus: http://localhost:30900" -ForegroundColor White
Write-Host "Grafana: http://localhost:30300 (usuário: admin, senha: admin)" -ForegroundColor White
Write-Host "Zabbix Web: http://localhost:30800 (usuário: Admin, senha: zabbix)" -ForegroundColor White

Write-Host "`n=== Deployment Concluído! ===" -ForegroundColor Green 