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
    /// <summary>
    /// Este controlador sirve para registrar un usuario y hacer un login
    /// El registro es cntra una lista static de usuarios a modo de prueba
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
       
        private readonly IAutenticar autenticar;

        //Lista donde se van a guardar los usuarios que se registran
        public static List<Usuario> Usuarios = new List<Usuario>();

        //mediante inyección de dependencias agrego el servicio autenticar
        public CuentasController(  IAutenticar autenticar)
        {
            
            this.autenticar = autenticar;
        }


        /// <summary>
        /// Controlador para registrar un nuevo usuario y devolverle su Token correspondiente
        /// </summary>
        /// <param name="credenciales"></param>
        /// <returns></returns>
        [HttpPost("Registrar")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(Credenciales credenciales)
        {
            var usuario = new Usuario { Logon = credenciales.Logon, Password=credenciales.Password};
            Usuarios.Add(usuario);
            
            //Uso el servicio autenticar que genera el Token
            return autenticar.CrearToken(credenciales);
        }

        /// <summary>
        /// Controlador para hacer un login contra la lista static
        /// Devuelve el Token si está todo OK
        /// </summary>
        /// <param name="credenciales"></param>
        /// <returns></returns>
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
