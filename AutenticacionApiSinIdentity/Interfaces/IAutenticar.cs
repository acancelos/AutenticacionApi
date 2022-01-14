using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutenticacionApiSinIdentity.Modelos;
using AutenticacionApiSinIdentity.ViewModels;

namespace AutenticacionApiSinIdentity.Interfaces
{
    public interface IAutenticar
    {
        string[] ObtenerCredenciales();
        string[] ObtenerRoles();

        bool VerificarCredenciales(Credenciales creds);

        Usuario ObtenerUsuario(Credenciales creds);
        
    }
}
