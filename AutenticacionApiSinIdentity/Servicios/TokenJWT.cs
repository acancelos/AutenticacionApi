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
            var expiracion = DateTime.UtcNow.AddSeconds(2);
            
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiracion, signingCredentials: creds);

            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion
            };
        }

        public RespuestaAutenticacion RefreshToken(Credenciales credencialesUsuario, string TokenRecibido)
        {
          

            if (ValidarToken(TokenRecibido)) return CrearToken(credencialesUsuario);
            else return null;
        }

        private bool ValidarToken(string TokenRecibido)
        {
            
            var handler = new JwtSecurityTokenHandler();
            //var jwtSecurityToken = handler.ReadJwtToken(TokenRecibido);
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
                if (tokenSecure.ValidTo > DateTime.UtcNow) return true;
                else return false;
               
            }
            catch (Exception)
            {

                return false;
            }

            
        }

        
    }
}
