# FastTech Foods MVP - Arquitetura DDD (Domain-Driven Design)

## 🏗️ Diagrama da Arquitetura da Solução

### 1. Visão Geral da Arquitetura DDD

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           FASTTECH FOODS MVP - DDD ARCHITECTURE                │
└─────────────────────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────────────────────┐
│                              PRESENTATION LAYER                                │
├─────────────────────────────────────────────────────────────────────────────────┤
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐           │
│  │ AuthService │  │MenuService  │  │KitchenService│  │OrderService │           │
│  │   :7001     │  │   :7002     │  │    :7003     │  │   :7004     │           │
│  │             │  │             │  │             │  │             │           │
│  │ Controllers │  │ Controllers │  │ Controllers │  │ Controllers │           │
│  │ - AuthCtrl  │  │ - MenuCtrl  │  │ - KitchenCtrl│  │ - OrderCtrl │           │
│  │ - TestCtrl  │  │ - TestCtrl  │  │ - TestCtrl  │  │ - ClienteCtrl│           │
│  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘           │
└─────────────────────────────────────────────────────────────────────────────────┘
                                    │
┌─────────────────────────────────────────────────────────────────────────────────┐
│                              APPLICATION LAYER                                 │
├─────────────────────────────────────────────────────────────────────────────────┤
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐           │
│  │   Services  │  │   Services  │  │   Services  │  │   Services  │           │
│  │             │  │             │  │             │  │             │           │
│  │ - JWT Auth  │  │ - Menu Mgmt │  │ - Kitchen   │  │ - Order Mgmt│           │
│  │ - Validation│  │ - Search    │  │   Mgmt      │  │ - Integration│           │
│  │ - Hash      │  │ - Filtering │  │ - Status    │  │ - Messaging │           │
│  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘           │
└─────────────────────────────────────────────────────────────────────────────────┘
                                    │
┌─────────────────────────────────────────────────────────────────────────────────┐
│                              DOMAIN LAYER                                      │
├─────────────────────────────────────────────────────────────────────────────────┤
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐           │
│  │   Models    │  │   Models    │  │   Models    │  │   Models    │           │
│  │             │  │             │  │             │  │             │           │
│  │ - Funcionario│  │ - Produto   │  │ - Pedido    │  │ - Pedido    │           │
│  │ - Cliente   │  │ - Categoria │  │ - ItemPedido│  │ - Cliente   │           │
│  │ - AuthSettings│ │ - Ingrediente│ │ - StatusPedido│ │ - FormaEntrega│         │
│  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘           │
│                                                                                 │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐           │
│  │ Repositories│  │ Repositories│  │ Repositories│  │ Repositories│           │
│  │             │  │             │  │             │  │             │           │
│  │ - Funcionario│  │ - Produto   │  │ - Pedido    │  │ - Pedido    │           │
│  │   Repository│  │   Repository│  │   Repository│  │   Repository│           │
│  │ - Cliente   │  │             │  │             │  │ - Cliente   │           │
│  │   Repository│  │             │  │             │  │   Repository│           │
│  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘           │
└─────────────────────────────────────────────────────────────────────────────────┘
                                    │
┌─────────────────────────────────────────────────────────────────────────────────┐
│                            INFRASTRUCTURE LAYER                               │
├─────────────────────────────────────────────────────────────────────────────────┤
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐           │
│  │   MongoDB   │  │   RabbitMQ  │  │   Prometheus│  │   Grafana   │           │
│  │   :27017    │  │   :5672     │  │   :9090     │  │   :3000     │           │
│  │             │  │             │  │             │  │             │           │
│  │ - fasttech_ │  │ - pedidos_  │  │ - Metrics   │  │ - Dashboards│           │
│  │   auth      │  │   queue     │  │ - Monitoring│  │ - Alerts    │           │
│  │ - fasttech_ │  │ - kitchen_  │  │ - Logs      │  │ - Reports   │           │
│  │   menu      │  │   queue     │  │             │  │             │           │
│  │ - fasttech_ │  │ - fasttech_ │  │             │  │             │           │
│  │   kitchen   │  │   exchange  │  │             │  │             │           │
│  │ - fasttech_ │  │             │  │             │  │             │           │
│  │   order     │  │             │  │             │  │             │           │
│  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘           │
└─────────────────────────────────────────────────────────────────────────────────┘
```

### 2. Domínios e Bounded Contexts

#### 🔐 Domínio de Autenticação (AuthService)
```
┌─────────────────────────────────────────────────────────────────┐
│                    AUTHENTICATION DOMAIN                       │
├─────────────────────────────────────────────────────────────────┤
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │   AGGREGATES    │  │   ENTITIES      │  │   VALUE OBJECTS │  │
│  │                 │  │                 │  │                 │  │
│  │ - Funcionario   │  │ - Funcionario   │  │ - Email         │  │
│  │ - Cliente       │  │ - Cliente       │  │ - Senha         │  │
│  │                 │  │                 │  │ - CPF           │  │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘  │
│                                                                 │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │   SERVICES      │  │ REPOSITORIES    │  │   EVENTS        │  │
│  │                 │  │                 │  │                 │  │
│  │ - AuthService   │  │ - Funcionario   │  │ - UserCreated   │  │
│  │ - JWTService    │  │   Repository    │  │ - UserLoggedIn  │  │
│  │ - HashService   │  │ - Cliente       │  │ - UserLoggedOut │  │
│  │                 │  │   Repository    │  │                 │  │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

