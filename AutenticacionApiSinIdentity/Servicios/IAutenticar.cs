using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutenticacionApiSinIdentity.ViewModels;

namespace AutenticacionApiSinIdentity.Servicios
{

    public interface IAutenticar
    {
        RespuestaAutenticacion CrearToken(Credenciales credencialesUsuario);

      
    }
}
