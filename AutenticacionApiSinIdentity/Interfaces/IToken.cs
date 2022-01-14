using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutenticacionApiSinIdentity.ViewModels;

namespace AutenticacionApiSinIdentity.Interfaces
{
    public interface IToken
    {
        RespuestaAutenticacion CrearToken(Credenciales credencialesUsuario);

        RespuestaAutenticacion RefreshToken(Credenciales credencialesUsuario);

    }
}
