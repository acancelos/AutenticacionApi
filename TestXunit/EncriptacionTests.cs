using System;
using System.Collections.Generic;
using AutenticacionApiSinIdentity.Servicios;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace TestXunit
{
    public class EncriptacionTests
    {
        [Fact]
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

            Assert.Equal("ITaS6dqSPAWpqQQKgeh6cPtX59YOKLRX", resultado);
        }
    }
}
