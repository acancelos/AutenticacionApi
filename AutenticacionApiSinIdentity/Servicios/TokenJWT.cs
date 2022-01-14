using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutenticacionApiSinIdentity.Controllers;
using AutenticacionApiSinIdentity.Datos;
using AutenticacionApiSinIdentity.Interfaces;
using AutenticacionApiSinIdentity.Modelos;
using AutenticacionApiSinIdentity.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace AutenticacionApiSinIdentity.Servicios
{

    public class TokenJWT : IToken
    {
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;
        private readonly IHttpContextAccessor accessor;
        private readonly IAutenticar autenticar;

        public TokenJWT(IConfiguration configuration, ApplicationDbContext context,IHttpContextAccessor accessor, 
            IAutenticar autenticar)
        {
            this.configuration = configuration;
            this.context = context;
            this.accessor = accessor;
            this.autenticar = autenticar;
        }

        /// <summary>
        /// Crea y devuelve el Token al cliente una vez que se registra o hace un login
        /// </summary>
        /// <param name="credencialesUsuario"></param>
        /// <returns></returns>
        public RespuestaAutenticacion CrearToken(Usuario usuario)
        {
            var claims = new List<Claim>()
            {
                new Claim("Logon", usuario.Logon),
            };

            
            
            foreach (var c in usuario.Claims)
            {
                claims.Add(new Claim(c.Clave, c.valor));
            }

            var llave = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["llavejwt"]));
            var creds = new SigningCredentials(llave, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddSeconds(2);
            
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiracion, signingCredentials: creds);

            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion
            };
        }

        public RespuestaAutenticacion RefreshToken(Usuario usuario, string TokenRecibido)
        {
            //Puedo usar el HTTPContext aca y sacarlo del cuentasController
          //var a = accessor.HttpContext.User.Identity.Name;

            if (ValidarToken(TokenRecibido)) return CrearToken(usuario);
            else return null;
        }

        private bool ValidarToken(string TokenRecibido)
        {  

            
            var handler = new JwtSecurityTokenHandler();
            
            //Valido el Token
            string secret = configuration["llaveJWT"];
            var key = Encoding.ASCII.GetBytes(secret);
            var validations = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };

            try
            {
                var claims = handler.ValidateToken(TokenRecibido, validations, out var tokenSecure);
                
                //ACa se puede verificar el umbral de refresh
                if (tokenSecure.ValidTo.AddHours(1) > DateTime.UtcNow) return true;
                else return false;
               
            }
            catch (Exception)
            {

                return false;
            }

            
        }

        
    }
}
