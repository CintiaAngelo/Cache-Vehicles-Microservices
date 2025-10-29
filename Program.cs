using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using VehicleManagementAPI.Data;
using VehicleManagementAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar Entity Framework
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("VehicleDB"));

// Configurar Cache
builder.Services.AddMemoryCache();

// Configurar Redis (se disponível) com fallback
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    try
    {
        var configuration = "localhost:6379,abortConnect=false,connectTimeout=5000";
        var redis = ConnectionMultiplexer.Connect(configuration);
        Console.WriteLine("✅ Conectado ao Redis com sucesso!");
        return redis;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Não foi possível conectar ao Redis: {ex.Message}");
        Console.WriteLine("🔶 Usando MemoryCache...");
        return null!;
    }
});

// Registrar serviços
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IVehicleService, VehicleService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();