using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace KitchenService.Models;

public class Pedido
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [Required]
    public string NumeroPedido { get; set; } = string.Empty;

    [Required]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    [Required]
    public List<ItemPedido> Itens { get; set; } = new();

    [Required]
    public string ClienteNome { get; set; } = string.Empty;

    [Required]
    public string ClienteTelefone { get; set; } = string.Empty;

    [Required]
    public string EnderecoEntrega { get; set; } = string.Empty;

    [Required]
    public decimal ValorTotal { get; set; }

    [Required]
    public StatusPedido Status { get; set; } = StatusPedido.Pendente;

    public string? Observacoes { get; set; }

    public DateTime? DataAceitacao { get; set; }

    public DateTime? DataRejeicao { get; set; }

    public string? MotivoRejeicao { get; set; }

    public string? FuncionarioId { get; set; }

    public string? FuncionarioNome { get; set; }
}

public class ItemPedido
{
    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public int Quantidade { get; set; }

    [Required]
    public decimal PrecoUnitario { get; set; }

    public string? Observacoes { get; set; }
}

public enum StatusPedido
{
    Pendente,
    Aceito,
    Rejeitado,
    EmPreparo,
    Pronto,
    Entregue,
    Cancelado
} 