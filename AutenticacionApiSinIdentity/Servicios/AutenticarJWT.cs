using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutenticacionApiSinIdentity.ViewModels;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AutenticacionApiSinIdentity.Servicios
{

    public class AutenticarJWT : IAutenticar
    {
        private readonly IConfiguration configuration;

        public AutenticarJWT(IConfiguration configuration)
        {
            this.configuration = configuration;
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
