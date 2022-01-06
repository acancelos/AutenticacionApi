using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutenticacionApiSinIdentity.Modelos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AutenticacionApiSinIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {

        public ClientesController()
        {

        }

        public static List<Cliente> ListaClientes = new List<Cliente>()
        {
            new Cliente(){Id=1,Nombre="Juan",Apellido="Perez"},
            new Cliente(){Id=1,Nombre="Jose",Apellido="Rodriguez"},
            new Cliente(){Id=1,Nombre="Pedro",Apellido="Gonzalez"},
        };

        [HttpGet("VerClientes")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public List<Cliente> GetClientes()
        {
            return  ListaClientes;
        }

    }
}
