using MongoDB.Driver;
using KitchenService.Models;
using Microsoft.Extensions.Options;

namespace KitchenService.Repositories;

public class PedidoRepository
{
    private readonly IMongoCollection<Pedido> _pedidos;

    public PedidoRepository(IOptions<KitchenSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _pedidos = database.GetCollection<Pedido>(settings.Value.PedidosCollectionName);
    }

    public async Task<List<Pedido>> ObterTodosAsync()
    {
        return await _pedidos.Find(_ => true).ToListAsync();
    }

    public async Task<List<Pedido>> ObterPorStatusAsync(StatusPedido status)
    {
        return await _pedidos.Find(p => p.Status == status).ToListAsync();
    }

    public async Task<Pedido?> ObterPorIdAsync(string id)
    {
        return await _pedidos.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Pedido?> ObterPorNumeroAsync(string numeroPedido)
    {
        return await _pedidos.Find(p => p.NumeroPedido == numeroPedido).FirstOrDefaultAsync();
    }

    public async Task<List<Pedido>> ObterPedidosPendentesAsync()
    {
        return await _pedidos.Find(p => p.Status == StatusPedido.Pendente)
                           .SortBy(p => p.DataCriacao)
                           .ToListAsync();
    }

    public async Task<Pedido> CriarAsync(Pedido pedido)
    {
        await _pedidos.InsertOneAsync(pedido);
        return pedido;
    }

    public async Task<bool> AtualizarStatusAsync(string id, StatusPedido novoStatus, string funcionarioId, string funcionarioNome, string? motivoRejeicao = null)
    {
        var update = Builders<Pedido>.Update
            .Set(p => p.Status, novoStatus)
            .Set(p => p.FuncionarioId, funcionarioId)
            .Set(p => p.FuncionarioNome, funcionarioNome);

        if (novoStatus == StatusPedido.Aceito)
        {
            update = update.Set(p => p.DataAceitacao, DateTime.UtcNow);
        }
        else if (novoStatus == StatusPedido.Rejeitado)
        {
            update = update
                .Set(p => p.DataRejeicao, DateTime.UtcNow)
                .Set(p => p.MotivoRejeicao, motivoRejeicao);
        }

        var result = await _pedidos.UpdateOneAsync(p => p.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> AtualizarAsync(Pedido pedido)
    {
        var result = await _pedidos.ReplaceOneAsync(p => p.Id == pedido.Id, pedido);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeletarAsync(string id)
    {
        var result = await _pedidos.DeleteOneAsync(p => p.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<Pedido>> ObterPedidosPorPeriodoAsync(DateTime inicio, DateTime fim)
    {
        return await _pedidos.Find(p => p.DataCriacao >= inicio && p.DataCriacao <= fim)
                           .SortByDescending(p => p.DataCriacao)
                           .ToListAsync();
    }

    public async Task<long> ContarPedidosPorStatusAsync(StatusPedido status)
    {
        return await _pedidos.CountDocumentsAsync(p => p.Status == status);
    }
} 