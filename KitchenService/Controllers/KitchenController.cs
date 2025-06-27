using Microsoft.AspNetCore.Mvc;
using KitchenService.Models;
using KitchenService.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace KitchenService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class KitchenController : ControllerBase
{
    private readonly PedidoRepository _pedidoRepository;
    private readonly ILogger<KitchenController> _logger;

    public KitchenController(PedidoRepository pedidoRepository, ILogger<KitchenController> logger)
    {
        _pedidoRepository = pedidoRepository;
        _logger = logger;
    }

    /// <summary>
    /// Obtém todos os pedidos
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
    /// Obtém pedidos por status
    /// </summary>
    [HttpGet("pedidos/status/{status}")]
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
    /// Obtém pedidos pendentes (para a cozinha visualizar)
    /// </summary>
    [HttpGet("pedidos/pendentes")]
    public async Task<ActionResult<List<Pedido>>> ObterPedidosPendentes()
    {
        try
        {
            var pedidos = await _pedidoRepository.ObterPedidosPendentesAsync();
            return Ok(pedidos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedidos pendentes");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém um pedido específico por ID
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
    /// Obtém um pedido específico por número
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
    /// Aceita um pedido (cozinha)
    /// </summary>
    [HttpPost("pedidos/{id}/aceitar")]
    public async Task<ActionResult> AceitarPedido(string id, [FromBody] PedidoAcaoRequest request)
    {
        try
        {
            if (id != request.PedidoId)
            {
                return BadRequest("ID do pedido não confere");
            }

            var pedido = await _pedidoRepository.ObterPorIdAsync(id);
            if (pedido == null)
            {
                return NotFound("Pedido não encontrado");
            }

            if (pedido.Status != StatusPedido.Pendente)
            {
                return BadRequest("Pedido não está pendente para aceitação");
            }

            var sucesso = await _pedidoRepository.AtualizarStatusAsync(
                id, 
                StatusPedido.Aceito, 
                request.FuncionarioId, 
                request.FuncionarioNome
            );

            if (sucesso)
            {
                _logger.LogInformation("Pedido {PedidoId} aceito pelo funcionário {FuncionarioId}", id, request.FuncionarioId);
                return Ok(new { message = "Pedido aceito com sucesso" });
            }

            return BadRequest("Não foi possível aceitar o pedido");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao aceitar pedido: {PedidoId}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Rejeita um pedido (cozinha)
    /// </summary>
    [HttpPost("pedidos/{id}/rejeitar")]
    public async Task<ActionResult> RejeitarPedido(string id, [FromBody] PedidoAcaoRequest request)
    {
        try
        {
            if (id != request.PedidoId)
            {
                return BadRequest("ID do pedido não confere");
            }

            var pedido = await _pedidoRepository.ObterPorIdAsync(id);
            if (pedido == null)
            {
                return NotFound("Pedido não encontrado");
            }

            if (pedido.Status != StatusPedido.Pendente)
            {
                return BadRequest("Pedido não está pendente para rejeição");
            }

            if (string.IsNullOrWhiteSpace(request.MotivoRejeicao))
            {
                return BadRequest("Motivo da rejeição é obrigatório");
            }

            var sucesso = await _pedidoRepository.AtualizarStatusAsync(
                id, 
                StatusPedido.Rejeitado, 
                request.FuncionarioId, 
                request.FuncionarioNome,
                request.MotivoRejeicao
            );

            if (sucesso)
            {
                _logger.LogInformation("Pedido {PedidoId} rejeitado pelo funcionário {FuncionarioId}. Motivo: {Motivo}", 
                    id, request.FuncionarioId, request.MotivoRejeicao);
                return Ok(new { message = "Pedido rejeitado com sucesso" });
            }

            return BadRequest("Não foi possível rejeitar o pedido");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao rejeitar pedido: {PedidoId}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Atualiza o status de um pedido
    /// </summary>
    [HttpPut("pedidos/{id}/status")]
    public async Task<ActionResult> AtualizarStatusPedido(string id, [FromBody] PedidoStatusUpdateRequest request)
    {
        try
        {
            if (id != request.PedidoId)
            {
                return BadRequest("ID do pedido não confere");
            }

            var pedido = await _pedidoRepository.ObterPorIdAsync(id);
            if (pedido == null)
            {
                return NotFound("Pedido não encontrado");
            }

            // Validações de transição de status
            if (!IsValidStatusTransition(pedido.Status, request.NovoStatus))
            {
                return BadRequest($"Transição de status inválida: {pedido.Status} -> {request.NovoStatus}");
            }

            var sucesso = await _pedidoRepository.AtualizarStatusAsync(
                id, 
                request.NovoStatus, 
                request.FuncionarioId, 
                request.FuncionarioNome
            );

            if (sucesso)
            {
                _logger.LogInformation("Status do pedido {PedidoId} alterado para {NovoStatus} pelo funcionário {FuncionarioId}", 
                    id, request.NovoStatus, request.FuncionarioId);
                return Ok(new { message = "Status do pedido atualizado com sucesso" });
            }

            return BadRequest("Não foi possível atualizar o status do pedido");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar status do pedido: {PedidoId}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém estatísticas dos pedidos
    /// </summary>
    [HttpGet("estatisticas")]
    public async Task<ActionResult<object>> ObterEstatisticas()
    {
        try
        {
            var pendentes = await _pedidoRepository.ContarPedidosPorStatusAsync(StatusPedido.Pendente);
            var aceitos = await _pedidoRepository.ContarPedidosPorStatusAsync(StatusPedido.Aceito);
            var emPreparo = await _pedidoRepository.ContarPedidosPorStatusAsync(StatusPedido.EmPreparo);
            var prontos = await _pedidoRepository.ContarPedidosPorStatusAsync(StatusPedido.Pronto);
            var rejeitados = await _pedidoRepository.ContarPedidosPorStatusAsync(StatusPedido.Rejeitado);

            return Ok(new
            {
                pendentes,
                aceitos,
                emPreparo,
                prontos,
                rejeitados,
                total = pendentes + aceitos + emPreparo + prontos + rejeitados
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter estatísticas");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém pedidos por período
    /// </summary>
    [HttpGet("pedidos/periodo")]
    public async Task<ActionResult<List<Pedido>>> ObterPedidosPorPeriodo(
        [FromQuery] DateTime inicio, 
        [FromQuery] DateTime fim)
    {
        try
        {
            var pedidos = await _pedidoRepository.ObterPedidosPorPeriodoAsync(inicio, fim);
            return Ok(pedidos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter pedidos por período");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    private bool IsValidStatusTransition(StatusPedido statusAtual, StatusPedido novoStatus)
    {
        return statusAtual switch
        {
            StatusPedido.Pendente => novoStatus == StatusPedido.Aceito || novoStatus == StatusPedido.Rejeitado || novoStatus == StatusPedido.Cancelado,
            StatusPedido.Aceito => novoStatus == StatusPedido.EmPreparo || novoStatus == StatusPedido.Cancelado,
            StatusPedido.EmPreparo => novoStatus == StatusPedido.Pronto || novoStatus == StatusPedido.Cancelado,
            StatusPedido.Pronto => novoStatus == StatusPedido.Entregue,
            StatusPedido.Rejeitado => false, // Não pode mudar de rejeitado
            StatusPedido.Entregue => false, // Não pode mudar de entregue
            StatusPedido.Cancelado => false, // Não pode mudar de cancelado
            _ => false
        };
    }
} 