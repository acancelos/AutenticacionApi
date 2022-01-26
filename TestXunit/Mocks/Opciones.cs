using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutenticacionApiSinIdentity.Modelos;
using Microsoft.Extensions.Options;

namespace TestXunit.Mocks
{
    class Opciones : IOptions<Usuario>
    {
        public Usuario Value => new Usuario { Id = 1, Logon = "admin", Password = "admin" };
    }
}
