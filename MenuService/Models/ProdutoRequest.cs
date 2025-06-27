using System.ComponentModel.DataAnnotations;

namespace MenuService.Models;

public class ProdutoCreateRequest
{
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

    public bool Disponivel { get; set; } = true;

    public string? ImagemUrl { get; set; }

    public List<string> Ingredientes { get; set; } = new();

    public List<string> Alergenos { get; set; } = new();

    public int TempoPreparoMinutos { get; set; } = 15;

    public bool Destaque { get; set; } = false;

    public int OrdemExibicao { get; set; } = 0;
}

public class ProdutoUpdateRequest
{
    [StringLength(100)]
    public string? Nome { get; set; }

    [StringLength(500)]
    public string? Descricao { get; set; }

    [Range(0.01, double.MaxValue, ErrorMessage = "O preço deve ser maior que zero")]
    public decimal? Preco { get; set; }

    public CategoriaProduto? Categoria { get; set; }

    public bool? Disponivel { get; set; }

    public string? ImagemUrl { get; set; }

    public List<string>? Ingredientes { get; set; }

    public List<string>? Alergenos { get; set; }

    public int? TempoPreparoMinutos { get; set; }

    public bool? Destaque { get; set; }

    public int? OrdemExibicao { get; set; }
}

public class ProdutoSearchRequest
{
    public string? Nome { get; set; }
    public CategoriaProduto? Categoria { get; set; }
    public bool? Disponivel { get; set; }
    public bool? Destaque { get; set; }
    public decimal? PrecoMinimo { get; set; }
    public decimal? PrecoMaximo { get; set; }
    public int? Page { get; set; } = 1;
    public int? PageSize { get; set; } = 20;
} 