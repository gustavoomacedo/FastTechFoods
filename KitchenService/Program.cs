using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using KitchenService.Models;
using KitchenService.Repositories;
using KitchenService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "KitchenService", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando o esquema Bearer. Exemplo: 'Bearer {token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configurar KitchenSettings
builder.Services.Configure<KitchenSettings>(builder.Configuration.GetSection("KitchenSettings"));

// Configurar AuthSettings (para validação JWT)
builder.Services.Configure<AuthSettings>(builder.Configuration.GetSection("AuthSettings"));
var authSettings = builder.Configuration.GetSection("AuthSettings").Get<AuthSettings>();
var key = Encoding.ASCII.GetBytes(authSettings?.JwtSecret ?? "default-secret-key-for-development");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Registrar repositórios
builder.Services.AddSingleton<PedidoRepository>();

// Registrar serviços de mensageria
builder.Services.AddHostedService<PedidoMessageConsumer>();

// Adicionar suporte a HttpClientFactory
builder.Services.AddHttpClient();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    builder.WebHost.ConfigureKestrel(serverOptions =>
//    {
//        serverOptions.ListenAnyIP(80);
//    });
//}

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(80);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () =>
{
    return "Healthy";
})
.WithName("GetHealth");

app.MapControllers();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

// Classe AuthSettings para compatibilidade com JWT
public class AuthSettings
{
    public string JwtSecret { get; set; } = string.Empty;
}
