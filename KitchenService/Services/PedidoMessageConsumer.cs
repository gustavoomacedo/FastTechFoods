using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using KitchenService.Models;
using KitchenService.Repositories;

namespace KitchenService.Services;

public class PedidoMessageConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<PedidoMessageConsumer> _logger;
    private readonly PedidoRepository _pedidoRepository;
    private readonly string _queueName = "kitchen_queue";
    private readonly string _exchangeName = "fasttech_exchange";

    public PedidoMessageConsumer(IOptions<KitchenSettings> settings, PedidoRepository pedidoRepository, ILogger<PedidoMessageConsumer> logger)
    {
        _pedidoRepository = pedidoRepository;
        _logger = logger;
        
        var factory = new ConnectionFactory()
        {
            HostName = settings.Value.RabbitMQHost ?? "localhost",
            UserName = settings.Value.RabbitMQUser ?? "admin",
            Password = settings.Value.RabbitMQPassword ?? "password123",
            Port = settings.Value.RabbitMQPort > 0 ? settings.Value.RabbitMQPort : 5672
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        
        // Declarar exchange
        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true);
        
        // Declarar fila específica para cozinha
        _channel.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false);
        
        // Binding da fila com exchange - cozinha recebe pedidos criados e atualizados
        _channel.QueueBind(_queueName, _exchangeName, "pedido.criado");
        _channel.QueueBind(_queueName, _exchangeName, "pedido.confirmado");
        _channel.QueueBind(_queueName, _exchangeName, "pedido.cancelado");
        
        _logger.LogInformation("PedidoMessageConsumer inicializado com sucesso");
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
                await ProcessarMensagem(message, ea.RoutingKey);
                _channel.BasicAck(ea.DeliveryTag, false);
                _logger.LogInformation("Mensagem processada com sucesso: {RoutingKey}", ea.RoutingKey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagem: {Message}", message);
                _channel.BasicNack(ea.DeliveryTag, false, true);
            }
        };

        _channel.BasicConsume(queue: _queueName, autoAck: false, consumer: consumer);
        _logger.LogInformation("Consumidor de mensagens da cozinha iniciado");
        
        return Task.CompletedTask;
    }

    private async Task ProcessarMensagem(string message, string routingKey)
    {
        try
        {
            var messageData = JsonSerializer.Deserialize<JsonElement>(message);
            var tipo = messageData.GetProperty("Tipo").GetString();
            var pedidoId = messageData.GetProperty("PedidoId").GetString();

            _logger.LogInformation("Processando mensagem do tipo {Tipo} para pedido {PedidoId}", tipo, pedidoId);

            switch (routingKey)
            {
                case "pedido.criado":
                    await ProcessarPedidoCriado(messageData);
                    break;
                case "pedido.confirmado":
                    await ProcessarPedidoConfirmado(messageData);
                    break;
                case "pedido.cancelado":
                    await ProcessarPedidoCancelado(messageData);
                    break;
                default:
                    _logger.LogWarning("Routing key não reconhecida: {RoutingKey}", routingKey);
                    break;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar mensagem: {Message}", message);
            throw;
        }
    }

    private async Task ProcessarPedidoCriado(JsonElement messageData)
    {
        var pedidoId = messageData.GetProperty("PedidoId").GetString();
        var numeroPedido = messageData.GetProperty("NumeroPedido").GetString();
        var clienteId = messageData.GetProperty("ClienteId").GetString();
        var valorTotal = messageData.GetProperty("ValorTotal").GetDecimal();

        _logger.LogInformation("Novo pedido recebido na cozinha: {NumeroPedido} - R$ {ValorTotal}", numeroPedido, valorTotal);

        // Atualizar status do pedido para Aceito no banco de dados
        if (!string.IsNullOrEmpty(pedidoId))
        {
            var sucesso = await _pedidoRepository.AtualizarStatusAsync(pedidoId, StatusPedido.Aceito, null, null);
            if (sucesso)
            {
                _logger.LogInformation("Status do pedido {NumeroPedido} atualizado para Aceito.", numeroPedido);
            }
            else
            {
                _logger.LogWarning("Não foi possível atualizar o status do pedido {NumeroPedido}.", numeroPedido);
            }
        }
        else
        {
            _logger.LogWarning("PedidoId não informado na mensagem recebida.");
        }
        // Aqui você pode implementar lógica específica da cozinha
        // Por exemplo, notificar cozinheiros, atualizar dashboard, etc.
    }

    private async Task ProcessarPedidoConfirmado(JsonElement messageData)
    {
        var pedidoId = messageData.GetProperty("PedidoId").GetString();
        var numeroPedido = messageData.GetProperty("NumeroPedido").GetString();

        _logger.LogInformation("Pedido confirmado e pronto para preparo: {NumeroPedido}", numeroPedido);

        // Aqui você pode implementar lógica para iniciar o preparo
        // Por exemplo, adicionar à fila de preparo, notificar cozinheiros, etc.
    }

    private async Task ProcessarPedidoCancelado(JsonElement messageData)
    {
        var pedidoId = messageData.GetProperty("PedidoId").GetString();
        var numeroPedido = messageData.GetProperty("NumeroPedido").GetString();
        var motivo = messageData.GetProperty("Motivo").GetString();

        _logger.LogInformation("Pedido cancelado: {NumeroPedido} - Motivo: {Motivo}", numeroPedido, motivo);

        // Aqui você pode implementar lógica para remover da fila de preparo
        // Por exemplo, notificar cozinheiros sobre o cancelamento, etc.
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
} 