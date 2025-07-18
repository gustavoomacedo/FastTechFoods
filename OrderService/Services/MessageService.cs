using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using OrderService.Models;

namespace OrderService.Services;

public class MessageService : IDisposable
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<MessageService> _logger;
    private readonly string _exchangeName = "fasttech_exchange";

    public MessageService(IOptions<OrderSettings> settings, ILogger<MessageService> logger)
    {
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
        
        // Declarar exchange
        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Topic, durable: true);
        
        _logger.LogInformation("MessageService inicializado com sucesso");
    }

    public void PublicarPedidoCriado(Pedido pedido)
    {
        try
        {
            var message = JsonSerializer.Serialize(new
            {
                Tipo = "PedidoCriado",
                PedidoId = pedido.Id,
                NumeroPedido = pedido.NumeroPedido,
                ClienteId = pedido.ClienteId,
                Status = pedido.Status,
                DataCriacao = pedido.DataCriacao,
                ValorTotal = pedido.ValorTotal
            });

            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: "pedido.criado",
                basicProperties: null,
                body: body);

            _logger.LogInformation("Pedido {PedidoId} publicado na fila", pedido.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar pedido {PedidoId} na fila", pedido.Id);
        }
    }

    public void PublicarPedidoAtualizado(Pedido pedido, string acao)
    {
        try
        {
            var message = JsonSerializer.Serialize(new
            {
                Tipo = "PedidoAtualizado",
                PedidoId = pedido.Id,
                NumeroPedido = pedido.NumeroPedido,
                Status = pedido.Status,
                Acao = acao,
                DataAtualizacao = DateTime.UtcNow
            });

            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: $"pedido.{acao.ToLower()}",
                basicProperties: null,
                body: body);

            _logger.LogInformation("Atualização do pedido {PedidoId} publicada na fila", pedido.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar atualização do pedido {PedidoId} na fila", pedido.Id);
        }
    }

    public void PublicarPedidoCancelado(Pedido pedido, string motivo)
    {
        try
        {
            var message = JsonSerializer.Serialize(new
            {
                Tipo = "PedidoCancelado",
                PedidoId = pedido.Id,
                NumeroPedido = pedido.NumeroPedido,
                ClienteId = pedido.ClienteId,
                Motivo = motivo,
                DataCancelamento = DateTime.UtcNow
            });

            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(
                exchange: _exchangeName,
                routingKey: "pedido.cancelado",
                basicProperties: null,
                body: body);

            _logger.LogInformation("Cancelamento do pedido {PedidoId} publicado na fila", pedido.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao publicar cancelamento do pedido {PedidoId} na fila", pedido.Id);
        }
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
} 