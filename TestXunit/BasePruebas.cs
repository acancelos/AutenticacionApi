using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutenticacionApiSinIdentity.Datos;
using Microsoft.EntityFrameworkCore;

namespace TestXunit
{
    public class BasePruebas
    {
        protected ApplicationDbContext ConstruirBase(string NombreBD)
        {
            var opciones = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(NombreBD).Options;
            var dbContext = new ApplicationDbContext(opciones);

            return dbContext;
        }
    }
}
