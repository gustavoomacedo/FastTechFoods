using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

public class ClienteRequest
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

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Senha { get; set; } = string.Empty;
}

public class ClienteLoginRequest
{
    [Required]
    public string Identificador { get; set; } = string.Empty; // CPF ou Email

    [Required]
    public string Senha { get; set; } = string.Empty;
} 