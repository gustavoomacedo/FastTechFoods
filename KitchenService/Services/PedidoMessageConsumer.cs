using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using KitchenService.Models;
using KitchenService.Repositories;
using System.Net.Http;

namespace KitchenService.Services;

public class PedidoMessageConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly ILogger<PedidoMessageConsumer> _logger;
    private readonly PedidoRepository _pedidoRepository;
    private readonly string _queueName = "kitchen_queue";
    private readonly string _exchangeName = "fasttech_exchange";
    private readonly IHttpClientFactory _httpClientFactory;

    public PedidoMessageConsumer(IOptions<KitchenSettings> settings, PedidoRepository pedidoRepository, ILogger<PedidoMessageConsumer> logger, IHttpClientFactory httpClientFactory)
    {
        _pedidoRepository = pedidoRepository;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        
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
        var valorTotal = messageData.GetProperty("ValorTotal").GetDecimal();

        _logger.LogInformation("Novo pedido recebido na cozinha: {NumeroPedido} - R$ {ValorTotal}", numeroPedido, valorTotal);

        if (!string.IsNullOrEmpty(pedidoId))
        {
            var pedidoExistente = await _pedidoRepository.ObterPorIdAsync(pedidoId);
            if (pedidoExistente == null)
            {
                // Buscar detalhes do pedido no OrderService
                var httpClient = _httpClientFactory.CreateClient();
                var response = await httpClient.GetAsync($"http://orderservice-service:80/api/order/pedidos/{pedidoId}");
                if (response.IsSuccessStatusCode)
                {
                    var pedidoOrderService = await response.Content.ReadFromJsonAsync<OrderServicePedidoDto>();
                    if (pedidoOrderService != null)
                    {
                        var novoPedido = new Pedido
                        {
                            Id = pedidoOrderService.Id,
                            NumeroPedido = pedidoOrderService.NumeroPedido,
                            DataCriacao = pedidoOrderService.DataCriacao,
                            ClienteNome = pedidoOrderService.ClienteNome,
                            ClienteTelefone = pedidoOrderService.ClienteTelefone,
                            EnderecoEntrega = pedidoOrderService.EnderecoEntrega,
                            ValorTotal = pedidoOrderService.ValorTotal,
                            Status = StatusPedido.Pendente,
                            Observacoes = pedidoOrderService.Observacoes,
                            Itens = pedidoOrderService.Itens.Select(i => new ItemPedido
                            {
                                Nome = i.ProdutoNome ?? i.Nome ?? "Produto",
                                Quantidade = i.Quantidade,
                                PrecoUnitario = i.PrecoUnitario,
                                Observacoes = i.Observacoes
                            }).ToList()
                        };
                        await _pedidoRepository.CriarAsync(novoPedido);
                        _logger.LogInformation("Pedido {NumeroPedido} criado no banco da cozinha.", numeroPedido);
                    }
                    else
                    {
                        _logger.LogWarning("Não foi possível desserializar o pedido do OrderService para o pedido {NumeroPedido}.", numeroPedido);
                    }
                }
                else
                {
                    _logger.LogWarning("Não foi possível obter detalhes do pedido {NumeroPedido} do OrderService. Status: {Status}", numeroPedido, response.StatusCode);
                }
            }
            else
            {
                // Atualizar status se já existir
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
        }
        else
        {
            _logger.LogWarning("PedidoId não informado na mensagem recebida.");
        }
    }

    private async Task ProcessarPedidoConfirmado(JsonElement messageData)
    {
        var pedidoId = messageData.GetProperty("PedidoId").GetString();
        var numeroPedido = messageData.GetProperty("NumeroPedido").GetString();

        _logger.LogInformation("Pedido confirmado e pronto para preparo: {NumeroPedido}", numeroPedido);

        if (!string.IsNullOrEmpty(pedidoId))
        {
            var pedidoExistente = await _pedidoRepository.ObterPorIdAsync(pedidoId);
            if (pedidoExistente != null)
            {
                var sucesso = await _pedidoRepository.AtualizarStatusAsync(pedidoId, StatusPedido.EmPreparo, null, null);
                if (sucesso)
                {
                    _logger.LogInformation("Status do pedido {NumeroPedido} atualizado para EmPreparo.", numeroPedido);
                }
                else
                {
                    _logger.LogWarning("Não foi possível atualizar o status do pedido {NumeroPedido} para EmPreparo.", numeroPedido);
                }
            }
            else
            {
                _logger.LogWarning("Pedido {NumeroPedido} não encontrado no banco da cozinha ao tentar confirmar.", numeroPedido);
            }
        }
        else
        {
            _logger.LogWarning("PedidoId não informado na mensagem recebida para confirmação.");
        }
    }

    private async Task ProcessarPedidoCancelado(JsonElement messageData)
    {
        var pedidoId = messageData.GetProperty("PedidoId").GetString();
        var numeroPedido = messageData.GetProperty("NumeroPedido").GetString();
        var motivo = messageData.GetProperty("Motivo").GetString();

        _logger.LogInformation("Pedido cancelado: {NumeroPedido} - Motivo: {Motivo}", numeroPedido, motivo);

        if (!string.IsNullOrEmpty(pedidoId))
        {
            var pedidoExistente = await _pedidoRepository.ObterPorIdAsync(pedidoId);
            if (pedidoExistente != null)
            {
                var sucesso = await _pedidoRepository.AtualizarStatusAsync(pedidoId, StatusPedido.Cancelado, null, null, motivo);
                if (sucesso)
                {
                    _logger.LogInformation("Status do pedido {NumeroPedido} atualizado para Cancelado.", numeroPedido);
                }
                else
                {
                    _logger.LogWarning("Não foi possível atualizar o status do pedido {NumeroPedido} para Cancelado.", numeroPedido);
                }
            }
            else
            {
                _logger.LogWarning("Pedido {NumeroPedido} não encontrado no banco da cozinha ao tentar cancelar.", numeroPedido);
            }
        }
        else
        {
            _logger.LogWarning("PedidoId não informado na mensagem recebida para cancelamento.");
        }
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
} 

// DTO auxiliar para desserializar o pedido do OrderService
public class OrderServicePedidoDto
{
    public string Id { get; set; } = string.Empty;
    public string NumeroPedido { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public List<OrderServiceItemPedidoDto> Itens { get; set; } = new();
    public string ClienteNome { get; set; } = string.Empty;
    public string ClienteTelefone { get; set; } = string.Empty;
    public string EnderecoEntrega { get; set; } = string.Empty;
    public decimal ValorTotal { get; set; }
    public string? Observacoes { get; set; }
}
public class OrderServiceItemPedidoDto
{
    public string? ProdutoNome { get; set; }
    public string? Nome { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public string? Observacoes { get; set; }
} 