using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutenticacionApiSinIdentity.Modelos;
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

        public static List<Usuario> Usuarios = new List<Usuario>();

        public CuentasController( IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost("Registrar")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(Credenciales credenciales)
        {
            var usuario = new Usuario { Logon = credenciales.Logon, Password=credenciales.Password};
            Usuarios.Add(usuario);
            return CrearToken(credenciales);
            
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
                        
                        return CrearToken(credenciales);
                    }
                }
            }
           
            return BadRequest("Login incorrecto");
            
        }
        private RespuestaAutenticacion CrearToken(Credenciales credencialesUsuario)
        {
            var claims = new List<Claim>()
            {
                new Claim("Logon", credencialesUsuario.Logon)

             };

            var llave = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["llavejwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiracion, signingCredentials: creds);

            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion
            };

        }
    }
}
