using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AutenticacionApiSinIdentity.Modelos
{
    public class ClaimUsuario
    {
        public int Id { get; set; }
        [Required]
        public string Clave { get; set; }
        [Required]
        public string valor { get; set; }

        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; }
    }
}
