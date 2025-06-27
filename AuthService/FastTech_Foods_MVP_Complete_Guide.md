# FastTech Foods MVP - Guia Completo de Execução

## 📋 Índice

1. [Visão Geral](#1-visão-geral)
2. [Resumo dos Projetos](#2-resumo-dos-projetos)
3. [Pré-requisitos](#3-pré-requisitos)
4. [Instalação e Configuração](#4-instalação-e-configuração)
5. [Execução dos Serviços](#5-execução-dos-serviços)
6. [Testes e Validação](#6-testes-e-validação)
7. [Monitoramento](#7-monitoramento)
8. [Troubleshooting](#8-troubleshooting)

---

## 1. Visão Geral

O **FastTech Foods MVP** é um sistema completo de gestão de restaurante baseado em microserviços, desenvolvido em .NET 9.0 com arquitetura moderna e escalável.

### 🏗️ Arquitetura

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│ AuthService │    │MenuService  │    │KitchenService│    │OrderService │
│    :7001    │    │   :7002     │    │    :7003     │    │   :7004     │
│             │    │             │    │             │    │             │
│ - Login     │    │ - Produtos  │    │ - Pedidos   │    │ - Pedidos   │
│ - Registro  │    │ - Menu      │    │ - Status    │    │ - Clientes  │
│ - JWT       │    │ - Busca     │    │ - Ações     │    │ - Integração│
└─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘
       │                   │                   │                   │
       └───────────────────┼───────────────────┼───────────────────┘
                           │                   │
                    ┌─────────────┐    ┌─────────────┐
                    │  MongoDB    │    │  RabbitMQ   │
                    │   :27017    │    │   :5672     │
                    │             │    │             │
                    │ - Usuários  │    │ - Mensagens │
                    │ - Produtos  │    │ - Filas     │
                    │ - Pedidos   │    │ - Exchange  │
                    └─────────────┘    └─────────────┘
```

---

## 2. Resumo dos Projetos

### 🔐 AuthService (Porta 7001)

**Propósito**: Autenticação e autorização centralizada

**Funcionalidades**:
- ✅ Login/registro de funcionários
- ✅ Login/registro de clientes  
- ✅ Geração de tokens JWT
- ✅ Validação de credenciais
- ✅ Hash seguro de senhas

**Tecnologias**: .NET 9.0, MongoDB, JWT, Swagger

**Endpoints Principais**:
```bash
POST /api/auth/funcionario/login     # Login funcionário
POST /api/auth/funcionario/registro  # Registro funcionário
POST /api/auth/cliente/login         # Login cliente
POST /api/auth/cliente/registro      # Registro cliente
GET  /api/auth/me                    # Info do usuário
```

### 🍽️ MenuService (Porta 7002)

**Propósito**: Gestão de produtos e menu

**Funcionalidades**:
- ✅ CRUD completo de produtos
- ✅ Busca e filtros avançados
- ✅ Paginação de resultados
- ✅ Gestão de categorias
- ✅ Controle de disponibilidade

**Tecnologias**: .NET 9.0, MongoDB, JWT, Swagger

**Endpoints Principais**:
```bash
GET    /api/menu/produtos           # Listar produtos
GET    /api/menu/produtos/{id}      # Buscar produto
POST   /api/menu/produtos           # Criar produto
PUT    /api/menu/produtos/{id}      # Atualizar produto
DELETE /api/menu/produtos/{id}      # Deletar produto
GET    /api/menu/buscar             # Busca com filtros
```

### 👨‍🍳 KitchenService (Porta 7003)

**Propósito**: Gestão de pedidos na cozinha

**Funcionalidades**:
- ✅ Visualização de pedidos pendentes
- ✅ Aceitar/rejeitar pedidos
- ✅ Atualizar status
- ✅ Histórico de ações
- ✅ Estatísticas da cozinha
- ✅ Consumo de mensagens RabbitMQ

**Tecnologias**: .NET 9.0, MongoDB, JWT, RabbitMQ, Swagger

**Endpoints Principais**:
```bash
GET  /api/kitchen/pedidos           # Listar pedidos
GET  /api/kitchen/pedidos/pendentes # Pedidos pendentes
PUT  /api/kitchen/pedidos/{id}/aceitar  # Aceitar pedido
PUT  /api/kitchen/pedidos/{id}/rejeitar # Rejeitar pedido
PUT  /api/kitchen/pedidos/{id}/status  # Atualizar status
GET  /api/kitchen/estatisticas      # Estatísticas
```

### 📦 OrderService (Porta 7004)

**Propósito**: Criação e gestão de pedidos

**Funcionalidades**:
- ✅ Criação de pedidos por clientes
- ✅ Cancelamento de pedidos
- ✅ Integração com MenuService
- ✅ Validação de produtos
- ✅ Publicação de mensagens RabbitMQ
- ✅ Gestão de clientes

**Tecnologias**: .NET 9.0, MongoDB, JWT, RabbitMQ, HttpClient, Swagger

**Endpoints Principais**:
```bash
POST /api/orders/pedidos            # Criar pedido
GET  /api/orders/pedidos/{id}       # Buscar pedido
POST /api/orders/pedidos/{id}/cancelar # Cancelar pedido
POST /api/orders/clientes           # Cadastrar cliente
GET  /api/orders/produtos           # Produtos disponíveis
```

---

## 3. Pré-requisitos

### 🛠️ Software Necessário

- **.NET 9.0 SDK**: [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **MongoDB 7.0+**: [Download](https://www.mongodb.com/try/download/community)
- **RabbitMQ 3.12+** (opcional): [Download](https://www.rabbitmq.com/download.html)
- **Visual Studio 2022** ou **VS Code**
- **Git**: [Download](https://git-scm.com/)

### 🔧 Verificação de Instalação

```bash
# Verificar .NET
dotnet --version

# Verificar MongoDB
mongosh --version

# Verificar Git
git --version
```

---

## 4. Instalação e Configuração

### 4.1 Clone do Repositório

```bash
git clone <repository-url>
cd fasttech-foods-mvp
```

### 4.2 Configuração do MongoDB

#### Windows
```bash
# Instalar MongoDB (usando chocolatey)
choco install mongodb

# Iniciar serviço
net start MongoDB

# Verificar status
sc query MongoDB
```

#### Linux/Mac
```bash
# Ubuntu/Debian
sudo apt update
sudo apt install mongodb

# Iniciar serviço
sudo systemctl start mongod
sudo systemctl enable mongod

# Verificar status
sudo systemctl status mongod
```

#### Verificar Conexão
```bash
mongosh
# Deve conectar sem erros
```

### 4.3 Configuração do RabbitMQ (Opcional)

#### Windows
```bash
# Instalar RabbitMQ
choco install rabbitmq

# Iniciar serviço
net start RabbitMQ

# Acessar interface web
# http://localhost:15672
# Usuário: admin
# Senha: password123
```

#### Linux/Mac
```bash
# Ubuntu/Debian
sudo apt install rabbitmq-server

# Iniciar serviço
sudo systemctl start rabbitmq-server
sudo systemctl enable rabbitmq-server

# Configurar usuário
sudo rabbitmqctl add_user admin password123
sudo rabbitmqctl set_user_tags admin administrator
sudo rabbitmqctl set_permissions -p / admin ".*" ".*" ".*"
```

---

## 5. Execução dos Serviços

### 5.1 Execução Individual

#### 🔐 AuthService
```bash
cd AuthService
dotnet restore
dotnet run
# Acesse: https://localhost:7001
# Swagger: https://localhost:7001/swagger
```

#### 🍽️ MenuService
```bash
cd MenuService
dotnet restore
dotnet run
# Acesse: https://localhost:7002
# Swagger: https://localhost:7002/swagger
```

#### 👨‍🍳 KitchenService
```bash
cd KitchenService
dotnet restore
dotnet run
# Acesse: https://localhost:7003
# Swagger: https://localhost:7003/swagger
```

#### 📦 OrderService
```bash
cd OrderService
dotnet restore
dotnet run
# Acesse: https://localhost:7004
# Swagger: https://localhost:7004/swagger
```

### 5.2 Execução com Docker Compose

```bash
cd deploy/docker

# Executar todos os serviços
docker-compose up -d

# Verificar status
docker-compose ps

# Ver logs
docker-compose logs -f

# Parar serviços
docker-compose down
```

### 5.3 Execução com Kubernetes

```bash
cd deploy/k8s

# Aplicar configurações
kubectl apply -f authservice-deployment.yaml
kubectl apply -f menuservice-deployment.yaml
kubectl apply -f kitchenservice-deployment.yaml
kubectl apply -f orderservice-deployment.yaml

# Verificar status
kubectl get pods
kubectl get services
```

---

## 6. Testes e Validação

### 6.1 Swagger/OpenAPI

Cada serviço possui documentação interativa:

- **AuthService**: https://localhost:7001/swagger
- **MenuService**: https://localhost:7002/swagger  
- **KitchenService**: https://localhost:7003/swagger
- **OrderService**: https://localhost:7004/swagger

### 6.2 Dados de Teste

#### Criar Dados de Teste
```bash
# AuthService
curl -X POST https://localhost:7001/api/test/criar-dados-teste

# MenuService
curl -X POST https://localhost:7002/api/test/criar-dados-teste

# KitchenService
curl -X POST https://localhost:7003/api/test/criar-dados-teste

# OrderService
curl -X POST https://localhost:7004/api/test/criar-dados-teste
```

#### Limpar Dados de Teste
```bash
# AuthService
curl -X DELETE https://localhost:7001/api/test/limpar-dados-teste

# MenuService
curl -X DELETE https://localhost:7002/api/test/limpar-dados-teste

# KitchenService
curl -X DELETE https://localhost:7003/api/test/limpar-dados-teste

# OrderService
curl -X DELETE https://localhost:7004/api/test/limpar-dados-teste
```

### 6.3 Fluxo de Teste Completo

#### 1. Registrar Funcionário
```bash
curl -X POST https://localhost:7001/api/auth/funcionario/registro \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "João Silva",
    "email": "joao@fasttech.com",
    "senha": "123456",
    "cargo": "Gerente"
  }'
```

#### 2. Login do Funcionário
```bash
curl -X POST https://localhost:7001/api/auth/funcionario/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "joao@fasttech.com",
    "senha": "123456"
  }'
# Guarde o token retornado
```

#### 3. Criar Produto
```bash
curl -X POST https://localhost:7002/api/menu/produtos \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {TOKEN}" \
  -d '{
    "nome": "X-Burger",
    "descricao": "Hambúrguer com queijo",
    "preco": 15.90,
    "categoria": "Pratos",
    "disponivel": true
  }'
```

#### 4. Registrar Cliente
```bash
curl -X POST https://localhost:7001/api/auth/cliente/registro \
  -H "Content-Type: application/json" \
  -d '{
    "nome": "Maria Santos",
    "email": "maria@email.com",
    "cpf": "12345678901",
    "telefone": "11999999999",
    "endereco": "Rua A, 123",
    "cep": "01234-567",
    "cidade": "São Paulo",
    "estado": "SP",
    "senha": "123456"
  }'
```

#### 5. Login do Cliente
```bash
curl -X POST https://localhost:7001/api/auth/cliente/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "maria@email.com",
    "senha": "123456"
  }'
# Guarde o token e clienteId retornados
```

#### 6. Criar Pedido
```bash
curl -X POST https://localhost:7004/api/orders/pedidos \
  -H "Content-Type: application/json" \
  -d '{
    "clienteId": "{CLIENTE_ID}",
    "clienteNome": "Maria Santos",
    "clienteTelefone": "11999999999",
    "clienteEmail": "maria@email.com",
    "itens": [
      {
        "produtoId": "{PRODUTO_ID}",
        "quantidade": 2
      }
    ],
    "formaEntrega": "Delivery",
    "enderecoEntrega": "Rua A, 123",
    "taxaEntrega": 5.00
  }'
```

#### 7. Aceitar Pedido na Cozinha
```bash
curl -X PUT https://localhost:7003/api/kitchen/pedidos/{PEDIDO_ID}/aceitar \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {TOKEN_FUNCIONARIO}" \
  -d '{
    "funcionarioId": "{FUNCIONARIO_ID}",
    "funcionarioNome": "João Silva",
    "observacoes": "Pedido aceito"
  }'
```

---

## 7. Monitoramento

### 7.1 Logs

Todos os serviços implementam logging estruturado:

```bash
# Ver logs em tempo real
docker-compose logs -f authservice
docker-compose logs -f menuservice
docker-compose logs -f kitchenservice
docker-compose logs -f orderservice
```

### 7.2 Métricas

- **Prometheus**: http://localhost:9090
- **Grafana**: http://localhost:3000
  - Usuário: admin
  - Senha: admin123

### 7.3 RabbitMQ Management

- **Interface Web**: http://localhost:15672
- **Usuário**: admin
- **Senha**: password123

---

## 8. Troubleshooting

### 8.1 Problemas Comuns

#### ❌ Erro de Conexão com MongoDB
```bash
# Verificar se MongoDB está rodando
net start MongoDB  # Windows
sudo systemctl status mongod  # Linux

# Verificar string de conexão
# Deve ser: mongodb://localhost:27017
```

#### ❌ Erro de Autenticação JWT
```bash
# Verificar se a chave secreta está configurada
# Verificar se o token não expirou
# Verificar se o issuer/audience estão corretos
```

#### ❌ Erro de Conexão com RabbitMQ
```bash
# Verificar se RabbitMQ está rodando
net start RabbitMQ  # Windows
sudo systemctl status rabbitmq-server  # Linux

# Verificar credenciais
# Usuário: admin
# Senha: password123
```

#### ❌ Erro de Integração entre Serviços
```bash
# Verificar se todos os serviços estão rodando
# Verificar URLs dos serviços
# Verificar logs de erro
```

### 8.2 Logs de Debug

Para habilitar logs detalhados, adicione ao `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  }
}
```

### 8.3 Verificação de Portas

```bash
# Verificar portas em uso
netstat -an | findstr :7001
netstat -an | findstr :7002
netstat -an | findstr :7003
netstat -an | findstr :7004
netstat -an | findstr :27017
netstat -an | findstr :5672
```

---

## 9. Configurações Avançadas

### 9.1 Variáveis de Ambiente

```bash
# MongoDB
ConnectionStrings__MongoDb=mongodb://localhost:27017

