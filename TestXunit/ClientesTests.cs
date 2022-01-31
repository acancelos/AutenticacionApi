using System;
using System.Collections.Generic;
using AutenticacionApiSinIdentity.Controllers;
using AutenticacionApiSinIdentity.Datos;
using AutenticacionApiSinIdentity.Modelos;
using AutenticacionApiSinIdentity.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using TestXunit.Mocks;
using Xunit;

namespace TestXunit
{
    public class ClientesTests : BasePruebas
    {
        [Fact]
        public void GetClientesTest()
        {
            var NombreDb = Guid.NewGuid().ToString();
            var contexto = ConstruirBase(NombreDb);
            var options = new Opciones();

            contexto.Clientes.Add(new Cliente { Nombre = "Juan", Apellido = "Pepe", Id = 1 });
            contexto.SaveChanges();

            var contexto2 = ConstruirBase(NombreDb);

            var controller = new ClientesController(options, contexto2);

            var respuesta = controller.GetClientes(null);


            Assert.True(respuesta.Count>0);
        }
    }
}
