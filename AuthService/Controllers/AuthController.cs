using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AuthService.Models;
using AuthService.Repositories;
using Microsoft.Extensions.Options;
using AuthService.Services;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly FuncionarioRepository _funcionarioRepo;
    private readonly ClienteRepository _clienteRepo;
    private readonly AuthSettings _authSettings;
    private readonly ClienteEventPublisher _clienteEventPublisher;

    public AuthController(FuncionarioRepository funcionarioRepo, ClienteRepository clienteRepo, IOptions<AuthSettings> authSettings, ClienteEventPublisher clienteEventPublisher)
    {
        _funcionarioRepo = funcionarioRepo;
        _clienteRepo = clienteRepo;
        _authSettings = authSettings.Value;
        _clienteEventPublisher = clienteEventPublisher;
    }

    // Cadastro de funcionário (restrito a gerente)
    [HttpPost("funcionario/registro")]
    public async Task<IActionResult> RegistrarFuncionario([FromBody] FuncionarioCreateRequest request)
    {
        var existing = await _funcionarioRepo.ObterPorEmailAsync(request.Email);
        if (existing != null)
            return BadRequest("E-mail já cadastrado.");

        var funcionario = new Funcionario
        {
            Nome = request.Nome,
            Email = request.Email,
            Senha = request.Senha, // Será hasheada no repositório
            Cargo = request.Cargo
        };
        
        await _funcionarioRepo.CriarAsync(funcionario);
        return Ok(new { message = "Funcionário cadastrado com sucesso" });
    }

    [HttpPost("funcionario/login")]
    public async Task<IActionResult> LoginFuncionario([FromBody] LoginRequest request)
    {
        var funcionario = await _funcionarioRepo.ObterPorEmailAsync(request.Email);
        if (funcionario == null || !await _funcionarioRepo.ValidarCredenciaisAsync(request.Email, request.Senha))
            return Unauthorized("Credenciais inválidas");

        if (!funcionario.Ativo)
            return Unauthorized("Funcionário inativo");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_authSettings.JwtSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, funcionario.Id),
                new Claim(ClaimTypes.Name, funcionario.Nome),
                new Claim(ClaimTypes.Email, funcionario.Email),
                new Claim(ClaimTypes.Role, funcionario.Cargo)
            }),
            Expires = DateTime.UtcNow.AddHours(_authSettings.JwtExpirationHours),
            Issuer = _authSettings.JwtIssuer,
            Audience = _authSettings.JwtAudience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new { 
            Token = tokenString,
            FuncionarioId = funcionario.Id,
            Nome = funcionario.Nome,
            Email = funcionario.Email,
            Cargo = funcionario.Cargo
        });
    }

    // Cadastro de cliente (público)
    [HttpPost("cliente/registro")]
    public async Task<IActionResult> RegistrarCliente([FromBody] ClienteRequest request)
    {
        // Verificar se email já existe
        if (await _clienteRepo.EmailExisteAsync(request.Email))
            return BadRequest("E-mail já cadastrado.");

        // Verificar se CPF já existe
        if (await _clienteRepo.CPFExisteAsync(request.CPF))
            return BadRequest("CPF já cadastrado.");

        var cliente = new Cliente
        {
            Nome = request.Nome,
            Email = request.Email,
            CPF = request.CPF,
            Telefone = request.Telefone,
            Endereco = request.Endereco,
            Complemento = request.Complemento,
            CEP = request.CEP,
            Cidade = request.Cidade,
            Estado = request.Estado,
            Senha = request.Senha // Será hasheada no repositório
        };

        await _clienteRepo.CriarAsync(cliente);
        _clienteEventPublisher.PublicarClienteCriado(cliente);
        return Ok(new { message = "Cliente cadastrado com sucesso" });
    }

    // Login de cliente (público)
    [HttpPost("cliente/login")]
    public async Task<IActionResult> LoginCliente([FromBody] LoginRequest request)
    {
        var cliente = await _clienteRepo.ObterPorIdentificadorAsync(request.Email);
        if (cliente == null || !await _clienteRepo.ValidarCredenciaisAsync(request.Email, request.Senha))
            return Unauthorized("Credenciais inválidas");

        if (!cliente.Ativo)
            return Unauthorized("Cliente inativo");

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_authSettings.JwtSecret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, cliente.Id),
                new Claim(ClaimTypes.Name, cliente.Nome),
                new Claim(ClaimTypes.Email, cliente.Email),
                new Claim(ClaimTypes.Role, "Cliente")
            }),
            Expires = DateTime.UtcNow.AddHours(_authSettings.JwtExpirationHours),
            Issuer = _authSettings.JwtIssuer,
            Audience = _authSettings.JwtAudience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new { 
            Token = tokenString,
            ClienteId = cliente.Id,
            Nome = cliente.Nome,
            Email = cliente.Email
        });
    }

    [Authorize]
    [HttpGet("me")]
    public IActionResult GetMe()
    {
        var id = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var nome = User.FindFirst(ClaimTypes.Name)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        
        return Ok(new { 
            Id = id,
            Nome = nome, 
            Email = email, 
            Role = role 
        });
    }
} 