#### 🍽️ Domínio de Menu (MenuService)
```
┌─────────────────────────────────────────────────────────────────┐
│                        MENU DOMAIN                             │
├─────────────────────────────────────────────────────────────────┤
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │   AGGREGATES    │  │   ENTITIES      │  │   VALUE OBJECTS │  │
│  │                 │  │                 │  │                 │  │
│  │ - Produto       │  │ - Produto       │  │ - Preco         │  │
│  │ - Categoria     │  │ - Categoria     │  │ - Nome          │  │
│  │                 │  │                 │  │ - Descricao     │  │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘  │
│                                                                 │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │   SERVICES      │  │ REPOSITORIES    │  │   EVENTS        │  │
│  │                 │  │                 │  │                 │  │
│  │ - MenuService   │  │ - Produto       │  │ - ProductCreated│  │
│  │ - SearchService │  │   Repository    │  │ - ProductUpdated│  │
│  │ - FilterService │  │                 │  │ - ProductDeleted│  │
│  │                 │  │                 │  │                 │  │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

#### 👨‍🍳 Domínio de Cozinha (KitchenService)
```
┌─────────────────────────────────────────────────────────────────┐
│                      KITCHEN DOMAIN                            │
├─────────────────────────────────────────────────────────────────┤
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │   AGGREGATES    │  │   ENTITIES      │  │   VALUE OBJECTS │  │
│  │                 │  │                 │  │                 │  │
│  │ - Pedido        │  │ - Pedido        │  │ - StatusPedido  │  │
│  │ - ItemPedido    │  │ - ItemPedido    │  │ - Observacoes   │  │
│  │                 │  │                 │  │ - TempoPreparo  │  │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘  │
│                                                                 │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │   SERVICES      │  │ REPOSITORIES    │  │   EVENTS        │  │
│  │                 │  │                 │  │                 │  │
│  │ - KitchenService│  │ - Pedido        │  │ - OrderAccepted │  │
│  │ - StatusService │  │   Repository    │  │ - OrderRejected │  │
│  │ - StatsService  │  │                 │  │ - OrderReady    │  │
│  │                 │  │                 │  │                 │  │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

#### 📦 Domínio de Pedidos (OrderService)
```
┌─────────────────────────────────────────────────────────────────┐
│                       ORDER DOMAIN                             │
├─────────────────────────────────────────────────────────────────┤
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │   AGGREGATES    │  │   ENTITIES      │  │   VALUE OBJECTS │  │
│  │                 │  │                 │  │                 │  │
│  │ - Pedido        │  │ - Pedido        │  │ - ValorTotal    │  │
│  │ - Cliente       │  │ - Cliente       │  │ - FormaEntrega  │  │
│  │ - ItemPedido    │  │ - ItemPedido    │  │ - Endereco      │  │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘  │
│                                                                 │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐  │
│  │   SERVICES      │  │ REPOSITORIES    │  │   EVENTS        │  │
│  │                 │  │                 │  │                 │  │
│  │ - OrderService  │  │ - Pedido        │  │ - OrderCreated  │  │
│  │ - MenuService   │  │   Repository    │  │ - OrderCanceled │  │
│  │   Client        │  │ - Cliente       │  │ - OrderUpdated  │  │
│  │ - MessageService│  │   Repository    │  │                 │  │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘  │
└─────────────────────────────────────────────────────────────────┘
```

