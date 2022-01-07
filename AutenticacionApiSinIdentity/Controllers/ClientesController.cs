using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutenticacionApiSinIdentity.Datos;
using AutenticacionApiSinIdentity.Modelos;
using AutenticacionApiSinIdentity.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AutenticacionApiSinIdentity.Controllers
{
    /// <summary>
    /// Este controlador lo uso solo para probar la autenticación.
    /// Solo recibe una petición get que devuelve un listado de clientes hardcodeado en una lista
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy ="Admin")]
    public class ClientesController : ControllerBase
    {

        public ClientesController(IOptions<Usuario> options, ApplicationDbContext context)
        {
            this.options = options;
            this.context = context;
        }

        private readonly IOptions<Usuario> options;
        private readonly ApplicationDbContext context;

        // le agrego el atribute authoriza especificando el AutheticationScheme
        [HttpGet("VerClientes")]   
        [AllowAnonymous]
        public List<Cliente> GetClientes()
        {
            //Leer los claims
            var logon = HttpContext.User.Claims.Where(x => x.Type == "Logon").FirstOrDefault();
            //Usando IOptions
            var x = options.Value;
            return  context.Clientes.ToList();
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AgregarClienteVM clienteVM)
        {
            Cliente cliente = new Cliente()
            {
                Nombre = clienteVM.Nombre,
                Apellido = clienteVM.Apellido
            };
            
            context.Add(cliente);
            await context.SaveChangesAsync();
            return Ok();
        }

        // DELETE: api/Clientes/numero
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var existe = await context.Clientes.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Cliente() { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }

    }
}
