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
        //añadido despues de claims en token
        public async Task<List<Empleado>> GetCompisAsync(int idDepartamento)
        {
            return await this.context.Empleados.Where(x => x.IdDepartamento == idDepartamento).ToListAsync();
        }

        //para probar peticiones con varios parametros en la url
        public async Task<List<string>> GetOficiosAsync()
        {
            var consulta = (from datos in this.context.Empleados
                            select datos.Oficio).Distinct();
            return await consulta.ToListAsync();
        }
        public async Task<List<Empleado>> GetEmpleadosOficiosAsync(List<string> oficios)
        {
            var consulta = from datos in this.context.Empleados
                           where oficios.Contains(datos.Oficio)
                           select datos;
            return await consulta.ToListAsync();
        }
        public async Task IncrementarSalariosAsync(int incremento, List<string> oficios)
        {
            List<Empleado> empleados = await this.GetEmpleadosOficiosAsync(oficios);
            foreach (Empleado emp in empleados)
            {
                emp.Salario += incremento;
            }
            await this.context.SaveChangesAsync();
        }
    }
}
