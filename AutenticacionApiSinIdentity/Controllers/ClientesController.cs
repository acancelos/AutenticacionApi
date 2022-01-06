using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutenticacionApiSinIdentity.Modelos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Options;

namespace AutenticacionApiSinIdentity.Controllers
{
    /// <summary>
    /// Este controlador lo uso solo para probar la autenticación.
    /// Solo recibe una petición get que devuelve un listado de clientes hardcodeado en una lista
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {

        public ClientesController(IOptions<Usuario> options)
        {
            this.options = options;
        }

        public static List<Cliente> ListaClientes = new List<Cliente>()
        {
            new Cliente(){Id=1,Nombre="Juan",Apellido="Perez"},
            new Cliente(){Id=1,Nombre="Jose",Apellido="Rodriguez"},
            new Cliente(){Id=1,Nombre="Pedro",Apellido="Gonzalez"},
        };
        private readonly IOptions<Usuario> options;

        // le agrego el atribute authoriza especificando el AutheticationScheme
        [HttpGet("VerClientes")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public List<Cliente> GetClientes()
        {
            var x = options.Value;
            return  ListaClientes;
        }

    }
}
