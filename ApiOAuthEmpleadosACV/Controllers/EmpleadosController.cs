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
        public EmpleadosController(RepositoryHospital repo)
        {
            this.repo = repo;
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
            string jsonEmpleado = claim.Value;
            Empleado empleado = JsonConvert.DeserializeObject<Empleado>(jsonEmpleado);
            //recuperamos el empleado desde el repo por si se ha actualizado entre el token y la llamada al perfil
            return await this.repo.FindEmpleadoAsync(empleado.IdEmpleado);
        }
        [Authorize]
        [HttpGet]
        [Route("[action]")]
        public async Task<ActionResult<List<Empleado>>> Compis()
        {
            Claim claim = HttpContext.User.FindFirst(z => z.Type == "UserData");
            string jsonEmpleado = claim.Value;
            Empleado empleado = JsonConvert.DeserializeObject<Empleado>(jsonEmpleado);
            return await this.repo.GetCompisAsync(empleado.IdDepartamento);
        }
    }
}
