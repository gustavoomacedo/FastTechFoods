using Microsoft.AspNetCore.Mvc;
using MenuService.Models;
using MenuService.Repositories;

namespace MenuService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly ProdutoRepository _produtoRepository;
    private readonly ILogger<TestController> _logger;

    public TestController(ProdutoRepository produtoRepository, ILogger<TestController> logger)
    {
        _produtoRepository = produtoRepository;
        _logger = logger;
    }

    /// <summary>
    /// Cria produtos de teste
    /// </summary>
    [HttpPost("produtos")]
    public async Task<ActionResult<List<Produto>>> CriarProdutosTeste()
    {
        try
        {
            var produtos = new List<Produto>
            {
                new Produto
                {
                    Nome = "X-Burger",
                    Descricao = "Hambúrguer com queijo, alface, tomate e molho especial",
                    Preco = 15.90m,
                    Categoria = CategoriaProduto.Lanche,
                    Disponivel = true,
                    Ingredientes = new List<string> { "Pão", "Carne", "Queijo", "Alface", "Tomate", "Molho" },
                    Alergenos = new List<string> { "Glúten", "Lactose" },
                    TempoPreparoMinutos = 10,
                    Destaque = true,
                    OrdemExibicao = 1,
                    CriadoPor = "Sistema"
                },
                new Produto
                {
                    Nome = "Batata Frita",
                    Descricao = "Batatas fritas crocantes",
                    Preco = 8.50m,
                    Categoria = CategoriaProduto.Acompanhamento,
                    Disponivel = true,
                    Ingredientes = new List<string> { "Batata", "Óleo", "Sal" },
                    Alergenos = new List<string>(),
                    TempoPreparoMinutos = 8,
                    Destaque = false,
                    OrdemExibicao = 2,
                    CriadoPor = "Sistema"
                },
                new Produto
                {
                    Nome = "Coca-Cola",
                    Descricao = "Refrigerante Coca-Cola 350ml",
                    Preco = 4.50m,
                    Categoria = CategoriaProduto.Bebida,
                    Disponivel = true,
                    Ingredientes = new List<string> { "Água", "Açúcar", "Corante", "Aromatizantes" },
                    Alergenos = new List<string>(),
                    TempoPreparoMinutos = 1,
                    Destaque = false,
                    OrdemExibicao = 3,
                    CriadoPor = "Sistema"
                },
                new Produto
                {
                    Nome = "Sorvete de Chocolate",
                    Descricao = "Sorvete cremoso de chocolate",
                    Preco = 6.90m,
                    Categoria = CategoriaProduto.Sobremesa,
                    Disponivel = true,
                    Ingredientes = new List<string> { "Leite", "Chocolate", "Açúcar", "Creme" },
                    Alergenos = new List<string> { "Lactose" },
                    TempoPreparoMinutos = 2,
                    Destaque = true,
                    OrdemExibicao = 4,
                    CriadoPor = "Sistema"
                },
                new Produto
                {
                    Nome = "Combo X-Burger + Batata + Refri",
                    Descricao = "X-Burger, batata frita e refrigerante",
                    Preco = 25.90m,
                    Categoria = CategoriaProduto.Combo,
                    Disponivel = true,
                    Ingredientes = new List<string> { "X-Burger", "Batata Frita", "Refrigerante" },
                    Alergenos = new List<string> { "Glúten", "Lactose" },
                    TempoPreparoMinutos = 12,
                    Destaque = true,
                    OrdemExibicao = 5,
                    CriadoPor = "Sistema"
                }
            };

            var produtosCriados = new List<Produto>();
            foreach (var produto in produtos)
            {
                var produtoCriado = await _produtoRepository.CriarAsync(produto);
                produtosCriados.Add(produtoCriado);
            }

            _logger.LogInformation("Criados {Count} produtos de teste", produtosCriados.Count);
            return Ok(produtosCriados);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao criar produtos de teste");
            return StatusCode(500, "Erro interno do servidor");
        }
    }

    /// <summary>
    /// Obtém todos os produtos (sem autenticação para testes)
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
    /// Limpa todos os produtos de teste
    /// </summary>
    [HttpDelete("produtos")]
    public async Task<ActionResult> LimparProdutosTeste()
    {
        try
        {
            var produtos = await _produtoRepository.ObterTodosAsync();
            var produtosTeste = produtos.Where(p => p.CriadoPor == "Sistema").ToList();
            
            int deletados = 0;
            foreach (var produto in produtosTeste)
            {
                await _produtoRepository.DeletarAsync(produto.Id);
                deletados++;
            }

            _logger.LogInformation("Deletados {Count} produtos de teste", deletados);
            return Ok(new { message = $"Deletados {deletados} produtos de teste" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao limpar produtos de teste");
            return StatusCode(500, "Erro interno do servidor");
        }
    }
} 