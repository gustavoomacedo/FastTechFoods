using MongoDB.Driver;
using AuthService.Models;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Repositories;

public class ClienteRepository
{
    private readonly IMongoCollection<Cliente> _clientes;

    public ClienteRepository(IOptions<AuthSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _clientes = database.GetCollection<Cliente>("clientes");
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
        // Hash da senha
        cliente.Senha = HashSenha(cliente.Senha);
        
        await _clientes.InsertOneAsync(cliente);
        return cliente;
    }

    public async Task<bool> AtualizarAsync(Cliente cliente)
    {
        var result = await _clientes.ReplaceOneAsync(c => c.Id == cliente.Id, cliente);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeletarAsync(string id)
    {
        var result = await _clientes.DeleteOneAsync(c => c.Id == id);
        return result.DeletedCount > 0;
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

    public async Task<bool> ValidarCredenciaisAsync(string identificador, string senha)
    {
        var cliente = await ObterPorIdentificadorAsync(identificador);
        if (cliente == null) return false;

        var senhaHash = HashSenha(senha);
        return cliente.Senha == senhaHash && cliente.Ativo;
    }

    public async Task<long> ContarClientesAtivosAsync()
    {
        return await _clientes.CountDocumentsAsync(c => c.Ativo);
    }

    private string HashSenha(string senha)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(senha);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
} 