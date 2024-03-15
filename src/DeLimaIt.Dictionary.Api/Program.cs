using DeLimaIt.Dictionary.Application.Features.Configuration.DependencyInjection;
using DeLimaIt.Dictionary.Application.Shared.Repository;

var builder = WebApplication.CreateBuilder(args);
var conn = builder.Configuration.GetValue<string>("SQL_DELIMAIT_DICTIONARY_CONN_STRING");

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSqlServerDictionary(conn, "30");
builder.Services.AddConfigurationParameters();



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
