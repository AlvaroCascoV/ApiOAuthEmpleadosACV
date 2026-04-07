using ApiOAuthEmpleadosACV.Data;
using ApiOAuthEmpleadosACV.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiOAuthEmpleadosACV.Repositories
{
    public class RepositoryHospital
    {
        private HospitalContext context;

        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<List<Empleado>> GetEmpleadosAsync()
        {
            List<Empleado> empleados = await context.Empleados.ToListAsync();
            return empleados;
        }
        public async Task<Empleado> FindEmpleadoAsync(int idempleado)
        {
            Empleado empleado = await context.Empleados.Where(x => x.IdEmpleado == idempleado).FirstOrDefaultAsync();
            return empleado;
        }
        public async Task<Empleado> LogInEmpleado(string apellido, int idEmpleado) 
        {
            return await this.context.Empleados.Where(x => x.Apellido == apellido && x.IdEmpleado == idEmpleado).FirstOrDefaultAsync();
        }
    }
}