### 3. Mapeamento de Entidades e Agregados

#### 🔐 AuthService - Entidades e Agregados
```csharp
// AGGREGATE ROOT
public class Funcionario
{
    public string Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string Senha { get; set; }
    public string Cargo { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
}

// AGGREGATE ROOT
public class Cliente
{
    public string Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string CPF { get; set; }
    public string Telefone { get; set; }
    public string Endereco { get; set; }
    public string CEP { get; set; }
    public string Cidade { get; set; }
    public string Estado { get; set; }
    public string Senha { get; set; }
    public bool Ativo { get; set; }
    public DateTime DataCriacao { get; set; }
}

// VALUE OBJECTS
public class Email
{
    public string Value { get; private set; }
    public Email(string value) { /* validation */ }
}

public class Senha
{
    public string Hash { get; private set; }
    public Senha(string senha) { /* hash */ }
}
```

#### 🍽️ MenuService - Entidades e Agregados
```csharp
// AGGREGATE ROOT
public class Produto
{
    public string Id { get; set; }
    public string Nome { get; set; }
    public string Descricao { get; set; }
    public decimal Preco { get; set; }
    public CategoriaProduto Categoria { get; set; }
    public bool Disponivel { get; set; }
    public bool Destaque { get; set; }
    public List<string> Ingredientes { get; set; }
    public List<string> Alergenos { get; set; }
    public DateTime DataCriacao { get; set; }
}

// VALUE OBJECTS
public enum CategoriaProduto
{
    Bebidas,
    Pratos,
    Sobremesas,
    Acompanhamentos,
    Saladas,
    Sanduiches
}

public class Preco
{
    public decimal Valor { get; private set; }
    public string Moeda { get; private set; }
    public Preco(decimal valor) { /* validation */ }
}
```

#### 👨‍🍳 KitchenService - Entidades e Agregados
```csharp
// AGGREGATE ROOT
public class Pedido
{
    public string Id { get; set; }
    public string NumeroPedido { get; set; }
    public string ClienteId { get; set; }
    public string ClienteNome { get; set; }
    public string ClienteTelefone { get; set; }
    public List<ItemPedido> Itens { get; set; }
    public StatusPedido Status { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime DataCriacao { get; set; }
    public List<HistoricoStatus> HistoricoStatus { get; set; }
    public string? Observacoes { get; set; }
    public string? FuncionarioId { get; set; }
    public string? FuncionarioNome { get; set; }
}

// ENTITY
public class ItemPedido
{
    public string ProdutoId { get; set; }
    public string Nome { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal Subtotal { get; set; }
}

// VALUE OBJECTS
public enum StatusPedido
{
    Criado,
    Confirmado,
    EmPreparo,
    Pronto,
    Entregue,
    Cancelado
}

public class HistoricoStatus
{
    public StatusPedido Status { get; set; }
    public DateTime Data { get; set; }
    public string? FuncionarioId { get; set; }
    public string? FuncionarioNome { get; set; }
    public string? Observacoes { get; set; }
}
```

#### 📦 OrderService - Entidades e Agregados
```csharp
// AGGREGATE ROOT
public class Pedido
{
    public string Id { get; set; }
    public string NumeroPedido { get; set; }
    public string ClienteId { get; set; }
    public string ClienteNome { get; set; }
    public string ClienteTelefone { get; set; }
    public string ClienteEmail { get; set; }
    public List<ItemPedido> Itens { get; set; }
    public StatusPedido Status { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal TaxaEntrega { get; set; }
    public FormaEntrega FormaEntrega { get; set; }
    public string? EnderecoEntrega { get; set; }
    public DateTime DataCriacao { get; set; }
}

// AGGREGATE ROOT
public class Cliente
{
    public string Id { get; set; }
    public string Nome { get; set; }
    public string Email { get; set; }
    public string CPF { get; set; }
    public string Telefone { get; set; }
    public string Endereco { get; set; }
    public string CEP { get; set; }
    public string Cidade { get; set; }
    public string Estado { get; set; }
    public bool Ativo { get; set; }
    public List<string> PedidosIds { get; set; }
    public DateTime DataCriacao { get; set; }
}

// VALUE OBJECTS
public enum FormaEntrega
{
    Balcao,
    DriveThru,
    Delivery
}
```

