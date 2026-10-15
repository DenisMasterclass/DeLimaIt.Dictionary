using DeLimaIt.Dictionary.Application.Features.Configuration.DependencyInjection;
using DeLimaIt.Dictionary.Application.Shared.Repository;

var builder = WebApplication.CreateBuilder(args);
var conn = builder.Configuration.GetValue<string>("SQL_DELIMAIT_DICTIONARY_CONN_STRING");
//string cnn = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=" + AppContext.BaseDirectory + "\\Shared\\Data\\Dictionary.mdf;Integrated Security=True";

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSqlServerDictionary(conn, "30");
builder.Services.AddConfigurationDictionaries();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Production traffic uses HTTP on the private cluster network; TLS ends at the edge.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = _ => false
});

app.Run();

public partial class Program { }
