using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PeliculasAPI.Controllers;
using PeliculasAPI.Dtos;
using PeliculasAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasAPI.Tests.PruebasUnitarias
{
    [TestClass]
    public class GenerosControllerTests: BasePruebas
    {
        [TestMethod]
        public async Task ObtenerTodosLosGeneros()
        {
            //preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            contexto.Add(new Genero() { Nombre = "Genero 1" });
            contexto.Add(new Genero() { Nombre = "Genero 2" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBD);

            //prueba

            var controller = new GenerosController(contexto2, mapper);
            var respuesta = await controller.Get();

            //verificación

            var generos = respuesta.Value;
            Assert.AreEqual(2, generos.Count);


        }

        [TestMethod]
        public async Task ObtenerGeneroPorIdNoExistente()
        {
            //preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();
                       
            //prueba

            var controller = new GenerosController(contexto, mapper);
            var respuesta = await controller.Get(1);

            //verificación

            var resultado = respuesta.Result as StatusCodeResult;
            Assert.AreEqual(404, resultado.StatusCode);


        }

        [TestMethod]
        public async Task ObtenerGeneroPorIdExistente()
        {
            //preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            contexto.Add(new Genero() { Nombre = "Genero 1" });
            contexto.Add(new Genero() { Nombre = "Genero 2" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBD);
            var id = 1;

            //prueba

            var controller = new GenerosController(contexto2, mapper);
            var respuesta = await controller.Get(id);

            //verificación

            var resultado = respuesta.Value;
            Assert.AreEqual(id, resultado.Id);
        }

        [TestMethod]
        public async Task CrearGenero()
        {
            //preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();
            var nuevoGenero = new GeneroCreacionDTO()
            {
                Nombre = "Genero 1"
            };
                    
            //prueba

            var controller = new GenerosController(contexto, mapper);
            var respuesta = await controller.Post(nuevoGenero);

            //verificación

            var resultado = respuesta as CreatedAtRouteResult;
            Assert.IsNotNull(resultado);

            var contexto2 = ConstruirContext(nombreBD);
            var cantidad = await contexto2.Generos.CountAsync();

            Assert.AreEqual(1, cantidad);
        }

        [TestMethod]
        public async Task ActualizarGenero()
        {
            //preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            contexto.Add(new Genero() { Nombre = "Genero 1" });
            contexto.Add(new Genero() { Nombre = "Genero 2" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBD);
            var id = 1;

            //prueba

            var controller = new GenerosController(contexto2, mapper);
            var generacionCreacionDTO = new GeneroCreacionDTO()
            {
                Nombre = "Genero 1 modificado"
            };

            var respuesta = await controller.Put(id, generacionCreacionDTO);

            //verificación

            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204,resultado.StatusCode);

            var contexto3 = ConstruirContext(nombreBD);
            var existe = await contexto3.Generos.AnyAsync(x => x.Nombre == "Genero 1 modificado");

            Assert.IsTrue(existe);
        }


        [TestMethod]
        public async Task BorrarGeneroNoExistente()
        {
            //preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            var id = 1;

            //prueba

            var controller = new GenerosController(contexto, mapper);
            var respuesta = await controller.Delete(id);

            //verificación

            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(404, resultado.StatusCode);

        }

        [TestMethod]
        public async Task BorrarGeneroExitoso()
        {
            //preparacion
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            contexto.Add(new Genero() { Nombre = "Genero 1" });
            contexto.Add(new Genero() { Nombre = "Genero 2" });
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBD);
            var id = 1;

            //prueba

            var controller = new GenerosController(contexto2, mapper);
            var respuesta = await controller.Delete(id);

            //verificación

            var resultado = respuesta as StatusCodeResult;
            Assert.AreEqual(204, resultado.StatusCode);

            var contexto3 = ConstruirContext(nombreBD);
            var existe = await contexto3.Generos.AnyAsync(x => x.Id == id);

            Assert.IsFalse(existe);
        }

    }
}
