using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AutenticacionApiSinIdentity.Modelos
{
    public class Usuario
    {
        public const string Key = "Usuario";
        public string Logon { get; set; }
        public string Password { get; set; }
    }
}
