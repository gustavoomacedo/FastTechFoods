namespace MenuService.Models;

public class MenuSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string ProdutosCollectionName { get; set; } = "produtos";
} 