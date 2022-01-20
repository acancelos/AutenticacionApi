using System;
using System.Collections.Generic;
using AutenticacionApiSinIdentity.Servicios;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using TestMSTest.Mocks;

namespace TestMSTest.PruebasUnitarias
{
    [TestClass]
    public class EncriptacionTests
    {
        [TestMethod]
        public void DevuelveCadenaEncriptada()
        {
            //Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"Llave", "Lllave de Test"}};

            IConfiguration configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();


            var encriptar = new Encriptacion(configuration);
            string cadena = "Cadena a encriptar";

            var resultado = encriptar.Encriptar(cadena);

            Assert.AreEqual("ITaS6dqSPAWpqQQKgeh6cPtX59YOKLRX", resultado);

        }

        
    }
}
