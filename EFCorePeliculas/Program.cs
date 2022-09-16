using EFCorePeliculas;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

// Definimos el tipo de aplicaci�n (web)
var builder = WebApplication.CreateBuilder(args); // Configure services

// Add services to the container.

// Definici�n de los servicios
builder.Services.AddControllers()
    .AddJsonOptions(opciones => opciones.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurando EF para despues inyectar en el DbContext
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(opciones =>
{
    opciones.UseSqlServer(connectionString, sqlServer => sqlServer.UseNetTopologySuite());
    opciones.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking); // Consultas m�s r�pidas en la palma de tu mano
    //opciones.UseLazyLoadingProxies();
});

// El auto-mapper
builder.Services.AddAutoMapper(typeof(Program));

// ======================================================================================

// Creamos la instancia de la app
var app = builder.Build(); // Configure

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
