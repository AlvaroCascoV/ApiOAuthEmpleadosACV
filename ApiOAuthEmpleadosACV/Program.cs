using ApiOAuthEmpleadosACV.Data;
using ApiOAuthEmpleadosACV.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

string connectionString = builder.Configuration.GetConnectionString("AZURETAJAMAR");
builder.Services.AddTransient<RepositoryHospital>();
builder.Services.AddDbContext<HospitalContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapOpenApi();
app.MapScalarApiReference();

app.MapGet("/", () => Results.Redirect("/scalar"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
