# Script para executar Zabbix localmente
# Execute este script como Administrador

Write-Host "=== FastTech Foods - Zabbix Local ===" -ForegroundColor Green

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

# Parar containers existentes
Write-Host "Parando containers Zabbix existentes..." -ForegroundColor Yellow
docker-compose -f docker-compose-zabbix.yml down

# Remover volumes antigos (opcional)
Write-Host "Removendo volumes antigos..." -ForegroundColor Yellow
docker volume rm $(docker volume ls -q | findstr zabbix) 2>$null

# Executar Zabbix
Write-Host "Iniciando Zabbix..." -ForegroundColor Yellow
docker-compose -f docker-compose-zabbix.yml up -d

# Aguardar inicialização
Write-Host "Aguardando inicialização..." -ForegroundColor Yellow
Start-Sleep -Seconds 30

# Verificar status
Write-Host "`n=== Status dos Containers ===" -ForegroundColor Green
docker-compose -f docker-compose-zabbix.yml ps

# Mostrar logs
Write-Host "`n=== Logs do Zabbix Web ===" -ForegroundColor Green
docker-compose -f docker-compose-zabbix.yml logs zabbix-web --tail=10

Write-Host "`n=== URLs de Acesso ===" -ForegroundColor Green
Write-Host "Zabbix Web: http://localhost:30800" -ForegroundColor White
Write-Host "Usuário: Admin" -ForegroundColor White
Write-Host "Senha: zabbix" -ForegroundColor White

Write-Host "`n=== Zabbix Iniciado! ===" -ForegroundColor Green
Write-Host "Acesse http://localhost:30800 para configurar o Zabbix." -ForegroundColor Yellow 