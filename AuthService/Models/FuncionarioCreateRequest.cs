using System.ComponentModel.DataAnnotations;

namespace AuthService.Models;

public class FuncionarioCreateRequest
{
    [Required]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Senha { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Cargo { get; set; } = string.Empty;
} 