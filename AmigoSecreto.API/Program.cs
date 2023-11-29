using AmigoSecreto.API.Data;
using AmigoSecreto.API.Data.Interfaces;
using AmigoSecreto.API.Services;
using AmigoSecreto.API.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

ConfiguresServices(builder);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

void ConfiguresServices(WebApplicationBuilder builder)
{
    builder.Services.AddSingleton<IAmigoDAO, AmigoDAO>();

    builder.Services.AddScoped<IAmigoService, AmigoService>();
    builder.Services.AddScoped<IParDAO, ParDAO>();
    builder.Services.AddScoped<IParService, ParService>();
}