using Microsoft.AspNetCore.Mvc;
using MenuService.Models;
using MenuService.Repositories;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace MenuService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuController : ControllerBase
{
    private readonly ProdutoRepository _produtoRepository;
    private readonly ILogger<MenuController> _logger;

    public MenuController(ProdutoRepository produtoRepository, ILogger<MenuController> logger)
    {
        _produtoRepository = produtoRepository;
        _logger = logger;
    }

    /// <summary>
    /// Obtém todos os produtos (público)
    /// </summary>
    [HttpGet("produtos")]
    public async Task<ActionResult<List<Produto>>> ObterTodosProdutos()
    {
        try
        {
            var produtos = await _produtoRepository.ObterTodosAsync();
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter todos os produtos");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém produtos disponíveis (público)
    /// </summary>
    [HttpGet("produtos/disponiveis")]
    public async Task<ActionResult<List<Produto>>> ObterProdutosDisponiveis()
    {
        try
        {
            var produtos = await _produtoRepository.ObterDisponiveisAsync();
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produtos disponíveis");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém produtos por categoria (público)
    /// </summary>
    [HttpGet("produtos/categoria/{categoria}")]
    public async Task<ActionResult<List<Produto>>> ObterProdutosPorCategoria(CategoriaProduto categoria)
    {
        try
        {
            var produtos = await _produtoRepository.ObterPorCategoriaAsync(categoria);
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produtos por categoria: {Categoria}", categoria);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém produtos em destaque (público)
    /// </summary>
    [HttpGet("produtos/destaques")]
    public async Task<ActionResult<List<Produto>>> ObterProdutosDestaque()
    {
        try
        {
            var produtos = await _produtoRepository.ObterDestaquesAsync();
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produtos em destaque");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Busca produtos com filtros (público)
    /// </summary>
    [HttpGet("produtos/buscar")]
    public async Task<ActionResult<List<Produto>>> BuscarProdutos([FromQuery] ProdutoSearchRequest request)
    {
        try
        {
            var produtos = await _produtoRepository.BuscarAsync(request);
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar produtos");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém um produto específico por ID (público)
    /// </summary>
    [HttpGet("produtos/{id}")]
    public async Task<ActionResult<Produto>> ObterProdutoPorId(string id)
    {
        try
        {
            var produto = await _produtoRepository.ObterPorIdAsync(id);
            if (produto == null)
            {
                return NotFound("Produto não encontrado");
            }
            return Ok(produto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produto por ID: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém produtos por IDs (público)
    /// </summary>
    [HttpPost("produtos/por-ids")]
    public async Task<ActionResult<List<Produto>>> ObterProdutosPorIds([FromBody] List<string> ids)
    {
        try
        {
            if (ids == null || !ids.Any())
            {
                return BadRequest("Lista de IDs é obrigatória");
            }

            var produtos = await _produtoRepository.ObterPorIdsAsync(ids);
            return Ok(produtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produtos por IDs");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    // Endpoints que requerem autenticação (gerente)

    /// <summary>
    /// Cria um novo produto (requer autenticação)
    /// </summary>
    [HttpPost("produtos")]
    [Authorize]
    public async Task<ActionResult<Produto>> CriarProduto([FromBody] ProdutoCreateRequest request)
    {
        try
        {
            var funcionarioId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var funcionarioNome = User.FindFirst(ClaimTypes.Name)?.Value;

            var produto = new Produto
            {
                Nome = request.Nome,
                Descricao = request.Descricao,
                Preco = request.Preco,
                Categoria = request.Categoria,
                Disponivel = request.Disponivel,
                ImagemUrl = request.ImagemUrl,
                Ingredientes = request.Ingredientes,
                Alergenos = request.Alergenos,
                TempoPreparoMinutos = request.TempoPreparoMinutos,
                Destaque = request.Destaque,
                OrdemExibicao = request.OrdemExibicao,
                CriadoPor = funcionarioNome
            };

            var produtoCriado = await _produtoRepository.CriarAsync(produto);
            
            _logger.LogInformation("Produto criado: {Nome} pelo funcionário {FuncionarioId}", 
                produtoCriado.Nome, funcionarioId);
            
            return CreatedAtAction(nameof(ObterProdutoPorId), new { id = produtoCriado.Id }, produtoCriado);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar produto");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Atualiza um produto (requer autenticação)
    /// </summary>
    [HttpPut("produtos/{id}")]
    [Authorize]
    public async Task<ActionResult> AtualizarProduto(string id, [FromBody] ProdutoUpdateRequest request)
    {
        try
        {
            var funcionarioNome = User.FindFirst(ClaimTypes.Name)?.Value;

            var produto = await _produtoRepository.ObterPorIdAsync(id);
            if (produto == null)
            {
                return NotFound("Produto não encontrado");
            }

            var sucesso = await _produtoRepository.AtualizarAsync(id, request, funcionarioNome ?? "Sistema");
            
            if (sucesso)
            {
                _logger.LogInformation("Produto atualizado: {Id} pelo funcionário {FuncionarioNome}", 
                    id, funcionarioNome);
                return Ok(new { message = "Produto atualizado com sucesso" });
            }

            return BadRequest("Não foi possível atualizar o produto");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao atualizar produto: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Deleta um produto (requer autenticação)
    /// </summary>
    [HttpDelete("produtos/{id}")]
    [Authorize]
    public async Task<ActionResult> DeletarProduto(string id)
    {
        try
        {
            var funcionarioNome = User.FindFirst(ClaimTypes.Name)?.Value;

            var produto = await _produtoRepository.ObterPorIdAsync(id);
            if (produto == null)
            {
                return NotFound("Produto não encontrado");
            }

            var sucesso = await _produtoRepository.DeletarAsync(id);
            
            if (sucesso)
            {
                _logger.LogInformation("Produto deletado: {Id} pelo funcionário {FuncionarioNome}", 
                    id, funcionarioNome);
                return Ok(new { message = "Produto deletado com sucesso" });
            }

            return BadRequest("Não foi possível deletar o produto");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao deletar produto: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Altera disponibilidade de um produto (requer autenticação)
    /// </summary>
    [HttpPatch("produtos/{id}/disponibilidade")]
    [Authorize]
    public async Task<ActionResult> AlterarDisponibilidade(string id, [FromBody] bool disponivel)
    {
        try
        {
            var funcionarioNome = User.FindFirst(ClaimTypes.Name)?.Value;

            var produto = await _produtoRepository.ObterPorIdAsync(id);
            if (produto == null)
            {
                return NotFound("Produto não encontrado");
            }

            var sucesso = await _produtoRepository.AlterarDisponibilidadeAsync(id, disponivel, funcionarioNome ?? "Sistema");
            
            if (sucesso)
            {
                _logger.LogInformation("Disponibilidade do produto alterada: {Id} para {Disponivel} pelo funcionário {FuncionarioNome}", 
                    id, disponivel, funcionarioNome);
                return Ok(new { message = $"Disponibilidade alterada para {(disponivel ? "disponível" : "indisponível")}" });
            }

            return BadRequest("Não foi possível alterar a disponibilidade do produto");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao alterar disponibilidade do produto: {Id}", id);
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém estatísticas dos produtos (requer autenticação)
    /// </summary>
    [HttpGet("estatisticas")]
    [Authorize]
    public async Task<ActionResult<object>> ObterEstatisticas()
    {
        try
        {
            var totalDisponiveis = await _produtoRepository.ContarDisponiveisAsync();
            var lanches = await _produtoRepository.ContarPorCategoriaAsync(CategoriaProduto.Lanche);
            var bebidas = await _produtoRepository.ContarPorCategoriaAsync(CategoriaProduto.Bebida);
            var sobremesas = await _produtoRepository.ContarPorCategoriaAsync(CategoriaProduto.Sobremesa);
            var acompanhamentos = await _produtoRepository.ContarPorCategoriaAsync(CategoriaProduto.Acompanhamento);

            return Ok(new
            {
                totalDisponiveis,
                porCategoria = new
                {
                    lanches,
                    bebidas,
                    sobremesas,
                    acompanhamentos
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter estatísticas");
            return StatusCode(500, "Erro interno do servidor");
        }
    }
} 