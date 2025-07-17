using System.Text.Json;
using OrderService.Models;

namespace OrderService.Services;

public class MenuServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MenuServiceClient> _logger;

    public MenuServiceClient(HttpClient httpClient, ILogger<MenuServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<ProdutoMenu>> ObterProdutosPorIdsAsync(List<string> ids)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/menu/produtos/por-ids", ids);
            
            if (response.IsSuccessStatusCode)
            {
                var produtos = await response.Content.ReadFromJsonAsync<List<ProdutoMenu>>();
                return produtos ?? new List<ProdutoMenu>();
            }
            
            _logger.LogWarning("Falha ao obter produtos do MenuService: {StatusCode}", response.StatusCode);
            return new List<ProdutoMenu>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produtos do MenuService");
            return new List<ProdutoMenu>();
        }
    }

    public async Task<ProdutoMenu?> ObterProdutoPorIdAsync(string id)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/menu/produtos/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var produto = await response.Content.ReadFromJsonAsync<ProdutoMenu>();
                return produto;
            }
            
            _logger.LogWarning("Falha ao obter produto do MenuService: {StatusCode}", response.StatusCode);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produto do MenuService");
            return null;
        }
    }

    public async Task<List<ProdutoMenu>> ObterProdutosDisponiveisAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/menu/produtos/disponiveis");
            
            if (response.IsSuccessStatusCode)
            {
                var produtos = await response.Content.ReadFromJsonAsync<List<ProdutoMenu>>();
                return produtos ?? new List<ProdutoMenu>();
            }
            
            _logger.LogWarning("Falha ao obter produtos disponíveis do MenuService: {StatusCode}", response.StatusCode);
            return new List<ProdutoMenu>();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao obter produtos disponíveis do MenuService");
            return new List<ProdutoMenu>();
        }
    }
}

// Modelo simplificado para produtos do MenuService
public class ProdutoMenu
{
    public string Id { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public decimal Preco { get; set; }
    public CategoriaProduto Categoria { get; set; }
    public bool Disponivel { get; set; }
    public List<string> Ingredientes { get; set; } = new();
    public List<string> Alergenos { get; set; } = new();
    public int TempoPreparoMinutos { get; set; }
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