using System.Reflection;
using MediatR;
using TaxCalculator.Infrastructure;
using TaxCalculator.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using TaxCalculator.Domain.Repositories;
using TaxCalculator.Domain.Services;
using TaxCalculator.Application.TaxCalculation.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddDbContext<TaxCalculatorDbContext>(options =>
    options.UseInMemoryDatabase("TaxCalculatorDb"));

builder.Services.AddScoped<ITaxBandRepository, TaxBandRepository>();
builder.Services.AddScoped<ITaxCalculatorService, TaxCalculatorService>();


var assemblies = AppDomain.CurrentDomain.GetAssemblies()
    .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
    .ToArray();

builder.Services.AddMediatR(cfg =>
{
    foreach (var assembly in assemblies)
    {
        cfg.RegisterServicesFromAssembly(assembly);
    }
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://localhost:7078") 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaxCalculatorDbContext>();
    dbContext.SeedTaxBands();
}


if (app.Environment.IsDevelopment())
{

    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();
app.Run();
