using MongoDB.Driver;
using MenuService.Models;
using Microsoft.Extensions.Options;

namespace MenuService.Repositories;

public class ProdutoRepository
{
    private readonly IMongoCollection<Produto> _produtos;

    public ProdutoRepository(IOptions<MenuSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _produtos = database.GetCollection<Produto>(settings.Value.ProdutosCollectionName);
    }

    public async Task<List<Produto>> ObterTodosAsync()
    {
        return await _produtos.Find(_ => true)
                           .SortBy(p => p.OrdemExibicao)
                           .ThenBy(p => p.Nome)
                           .ToListAsync();
    }

    public async Task<List<Produto>> ObterDisponiveisAsync()
    {
        return await _produtos.Find(p => p.Disponivel)
                           .SortBy(p => p.OrdemExibicao)
                           .ThenBy(p => p.Nome)
                           .ToListAsync();
    }

    public async Task<List<Produto>> ObterPorCategoriaAsync(CategoriaProduto categoria)
    {
        return await _produtos.Find(p => p.Categoria == categoria && p.Disponivel)
                           .SortBy(p => p.OrdemExibicao)
                           .ThenBy(p => p.Nome)
                           .ToListAsync();
    }

    public async Task<List<Produto>> ObterDestaquesAsync()
    {
        return await _produtos.Find(p => p.Destaque && p.Disponivel)
                           .SortBy(p => p.OrdemExibicao)
                           .ThenBy(p => p.Nome)
                           .ToListAsync();
    }

    public async Task<Produto?> ObterPorIdAsync(string id)
    {
        return await _produtos.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Produto>> BuscarAsync(ProdutoSearchRequest request)
    {
        var filter = Builders<Produto>.Filter.Empty;

        if (!string.IsNullOrWhiteSpace(request.Nome))
        {
            filter &= Builders<Produto>.Filter.Regex(p => p.Nome, 
                new MongoDB.Bson.BsonRegularExpression(request.Nome, "i"));
        }

        if (request.Categoria.HasValue)
        {
            filter &= Builders<Produto>.Filter.Eq(p => p.Categoria, request.Categoria.Value);
        }

        if (request.Disponivel.HasValue)
        {
            filter &= Builders<Produto>.Filter.Eq(p => p.Disponivel, request.Disponivel.Value);
        }

        if (request.Destaque.HasValue)
        {
            filter &= Builders<Produto>.Filter.Eq(p => p.Destaque, request.Destaque.Value);
        }

        if (request.PrecoMinimo.HasValue)
        {
            filter &= Builders<Produto>.Filter.Gte(p => p.Preco, request.PrecoMinimo.Value);
        }

        if (request.PrecoMaximo.HasValue)
        {
            filter &= Builders<Produto>.Filter.Lte(p => p.Preco, request.PrecoMaximo.Value);
        }

        var produtos = await _produtos.Find(filter)
                                   .SortBy(p => p.OrdemExibicao)
                                   .ThenBy(p => p.Nome)
                                   .ToListAsync();

        // Paginação manual (para melhor performance, considere usar Skip/Limit do MongoDB)
        var page = request.Page ?? 1;
        var pageSize = request.PageSize ?? 20;
        var skip = (page - 1) * pageSize;

        return produtos.Skip(skip).Take(pageSize).ToList();
    }

    public async Task<Produto> CriarAsync(Produto produto)
    {
        await _produtos.InsertOneAsync(produto);
        return produto;
    }

    public async Task<bool> AtualizarAsync(string id, ProdutoUpdateRequest request, string atualizadoPor)
    {
        var update = Builders<Produto>.Update
            .Set(p => p.DataAtualizacao, DateTime.UtcNow)
            .Set(p => p.AtualizadoPor, atualizadoPor);

        if (!string.IsNullOrWhiteSpace(request.Nome))
            update = update.Set(p => p.Nome, request.Nome);

        if (request.Descricao != null)
            update = update.Set(p => p.Descricao, request.Descricao);

        if (request.Preco.HasValue)
            update = update.Set(p => p.Preco, request.Preco.Value);

        if (request.Categoria.HasValue)
            update = update.Set(p => p.Categoria, request.Categoria.Value);

        if (request.Disponivel.HasValue)
            update = update.Set(p => p.Disponivel, request.Disponivel.Value);

        if (request.ImagemUrl != null)
            update = update.Set(p => p.ImagemUrl, request.ImagemUrl);

        if (request.Ingredientes != null)
            update = update.Set(p => p.Ingredientes, request.Ingredientes);

        if (request.Alergenos != null)
            update = update.Set(p => p.Alergenos, request.Alergenos);

        if (request.TempoPreparoMinutos.HasValue)
            update = update.Set(p => p.TempoPreparoMinutos, request.TempoPreparoMinutos.Value);

        if (request.Destaque.HasValue)
            update = update.Set(p => p.Destaque, request.Destaque.Value);

        if (request.OrdemExibicao.HasValue)
            update = update.Set(p => p.OrdemExibicao, request.OrdemExibicao.Value);

        var result = await _produtos.UpdateOneAsync(p => p.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeletarAsync(string id)
    {
        var result = await _produtos.DeleteOneAsync(p => p.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> AlterarDisponibilidadeAsync(string id, bool disponivel, string atualizadoPor)
    {
        var update = Builders<Produto>.Update
            .Set(p => p.Disponivel, disponivel)
            .Set(p => p.DataAtualizacao, DateTime.UtcNow)
            .Set(p => p.AtualizadoPor, atualizadoPor);

        var result = await _produtos.UpdateOneAsync(p => p.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<List<Produto>> ObterPorIdsAsync(List<string> ids)
    {
        var filter = Builders<Produto>.Filter.In(p => p.Id, ids);
        return await _produtos.Find(filter).ToListAsync();
    }

    public async Task<long> ContarPorCategoriaAsync(CategoriaProduto categoria)
    {
        return await _produtos.CountDocumentsAsync(p => p.Categoria == categoria && p.Disponivel);
    }

    public async Task<long> ContarDisponiveisAsync()
    {
        return await _produtos.CountDocumentsAsync(p => p.Disponivel);
    }
} 