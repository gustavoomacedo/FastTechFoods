using System.ComponentModel.DataAnnotations;

namespace KitchenService.Models;

public class PedidoAcaoRequest
{
    [Required]
    public string PedidoId { get; set; } = string.Empty;

    [Required]
    public string FuncionarioId { get; set; } = string.Empty;

    [Required]
    public string FuncionarioNome { get; set; } = string.Empty;

    public string? MotivoRejeicao { get; set; }
}

public class PedidoStatusUpdateRequest
{
    [Required]
    public string PedidoId { get; set; } = string.Empty;

    [Required]
    public StatusPedido NovoStatus { get; set; }

    [Required]
    public string FuncionarioId { get; set; } = string.Empty;

    [Required]
    public string FuncionarioNome { get; set; } = string.Empty;

    public string? Observacoes { get; set; }
} 