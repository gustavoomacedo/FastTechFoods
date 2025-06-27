using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Repositories;
using OrderService.Services;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly PedidoRepository _pedidoRepository;
    private readonly ClienteRepository _clienteRepository;
    private readonly MenuServiceClient _menuServiceClient;
    private readonly ILogger<TestController> _logger;

    public TestController(
        PedidoRepository pedidoRepository,
        ClienteRepository clienteRepository,
        MenuServiceClient menuServiceClient,
        ILogger<TestController> logger)
    {
        _pedidoRepository = pedidoRepository;
        _clienteRepository = clienteRepository;
        _menuServiceClient = menuServiceClient;
        _logger = logger;
    }

    /// <summary>
    /// Cria clientes de teste
    /// </summary>
    [HttpPost("clientes")]
    public async Task<ActionResult<List<Cliente>>> CriarClientesTeste()
    {
        try
        {
            var clientes = new List<Cliente>
            {
                new Cliente
                {
                    Nome = "João Silva",
                    Email = "joao.silva@email.com",
                    CPF = "12345678901",
                    Telefone = "(11) 99999-1111",
                    Endereco = "Rua das Flores, 123",
                    Complemento = "Apto 45",
                    CEP = "01234-567",
                    Cidade = "São Paulo",
                    Estado = "SP"
                },
                new Cliente
                {
                    Nome = "Maria Santos",
                    Email = "maria.santos@email.com",
                    CPF = "98765432100",
                    Telefone = "(11) 88888-2222",
                    Endereco = "Av. Paulista, 1000",
                    Complemento = "Sala 101",
                    CEP = "01310-100",
                    Cidade = "São Paulo",
                    Estado = "SP"
                },
                new Cliente
                {
                    Nome = "Pedro Oliveira",
                    Email = "pedro.oliveira@email.com",
                    CPF = "11122233344",
                    Telefone = "(11) 77777-3333",
                    Endereco = "Rua Augusta, 500",
                    CEP = "01412-000",
                    Cidade = "São Paulo",
                    Estado = "SP"
                }
            };

            var clientesCriados = new List<Cliente>();
            foreach (var cliente in clientes)
            {
                var clienteCriado = await _clienteRepository.CriarAsync(cliente);
                clientesCriados.Add(clienteCriado);
            }

            _logger.LogInformation("Criados {Count} clientes de teste", clientesCriados.Count);
            return Ok(clientesCriados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar clientes de teste");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Cria pedidos de teste
    /// </summary>
    [HttpPost("pedidos")]
    public async Task<ActionResult<List<Pedido>>> CriarPedidosTeste()
    {
        try
        {
            // Primeiro, obter produtos do MenuService
            var produtos = await _menuServiceClient.ObterProdutosDisponiveisAsync();
            if (!produtos.Any())
            {
                return BadRequest("Nenhum produto disponível no MenuService. Crie produtos primeiro.");
            }

            // Obter clientes
            var clientes = await _clienteRepository.ObterTodosAsync();
            if (!clientes.Any())
            {
                return BadRequest("Nenhum cliente cadastrado. Crie clientes primeiro.");
            }

            var pedidos = new List<Pedido>();
            var random = new Random();

            for (int i = 0; i < 3; i++)
            {
                var cliente = clientes[random.Next(clientes.Count)];
                var produtosPedido = produtos.Take(random.Next(1, 4)).ToList();

                var itensPedido = new List<ItemPedido>();
                decimal subtotal = 0;

                foreach (var produto in produtosPedido)
                {
                    var quantidade = random.Next(1, 4);
                    var precoTotal = produto.Preco * quantidade;
                    subtotal += precoTotal;

                    itensPedido.Add(new ItemPedido
                    {
                        ProdutoId = produto.Id,
                        ProdutoNome = produto.Nome,
                        Quantidade = quantidade,
                        PrecoUnitario = produto.Preco,
                        PrecoTotal = precoTotal,
                        Observacoes = $"Pedido de teste {i + 1}"
                    });
                }

                var formaEntrega = (FormaEntrega)random.Next(3);
                var taxaEntrega = formaEntrega == FormaEntrega.Delivery ? 5.00m : 0;
                var valorTotal = subtotal + taxaEntrega;

                var pedido = new Pedido
                {
                    ClienteId = cliente.Id,
                    ClienteNome = cliente.Nome,
                    ClienteTelefone = cliente.Telefone,
                    ClienteEmail = cliente.Email,
                    Itens = itensPedido,
                    Subtotal = subtotal,
                    TaxaEntrega = taxaEntrega,
                    ValorTotal = valorTotal,
                    FormaEntrega = formaEntrega,
                    EnderecoEntrega = cliente.Endereco,
                    ComplementoEntrega = cliente.Complemento,
                    Status = StatusPedido.Criado,
                    Observacoes = $"Pedido de teste {i + 1} criado automaticamente"
                };

                var pedidoCriado = await _pedidoRepository.CriarAsync(pedido);
                await _clienteRepository.AdicionarPedidoAsync(cliente.Id, pedidoCriado.Id);
                pedidos.Add(pedidoCriado);
            }

            _logger.LogInformation("Criados {Count} pedidos de teste", pedidos.Count);
            return Ok(pedidos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedidos de teste");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém todos os clientes (sem autenticação para testes)
    /// </summary>
    [HttpGet("clientes")]
    public async Task<ActionResult<List<Cliente>>> ObterTodosClientes()
    {
        try
        {
            var clientes = await _clienteRepository.ObterTodosAsync();
            return Ok(clientes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todos os clientes");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém todos os pedidos (sem autenticação para testes)
    /// </summary>
    [HttpGet("pedidos")]
    public async Task<ActionResult<List<Pedido>>> ObterTodosPedidos()
    {
        try
        {
            var pedidos = await _pedidoRepository.ObterTodosAsync();
            return Ok(pedidos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todos os pedidos");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém produtos do MenuService (sem autenticação para testes)
    /// </summary>
    [HttpGet("produtos")]
    public async Task<ActionResult<List<ProdutoMenu>>> ObterProdutos()
    {
        try
        {
            var produtos = await _menuServiceClient.ObterProdutosDisponiveisAsync();
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produtos do MenuService");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Limpa todos os dados de teste
    /// </summary>
    [HttpDelete("limpar")]
    public async Task<ActionResult> LimparDadosTeste()
    {
        try
        {
            var clientes = await _clienteRepository.ObterTodosAsync();
            var pedidos = await _pedidoRepository.ObterTodosAsync();

            int clientesDeletados = 0;
            int pedidosDeletados = 0;

            // Deletar pedidos primeiro
            foreach (var pedido in pedidos)
            {
                await _pedidoRepository.DeletarAsync(pedido.Id);
                pedidosDeletados++;
            }

            // Deletar clientes
            foreach (var cliente in clientes)
            {
                await _clienteRepository.DeletarAsync(cliente.Id);
                clientesDeletados++;
            }

            _logger.LogInformation("Deletados {Pedidos} pedidos e {Clientes} clientes de teste", 
                pedidosDeletados, clientesDeletados);

            return Ok(new { 
                message = $"Deletados {pedidosDeletados} pedidos e {clientesDeletados} clientes de teste" 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao limpar dados de teste");
            return StatusCode(500, "Erro interno do servidor");
        }
    }
} 