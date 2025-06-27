using MongoDB.Driver;
using AuthService.Models;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Repositories;

public class FuncionarioRepository
{
    private readonly IMongoCollection<Funcionario> _funcionarios;

    public FuncionarioRepository(IOptions<AuthSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.DatabaseName);
        _funcionarios = database.GetCollection<Funcionario>("funcionarios");
    }

    public async Task<Funcionario?> ObterPorEmailAsync(string email)
    {
        return await _funcionarios.Find(f => f.Email == email).FirstOrDefaultAsync();
    }

    public async Task<Funcionario?> ObterPorIdAsync(string id)
    {
        return await _funcionarios.Find(f => f.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Funcionario>> ObterTodosAsync()
    {
        return await _funcionarios.Find(_ => true).ToListAsync();
    }

    public async Task<Funcionario> CriarAsync(Funcionario funcionario)
    {
        // Hash da senha
        funcionario.Senha = HashSenha(funcionario.Senha);
        
        await _funcionarios.InsertOneAsync(funcionario);
        return funcionario;
    }

    public async Task<bool> AtualizarAsync(Funcionario funcionario)
    {
        var result = await _funcionarios.ReplaceOneAsync(f => f.Id == funcionario.Id, funcionario);
        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeletarAsync(string id)
    {
        var result = await _funcionarios.DeleteOneAsync(f => f.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<bool> EmailExisteAsync(string email)
    {
        var count = await _funcionarios.CountDocumentsAsync(f => f.Email == email);
        return count > 0;
    }

    public async Task<bool> ValidarCredenciaisAsync(string email, string senha)
    {
        var funcionario = await ObterPorEmailAsync(email);
        if (funcionario == null) return false;

        var senhaHash = HashSenha(senha);
        return funcionario.Senha == senhaHash && funcionario.Ativo;
    }

    private string HashSenha(string senha)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(senha);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }
} 