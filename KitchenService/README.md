# KitchenService

Serviço responsável por gerenciar a aceitação ou rejeição de pedidos pela equipe da cozinha no sistema FastTech Foods.

## Funcionalidades

### Principais
- **Visualização de Pedidos**: A equipe da cozinha pode visualizar todos os pedidos recebidos
- **Aceitação de Pedidos**: Aceitar pedidos pendentes para preparo
- **Rejeição de Pedidos**: Rejeitar pedidos com motivo específico
- **Controle de Status**: Gerenciar o fluxo de status dos pedidos (Pendente → Aceito → Em Preparo → Pronto → Entregue)
- **Estatísticas**: Visualizar estatísticas dos pedidos por status

### Endpoints Principais

#### Autenticados (requer JWT)
- `GET /api/kitchen/pedidos` - Lista todos os pedidos
- `GET /api/kitchen/pedidos/pendentes` - Lista pedidos pendentes
- `GET /api/kitchen/pedidos/status/{status}` - Lista pedidos por status
- `GET /api/kitchen/pedidos/{id}` - Obtém pedido por ID
- `POST /api/kitchen/pedidos/{id}/aceitar` - Aceita um pedido
- `POST /api/kitchen/pedidos/{id}/rejeitar` - Rejeita um pedido
- `PUT /api/kitchen/pedidos/{id}/status` - Atualiza status do pedido
- `GET /api/kitchen/estatisticas` - Obtém estatísticas

#### Teste (sem autenticação)
- `POST /api/test/pedidos` - Cria pedido de teste
- `GET /api/test/pedidos` - Lista todos os pedidos (para teste)
- `POST /api/test/pedidos/multiplos` - Cria múltiplos pedidos de teste
- `DELETE /api/test/pedidos` - Limpa pedidos de teste

## Status dos Pedidos

O sistema gerencia os seguintes status para os pedidos:

1. **Pendente** - Pedido recebido, aguardando aceitação da cozinha
2. **Aceito** - Pedido aceito pela cozinha
3. **Em Preparo** - Pedido sendo preparado
4. **Pronto** - Pedido finalizado, aguardando entrega
5. **Entregue** - Pedido entregue ao cliente
6. **Rejeitado** - Pedido rejeitado pela cozinha
7. **Cancelado** - Pedido cancelado

## Configuração

### Arquivos de Configuração

#### appsettings.json
```json
{
  "KitchenSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "fasttech_kitchen",
    "PedidosCollectionName": "pedidos"
  },
  "AuthSettings": {
    "JwtSecret": "your-super-secret-jwt-key"
  }
}
```

#### appsettings.Development.json
```json
{
  "KitchenSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "fasttech_kitchen_dev",
    "PedidosCollectionName": "pedidos"
  },
  "AuthSettings": {
    "JwtSecret": "dev-secret-key"
  }
}
```

## Executando o Projeto

### Pré-requisitos
- .NET 9.0
- MongoDB (local ou remoto)
- JWT Token válido (para endpoints autenticados)

### Comandos

```bash
# Restaurar dependências
dotnet restore

# Executar em modo desenvolvimento
dotnet run

# Executar em modo produção
dotnet run --environment Production

# Build do projeto
dotnet build

# Testes (se implementados)
dotnet test
```

### Porta Padrão
- **Desenvolvimento**: https://localhost:7001
- **Swagger UI**: https://localhost:7001/swagger

## Exemplos de Uso

### 1. Criar Pedidos de Teste
```bash
# Criar um pedido de teste
curl -X POST https://localhost:7001/api/test/pedidos

# Criar múltiplos pedidos
curl -X POST "https://localhost:7001/api/test/pedidos/multiplos?quantidade=3"
```

### 2. Visualizar Pedidos Pendentes
```bash
curl -H "Authorization: Bearer YOUR_JWT_TOKEN" \
     https://localhost:7001/api/kitchen/pedidos/pendentes
```

### 3. Aceitar um Pedido
```bash
curl -X POST \
     -H "Authorization: Bearer YOUR_JWT_TOKEN" \
     -H "Content-Type: application/json" \
     -d '{
       "pedidoId": "PEDIDO_ID",
       "funcionarioId": "func-001",
       "funcionarioNome": "João Cozinheiro"
     }' \
     https://localhost:7001/api/kitchen/pedidos/PEDIDO_ID/aceitar
```

### 4. Rejeitar um Pedido
```bash
curl -X POST \
     -H "Authorization: Bearer YOUR_JWT_TOKEN" \
     -H "Content-Type: application/json" \
     -d '{
       "pedidoId": "PEDIDO_ID",
       "funcionarioId": "func-001",
       "funcionarioNome": "João Cozinheiro",
       "motivoRejeicao": "Ingrediente indisponível"
     }' \
     https://localhost:7001/api/kitchen/pedidos/PEDIDO_ID/rejeitar
```

### 5. Atualizar Status
```bash
curl -X PUT \
     -H "Authorization: Bearer YOUR_JWT_TOKEN" \
     -H "Content-Type: application/json" \
     -d '{
       "pedidoId": "PEDIDO_ID",
       "novoStatus": "EmPreparo",
       "funcionarioId": "func-001",
       "funcionarioNome": "João Cozinheiro",
       "observacoes": "Iniciando preparo"
     }' \
     https://localhost:7001/api/kitchen/pedidos/PEDIDO_ID/status
```

## Estrutura do Projeto

```
KitchenService/
├── Controllers/
│   ├── KitchenController.cs    # Controller principal da cozinha
│   └── TestController.cs       # Controller para testes
├── Models/
│   ├── Pedido.cs              # Modelo principal do pedido
│   ├── PedidoAcaoRequest.cs   # Modelos de requisição
│   └── KitchenSettings.cs     # Configurações
├── Repositories/
│   └── PedidoRepository.cs    # Repositório para MongoDB
├── Program.cs                 # Configuração da aplicação
├── appsettings.json          # Configurações
└── README.md                 # Esta documentação
```

## Integração com Outros Serviços

O KitchenService é projetado para trabalhar em conjunto com:

- **AuthService**: Para autenticação e autorização
- **OrderService**: Para receber pedidos
- **MenuService**: Para informações do cardápio

## Segurança

- Todos os endpoints principais requerem autenticação JWT
- Validação de transições de status para evitar estados inválidos
- Logs detalhados para auditoria
- CORS configurado para desenvolvimento

## Monitoramento

O serviço inclui:
- Logs estruturados para todas as operações
- Estatísticas em tempo real
- Rastreamento de funcionários que executam ações
- Timestamps para todas as mudanças de status

## Próximos Passos

- [ ] Implementar notificações em tempo real
- [ ] Adicionar métricas de performance
- [ ] Implementar cache para consultas frequentes
- [ ] Adicionar testes unitários e de integração
- [ ] Implementar rate limiting
- [ ] Adicionar documentação OpenAPI mais detalhada 