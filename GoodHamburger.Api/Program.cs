using GoodHamburger.Api.Data;
using GoodHamburger.Api.Middleware;
using GoodHamburger.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// === Serviços ===

// Entity Framework Core com SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Injeção de dependência dos serviços
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Controllers
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS (para o frontend Blazor se conectar futuramente)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// === Middleware Pipeline ===

// Tratamento global de exceções (deve ser o primeiro middleware)
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger (disponível em todos os ambientes para facilitar testes)
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Good Hamburger API v1");
    options.RoutePrefix = string.Empty; // Swagger na raiz
});

app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");
app.MapControllers();

// Aplicar migrations e seed automaticamente ao iniciar
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
