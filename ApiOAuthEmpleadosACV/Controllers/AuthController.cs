using ApiOAuthEmpleadosACV.Helpers;
using ApiOAuthEmpleadosACV.Models;
using ApiOAuthEmpleadosACV.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.Pkcs;

namespace ApiOAuthEmpleadosACV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private RepositoryHospital repo;
        private HelperActionOAuthService helper;
        private HelperCifrado helperCrypt;
        public AuthController(RepositoryHospital repo, HelperActionOAuthService helper, HelperCifrado helperCrypt)
        {
            this.repo = repo;
            this.helper = helper;
            this.helperCrypt = helperCrypt;
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<ActionResult> Login(LoginModel model)
        {
            Empleado empleado = await this.repo.LogInEmpleado(model.UserName, int.Parse(model.Password));
            if (empleado == null)
            {
                return Unauthorized();
            }
            else
            {
                //DEBEMOS CREAR UNAS CREDENCIALES CON NUESTRO
                //TOKEN
                SigningCredentials credentials = new SigningCredentials(this.helper.GetKeyToken(), SecurityAlgorithms.HmacSha256);

                //añadido posteriormente: añadimos mas info al token
                //string jsonEmpleado = JsonConvert.SerializeObject(empleado);
                //encriptamos los datos con nuestro nuevo helper
                //string jsonEncryptEmpleado = this.helperCrypt.EncryptObject(empleado);

                //usamos un model intermedio para almacenar solo los datos que queremos en el token
                //CREAMOS NUESTRO MODELO PARA ALMACENARLO EN EL TOKEN
                EmpleadoModel modelEmp = new EmpleadoModel 
                { 
                    IdEmpleado = empleado.IdEmpleado,
                    Apellido = empleado.Apellido,
                    Oficio = empleado.Oficio,
                    Salario = empleado.Salario,
                    IdDepartamento = empleado.IdDepartamento
                };
                string jsonEmpleado = JsonConvert.SerializeObject(modelEmp);
                string jsonCypher = HelperCifrado.CifrarString(jsonEmpleado);

                //CREAMOS UN ARRAY DE CLAIMS PARA EL TOKEN
                Claim[] informacion = new[]
                {
                    new Claim("UserData", jsonCypher)
                };

                //EL TOKEN SE GENERA CON UNA CLASE Y DEBEMOS
                //ALMACENAR LOS DATOS DE ISSUER, CREDENTIALS...
                JwtSecurityToken token = new JwtSecurityToken(
                    issuer: this.helper.Issuer,
                    audience: this.helper.Audience,
                    signingCredentials: credentials,
                    expires: DateTime.UtcNow.AddMinutes(20),
                    notBefore: DateTime.UtcNow,
                    //añadimos los claims
                    claims: informacion
                    );
                //POR ULTIMO, DEVOLVEMOS LA RESPUESTA AFIRMATIVA
                //CON EL TOKEN
                return Ok(new
                {
                    response = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
        }
    }
}
