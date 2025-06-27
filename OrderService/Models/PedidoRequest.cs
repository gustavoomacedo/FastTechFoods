using System.ComponentModel.DataAnnotations;

namespace OrderService.Models;

public class PedidoCreateRequest
{
    [Required]
    public string ClienteId { get; set; } = string.Empty;

    [Required]
    public string ClienteNome { get; set; } = string.Empty;

    [Required]
    public string ClienteTelefone { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string ClienteEmail { get; set; } = string.Empty;

    [Required]
    public List<ItemPedidoRequest> Itens { get; set; } = new();

    [Required]
    public FormaEntrega FormaEntrega { get; set; }

    [Required]
    public string EnderecoEntrega { get; set; } = string.Empty;

    public string? ComplementoEntrega { get; set; }

    public string? Observacoes { get; set; }

    public string? FormaPagamento { get; set; }

    public decimal TaxaEntrega { get; set; } = 0;

    public decimal Desconto { get; set; } = 0;
}

public class ItemPedidoRequest
{
    [Required]
    public string ProdutoId { get; set; } = string.Empty;

    [Required]
    public int Quantidade { get; set; }

    public string? Observacoes { get; set; }

    public List<string> Opcionais { get; set; } = new();
}

public class PedidoUpdateRequest
{
    public StatusPedido? Status { get; set; }
    public string? Observacoes { get; set; }
    public string? MotivoCancelamento { get; set; }
    public string? FuncionarioId { get; set; }
    public string? FuncionarioNome { get; set; }
    public bool? PagamentoConfirmado { get; set; }
}

public class PedidoCancelRequest
{
    [Required]
    public string MotivoCancelamento { get; set; } = string.Empty;

    [Required]
    public string ClienteId { get; set; } = string.Empty;
}

public class ClienteCreateRequest
{
    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(11)]
    public string CPF { get; set; } = string.Empty;

    [Required]
    [StringLength(15)]
    public string Telefone { get; set; } = string.Empty;

    [Required]
    public string Endereco { get; set; } = string.Empty;

    public string? Complemento { get; set; }

    [Required]
    [StringLength(8)]
    public string CEP { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Cidade { get; set; } = string.Empty;

    [Required]
    [StringLength(2)]
    public string Estado { get; set; } = string.Empty;
}

public class ClienteUpdateRequest
{
    [StringLength(100)]
    public string? Nome { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [StringLength(15)]
    public string? Telefone { get; set; }

    public string? Endereco { get; set; }

    public string? Complemento { get; set; }

    [StringLength(8)]
    public string? CEP { get; set; }

    [StringLength(50)]
    public string? Cidade { get; set; }

    [StringLength(2)]
    public string? Estado { get; set; }
}

public class ClienteLoginRequest
{
    [Required]
    public string Identificador { get; set; } = string.Empty; // CPF ou Email

    [Required]
    public string Senha { get; set; } = string.Empty;
} 