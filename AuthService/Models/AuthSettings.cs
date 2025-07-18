public class AuthSettings
{
    public string ConnectionString { get; set; } = string.Empty;
    public string DatabaseName { get; set; } = string.Empty;
    public string JwtSecret { get; set; } = string.Empty;
    public string JwtIssuer { get; set; } = string.Empty;
    public string JwtAudience { get; set; } = string.Empty;
    public int JwtExpirationHours { get; set; } = 24;
    public string RabbitMQHost { get; set; } = "localhost";
    public int RabbitMQPort { get; set; } = 5672;
    public string RabbitMQUser { get; set; } = "guest";
    public string RabbitMQPassword { get; set; } = "guest";
} 