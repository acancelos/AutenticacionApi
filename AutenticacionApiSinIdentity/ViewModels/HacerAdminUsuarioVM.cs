using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AutenticacionApiSinIdentity.ViewModels
{
    public class HacerAdminUsuarioVM
    {
        [Required]
        public string Logon { get; set; }
    }
}