### 4. Fluxo de Comunicação entre Domínios

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                           CROSS-DOMAIN COMMUNICATION                           │
└─────────────────────────────────────────────────────────────────────────────────┘

┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│ AuthService │    │MenuService  │    │KitchenService│    │OrderService │
│             │    │             │    │             │    │             │
│ 1. Login    │    │ 2. Produtos │    │ 3. Pedidos  │    │ 4. Pedidos  │
│    JWT      │    │   Menu      │    │   Status    │    │   Cliente   │
└─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘
       │                   │                   │                   │
       │                   │                   │                   │
       ▼                   ▼                   ▼                   ▼
┌─────────────┐    ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
│   JWT       │    │  HTTP GET   │    │ RabbitMQ    │    │ RabbitMQ    │
│  Token      │    │ /api/menu/  │    │ Consumer    │    │ Publisher   │
│ Validation  │    │ produtos    │    │ pedidos_    │    │ pedidos_    │
│             │    │             │    │ queue       │    │ queue       │
└─────────────┘    └─────────────┘    └─────────────┘    └─────────────┘

FLUXO DE PEDIDO:
1. Cliente faz login (AuthService)
2. Cliente busca produtos (MenuService)
3. Cliente cria pedido (OrderService)
4. OrderService publica mensagem (RabbitMQ)
5. KitchenService consome mensagem (RabbitMQ)
6. Cozinha atualiza status (KitchenService)
```

### 5. Eventos de Domínio

#### 🔐 AuthService Events
```csharp
public class UserCreatedEvent
{
    public string UserId { get; set; }
    public string UserType { get; set; } // Funcionario | Cliente
    public DateTime CreatedAt { get; set; }
}

public class UserLoggedInEvent
{
    public string UserId { get; set; }
    public string UserType { get; set; }
    public DateTime LoggedInAt { get; set; }
}

