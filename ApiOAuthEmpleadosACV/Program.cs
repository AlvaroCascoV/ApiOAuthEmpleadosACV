using ApiOAuthEmpleadosACV.Data;
using ApiOAuthEmpleadosACV.Helpers;
using ApiOAuthEmpleadosACV.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

//CREAMOS UNA INSTANCIA DE NUESTRO HELPER
HelperActionOAuthService helper = new HelperActionOAuthService(builder.Configuration);
//ESTA ISNTANCIA SOLAMENTE DEBEMOS CREARLA UNA VEZ
builder.Services.AddSingleton<HelperActionOAuthService>(helper);
//HABILITAMOS LA SEGURIDAD DENTRO DE PROGRAM
builder.Services.AddAuthentication(helper.GetAuthenticationSchema())
    .AddJwtBearer(helper.GetJwtBearerOptions());

// Add services to the container.

string connectionStringLocal = builder.Configuration.GetConnectionString("SQLHospital");
string connectionStringAzure = builder.Configuration.GetConnectionString("AZURETAJAMAR");
builder.Services.AddTransient<RepositoryHospital>();
builder.Services.AddDbContext<HospitalContext>(options => options.UseSqlServer(connectionStringAzure));

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.MapOpenApi();
app.MapScalarApiReference();

app.MapGet("/", () => Results.Redirect("/scalar"));

app.UseHttpsRedirection();
//importante el orden
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
