using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AutenticacionApiSinIdentity.Modelos
{
    public class Usuario
    {
        [NotMapped]
        public const string Key = "Usuario";
        public int Id { get; set; }
        [Required]
        public string Logon { get; set; }
        [Required]
        public string Password { get; set; }

        public List<ClaimUsuario> Claims { get; set; }

        public Usuario()
        {
            Claims = new List<ClaimUsuario>();
        }
    }
}
