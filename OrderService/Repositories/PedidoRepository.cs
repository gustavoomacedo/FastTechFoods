using MongoDB.Driver;
using MongoDB.Bson;
using OrderService.Models;
using Microsoft.Extensions.Options;

namespace OrderService.Repositories;

public class PedidoRepository
{
    private readonly IMongoCollection<Pedido> _pedidos;

    public PedidoRepository(IOptions<OrderSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _pedidos = database.GetCollection<Pedido>(settings.Value.PedidosCollectionName);
    }

    public async Task<List<Pedido>> ObterTodosAsync()
    {
        return await _pedidos.Find(_ => true)
                           .SortByDescending(p => p.DataCriacao)
                           .ToListAsync();
    }

    public async Task<Pedido?> ObterPorIdAsync(string id)
    {
        return await _pedidos.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Pedido?> ObterPorNumeroAsync(string numeroPedido)
    {
        return await _pedidos.Find(p => p.NumeroPedido == numeroPedido).FirstOrDefaultAsync();
    }

    public async Task<List<Pedido>> ObterPorClienteAsync(string clienteId)
    {
        return await _pedidos.Find(p => p.ClienteId == clienteId)
                           .SortByDescending(p => p.DataCriacao)
                           .ToListAsync();
    }

    public async Task<List<Pedido>> ObterPorStatusAsync(StatusPedido status)
    {
        return await _pedidos.Find(p => p.Status == status)
                           .SortBy(p => p.DataCriacao)
                           .ToListAsync();
    }

    public async Task<List<Pedido>> ObterPedidosRecentesAsync(int limite = 10)
    {
        return await _pedidos.Find(_ => true)
                           .SortByDescending(p => p.DataCriacao)
                           .Limit(limite)
                           .ToListAsync();
    }

    public async Task<Pedido> CriarAsync(Pedido pedido)
    {
        // Gerar número único do pedido
        pedido.NumeroPedido = await GerarNumeroPedidoAsync();
        
        // Adicionar histórico inicial
        pedido.HistoricoStatus.Add(new HistoricoStatus
        {
            Status = pedido.Status,
            Data = pedido.DataCriacao,
            Observacoes = "Pedido criado"
        });

        await _pedidos.InsertOneAsync(pedido);
        return pedido;
    }

    public async Task<bool> AtualizarStatusAsync(string id, StatusPedido novoStatus, string? funcionarioId = null, string? funcionarioNome = null, string? observacoes = null)
    {
        var pedido = await ObterPorIdAsync(id);
        if (pedido == null) return false;

        var update = Builders<Pedido>.Update
            .Set(p => p.Status, novoStatus);

        // Atualizar datas específicas baseado no status
        switch (novoStatus)
        {
            case StatusPedido.Confirmado:
                update = update.Set(p => p.DataConfirmacao, DateTime.UtcNow);
                break;
            case StatusPedido.EmPreparo:
                update = update.Set(p => p.DataPreparo, DateTime.UtcNow);
                break;
            case StatusPedido.Pronto:
                update = update.Set(p => p.DataPronto, DateTime.UtcNow);
                break;
            case StatusPedido.Entregue:
                update = update.Set(p => p.DataEntrega, DateTime.UtcNow);
                break;
            case StatusPedido.Cancelado:
                update = update.Set(p => p.DataCancelamento, DateTime.UtcNow);
                break;
        }

        // Adicionar ao histórico
        var historico = new HistoricoStatus
        {
            Status = novoStatus,
            Data = DateTime.UtcNow,
            Observacoes = observacoes,
            FuncionarioId = funcionarioId,
            FuncionarioNome = funcionarioNome
        };

        update = update.Push(p => p.HistoricoStatus, historico);

        var result = await _pedidos.UpdateOneAsync(p => p.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> CancelarPedidoAsync(string id, string motivoCancelamento, string clienteId)
    {
        var pedido = await ObterPorIdAsync(id);
        if (pedido == null) return false;

        // Verificar se o pedido pode ser cancelado
        if (pedido.Status != StatusPedido.Criado && pedido.Status != StatusPedido.Confirmado)
        {
            return false; // Não pode cancelar se já está em preparo
        }

        var update = Builders<Pedido>.Update
            .Set(p => p.Status, StatusPedido.Cancelado)
            .Set(p => p.MotivoCancelamento, motivoCancelamento)
            .Set(p => p.DataCancelamento, DateTime.UtcNow);

        // Adicionar ao histórico
        var historico = new HistoricoStatus
        {
            Status = StatusPedido.Cancelado,
            Data = DateTime.UtcNow,
            Observacoes = $"Pedido cancelado pelo cliente. Motivo: {motivoCancelamento}"
        };

        update = update.Push(p => p.HistoricoStatus, historico);

        var result = await _pedidos.UpdateOneAsync(p => p.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> ConfirmarPagamentoAsync(string id)
    {
        var update = Builders<Pedido>.Update
            .Set(p => p.PagamentoConfirmado, true);

        var result = await _pedidos.UpdateOneAsync(p => p.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> AtualizarAsync(string id, PedidoUpdateRequest request)
    {
        var updateBuilder = Builders<Pedido>.Update;
        var update = updateBuilder.Set(p => p.DataAtualizacao, DateTime.UtcNow);

        if (request.Status.HasValue)
        {
            update = update.Set(p => p.Status, request.Status.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Observacoes))
        {
            update = update.Set(p => p.Observacoes, request.Observacoes);
        }

        if (!string.IsNullOrWhiteSpace(request.MotivoCancelamento))
        {
            update = update.Set(p => p.MotivoCancelamento, request.MotivoCancelamento);
        }

        if (!string.IsNullOrWhiteSpace(request.FuncionarioId))
        {
            update = update.Set(p => p.FuncionarioId, request.FuncionarioId);
        }

        if (!string.IsNullOrWhiteSpace(request.FuncionarioNome))
        {
            update = update.Set(p => p.FuncionarioNome, request.FuncionarioNome);
        }

        if (request.PagamentoConfirmado.HasValue)
        {
            update = update.Set(p => p.PagamentoConfirmado, request.PagamentoConfirmado.Value);
        }

        var result = await _pedidos.UpdateOneAsync(p => p.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeletarAsync(string id)
    {
        var result = await _pedidos.DeleteOneAsync(p => p.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<List<Pedido>> ObterPorPeriodoAsync(DateTime inicio, DateTime fim)
    {
        return await _pedidos.Find(p => p.DataCriacao >= inicio && p.DataCriacao <= fim)
                           .SortByDescending(p => p.DataCriacao)
                           .ToListAsync();
    }

    public async Task<long> ContarPorStatusAsync(StatusPedido status)
    {
        return await _pedidos.CountDocumentsAsync(p => p.Status == status);
    }

    public async Task<decimal> CalcularReceitaPorPeriodoAsync(DateTime inicio, DateTime fim)
    {
        var pedidos = await _pedidos.Find(p => 
            p.DataCriacao >= inicio && 
            p.DataCriacao <= fim && 
            p.Status != StatusPedido.Cancelado)
            .ToListAsync();
        
        return pedidos.Sum(p => p.ValorTotal);
    }

    private async Task<string> GerarNumeroPedidoAsync()
    {
        var hoje = DateTime.Now.ToString("yyyyMMdd");
        var filter = Builders<Pedido>.Filter.Regex(p => p.NumeroPedido, new MongoDB.Bson.BsonRegularExpression($"^PED-{hoje}-"));
        
        var count = await _pedidos.CountDocumentsAsync(filter);
        var numero = count + 1;
        
        return $"PED-{hoje}-{numero:D4}";
    }
} 