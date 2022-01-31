using AutenticacionApiSinIdentity.Controllers;
using AutenticacionApiSinIdentity.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMSTest.PruebasUnitarias
{
    [TestClass]
    public class TestLocalDb
    {
        [TestMethod]
        public void ProbarLocalDbTest()
        {
            var context = LocalDbDatabaseInitializer.GetDbContextLocalDb(false);
            var opciones = new Mock<IOptions<Usuario>>();
            

            var controller = new ClientesController(opciones.Object, context);

            var res = controller.GetClientes(null);

            Assert.IsTrue(res.Count>0);
        }
    }
}
