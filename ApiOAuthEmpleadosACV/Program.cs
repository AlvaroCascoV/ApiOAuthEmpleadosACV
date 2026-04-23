using ApiOAuthEmpleadosACV.Data;
using ApiOAuthEmpleadosACV.Helpers;
using ApiOAuthEmpleadosACV.Repositories;
using Azure.Security.KeyVault.Secrets;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);


//CREAMOS UNA INSTANCIA DE NUESTRO HELPER
HelperActionOAuthService helper = new HelperActionOAuthService(builder.Configuration);

//HelperCifrado helperCifrado = new HelperCifrado(builder.Configuration);
HelperCifrado.Initialize(builder.Configuration);

//ESTA ISNTANCIA SOLAMENTE DEBEMOS CREARLA UNA VEZ
builder.Services.AddSingleton<HelperActionOAuthService>(helper);

builder.Services.AddSingleton<HelperCifrado>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<HelperEmpleadoToken>();

//HABILITAMOS LA SEGURIDAD DENTRO DE PROGRAM
builder.Services.AddAuthentication(helper.GetAuthenticationSchema())
    .AddJwtBearer(helper.GetJwtBearerOptions());

// Add services to the container.


//SERVICIOS PARA SECRETS

builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient(builder.Configuration.GetSection("KeyVault"));
});
//ESTE OBJETO SOLAMENTE LO NECESITAMOS AQUI, LO DICHO, RECUPERAMOS
//LOS VALORES Y LOS ASIGNAMOS A UNA CLASE O VARIABLE
//RECUPERAMOS EL SecretClient PARA LOS SECRETOS DE KEYVAULT

SecretClient secretClient = builder.Services.BuildServiceProvider().GetService<SecretClient>();
//ACCEDEMOS AL SECRETO
KeyVaultSecret secreto = secretClient.GetSecret("secretsqlazureacv");
string connectionStringSecret = secreto.Value;

//conexiones a bbdd
string connectionStringLocal = builder.Configuration.GetConnectionString("SQLHospital");
string connectionStringAzure = builder.Configuration.GetConnectionString("AZURETAJAMAR");
builder.Services.AddTransient<RepositoryHospital>();
builder.Services.AddDbContext<HospitalContext>(options => options.UseSqlServer(connectionStringSecret));

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