public class UserLoggedOutEvent
{
    public string UserId { get; set; }
    public DateTime LoggedOutAt { get; set; }
}
```

#### 🍽️ MenuService Events
```csharp
public class ProductCreatedEvent
{
    public string ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ProductUpdatedEvent
{
    public string ProductId { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
    public bool Available { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ProductDeletedEvent
{
    public string ProductId { get; set; }
    public DateTime DeletedAt { get; set; }
}
```

#### 👨‍🍳 KitchenService Events
```csharp
public class OrderAcceptedEvent
{
    public string OrderId { get; set; }
    public string EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public DateTime AcceptedAt { get; set; }
}

public class OrderRejectedEvent
{
    public string OrderId { get; set; }
    public string EmployeeId { get; set; }
    public string EmployeeName { get; set; }
    public string Reason { get; set; }
    public DateTime RejectedAt { get; set; }
}

public class OrderStatusChangedEvent
{
    public string OrderId { get; set; }
    public string OldStatus { get; set; }
    public string NewStatus { get; set; }
    public string EmployeeId { get; set; }
    public DateTime ChangedAt { get; set; }
}
```

#### 📦 OrderService Events
```csharp
public class OrderCreatedEvent
{
    public string OrderId { get; set; }
    public string CustomerId { get; set; }
    public string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class OrderCanceledEvent
{
    public string OrderId { get; set; }
    public string CustomerId { get; set; }
    public string Reason { get; set; }
    public DateTime CanceledAt { get; set; }
}
```

### 6. Repositórios e Persistência

#### 🔐 AuthService Repositories
```csharp
public interface IFuncionarioRepository
{
    Task<Funcionario> GetByIdAsync(string id);
    Task<Funcionario> GetByEmailAsync(string email);
    Task<IEnumerable<Funcionario>> GetAllAsync();
    Task<Funcionario> CreateAsync(Funcionario funcionario);
    Task<Funcionario> UpdateAsync(Funcionario funcionario);
    Task DeleteAsync(string id);
}

public interface IClienteRepository
{
    Task<Cliente> GetByIdAsync(string id);
    Task<Cliente> GetByEmailAsync(string email);
    Task<Cliente> GetByCPFAsync(string cpf);
    Task<IEnumerable<Cliente>> GetAllAsync();
    Task<Cliente> CreateAsync(Cliente cliente);
    Task<Cliente> UpdateAsync(Cliente cliente);
    Task DeleteAsync(string id);
}
```

#### 🍽️ MenuService Repositories
```csharp
public interface IProdutoRepository
{
    Task<Produto> GetByIdAsync(string id);
    Task<IEnumerable<Produto>> GetAllAsync();
    Task<IEnumerable<Produto>> GetByCategoryAsync(CategoriaProduto categoria);
    Task<IEnumerable<Produto>> SearchAsync(string searchTerm);
    Task<IEnumerable<Produto>> GetAvailableAsync();
    Task<IEnumerable<Produto>> GetFeaturedAsync();
    Task<Produto> CreateAsync(Produto produto);
    Task<Produto> UpdateAsync(Produto produto);
    Task DeleteAsync(string id);
}
```

#### 👨‍🍳 KitchenService Repositories
```csharp
public interface IPedidoRepository
{
    Task<Pedido> GetByIdAsync(string id);
    Task<IEnumerable<Pedido>> GetAllAsync();
    Task<IEnumerable<Pedido>> GetPendingAsync();
    Task<IEnumerable<Pedido>> GetByStatusAsync(StatusPedido status);
    Task<Pedido> CreateAsync(Pedido pedido);
    Task<Pedido> UpdateAsync(Pedido pedido);
    Task<Pedido> UpdateStatusAsync(string id, StatusPedido status, string funcionarioId, string funcionarioNome, string observacoes);
    Task DeleteAsync(string id);
}
```

#### 📦 OrderService Repositories
```csharp
public interface IPedidoRepository
{
    Task<Pedido> GetByIdAsync(string id);
    Task<IEnumerable<Pedido>> GetByCustomerAsync(string customerId);
    Task<IEnumerable<Pedido>> GetAllAsync();
    Task<Pedido> CreateAsync(Pedido pedido);
    Task<Pedido> UpdateAsync(Pedido pedido);
    Task<Pedido> CancelAsync(string id);
    Task DeleteAsync(string id);
}

public interface IClienteRepository
{
    Task<Cliente> GetByIdAsync(string id);
    Task<Cliente> GetByEmailAsync(string email);
    Task<Cliente> GetByCPFAsync(string cpf);
    Task<IEnumerable<Cliente>> GetAllAsync();
    Task<Cliente> CreateAsync(Cliente cliente);
    Task<Cliente> UpdateAsync(Cliente cliente);
    Task DeleteAsync(string id);
}
```

### 7. Serviços de Aplicação

#### 🔐 AuthService Services
```csharp
public interface IAuthService
{
    Task<AuthResponse> LoginFuncionarioAsync(LoginRequest request);
    Task<AuthResponse> LoginClienteAsync(LoginRequest request);
    Task<AuthResponse> RegisterFuncionarioAsync(FuncionarioCreateRequest request);
    Task<AuthResponse> RegisterClienteAsync(ClienteRequest request);
    Task<UserInfo> GetUserInfoAsync(string token);
}

public interface IJwtService
{
    string GenerateToken(UserInfo user);
    UserInfo ValidateToken(string token);
    bool IsTokenExpired(string token);
}

public interface IHashService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
}
```

#### 🍽️ MenuService Services
```csharp
public interface IMenuService
{
    Task<IEnumerable<Produto>> GetAllProductsAsync();
    Task<Produto> GetProductByIdAsync(string id);
    Task<IEnumerable<Produto>> SearchProductsAsync(string searchTerm);
    Task<IEnumerable<Produto>> GetProductsByCategoryAsync(CategoriaProduto category);
    Task<IEnumerable<Produto>> GetAvailableProductsAsync();
    Task<Produto> CreateProductAsync(ProdutoRequest request);
    Task<Produto> UpdateProductAsync(string id, ProdutoRequest request);
    Task DeleteProductAsync(string id);
}
```

#### 👨‍🍳 KitchenService Services
```csharp
public interface IKitchenService
{
    Task<IEnumerable<Pedido>> GetAllOrdersAsync();
    Task<IEnumerable<Pedido>> GetPendingOrdersAsync();
    Task<Pedido> GetOrderByIdAsync(string id);
    Task<Pedido> AcceptOrderAsync(string id, string funcionarioId, string funcionarioNome, string observacoes);
    Task<Pedido> RejectOrderAsync(string id, string funcionarioId, string funcionarioNome, string reason);
    Task<Pedido> UpdateOrderStatusAsync(string id, StatusPedido status, string funcionarioId, string funcionarioNome, string observacoes);
    Task<KitchenStats> GetKitchenStatsAsync();
}
```

#### 📦 OrderService Services
```csharp
public interface IOrderService
{
    Task<Pedido> CreateOrderAsync(PedidoRequest request);
    Task<Pedido> GetOrderByIdAsync(string id);
    Task<IEnumerable<Pedido>> GetOrdersByCustomerAsync(string customerId);
    Task<Pedido> CancelOrderAsync(string id);
    Task<Pedido> UpdateOrderStatusAsync(string id, StatusPedido status);
    Task<IEnumerable<Produto>> GetAvailableProductsAsync();
}

public interface IMenuServiceClient
{
    Task<IEnumerable<Produto>> GetProductsAsync();
    Task<Produto> GetProductByIdAsync(string id);
    Task<IEnumerable<Produto>> GetProductsByIdsAsync(List<string> ids);
}

public interface IMessageService
{
    Task PublishOrderCreatedAsync(Pedido pedido);
    Task PublishOrderCanceledAsync(string orderId, string reason);
    Task PublishOrderStatusChangedAsync(string orderId, StatusPedido status);
}
```

### 8. Configurações e Infraestrutura

#### Configurações por Domínio
```json
{
  "AuthSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "fasttech_auth",
    "JwtSecret": "sua_chave_secreta_aqui",
    "JwtIssuer": "FastTech Foods",
    "JwtAudience": "FastTech Users",
    "JwtExpirationHours": 24
  },
  "MenuSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "fasttech_menu",
    "ProdutosCollectionName": "produtos"
  },
  "KitchenSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "fasttech_kitchen",
    "PedidosCollectionName": "pedidos",
    "RabbitMQHost": "localhost",
    "RabbitMQPort": 5672,
    "RabbitMQUser": "admin",
    "RabbitMQPassword": "password123"
  },
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

### 9. Princípios DDD Aplicados

#### ✅ Bounded Contexts
- **AuthContext**: Autenticação e autorização
- **MenuContext**: Gestão de produtos e menu
- **KitchenContext**: Gestão de pedidos na cozinha
- **OrderContext**: Criação e gestão de pedidos

#### ✅ Aggregates
- **Funcionario**: Raiz do agregado de funcionários
- **Cliente**: Raiz do agregado de clientes
- **Produto**: Raiz do agregado de produtos
- **Pedido**: Raiz do agregado de pedidos

#### ✅ Entities
- **ItemPedido**: Entidade dentro do agregado Pedido
- **HistoricoStatus**: Entidade dentro do agregado Pedido

#### ✅ Value Objects
- **Email**: Valor imutável para emails
- **Senha**: Valor imutável para senhas hash
- **Preco**: Valor imutável para preços
- **StatusPedido**: Enum como value object
- **CategoriaProduto**: Enum como value object

#### ✅ Repositories
- Interfaces bem definidas para cada agregado
- Implementação específica para MongoDB
- Separação clara de responsabilidades

#### ✅ Domain Services
- Serviços específicos de domínio
- Lógica de negócio encapsulada
- Integração entre bounded contexts

#### ✅ Domain Events
- Eventos de domínio para comunicação
- Desacoplamento entre serviços
- Auditoria e rastreabilidade

---

## 🎯 Conclusão

A arquitetura DDD implementada no FastTech Foods MVP segue os princípios fundamentais:

1. **Separação clara de domínios** com bounded contexts bem definidos
2. **Agregados como unidades de consistência** com raízes claras
3. **Entidades e value objects** bem modelados
4. **Repositórios** para persistência desacoplada
5. **Serviços de domínio** para lógica de negócio
6. **Eventos de domínio** para comunicação assíncrona
7. **Infraestrutura** separada da lógica de negócio

Esta arquitetura permite:
- ✅ **Escalabilidade** horizontal dos serviços
- ✅ **Manutenibilidade** com responsabilidades claras
- ✅ **Testabilidade** com dependências bem definidas
- ✅ **Flexibilidade** para evolução do sistema
- ✅ **Performance** com otimizações específicas por domínio

O sistema está pronto para produção e pode ser expandido seguindo os mesmos princípios DDD. 