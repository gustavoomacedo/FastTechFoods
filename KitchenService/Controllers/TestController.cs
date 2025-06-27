using Microsoft.AspNetCore.Mvc;
using KitchenService.Models;
using KitchenService.Repositories;

namespace KitchenService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly PedidoRepository _pedidoRepository;
    private readonly ILogger<TestController> _logger;

    public TestController(PedidoRepository pedidoRepository, ILogger<TestController> logger)
    {
        _pedidoRepository = pedidoRepository;
        _logger = logger;
    }

    /// <summary>
    /// Cria um pedido de teste
    /// </summary>
    [HttpPost("pedidos")]
    public async Task<ActionResult<Pedido>> CriarPedidoTeste()
    {
        try
        {
            var pedido = new Pedido
            {
                NumeroPedido = $"PED-{DateTime.Now:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}",
                DataCriacao = DateTime.UtcNow,
                ClienteNome = "Cliente Teste",
                ClienteTelefone = "(11) 99999-9999",
                EnderecoEntrega = "Rua Teste, 123 - São Paulo, SP",
                ValorTotal = 45.50m,
                Status = StatusPedido.Pendente,
                Observacoes = "Pedido de teste criado automaticamente",
                Itens = new List<ItemPedido>
                {
                    new ItemPedido
                    {
                        Nome = "X-Burger",
                        Quantidade = 2,
                        PrecoUnitario = 15.00m,
                        Observacoes = "Sem cebola"
                    },
                    new ItemPedido
                    {
                        Nome = "Batata Frita",
                        Quantidade = 1,
                        PrecoUnitario = 12.00m
                    },
                    new ItemPedido
                    {
                        Nome = "Refrigerante",
                        Quantidade = 1,
                        PrecoUnitario = 3.50m
                    }
                }
            };

            var pedidoCriado = await _pedidoRepository.CriarAsync(pedido);
            _logger.LogInformation("Pedido de teste criado: {NumeroPedido}", pedidoCriado.NumeroPedido);
            
            return CreatedAtAction(nameof(ObterPedidoPorId), new { id = pedidoCriado.Id }, pedidoCriado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedido de teste");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém um pedido por ID (sem autenticação para testes)
    /// </summary>
    [HttpGet("pedidos/{id}")]
    public async Task<ActionResult<Pedido>> ObterPedidoPorId(string id)
    {
        try
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(id);
            if (pedido == null)
            {
                return NotFound("Pedido não encontrado");
            }
            return Ok(pedido);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedido por ID: {Id}", id);
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
    /// Limpa todos os pedidos de teste
    /// </summary>
    [HttpDelete("pedidos")]
    public async Task<ActionResult> LimparPedidosTeste()
    {
        try
        {
            var pedidos = await _pedidoRepository.ObterTodosAsync();
            var pedidosTeste = pedidos.Where(p => p.ClienteNome == "Cliente Teste").ToList();
            
            int deletados = 0;
            foreach (var pedido in pedidosTeste)
            {
                await _pedidoRepository.DeletarAsync(pedido.Id);
                deletados++;
            }

            _logger.LogInformation("Deletados {Count} pedidos de teste", deletados);
            return Ok(new { message = $"Deletados {deletados} pedidos de teste" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao limpar pedidos de teste");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Cria múltiplos pedidos de teste
    /// </summary>
    [HttpPost("pedidos/multiplos")]
    public async Task<ActionResult<List<Pedido>>> CriarMultiplosPedidosTeste([FromQuery] int quantidade = 5)
    {
        try
        {
            var pedidosCriados = new List<Pedido>();
            
            for (int i = 0; i < quantidade; i++)
            {
                var pedido = new Pedido
                {
                    NumeroPedido = $"PED-{DateTime.Now:yyyyMMdd}-{Random.Shared.Next(1000, 9999)}",
                    DataCriacao = DateTime.UtcNow.AddMinutes(-Random.Shared.Next(0, 60)),
                    ClienteNome = $"Cliente Teste {i + 1}",
                    ClienteTelefone = $"(11) 99999-{Random.Shared.Next(1000, 9999)}",
                    EnderecoEntrega = $"Rua Teste {i + 1}, {Random.Shared.Next(1, 999)} - São Paulo, SP",
                    ValorTotal = Random.Shared.Next(20, 100),
                    Status = StatusPedido.Pendente,
                    Observacoes = $"Pedido de teste {i + 1} criado automaticamente",
                    Itens = new List<ItemPedido>
                    {
                        new ItemPedido
                        {
                            Nome = "X-Burger",
                            Quantidade = Random.Shared.Next(1, 3),
                            PrecoUnitario = 15.00m
                        },
                        new ItemPedido
                        {
                            Nome = "Batata Frita",
                            Quantidade = Random.Shared.Next(0, 2),
                            PrecoUnitario = 12.00m
                        }
                    }
                };

                var pedidoCriado = await _pedidoRepository.CriarAsync(pedido);
                pedidosCriados.Add(pedidoCriado);
            }

            _logger.LogInformation("Criados {Count} pedidos de teste", pedidosCriados.Count);
            return Ok(pedidosCriados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar múltiplos pedidos de teste");
            return StatusCode(500, "Erro interno do servidor");
        }
    }
} 