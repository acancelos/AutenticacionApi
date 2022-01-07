using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutenticacionApiSinIdentity.Datos;
using AutenticacionApiSinIdentity.Modelos;
using AutenticacionApiSinIdentity.Servicios;
using AutenticacionApiSinIdentity.ViewModels;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AutenticacionApiSinIdentity.Controllers
{
    /// <summary>
    /// Este controlador sirve para registrar un usuario y hacer un login
    /// El registro se almacena en una LocalDb
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CuentasController : ControllerBase
    {
       
        private readonly IGenerarToken autenticar;
        private readonly ApplicationDbContext context;
        private readonly Encriptacion encriptacion;
        private readonly IDataProtector dataProtector;
        

        //mediante inyección de dependencias agrego el servicio autenticar
        public CuentasController(  IGenerarToken autenticar, ApplicationDbContext context, Encriptacion encriptacion)
        {
            
            this.autenticar = autenticar;
            this.context = context;
            this.encriptacion = encriptacion;
        }


        /// <summary>
        /// Controlador para registrar un nuevo usuario y devolverle su Token correspondiente
        /// </summary>
        /// <param name="credenciales"></param>
        /// <returns></returns>
        [HttpPost("Registrar")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar(Credenciales credenciales)
        {
            var PassEncriptada = encriptacion.Encriptar(credenciales.Password);
            var usuario = new Usuario { Logon = credenciales.Logon, Password= PassEncriptada };
            //ANtes de agregar el usuario verifico que el Logon sea unico

            

            var existe = await context.Usuarios.AnyAsync(x => x.Logon == credenciales.Logon);
            if (existe)
            {
                return BadRequest($"El Logon {credenciales.Logon} ya existe en el sistema. Elija otro.");
            }
            context.Usuarios.Add(usuario);
            await context.SaveChangesAsync();
            
            //Uso el servicio autenticar que genera el Token
            return autenticar.CrearToken(credenciales);
        }

        /// <summary>
        /// Controlador para hacer un login 
        /// Devuelve el Token si está todo OK
        /// </summary>
        /// <param name="credenciales"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacion>> Login(Credenciales credenciales)
        {
            var PassEncriptada = encriptacion.Encriptar(credenciales.Password);
            var resultado = new Usuario { Logon = credenciales.Logon, Password = PassEncriptada };


            var usuarioDb = await context.Usuarios.Where(x => x.Logon == credenciales.Logon).Include(x => x.Claims).FirstOrDefaultAsync();

            if (usuarioDb == null)
            {
                return BadRequest("Login incorrecto");
            }

            if(usuarioDb.Password!= PassEncriptada)
            {
                return BadRequest("Login incorrecto");
            }

            return autenticar.CrearToken(credenciales);

           
        }

        [HttpGet("RenovarToken")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<RespuestaAutenticacion> RenovarToken()
        {
            var logonClaim = HttpContext.User.Claims.Where(x => x.Type == "Logon").FirstOrDefault();
            var logon = logonClaim.Value;
            var credenciales = new Credenciales()
            {
                Logon = logon
            };
            return autenticar.CrearToken(credenciales);
        }

        [HttpPost("HacerAdmin")]
        public ActionResult HacerAdmin(HacerAdminUsuarioVM hacerAdminUsuario)
        {
            var usuario = context.Usuarios.Where(x => x.Logon == hacerAdminUsuario.Logon).Include(x => x.Claims).FirstOrDefault();
            if (usuario == null)
            {
                return NotFound();
            }
            else
            {
                if (!usuario.Claims.Exists(x => x.Clave == "Admin"))
                {
                    usuario.Claims.Add(new ClaimUsuario() { Clave = "Admin", valor = "1" });
                    context.SaveChanges();
                }
                else
                {
                    return BadRequest("El usuario ya es administrador");
                }
                
            }

            return NoContent();
        }

        [HttpPost("RemoverAdmin")]
        public ActionResult RemoverAdmin(HacerAdminUsuarioVM hacerAdminUsuario)
        {
            var usuario = context.Usuarios.Where(x => x.Logon == hacerAdminUsuario.Logon).Include(x=>x.Claims).FirstOrDefault();
            if (usuario == null)
            {
                return NotFound();
            }
            else
            {
                if (!usuario.Claims.Exists(x => x.Clave == "Admin"))
                {
                    return BadRequest("El usuario NO es administrador");
                }
                usuario.Claims.Remove(usuario.Claims.Find(x => x.Clave == "Admin"));
                context.SaveChanges();
            }

            return NoContent();
        }

    }
}
