using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using OrderService.Models;
using OrderService.Repositories;
using Microsoft.Extensions.Options;

namespace OrderService.Services;

public class ClienteMessageConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<ClienteMessageConsumer> _logger;
    private readonly ClienteRepository _clienteRepository;
    private readonly string _queueName = "orderservice_clientes_queue";
    private readonly string _exchangeName = "fasttech_exchange";

    public ClienteMessageConsumer(IOptions<OrderSettings> settings, ClienteRepository clienteRepository, ILogger<ClienteMessageConsumer> logger)
    {
        _clienteRepository = clienteRepository;
        _logger = logger;

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
        _channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(_queueName, _exchangeName, "cliente.criado");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            try
            {
                await ProcessarMensagem(message);
                _channel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem de cliente: {Message}", message);
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };
        _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
        _logger.LogInformation("Consumidor de eventos de cliente iniciado");
        return Task.CompletedTask;
    }

    private async Task ProcessarMensagem(string message)
    {
        var messageData = JsonSerializer.Deserialize<JsonElement>(message);
        var tipo = messageData.GetProperty("Tipo").GetString();
        if (tipo == "ClienteCriado")
        {
            var clienteId = messageData.GetProperty("ClienteId").GetString();
            var nome = messageData.GetProperty("Nome").GetString();
            var email = messageData.GetProperty("Email").GetString();
            var cpf = messageData.GetProperty("CPF").GetString();
            var telefone = messageData.GetProperty("Telefone").GetString();
            var endereco = messageData.GetProperty("Endereco").GetString();
            var complemento = messageData.TryGetProperty("Complemento", out var comp) ? comp.GetString() : null;
            var cep = messageData.GetProperty("CEP").GetString();
            var cidade = messageData.GetProperty("Cidade").GetString();
            var estado = messageData.GetProperty("Estado").GetString();
            var dataCriacao = messageData.GetProperty("DataCriacao").GetDateTime();

            var cliente = new Cliente
            {
                Id = clienteId ?? string.Empty,
                Nome = nome ?? string.Empty,
                Email = email ?? string.Empty,
                CPF = cpf ?? string.Empty,
                Telefone = telefone ?? string.Empty,
                Endereco = endereco ?? string.Empty,
                Complemento = complemento,
                CEP = cep ?? string.Empty,
                Cidade = cidade ?? string.Empty,
                Estado = estado ?? string.Empty,
                DataCriacao = dataCriacao,
                Ativo = true
            };
            await _clienteRepository.CriarOuAtualizarAsync(cliente);
            _logger.LogInformation("Cliente sincronizado no OrderService: {ClienteId} - {Nome}", cliente.Id, cliente.Nome);
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
} 