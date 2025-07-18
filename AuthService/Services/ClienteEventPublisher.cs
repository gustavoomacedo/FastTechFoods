using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using AuthService.Models;
using Microsoft.Extensions.Options;

namespace AuthService.Services;

public class ClienteEventPublisher : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _exchangeName = "fasttech_exchange";

    public ClienteEventPublisher(IOptions<AuthSettings> settings)
    {
        var factory = new ConnectionFactory()
        {
            HostName = settings.Value.RabbitMQHost ?? "localhost",
            UserName = settings.Value.RabbitMQUser ?? "guest",
            Password = settings.Value.RabbitMQPassword ?? "guest",
            Port = settings.Value.RabbitMQPort > 0 ? settings.Value.RabbitMQPort : 5672
        };
        
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true);
    }

    public void PublicarClienteCriado(Cliente cliente)
    {
        var message = JsonSerializer.Serialize(new
        {
            Tipo = "ClienteCriado",
            ClienteId = cliente.Id,
            Nome = cliente.Nome,
            Email = cliente.Email,
            CPF = cliente.CPF,
            Telefone = cliente.Telefone,
            Endereco = cliente.Endereco,
            Complemento = cliente.Complemento,
            CEP = cliente.CEP,
            Cidade = cliente.Cidade,
            Estado = cliente.Estado,
            DataCriacao = cliente.DataCriacao
        });
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(
            exchange: _exchangeName,
            routingKey: "cliente.criado",
            basicProperties: null,
            body: body);
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
} 