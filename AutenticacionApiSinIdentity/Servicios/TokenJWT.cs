using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutenticacionApiSinIdentity.Controllers;
using AutenticacionApiSinIdentity.Datos;
using AutenticacionApiSinIdentity.Interfaces;
using AutenticacionApiSinIdentity.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AutenticacionApiSinIdentity.Servicios
{

    public class TokenJWT : IToken
    {
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;

        public TokenJWT(IConfiguration configuration, ApplicationDbContext context)
        {
            this.configuration = configuration;
            this.context = context;
        }

        /// <summary>
        /// Crea y devuelve el Token al cliente una vez que se registra o hace un login
        /// </summary>
        /// <param name="credencialesUsuario"></param>
        /// <returns></returns>
        public RespuestaAutenticacion CrearToken(Credenciales credencialesUsuario)
        {
            var claims = new List<Claim>()
            {
                new Claim("Logon", credencialesUsuario.Logon),
                new Claim("esAdmin", "1")
             };

            var usuario = context.Usuarios.Where(x => x.Logon == credencialesUsuario.Logon).Include(x => x.Claims).FirstOrDefault();
            foreach (var c in usuario.Claims)
            {
                claims.Add(new Claim(c.Clave, c.valor));
            }

            var llave = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["llavejwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddMonths(2);
            
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiracion, signingCredentials: creds);

            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion
            };
        }

        public RespuestaAutenticacion RefreshToken(Credenciales credencialesUsuario)
        {

            return null;
        }
    }
}
