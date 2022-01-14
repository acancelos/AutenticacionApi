using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutenticacionApiSinIdentity.Modelos;
using AutenticacionApiSinIdentity.ViewModels;

namespace AutenticacionApiSinIdentity.Interfaces
{
    public interface IToken
    {
        RespuestaAutenticacion CrearToken(Usuario usuario);

        RespuestaAutenticacion RefreshToken(Usuario usuario, string TokenRecibido);

    }
}
