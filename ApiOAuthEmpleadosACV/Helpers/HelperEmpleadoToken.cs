using ApiOAuthEmpleadosACV.Models;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ApiOAuthEmpleadosACV.Helpers
{
    public class HelperEmpleadoToken
    {
        private IHttpContextAccessor contextAccessor;

        public HelperEmpleadoToken(IHttpContextAccessor contextAccessor)
        {
            this.contextAccessor = contextAccessor;
        }

        public EmpleadoModel GetEmpleado()
        {
            Claim claim = this.contextAccessor.HttpContext.User.FindFirst(z => z.Type == "UserData");
            string json = claim.Value;
            string jsonEmpleado = HelperCifrado.DescifrarString(json);
            EmpleadoModel model = JsonConvert.DeserializeObject<EmpleadoModel>(jsonEmpleado);
            return model;
        }
    }
}
