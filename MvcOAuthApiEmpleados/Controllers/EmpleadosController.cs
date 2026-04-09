using Microsoft.AspNetCore.Mvc;
using MvcOAuthApiEmpleados.FIlters;
using MvcOAuthApiEmpleados.Models;
using MvcOAuthApiEmpleados.Services;
using NuGet.Common;
using System.Security.Claims;

namespace MvcOAuthApiEmpleados.Controllers
{
    public class EmpleadosController : Controller
    {
        private ServiceEmpleados service;
        public EmpleadosController(ServiceEmpleados service)
        {
            this.service = service;
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> Index()
        {
            List<Empleado> empleados = await this.service.GetEmpleadosAsync();
            return View(empleados);
        }

        [AuthorizeEmpleados]
        public async Task<IActionResult> Details(int idempleado)
        {
            //TENDREMOS EL TOKEN EN SESSION
            //string token = HttpContext.Session.GetString("TOKEN");
            //if(token == null)
            //{
            //    ViewData["MENSAJE"] = "Debe hacer Login";
            //    return View();
            //}
            //else
            //{
            //    Empleado empleado = await this.service.FindEmpleadoAsync(idempleado, token);
            //    return View(empleado);
            //} 
            //cambiamos a token por claims

            Empleado empleado = await this.service.FindEmpleadoAsync(idempleado);
            return View(empleado);
        }
        [AuthorizeEmpleados]
        public async Task<IActionResult> PerfilEmpleado()
        {
            //NECESITAMOS BUSCAR EL EMPLEADO CON SU CLAIM Y
            //NAME IDENTIFIER
            var data = HttpContext.User.FindFirst(z => z.Type == ClaimTypes.NameIdentifier);
            int idEmpleado = int.Parse(data.Value);
            Empleado empleado = await this.service.FindEmpleadoAsync(idEmpleado);
            return View(empleado);
        }
        //perfil token
        [AuthorizeEmpleados]
        public async Task<IActionResult> Perfil()
        {
            Empleado empleado = await this.service.GetPerfilAsync();
            return View(empleado);
        }
        [AuthorizeEmpleados]
        public async Task<IActionResult> Compis()
        {
            List<Empleado> compis = await this.service.GetCompisAsync();
            return View(compis);
        }

        public async Task<IActionResult> EmpleadosOficios()
        {
            List<string> oficios = await this.service.GetOficiosAsync();
            ViewData["OFICIOS"] = oficios;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> EmpleadosOficios(int? incremento, List<string> oficio, string accion)
        {
            List<string> oficios = await this.service.GetOficiosAsync();
            ViewData["OFICIOS"] = oficios;

            if (accion.ToLower() == "update")
            {
                await this.service.UpdateEmpleadosAsync(incremento.Value, oficio);
            }
            List<Empleado> empleados = await this.service.GetEmpleadosOficiosAsync(oficio);
            return View(empleados);
        }
    }
}
