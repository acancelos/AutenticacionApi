using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AutenticacionApiSinIdentity.ViewModels
{
    public class Credenciales
    {
        [Required]
        public string Logon { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
