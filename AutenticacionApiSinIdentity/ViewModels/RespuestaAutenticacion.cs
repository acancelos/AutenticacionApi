using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutenticacionApiSinIdentity.ViewModels
{
    /// <summary>
    /// Esta es la respuesta que recibe el cliente luego de registrarse o autenticarse
    /// </summary>
    public class RespuestaAutenticacion
    {
        public string Token { get; set; }
        public DateTime Expiracion { get; set; }
    }
}
