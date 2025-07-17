# FastTech Foods - Kubernetes Deployment (Windows)

Este diretório contém todos os arquivos necessários para fazer o deployment da aplicação FastTech Foods em um cluster Kubernetes usando kubeadm no Windows.

## Pré-requisitos

### 1. Instalar kubectl
```powershell
# Baixar kubectl para Windows
curl.exe -LO "https://dl.k8s.io/release/v1.28.0/bin/windows/amd64/kubectl.exe"

# Mover para PATH (como administrador)
move kubectl.exe C:\Windows\System32\
```

### 2. Configurar cluster Kubernetes
Se você ainda não tem um cluster Kubernetes rodando:

#### Opção A: Usar Docker Desktop
1. Instalar Docker Desktop
2. Habilitar Kubernetes nas configurações
3. Aguardar o cluster estar pronto

#### Opção B: Usar Minikube
```powershell
# Instalar Minikube
winget install minikube

# Iniciar cluster
minikube start --driver=docker
```

#### Opção C: Usar kubeadm (para cluster local)
```powershell
# Instalar kubeadm, kubelet e kubectl
# Seguir a documentação oficial: https://kubernetes.io/docs/setup/production-environment/tools/kubeadm/install-kubeadm/
```

## Arquivos de Configuração

### Estrutura dos Arquivos
```
deploy/k8s/
├── namespace.yaml              # Namespace para organizar recursos
├── secrets.yaml                # Secrets para configurações sensíveis
├── mongodb-deployment.yaml     # Deployment do MongoDB
├── authservice-deployment.yaml # AuthService
├── kitchenservice-deployment.yaml # KitchenService
├── menuservice-deployment.yaml # MenuService
├── orderservice-deployment.yaml # OrderService
├── deploy-windows.ps1         # Script de deployment automatizado
├── deploy-simple.ps1          # Script de deployment simplificado
├── build-images.ps1           # Script para build das imagens
├── cleanup.ps1                # Script de limpeza
└── README.md                  # Esta documentação
```

### Configurações Principais

#### Namespace
- **Nome**: `fasttech-foods`
- **Propósito**: Isolar recursos da aplicação

#### Secrets
- **mongodb-secret**: String de conexão do MongoDB
- **jwt-secret**: Chave secreta para JWT

#### Serviços
- **AuthService**: Porta 80, 2 réplicas
- **KitchenService**: Porta 80, 2 réplicas
- **MenuService**: Porta 80, 2 réplicas
- **OrderService**: Porta 80, 2 réplicas

#### Acesso aos Serviços
- **URLs diretas**:
  - `http://localhost:30001` (AuthService)
  - `http://localhost:30002` (KitchenService)
  - `http://localhost:30003` (MenuService)
  - `http://localhost:30004` (OrderService)

## Deployment

### Método 1: Script Automatizado (Recomendado)
```powershell
# Executar como Administrador
cd deploy/k8s
.\deploy-windows.ps1
```

### Método 2: Deployment Manual
```powershell
# 1. Criar namespace
kubectl apply -f namespace.yaml

# 2. Aplicar secrets
kubectl apply -f secrets.yaml

# 3. Deploy MongoDB
kubectl apply -f mongodb-deployment.yaml

# 4. Deploy serviços
kubectl apply -f authservice-deployment.yaml
kubectl apply -f kitchenservice-deployment.yaml
kubectl apply -f menuservice-deployment.yaml
kubectl apply -f orderservice-deployment.yaml
```

## Verificação do Deployment

### Verificar Status dos Pods
```powershell
kubectl get pods -n fasttech-foods
```

### Verificar Status dos Serviços
```powershell
kubectl get services -n fasttech-foods
```



### Verificar Logs
```powershell
# Logs do AuthService
kubectl logs -f deployment/authservice -n fasttech-foods

# Logs do MongoDB
kubectl logs -f deployment/mongodb -n fasttech-foods
```

## Acesso aos Serviços

### URLs de Acesso Direto
- **AuthService**: `http://localhost:30001`
- **KitchenService**: `http://localhost:30002`
- **MenuService**: `http://localhost:30003`
- **OrderService**: `http://localhost:30004`

## Troubleshooting

### Problemas Comuns

#### 1. Pods em estado Pending
```powershell
# Verificar recursos disponíveis
kubectl describe pod <pod-name> -n fasttech-foods
```

#### 2. Erro de conexão com MongoDB
```powershell
# Verificar se o MongoDB está rodando
kubectl get pods -l app=mongodb -n fasttech-foods

# Verificar logs do MongoDB
kubectl logs deployment/mongodb -n fasttech-foods
```

#### 3. Erro de conexão
- Verificar se as portas estão acessíveis: `netstat -an | findstr 3000`
- Tentar acessar via localhost diretamente

### Comandos Úteis

#### Limpar Deployment
```powershell
kubectl delete namespace fasttech-foods
kubectl delete namespace ingress-nginx
```

#### Reiniciar Deployment
```powershell
kubectl rollout restart deployment/authservice -n fasttech-foods
kubectl rollout restart deployment/kitchenservice -n fasttech-foods
kubectl rollout restart deployment/menuservice -n fasttech-foods
kubectl rollout restart deployment/orderservice -n fasttech-foods
```

#### Escalar Serviços
```powershell
# Escalar AuthService para 3 réplicas
kubectl scale deployment authservice --replicas=3 -n fasttech-foods
```

## Monitoramento

### Métricas Básicas
```powershell
# Uso de CPU e Memória
kubectl top pods -n fasttech-foods

# Status dos recursos
kubectl get all -n fasttech-foods
```

### Logs Centralizados
Para logs mais avançados, considere implementar:
- Elasticsearch + Kibana
- Fluentd
- Prometheus + Grafana

## Segurança

### Recomendações
1. **Não usar em produção**: Esta configuração é para desenvolvimento
2. **Alterar secrets**: Mude as chaves JWT e senhas do MongoDB
3. **Network Policies**: Implementar políticas de rede
4. **RBAC**: Configurar controle de acesso baseado em roles
5. **TLS**: Configurar certificados SSL para produção

### Secrets em Produção
```powershell
# Criar secrets de forma segura
kubectl create secret generic mongodb-secret \
  --from-literal=connection-string="mongodb://user:pass@host:port/db" \
  -n fasttech-foods

kubectl create secret generic jwt-secret \
  --from-literal=secret-key="sua-chave-super-secreta-aqui" \
  -n fasttech-foods
```

## Próximos Passos

1. **Implementar Health Checks**: Adicionar endpoints de health mais robustos
2. **Configurar HPA**: Horizontal Pod Autoscaler para escalabilidade automática
3. **Implementar CI/CD**: Pipeline de deployment automatizado
4. **Monitoramento**: Implementar Prometheus + Grafana
5. **Backup**: Configurar backup automático do MongoDB
6. **SSL**: Configurar certificados SSL para HTTPS 