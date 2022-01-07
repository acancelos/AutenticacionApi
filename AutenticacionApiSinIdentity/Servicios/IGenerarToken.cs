using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutenticacionApiSinIdentity.ViewModels;

namespace AutenticacionApiSinIdentity.Servicios
{

    public interface IGenerarToken
    {
        RespuestaAutenticacion CrearToken(Credenciales credencialesUsuario);

       
    }
}
