using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace OrderService.Models;

public class Pedido
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [Required]
    public string NumeroPedido { get; set; } = string.Empty;

    [Required]
    public string ClienteId { get; set; } = string.Empty;

    [Required]
    public string ClienteNome { get; set; } = string.Empty;

    [Required]
    public string ClienteTelefone { get; set; } = string.Empty;

    [Required]
    public string ClienteEmail { get; set; } = string.Empty;

    [Required]
    public List<ItemPedido> Itens { get; set; } = new();

    [Required]
    public decimal Subtotal { get; set; }

    public decimal TaxaEntrega { get; set; } = 0;

    public decimal Desconto { get; set; } = 0;

    [Required]
    public decimal ValorTotal { get; set; }

    [Required]
    public FormaEntrega FormaEntrega { get; set; }

    [Required]
    public string EnderecoEntrega { get; set; } = string.Empty;

    public string? ComplementoEntrega { get; set; }

    [Required]
    public StatusPedido Status { get; set; } = StatusPedido.Criado;

    public string? Observacoes { get; set; }

    public string? MotivoCancelamento { get; set; }

    [Required]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public DateTime? DataAtualizacao { get; set; }

    public DateTime? DataConfirmacao { get; set; }

    public DateTime? DataPreparo { get; set; }

    public DateTime? DataPronto { get; set; }

    public DateTime? DataEntrega { get; set; }

    public DateTime? DataCancelamento { get; set; }

    public DateTime? DataEstimadaEntrega { get; set; }

    public string? FuncionarioId { get; set; }

    public string? FuncionarioNome { get; set; }

    public List<HistoricoStatus> HistoricoStatus { get; set; } = new();

    public bool PagamentoConfirmado { get; set; } = false;

    public string? FormaPagamento { get; set; }
}

public class ItemPedido
{
    [Required]
    public string ProdutoId { get; set; } = string.Empty;

    [Required]
    public string ProdutoNome { get; set; } = string.Empty;

    [Required]
    public int Quantidade { get; set; }

    [Required]
    public decimal PrecoUnitario { get; set; }

    [Required]
    public decimal PrecoTotal { get; set; }

    public string? Observacoes { get; set; }

    public List<string> Opcionais { get; set; } = new();
}

public class HistoricoStatus
{
    [Required]
    public StatusPedido Status { get; set; }

    [Required]
    public DateTime Data { get; set; } = DateTime.UtcNow;

    public string? Observacoes { get; set; }

    public string? FuncionarioId { get; set; }

    public string? FuncionarioNome { get; set; }
}

public enum StatusPedido
{
    Criado,
    Confirmado,
    EmPreparo,
    Pronto,
    EmEntrega,
    Entregue,
    Cancelado
}

public enum FormaEntrega
{
    Balcao,
    DriveThru,
    Delivery
} 