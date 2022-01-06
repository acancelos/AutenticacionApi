using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutenticacionApiSinIdentity.Modelos;
using AutenticacionApiSinIdentity.Servicios;
using AutenticacionApiSinIdentity.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AutenticacionApiSinIdentity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly IAutenticar autenticar;
        public static List<Usuario> Usuarios = new List<Usuario>();

        public CuentasController( IConfiguration configuration, IAutenticar autenticar)
        {
            this.configuration = configuration;
            this.autenticar = autenticar;
        }

        [HttpPost("Registrar")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(Credenciales credenciales)
        {
            var usuario = new Usuario { Logon = credenciales.Logon, Password=credenciales.Password};
            Usuarios.Add(usuario);
            
            return autenticar.CrearToken(credenciales);
        }

        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login(Credenciales credenciales)
        {
            var resultado = new Usuario { Logon = credenciales.Logon, Password = credenciales.Password };
            
            foreach(var x in Usuarios)
            {
                if (x.Logon == resultado.Logon)
                {
                    if (x.Password == resultado.Password)
                    {
                        
                        return autenticar.CrearToken(credenciales);
                    }
                }
            }
           
            return BadRequest("Login incorrecto");
            
        }
        
    }
}
