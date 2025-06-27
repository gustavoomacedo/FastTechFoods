using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Repositories;
using OrderService.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly PedidoRepository _pedidoRepository;
    private readonly ClienteRepository _clienteRepository;
    private readonly MenuServiceClient _menuServiceClient;
    private readonly MessageService _messageService;
    private readonly ILogger<OrderController> _logger;

    public OrderController(
        PedidoRepository pedidoRepository,
        ClienteRepository clienteRepository,
        MenuServiceClient menuServiceClient,
        MessageService messageService,
        ILogger<OrderController> logger)
    {
        _pedidoRepository = pedidoRepository;
        _clienteRepository = clienteRepository;
        _menuServiceClient = menuServiceClient;
        _messageService = messageService;
        _logger = logger;
    }

    /// <summary>
    /// Cria um novo pedido
    /// </summary>
    [HttpPost("pedidos")]
    public async Task<ActionResult<Pedido>> CriarPedido([FromBody] PedidoCreateRequest request)
    {
        try
        {
            // Validar cliente
            var cliente = await _clienteRepository.ObterPorIdAsync(request.ClienteId);
            if (cliente == null)
            {
                return BadRequest("Cliente não encontrado");
            }

            if (!cliente.Ativo)
            {
                return BadRequest("Cliente inativo");
            }

            // Obter produtos do MenuService
            var produtoIds = request.Itens.Select(i => i.ProdutoId).ToList();
            var produtos = await _menuServiceClient.ObterProdutosPorIdsAsync(produtoIds);

            if (produtos.Count != produtoIds.Count)
            {
                return BadRequest("Alguns produtos não foram encontrados");
            }

            // Validar disponibilidade dos produtos
            var produtosIndisponiveis = produtos.Where(p => !p.Disponivel).ToList();
            if (produtosIndisponiveis.Any())
            {
                var nomes = string.Join(", ", produtosIndisponiveis.Select(p => p.Nome));
                return BadRequest($"Produtos indisponíveis: {nomes}");
            }

            // Criar itens do pedido
            var itensPedido = new List<ItemPedido>();
            decimal subtotal = 0;

            foreach (var itemRequest in request.Itens)
            {
                var produto = produtos.First(p => p.Id == itemRequest.ProdutoId);
                var precoTotal = produto.Preco * itemRequest.Quantidade;
                subtotal += precoTotal;

                itensPedido.Add(new ItemPedido
                {
                    ProdutoId = produto.Id,
                    ProdutoNome = produto.Nome,
                    Quantidade = itemRequest.Quantidade,
                    PrecoUnitario = produto.Preco,
                    PrecoTotal = precoTotal,
                    Observacoes = itemRequest.Observacoes,
                    Opcionais = itemRequest.Opcionais
                });
            }

            // Calcular valor total
            var valorTotal = subtotal + request.TaxaEntrega - request.Desconto;

            // Criar pedido
            var pedido = new Pedido
            {
                ClienteId = cliente.Id,
                ClienteNome = cliente.Nome,
                ClienteTelefone = cliente.Telefone,
                ClienteEmail = cliente.Email,
                Itens = itensPedido,
                Subtotal = subtotal,
                TaxaEntrega = request.TaxaEntrega,
                Desconto = request.Desconto,
                ValorTotal = valorTotal,
                FormaEntrega = request.FormaEntrega,
                EnderecoEntrega = request.EnderecoEntrega,
                ComplementoEntrega = request.ComplementoEntrega,
                Observacoes = request.Observacoes,
                FormaPagamento = request.FormaPagamento,
                Status = StatusPedido.Criado
            };

            var pedidoCriado = await _pedidoRepository.CriarAsync(pedido);

            // Adicionar pedido ao cliente
            await _clienteRepository.AdicionarPedidoAsync(cliente.Id, pedidoCriado.Id);

            // Publicar mensagem de pedido criado
            _messageService.PublicarPedidoCriado(pedidoCriado);

            _logger.LogInformation("Pedido criado: {NumeroPedido} para cliente {ClienteId}", 
                pedidoCriado.NumeroPedido, cliente.Id);

            return CreatedAtAction(nameof(ObterPedidoPorId), new { id = pedidoCriado.Id }, pedidoCriado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar pedido");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém um pedido por ID
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
    /// Obtém pedido por número
    /// </summary>
    [HttpGet("pedidos/numero/{numeroPedido}")]
    public async Task<ActionResult<Pedido>> ObterPedidoPorNumero(string numeroPedido)
    {
        try
        {
            var pedido = await _pedidoRepository.ObterPorNumeroAsync(numeroPedido);
            if (pedido == null)
            {
                return NotFound("Pedido não encontrado");
            }
            return Ok(pedido);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedido por número: {NumeroPedido}", numeroPedido);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém pedidos de um cliente
    /// </summary>
    [HttpGet("pedidos/cliente/{clienteId}")]
    public async Task<ActionResult<List<Pedido>>> ObterPedidosPorCliente(string clienteId)
    {
        try
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(clienteId);
            if (cliente == null)
            {
                return NotFound("Cliente não encontrado");
            }

            var pedidos = await _pedidoRepository.ObterPorClienteAsync(clienteId);
            return Ok(pedidos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedidos do cliente: {ClienteId}", clienteId);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Cancela um pedido
    /// </summary>
    [HttpPost("pedidos/{id}/cancelar")]
    public async Task<ActionResult> CancelarPedido(string id, [FromBody] PedidoCancelRequest request)
    {
        try
        {
            var pedido = await _pedidoRepository.ObterPorIdAsync(id);
            if (pedido == null)
            {
                return NotFound("Pedido não encontrado");
            }

            // Verificar se o cliente é o dono do pedido
            if (pedido.ClienteId != request.ClienteId)
            {
                return Forbid("Você não tem permissão para cancelar este pedido");
            }

            var sucesso = await _pedidoRepository.CancelarPedidoAsync(id, request.MotivoCancelamento, request.ClienteId);
            
            if (sucesso)
            {
                // Publicar mensagem de pedido cancelado
                _messageService.PublicarPedidoCancelado(pedido, request.MotivoCancelamento);
                
                _logger.LogInformation("Pedido cancelado: {NumeroPedido} pelo cliente {ClienteId}", 
                    pedido.NumeroPedido, request.ClienteId);
                return Ok(new { message = "Pedido cancelado com sucesso" });
            }

            return BadRequest("Não foi possível cancelar o pedido");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cancelar pedido: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Atualiza status do pedido (requer autenticação)
    /// </summary>
    [HttpPut("pedidos/{id}/status")]
    [Authorize]
    public async Task<ActionResult> AtualizarStatusPedido(string id, [FromBody] PedidoUpdateRequest request)
    {
        try
        {
            var funcionarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var funcionarioNome = User.FindFirst(ClaimTypes.Name)?.Value;

            var pedido = await _pedidoRepository.ObterPorIdAsync(id);
            if (pedido == null)
            {
                return NotFound("Pedido não encontrado");
            }

            var sucesso = await _pedidoRepository.AtualizarAsync(id, request);
            
            if (sucesso && request.Status.HasValue)
            {
                await _pedidoRepository.AtualizarStatusAsync(id, request.Status.Value, funcionarioId, funcionarioNome, request.Observacoes);
            }

            if (sucesso)
            {
                // Publicar mensagem de pedido atualizado
                if (request.Status.HasValue)
                {
                    _messageService.PublicarPedidoAtualizado(pedido, request.Status.Value.ToString());
                }
                
                _logger.LogInformation("Pedido atualizado: {NumeroPedido} pelo funcionário {FuncionarioId}", 
                    pedido.NumeroPedido, funcionarioId);
                return Ok(new { message = "Pedido atualizado com sucesso" });
            }

            return BadRequest("Não foi possível atualizar o pedido");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar pedido: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém todos os pedidos (requer autenticação)
    /// </summary>
    [HttpGet("pedidos")]
    [Authorize]
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
    /// Obtém pedidos por status (requer autenticação)
    /// </summary>
    [HttpGet("pedidos/status/{status}")]
    [Authorize]
    public async Task<ActionResult<List<Pedido>>> ObterPedidosPorStatus(StatusPedido status)
    {
        try
        {
            var pedidos = await _pedidoRepository.ObterPorStatusAsync(status);
            return Ok(pedidos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedidos por status: {Status}", status);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém pedidos recentes (requer autenticação)
    /// </summary>
    [HttpGet("pedidos/recentes")]
    [Authorize]
    public async Task<ActionResult<List<Pedido>>> ObterPedidosRecentes([FromQuery] int limite = 10)
    {
        try
        {
            var pedidos = await _pedidoRepository.ObterPedidosRecentesAsync(limite);
            return Ok(pedidos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedidos recentes");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém estatísticas dos pedidos (requer autenticação)
    /// </summary>
    [HttpGet("estatisticas")]
    [Authorize]
    public async Task<ActionResult<object>> ObterEstatisticas()
    {
        try
        {
            var criados = await _pedidoRepository.ContarPorStatusAsync(StatusPedido.Criado);
            var confirmados = await _pedidoRepository.ContarPorStatusAsync(StatusPedido.Confirmado);
            var emPreparo = await _pedidoRepository.ContarPorStatusAsync(StatusPedido.EmPreparo);
            var prontos = await _pedidoRepository.ContarPorStatusAsync(StatusPedido.Pronto);
            var entregues = await _pedidoRepository.ContarPorStatusAsync(StatusPedido.Entregue);
            var cancelados = await _pedidoRepository.ContarPorStatusAsync(StatusPedido.Cancelado);

            // Calcular receita do dia
            var hoje = DateTime.Today;
            var amanha = hoje.AddDays(1);
            var receitaHoje = await _pedidoRepository.CalcularReceitaPorPeriodoAsync(hoje, amanha);

            return Ok(new
            {
                porStatus = new
                {
                    criados,
                    confirmados,
                    emPreparo,
                    prontos,
                    entregues,
                    cancelados
                },
                receitaHoje,
                total = criados + confirmados + emPreparo + prontos + entregues + cancelados
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter estatísticas");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém produtos disponíveis do MenuService
    /// </summary>
    [HttpGet("produtos")]
    public async Task<ActionResult<List<ProdutoMenu>>> ObterProdutosDisponiveis()
    {
        try
        {
            var produtos = await _menuServiceClient.ObterProdutosDisponiveisAsync();
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produtos disponíveis");
            return StatusCode(500, "Erro interno do servidor");
        }
    }
} 