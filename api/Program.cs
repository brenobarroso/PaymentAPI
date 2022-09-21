using api.Interfaces;
using Microsoft.EntityFrameworkCore;
using PaymentAPI.Data;
using PaymentAPI.Validations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var serverDbOptions = new DbContextOptionsBuilder<PaymentDbContext>();
            serverDbOptions.UseNpgsql(builder.Configuration.GetSection("ServerDbSettings")["ConnectionString"]);
builder.Services.AddSingleton<PaymentDbContext>(new PaymentDbContext(serverDbOptions.Options));
builder.Services.AddTransient<ITransactionsManager, TransactionsManager>();
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
