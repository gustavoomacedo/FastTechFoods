using MongoDB.Driver;
using OrderService.Models;
using Microsoft.Extensions.Options;

namespace OrderService.Repositories;

public class ClienteRepository
{
    private readonly IMongoCollection<Cliente> _clientes;

    public ClienteRepository(IOptions<OrderSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _clientes = database.GetCollection<Cliente>(settings.Value.ClientesCollectionName);
    }

    public async Task<List<Cliente>> ObterTodosAsync()
    {
        return await _clientes.Find(_ => true).ToListAsync();
    }

    public async Task<Cliente?> ObterPorIdAsync(string id)
    {
        return await _clientes.Find(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<Cliente?> ObterPorEmailAsync(string email)
    {
        return await _clientes.Find(c => c.Email == email).FirstOrDefaultAsync();
    }

    public async Task<Cliente?> ObterPorCPFAsync(string cpf)
    {
        return await _clientes.Find(c => c.CPF == cpf).FirstOrDefaultAsync();
    }

    public async Task<Cliente?> ObterPorIdentificadorAsync(string identificador)
    {
        // Busca por email ou CPF
        var filter = Builders<Cliente>.Filter.Or(
            Builders<Cliente>.Filter.Eq(c => c.Email, identificador),
            Builders<Cliente>.Filter.Eq(c => c.CPF, identificador)
        );
        
        return await _clientes.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<Cliente> CriarAsync(Cliente cliente)
    {
        await _clientes.InsertOneAsync(cliente);
        return cliente;
    }

    public async Task<bool> AtualizarAsync(string id, ClienteUpdateRequest request)
    {
        var update = Builders<Cliente>.Update
            .Set(c => c.DataAtualizacao, DateTime.UtcNow);

        if (!string.IsNullOrWhiteSpace(request.Nome))
            update = update.Set(c => c.Nome, request.Nome);

        if (!string.IsNullOrWhiteSpace(request.Email))
            update = update.Set(c => c.Email, request.Email);

        if (!string.IsNullOrWhiteSpace(request.Telefone))
            update = update.Set(c => c.Telefone, request.Telefone);

        if (!string.IsNullOrWhiteSpace(request.Endereco))
            update = update.Set(c => c.Endereco, request.Endereco);

        if (request.Complemento != null)
            update = update.Set(c => c.Complemento, request.Complemento);

        if (!string.IsNullOrWhiteSpace(request.CEP))
            update = update.Set(c => c.CEP, request.CEP);

        if (!string.IsNullOrWhiteSpace(request.Cidade))
            update = update.Set(c => c.Cidade, request.Cidade);

        if (!string.IsNullOrWhiteSpace(request.Estado))
            update = update.Set(c => c.Estado, request.Estado);

        var result = await _clientes.UpdateOneAsync(c => c.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeletarAsync(string id)
    {
        var result = await _clientes.DeleteOneAsync(c => c.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> DesativarAsync(string id)
    {
        var update = Builders<Cliente>.Update
            .Set(c => c.Ativo, false)
            .Set(c => c.DataAtualizacao, DateTime.UtcNow);

        var result = await _clientes.UpdateOneAsync(c => c.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> AtivarAsync(string id)
    {
        var update = Builders<Cliente>.Update
            .Set(c => c.Ativo, true)
            .Set(c => c.DataAtualizacao, DateTime.UtcNow);

        var result = await _clientes.UpdateOneAsync(c => c.Id == id, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> AdicionarPedidoAsync(string clienteId, string pedidoId)
    {
        var update = Builders<Cliente>.Update
            .Push(c => c.PedidosIds, pedidoId)
            .Set(c => c.DataAtualizacao, DateTime.UtcNow);

        var result = await _clientes.UpdateOneAsync(c => c.Id == clienteId, update);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> EmailExisteAsync(string email)
    {
        var count = await _clientes.CountDocumentsAsync(c => c.Email == email);
        return count > 0;
    }

    public async Task<bool> CPFExisteAsync(string cpf)
    {
        var count = await _clientes.CountDocumentsAsync(c => c.CPF == cpf);
        return count > 0;
    }

    public async Task<long> ContarClientesAtivosAsync()
    {
        return await _clientes.CountDocumentsAsync(c => c.Ativo);
    }
} 