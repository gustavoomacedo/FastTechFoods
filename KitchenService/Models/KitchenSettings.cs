namespace KitchenService.Models;

public class KitchenSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string PedidosCollectionName { get; set; } = "pedidos";
    
    // RabbitMQ Configuration
    public string RabbitMQHost { get; set; } = "localhost";
    public int RabbitMQPort { get; set; } = 5672;
    public string RabbitMQUser { get; set; } = "admin";
    public string RabbitMQPassword { get; set; } = "password123";
    public string RabbitMQVirtualHost { get; set; } = "/";
} 