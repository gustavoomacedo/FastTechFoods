using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace MenuService.Models;

public class Produto
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Descricao { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
    public decimal Preco { get; set; }

    [Required]
    public CategoriaProduto Categoria { get; set; }

    [Required]
    public bool Disponivel { get; set; } = true;

    public string? ImagemUrl { get; set; }

    public List<string> Ingredientes { get; set; } = new();

    public List<string> Alergenos { get; set; } = new();

    [Required]
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;

    public DateTime? DataAtualizacao { get; set; }

    public string? CriadoPor { get; set; }

    public string? AtualizadoPor { get; set; }

    public int TempoPreparoMinutos { get; set; } = 15;

    public bool Destaque { get; set; } = false;

    public int OrdemExibicao { get; set; } = 0;
}

public enum CategoriaProduto
{
    Lanche,
    Bebida,
    Sobremesa,
    Acompanhamento,
    Combo,
    Promocao
} 