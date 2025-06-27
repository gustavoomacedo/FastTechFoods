using Microsoft.AspNetCore.Mvc;
using OrderService.Models;
using OrderService.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace OrderService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClienteController : ControllerBase
{
    private readonly ClienteRepository _clienteRepository;
    private readonly ILogger<ClienteController> _logger;

    public ClienteController(ClienteRepository clienteRepository, ILogger<ClienteController> logger)
    {
        _clienteRepository = clienteRepository;
        _logger = logger;
    }

    /// <summary>
    /// Cadastra um novo cliente
    /// </summary>
    [HttpPost("cadastro")]
    public async Task<ActionResult<Cliente>> CadastrarCliente([FromBody] ClienteCreateRequest request)
    {
        try
        {
            // Verificar se email já existe
            if (await _clienteRepository.EmailExisteAsync(request.Email))
            {
                return BadRequest("Email já cadastrado");
            }

            // Verificar se CPF já existe
            if (await _clienteRepository.CPFExisteAsync(request.CPF))
            {
                return BadRequest("CPF já cadastrado");
            }

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
                Estado = request.Estado
            };

            var clienteCriado = await _clienteRepository.CriarAsync(cliente);
            
            _logger.LogInformation("Cliente cadastrado: {Email}", clienteCriado.Email);
            
            return CreatedAtAction(nameof(ObterClientePorId), new { id = clienteCriado.Id }, clienteCriado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao cadastrar cliente");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Login do cliente (simulado - sem senha por enquanto)
    /// </summary>
    [HttpPost("login")]
    public async Task<ActionResult<object>> LoginCliente([FromBody] ClienteLoginRequest request)
    {
        try
        {
            var cliente = await _clienteRepository.ObterPorIdentificadorAsync(request.Identificador);
            
            if (cliente == null)
            {
                return Unauthorized("Cliente não encontrado");
            }

            if (!cliente.Ativo)
            {
                return Unauthorized("Cliente inativo");
            }

            // TODO: Implementar validação de senha quando necessário
            // Por enquanto, apenas simula o login

            _logger.LogInformation("Cliente logado: {Email}", cliente.Email);
            
            return Ok(new
            {
                clienteId = cliente.Id,
                nome = cliente.Nome,
                email = cliente.Email,
                message = "Login realizado com sucesso"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no login do cliente");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém cliente por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<Cliente>> ObterClientePorId(string id)
    {
        try
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(id);
            if (cliente == null)
            {
                return NotFound("Cliente não encontrado");
            }
            return Ok(cliente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter cliente por ID: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém cliente por email
    /// </summary>
    [HttpGet("email/{email}")]
    public async Task<ActionResult<Cliente>> ObterClientePorEmail(string email)
    {
        try
        {
            var cliente = await _clienteRepository.ObterPorEmailAsync(email);
            if (cliente == null)
            {
                return NotFound("Cliente não encontrado");
            }
            return Ok(cliente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter cliente por email: {Email}", email);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém cliente por CPF
    /// </summary>
    [HttpGet("cpf/{cpf}")]
    public async Task<ActionResult<Cliente>> ObterClientePorCPF(string cpf)
    {
        try
        {
            var cliente = await _clienteRepository.ObterPorCPFAsync(cpf);
            if (cliente == null)
            {
                return NotFound("Cliente não encontrado");
            }
            return Ok(cliente);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter cliente por CPF: {CPF}", cpf);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Atualiza dados do cliente
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> AtualizarCliente(string id, [FromBody] ClienteUpdateRequest request)
    {
        try
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(id);
            if (cliente == null)
            {
                return NotFound("Cliente não encontrado");
            }

            var sucesso = await _clienteRepository.AtualizarAsync(id, request);
            
            if (sucesso)
            {
                _logger.LogInformation("Cliente atualizado: {Id}", id);
                return Ok(new { message = "Cliente atualizado com sucesso" });
            }

            return BadRequest("Não foi possível atualizar o cliente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar cliente: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Desativa um cliente
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DesativarCliente(string id)
    {
        try
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(id);
            if (cliente == null)
            {
                return NotFound("Cliente não encontrado");
            }

            var sucesso = await _clienteRepository.DesativarAsync(id);
            
            if (sucesso)
            {
                _logger.LogInformation("Cliente desativado: {Id}", id);
                return Ok(new { message = "Cliente desativado com sucesso" });
            }

            return BadRequest("Não foi possível desativar o cliente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao desativar cliente: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém estatísticas dos clientes (requer autenticação)
    /// </summary>
    [HttpGet("estatisticas")]
    [Authorize]
    public async Task<ActionResult<object>> ObterEstatisticas()
    {
        try
        {
            var totalAtivos = await _clienteRepository.ContarClientesAtivosAsync();
            var totalClientes = await _clienteRepository.ObterTodosAsync();

            return Ok(new
            {
                totalClientes = totalClientes.Count,
                totalAtivos,
                totalInativos = totalClientes.Count - totalAtivos
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter estatísticas dos clientes");
            return StatusCode(500, "Erro interno do servidor");
        }
    }
} 