# JWT
AuthSettings__JwtSecret=sua_chave_secreta_aqui
AuthSettings__JwtIssuer=FastTech Foods
AuthSettings__JwtAudience=FastTech Users
AuthSettings__JwtExpirationHours=24

# RabbitMQ
RabbitMQHost=localhost
RabbitMQPort=5672
RabbitMQUser=admin
RabbitMQPassword=password123
```

### 9.2 Configurações por Serviço

#### AuthService
```json
{
  "AuthSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "fasttech_auth",
    "JwtSecret": "sua_chave_secreta_aqui",
    "JwtIssuer": "FastTech Foods",
    "JwtAudience": "FastTech Users",
    "JwtExpirationHours": 24
  }
}
```

#### MenuService
```json
{
  "MenuSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "fasttech_menu",
    "ProdutosCollectionName": "produtos"
  }
}
```

#### KitchenService
```json
{
  "KitchenSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "fasttech_kitchen",
    "PedidosCollectionName": "pedidos",
    "RabbitMQHost": "localhost",
    "RabbitMQPort": 5672,
    "RabbitMQUser": "admin",
    "RabbitMQPassword": "password123"
  }
}
```

#### OrderService
```json
{
  "OrderSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "fasttech_order",
    "PedidosCollectionName": "pedidos",
    "ClientesCollectionName": "clientes",
    "MenuServiceUrl": "https://localhost:7002",
    "RabbitMQHost": "localhost",
    "RabbitMQPort": 5672,
    "RabbitMQUser": "admin",
    "RabbitMQPassword": "password123"
  }
}
```

---

## 10. Próximos Passos

### 10.1 Melhorias Sugeridas

- [ ] Implementar cache Redis
- [ ] Adicionar notificações em tempo real (SignalR)
- [ ] Implementar paginação nos endpoints
- [ ] Adicionar validação mais robusta
- [ ] Implementar testes unitários
- [ ] Adicionar rate limiting
- [ ] Implementar circuit breakers

### 10.2 Escalabilidade

- [ ] Containerização com Docker
- [ ] Orquestração com Kubernetes
- [ ] Load balancing
- [ ] Database sharding
- [ ] Microserviços adicionais

### 10.3 Funcionalidades Futuras

- [ ] Sistema de pagamentos
- [ ] Avaliações e reviews
- [ ] Relatórios e analytics
- [ ] App mobile
- [ ] Dashboard administrativo

---

---

**🎉 Sistema FastTech Foods MVP - Pronto para Uso!**
