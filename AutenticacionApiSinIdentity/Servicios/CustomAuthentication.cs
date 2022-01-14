using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutenticacionApiSinIdentity.Datos;
using AutenticacionApiSinIdentity.Interfaces;
using AutenticacionApiSinIdentity.Modelos;
using AutenticacionApiSinIdentity.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace AutenticacionApiSinIdentity.Servicios
{
    public class CustomAuthentication : IAutenticar
    {
        private readonly Encriptacion encriptacion;
        private readonly ApplicationDbContext context;

        public CustomAuthentication(Encriptacion encriptacion, ApplicationDbContext context)
        {
            this.encriptacion = encriptacion;
            this.context = context;
        }

        public string[] ObtenerCredenciales()
        {
            throw new NotImplementedException();
        }

        public string[] ObtenerRoles()
        {
            throw new NotImplementedException();
        }

        public Usuario  ObtenerUsuario(Credenciales credenciales)
        {
            var usuario = context.Usuarios.Where(x => x.Logon == credenciales.Logon).Include(x => x.Claims).FirstOrDefault();

            return usuario;
        }

        public  bool  VerificarCredenciales(Credenciales creds)
        {
            Task<bool> resp = ValidarUsuarioPass(creds);
            if (resp.Result) { return true; }
            else return false;
        }

        private async Task<bool> ValidarUsuarioPass(Credenciales cred)
        {
            var PassEncriptada = encriptacion.Encriptar(cred.Password);
            var resultado = new Usuario { Logon = cred.Logon, Password = PassEncriptada };


            var usuarioDb = await context.Usuarios.Where(x => x.Logon == cred.Logon).Include(x => x.Claims).FirstOrDefaultAsync();

            if (usuarioDb == null)
            {
                return false;
            }

            if (usuarioDb.Password != PassEncriptada)
            {
                return false;
            }

            return true;
        }
    }
}
