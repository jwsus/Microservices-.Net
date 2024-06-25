using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OrchestratorService.Data;
using OrchestratorService.IntegrationEvents;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "CustomerRegistrationService", Version = "v1" });
    });

builder.Services.AddDbContext<OrchestratorContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.Configure<RabbitMQSettings>(builder.Configuration.GetSection("RabbitMQ"));

builder.Services.AddSingleton<ICustomerIntegrationEventService, CustomerIntegrationEventService>();
builder.Services.AddSingleton<IReceive, Receive>();

// Adicionar política de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CustomerRegistrationService v1");
        c.RoutePrefix = "swagger"; 
    });
}

app.UseHttpsRedirection();
// Use a política de CORS
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

app.Run();
