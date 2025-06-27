namespace OrderService.Models;

public class OrderSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string PedidosCollectionName { get; set; } = "pedidos";
    public string ClientesCollectionName { get; set; } = "clientes";
    public string MenuServiceUrl { get; set; } = "https://localhost:7002";
    
    // RabbitMQ Configuration
    public string RabbitMQHost { get; set; } = "localhost";
    public int RabbitMQPort { get; set; } = 5672;
    public string RabbitMQUser { get; set; } = "admin";
    public string RabbitMQPassword { get; set; } = "password123";
    public string RabbitMQVirtualHost { get; set; } = "/";
} 