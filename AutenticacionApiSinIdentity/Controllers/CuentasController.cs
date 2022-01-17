using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutenticacionApiSinIdentity.Datos;
using AutenticacionApiSinIdentity.Interfaces;
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
using Microsoft.Extensions.Primitives;
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

        private readonly IToken token;
        private readonly ApplicationDbContext context;
        private readonly Encriptacion encriptacion;
        private readonly IAuthorizationService authorizationService;
        private readonly IAutenticar autenticar;
        private readonly IConfiguration configuration;
        private readonly IDataProtector dataProtector;



        //mediante inyección de dependencias agrego el servicio autenticar
        public CuentasController(IToken token, ApplicationDbContext context, Encriptacion encriptacion,
            IAuthorizationService authorizationService, IAutenticar autenticar, IConfiguration configuration)
        {
            
            this.token = token;
            this.context = context;
            this.encriptacion = encriptacion;
            this.authorizationService = authorizationService;
            this.autenticar = autenticar;
            this.configuration = configuration;
        }


        /// <summary>
        /// Controlador para registrar un nuevo usuario y devolverle su Token correspondiente
        /// </summary>
        /// <param name="credenciales">Recibe las credenciales del usuario</param>
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
            return token.CrearToken(autenticar.ObtenerUsuario(credenciales));
        }

        /// <summary>
        /// Controlador para hacer un login 
        /// Devuelve el Token si está todo OK
        /// </summary>
        /// <param name="credenciales">Logon y password del usuario.
        /// Para pruebas Logon: admin Password: admin</param>
        /// <returns></returns>
        [HttpPost("login")]
        public  ActionResult<RespuestaAutenticacion> Login(Credenciales credenciales)
        {

            if (autenticar.VerificarCredenciales(credenciales))
            {
                return token.CrearToken(autenticar.ObtenerUsuario(credenciales));
            }
            else
            {
                return BadRequest("Login incorrecto");
            }      
        }

        /// <summary>
        /// Refresca el Token en caso de recibir un token valido que esté
        /// dentro del umbral de renovación. Caso contrario devuelve un mensaje personalizado.
        /// </summary>
        /// <returns></returns>
        [HttpGet("RefreshToken")]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ActionResult<RespuestaAutenticacion> RenovarToken()
        {
            StringValues ResultadoHeadear;
            HttpContext.Request.Headers.TryGetValue("Authorization", out ResultadoHeadear);
            try
            {
                if ((string)ResultadoHeadear != null)
                {
                    string[] TokenArray = ((string)ResultadoHeadear).Split(' ');

                    string TokenRecibido = "";
                    try
                    {
                        TokenRecibido = TokenArray[1];
                    }
                    catch (Exception)
                    {

                        throw new Exception("El formato de autorización es incorrecto");
                    }
                    
                    var handler = new JwtSecurityTokenHandler();


                    var jwtSecurityToken = handler.ReadJwtToken(TokenRecibido);
                    Credenciales credenciales = new Credenciales();
                    var logonClaim = jwtSecurityToken.Claims.Where(x => x.Type == "Logon").FirstOrDefault();
                    if (logonClaim != null)
                    {

                        credenciales.Logon = logonClaim.Value;
                    }

                    RespuestaAutenticacion respuesta = token.RefreshToken(autenticar.ObtenerUsuario(credenciales), TokenRecibido);

                    if (respuesta.Token != null) return respuesta;
                    return BadRequest(respuesta.Mensaje);
                }
                else
                {
                    return BadRequest("No se envió ningún Token");
                }
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
            

        }

        /// <summary>
        /// Hace Administrador a un usuario registrado. Esto es solo a modo de prueba.
        /// </summary>
        /// <param name="hacerAdminUsuario">Debe ingresar el Logon del usuario que desea hacer Admin</param>
        /// <returns></returns>
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

        /// <summary>
        /// Retira el rol Administrador a un usuario. Solo a modo de Prueba.
        /// </summary>
        /// <param name="hacerAdminUsuario">Debe ingresar el Logon del usuario al que desea quitar permisos de Administrador</param>
        /// <returns></returns>
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
