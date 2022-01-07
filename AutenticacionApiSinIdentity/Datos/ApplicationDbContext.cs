using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutenticacionApiSinIdentity.Modelos;
using Microsoft.EntityFrameworkCore;

namespace AutenticacionApiSinIdentity.Datos
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<ClaimUsuario> Claims { get; set; }

        public DbSet<Cliente> Clientes { get; set; }
    }
}
