using ApiOAuthEmpleadosACV.Helpers;
using ApiOAuthEmpleadosACV.Models;
using ApiOAuthEmpleadosACV.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ApiOAuthEmpleadosACV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpleadosController : ControllerBase
    {
        private RepositoryHospital repo;
        private HelperCifrado helperCrypt;
        private HelperEmpleadoToken helperEmpleadoToken;
        public EmpleadosController(RepositoryHospital repo, HelperCifrado helperCrypt, HelperEmpleadoToken helperEmpleadoToken)
        {
            this.repo = repo;
            this.helperCrypt = helperCrypt;
            this.helperEmpleadoToken = helperEmpleadoToken;
        }
        [HttpGet]
        public async Task<ActionResult<List<Empleado>>> GetEmpleadosAsync()
        {
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return empleados;
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Empleado>> FindEmpleadoAsync(int id)
        {
            Empleado empleado = await this.repo.FindEmpleadoAsync(id);
            return empleado;
        }

        //añadido de claims
        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<Empleado>> Perfil()
        {
            Claim claim = HttpContext.User.FindFirst(z => z.Type == "UserData");
            ////desencriptamos el empleado del claim para recuperar su departamento
            //string jsonEmpleado = claim.Value;
            ////Empleado empleado = JsonConvert.DeserializeObject<Empleado>(jsonEmpleado);
            //Empleado empleadoDecrypt = this.helperCrypt.DecryptObject<Empleado>(jsonEmpleado);

            //string jsonEmpleado = claim.Value;
            //string jsonEmpleadoDecrypt = HelperCifrado.CifrarString(jsonEmpleado);
            //Empleado empleadoDecrypt = JsonConvert.DeserializeObject<Empleado>(jsonEmpleadoDecrypt);

            EmpleadoModel emp = this.helperEmpleadoToken.GetEmpleado();

            return await this.repo.FindEmpleadoAsync(emp.IdEmpleado);
        }
        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Empleado>>> Compis()
        {
            Claim claim = HttpContext.User.FindFirst(z => z.Type == "UserData");
            ////desencriptamos el empleado del claim para recuperar su departamento
            //string jsonEmpleado = claim.Value;
            ////Empleado empleado = JsonConvert.DeserializeObject<Empleado>(jsonEmpleado);
            //Empleado empleadoDecrypt = this.helperCrypt.DecryptObject<Empleado>(jsonEmpleado);

            //string jsonEmpleado = claim.Value;
            //string jsonEmpleadoDecrypt = HelperCifrado.CifrarString(jsonEmpleado);
            //Empleado empleadoDecrypt = JsonConvert.DeserializeObject<Empleado>(jsonEmpleadoDecrypt);

            EmpleadoModel emp = this.helperEmpleadoToken.GetEmpleado();

            return await this.repo.GetCompisAsync(emp.IdDepartamento);
        }
    }
}
