using ApiOAuthEmpleadosACV.Models;
using ApiOAuthEmpleadosACV.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            List<Empleado> empleados = await this.repo.GetEmpleadosAsync();
            return empleados;
        }
        [HttpGet("{id}")]
        [Authorize]
        public async Task<Empleado> FindEmpleadoAsync(int id)
        {
            Empleado empleado = await this.repo.FindEmpleadoAsync(id);
            return empleado;
        }
    }
}
