# FastTech Foods MVP - Complete Execution Guide

## 📋 Table of Contents

1. [Overview](#1-overview)
2. [Project Summary](#2-project-summary)
3. [Prerequisites](#3-prerequisites)
4. [Installation and Configuration](#4-installation-and-configuration)
5. [Service Execution](#5-service-execution)
6. [Testing and Validation](#6-testing-and-validation)
7. [Monitoring](#7-monitoring)
8. [Troubleshooting](#8-troubleshooting)

---

## 1. Overview

The **FastTech Foods MVP** is a complete restaurant management system based on microservices, developed in .NET 8.0 with modern and scalable architecture.

### 🏗️ Architecture

```
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│ AuthService │    │MenuService  │    │KitchenService│    │OrderService │
│    :7001    │    │   :7002     │    │    :7003     │    │   :7004     │
│             │    │             │    │             │    │             │
│ - Login     │    │ - Products  │    │ - Orders    │    │ - Orders    │
│ - Register  │    │ - Menu      │    │ - Status    │    │ - Customers │
│ - JWT       │    │ - Search    │    │ - Actions   │    │ - Integration│
└─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘
       │                   │                   │                   │
       └───────────────────┼───────────────────┼───────────────────┘
                           │                   │
                    ┌─────────────┐    ┌─────────────┐
                    │  MongoDB    │    │  RabbitMQ   │
                    │   :27017    │    │   :5672     │
                    │             │    │             │
                    │ - Users     │    │ - Messages  │
                    │ - Products  │    │ - Queues    │
                    │ - Orders    │    │ - Exchange  │
                    └─────────────┘    └─────────────┘
```

---

## 2. Project Summary

### 🔐 AuthService (Port 7001)

**Purpose**: Centralized authentication and authorization

**Features**:
- ✅ Employee login/registration
- ✅ Customer login/registration  
- ✅ JWT token generation
- ✅ Credential validation
- ✅ Secure password hashing

**Technologies**: .NET 8.0, MongoDB, JWT, Swagger

**Main Endpoints**:
```bash
POST /api/auth/funcionario/login     # Employee login
POST /api/auth/funcionario/registro  # Employee registration
POST /api/auth/cliente/login         # Customer login
POST /api/auth/cliente/registro      # Customer registration
GET  /api/auth/me                    # User info
```

### 🍽️ MenuService (Port 7002)

**Purpose**: Product and menu management

**Features**:
- ✅ Complete product CRUD
- ✅ Advanced search and filters
- ✅ Result pagination
- ✅ Category management
- ✅ Availability control

**Technologies**: .NET 8.0, MongoDB, JWT, Swagger

**Main Endpoints**:
```bash
GET    /api/menu/produtos           # List products
GET    /api/menu/produtos/{id}      # Get product
POST   /api/menu/produtos           # Create product
PUT    /api/menu/produtos/{id}      # Update product
DELETE /api/menu/produtos/{id}      # Delete product
GET    /api/menu/buscar             # Search with filters
```

### 👨‍🍳 KitchenService (Port 7003)

**Purpose**: Kitchen order management

**Features**:
- ✅ View pending orders
- ✅ Accept/reject orders
- ✅ Update status
- ✅ Action history
- ✅ Kitchen statistics
- ✅ RabbitMQ message consumption

**Technologies**: .NET 8.0, MongoDB, JWT, RabbitMQ, Swagger

**Main Endpoints**:
```bash
GET  /api/kitchen/pedidos           # List orders
GET  /api/kitchen/pedidos/pendentes # Pending orders
PUT  /api/kitchen/pedidos/{id}/aceitar  # Accept order
PUT  /api/kitchen/pedidos/{id}/rejeitar # Reject order
PUT  /api/kitchen/pedidos/{id}/status  # Update status
GET  /api/kitchen/estatisticas      # Statistics
```

### 📦 OrderService (Port 7004)

**Purpose**: Order creation and management

**Features**:
- ✅ Order creation by customers
- ✅ Order cancellation
- ✅ MenuService integration
- ✅ Product validation
- ✅ RabbitMQ message publishing
- ✅ Customer management

**Technologies**: .NET 8.0, MongoDB, JWT, RabbitMQ, HttpClient, Swagger

**Main Endpoints**:
```bash
POST /api/orders/pedidos            # Create order
GET  /api/orders/pedidos/{id}       # Get order
POST /api/orders/pedidos/{id}/cancelar # Cancel order
POST /api/orders/clientes           # Register customer
GET  /api/orders/produtos           # Available products
```

---

## 3. Prerequisites

### 🛠️ Required Software

- **.NET 8.0 SDK**: [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **MongoDB 7.0+**: [Download](https://www.mongodb.com/try/download/community)
- **RabbitMQ 3.12+** (optional): [Download](https://www.rabbitmq.com/download.html)
- **Visual Studio 2022** or **VS Code**
- **Git**: [Download](https://git-scm.com/)

### 🔧 Installation Verification

```bash
# Verify .NET
dotnet --version

# Verify MongoDB
mongosh --version

# Verify Git
git --version
```

---

## 4. Installation and Configuration

### 4.1 Repository Clone

```bash
git clone <repository-url>
cd fasttech-foods-mvp
```

### 4.2 MongoDB Configuration

#### Windows
```bash
# Install MongoDB (using chocolatey)
choco install mongodb

# Start service
net start MongoDB

# Check status
sc query MongoDB
```

#### Linux/Mac
```bash
# Ubuntu/Debian
sudo apt update
sudo apt install mongodb

# Start service
sudo systemctl start mongod
sudo systemctl enable mongod

# Check status
sudo systemctl status mongod
```

#### Verify Connection
```bash
mongosh
# Should connect without errors
```

### 4.3 RabbitMQ Configuration (Optional)

#### Windows
```bash
# Install RabbitMQ
choco install rabbitmq

# Start service
net start RabbitMQ

# Access web interface
# http://localhost:15672
# User: admin
# Password: password123
```

#### Linux/Mac
```bash
# Ubuntu/Debian
sudo apt install rabbitmq-server

# Start service
sudo systemctl start rabbitmq-server
sudo systemctl enable rabbitmq-server

# Configure user
sudo rabbitmqctl add_user admin password123
sudo rabbitmqctl set_user_tags admin administrator
sudo rabbitmqctl set_permissions -p / admin ".*" ".*" ".*"
```

---

## 5. Service Execution

### 5.1 Individual Execution

#### 🔐 AuthService
```bash
cd AuthService
dotnet restore
dotnet run
# Access: https://localhost:7001
# Swagger: https://localhost:7001/swagger
```

#### 🍽️ MenuService
```bash
cd MenuService
dotnet restore
dotnet run
# Access: https://localhost:7002
# Swagger: https://localhost:7002/swagger
```

#### 👨‍🍳 KitchenService
```bash
cd KitchenService
dotnet restore
dotnet run
# Access: https://localhost:7003
# Swagger: https://localhost:7003/swagger
```

#### 📦 OrderService
```bash
cd OrderService
dotnet restore
dotnet run
# Access: https://localhost:7004
# Swagger: https://localhost:7004/swagger
```

### 5.2 Docker Compose Execution

```bash
cd deploy/docker

# Run all services
docker-compose up -d

# Check status
docker-compose ps

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

### 5.3 Kubernetes Execution

```bash
cd deploy/k8s

# Apply configurations
kubectl apply -f authservice-deployment.yaml
kubectl apply -f menuservice-deployment.yaml
kubectl apply -f kitchenservice-deployment.yaml
kubectl apply -f orderservice-deployment.yaml

# Check status
kubectl get pods
kubectl get services
```

---

## 6. Testing and Validation

### 6.1 Swagger/OpenAPI

Each service has interactive documentation:

- **AuthService**: https://localhost:7001/swagger
- **MenuService**: https://localhost:7002/swagger  
- **KitchenService**: https://localhost:7003/swagger
- **OrderService**: https://localhost:7004/swagger

### 6.2 Test Data

#### Create Test Data
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

#### Clear Test Data
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

### 6.3 Complete Test Flow

#### 1. Register Employee
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

#### 2. Employee Login
```bash
curl -X POST https://localhost:7001/api/auth/funcionario/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "joao@fasttech.com",
    "senha": "123456"
  }'
# Save the returned token
```

#### 3. Create Product
```bash
curl -X POST https://localhost:7002/api/menu/produtos \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {TOKEN}" \
  -d '{
        "nome": "Suco de Laranja",
        "descricao": "Suco natural de laranja, sem adição de açúcar.",
        "preco": 7.50,
        "categoria": 1,
        "disponivel": true,
        "imagemUrl": "https://exemplo.com/suco-laranja.jpg",
        "ingredientes": ["Laranja"],
        "alergenos": [],
        "tempoPreparoMinutos": 3,
        "destaque": true,
        "ordemExibicao": 1
      }'
```

#### 4. Register Customer
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

#### 5. Customer Login
```bash
curl -X POST https://localhost:7001/api/auth/cliente/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "maria@email.com",
    "senha": "123456"
  }'
# Save the returned token and clienteId
```

#### 6. Create Order
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

#### 7. Accept Order in Kitchen
```bash
curl -X PUT https://localhost:7003/api/kitchen/pedidos/{PEDIDO_ID}/aceitar \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer {TOKEN_FUNCIONARIO}" \
  -d '{
    "pedidoId": "{PEDIDO_ID}",
    "funcionarioId": "{FUNCIONARIO_ID}",
    "funcionarioNome": "João Silva",
    "observacoes": "Pedido aceito"
  }'
```

---

## 7. Monitoring

### 7.1 Logs

All services implement structured logging:

```bash
# View real-time logs
docker-compose logs -f authservice
docker-compose logs -f menuservice
docker-compose logs -f kitchenservice
docker-compose logs -f orderservice
```

### 7.2 Metrics

- **Prometheus**: http://localhost:30900
- **Grafana**: http://localhost:30300
  - User: admin
  - Password: admin
- **Zabbix Web**: http://localhost:30800
  - User: Admin
  - Password: zabbix

### 7.3 RabbitMQ Management

- **Web Interface**: http://localhost:15672
- **User**: admin
- **Password**: password123

---

## 8. Troubleshooting

### 8.1 Common Issues

#### ❌ MongoDB Connection Error
```bash
# Check if MongoDB is running
net start MongoDB  # Windows
sudo systemctl status mongod  # Linux

# Check connection string
# Should be: mongodb://localhost:27017
```

#### ❌ JWT Authentication Error
```bash
# Check if secret key is configured
# Check if token hasn't expired
# Check if issuer/audience are correct
```

#### ❌ RabbitMQ Connection Error
```bash
# Check if RabbitMQ is running
net start RabbitMQ  # Windows
sudo systemctl status rabbitmq-server  # Linux

# Check credentials
# User: admin
# Password: password123
```

#### ❌ Service Integration Error
```bash
# Check if all services are running
# Check service URLs
# Check error logs
```

### 8.2 Debug Logs

To enable detailed logs, add to `appsettings.Development.json`:

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

### 8.3 Port Verification

```bash
# Check ports in use
netstat -an | findstr :7001
netstat -an | findstr :7002
netstat -an | findstr :7003
netstat -an | findstr :7004
netstat -an | findstr :27017
netstat -an | findstr :5672
```

---

## 9. Advanced Configuration

### 9.1 Environment Variables

```bash
# MongoDB
ConnectionStrings__MongoDb=mongodb://localhost:27017

# JWT
AuthSettings__JwtSecret=your_secret_key_here
AuthSettings__JwtIssuer=FastTech Foods
AuthSettings__JwtAudience=FastTech Users
AuthSettings__JwtExpirationHours=24

# RabbitMQ
RabbitMQHost=localhost
RabbitMQPort=5672
RabbitMQUser=admin
RabbitMQPassword=password123
```

### 9.2 Service-Specific Configuration

#### AuthService
```json
{
  "AuthSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "fasttech_auth",
    "JwtSecret": "your_secret_key_here",
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
