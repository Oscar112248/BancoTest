using Banco.Application.UseCases;
using Banco.Domain.Interfaces;
using Banco.Domain.Services;
using Banco.Infrastructure.Persistence.Context;
using Banco.Infrastructure.Persistence.Repositories;
using BancoTest.Excepciones;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using static Banco.Application.UseCases.CrearClienteHandler;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BANCO_DbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("BancoConnection"),
        b => b.MigrationsAssembly("Banco.Infrastructure")));

builder.Services.AddScoped<CrearCuentaHandler>();
builder.Services.AddScoped<GetCuentaByNumeroHandler>();
builder.Services.AddScoped<ActualizarCuentaHandler>();
builder.Services.AddScoped<ActualizarEstadoCuentaHandler>();
builder.Services.AddScoped<CrearClienteHandler>();
builder.Services.AddScoped<GetClienteByIdNumeroHandler>();
builder.Services.AddScoped<ActualizarClienteHandler>();
builder.Services.AddScoped<RealizarDepositoHandler>();
builder.Services.AddScoped<RealizarRetiroHandler>();
builder.Services.AddScoped<MovimientoDomainService>();
builder.Services.AddScoped<GetClientesHandler>();
builder.Services.AddScoped<ActualizarClienteSoftHandler>();
builder.Services.AddScoped<GetCuentasHandler>();
builder.Services.AddScoped<GetMovimientosHandler>();


builder.Services.AddScoped<ICuentaRepository, CuentaRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<IMovimientoRepository, MovimientoRepository>();
builder.Services.AddScoped<IParametroSistemaRepository, ParametroSistemaRepository>();

builder.Services.AddControllers();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// Swagger
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "MDMQ SAO SERVICIOS",
        Description = "ENDPOINTS SAO",
        Contact = new OpenApiContact
        {
            Name = "STICS - DMIST - Ingeniería de Soluciones",
            Email = "ingsoluciones@quito.gob.ec",
        }
    });
});


var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MiPoliticaCors", policy =>
    {
        policy.WithOrigins(allowedOrigins)  
              .AllowAnyMethod()                     
              .AllowAnyHeader()                     
              .AllowCredentials();                  
    });
});

var app = builder.Build();
app.UseCors("MiPoliticaCors");

app.UseExceptionHandler();

// (Opcional) redirección https
app.UseHttpsRedirection();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
    options.RoutePrefix = "swagger";
});

app.UseAuthorization();

app.MapControllers();

app.Run();
