using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
    public class PeliculasControllerTests: BasePruebas
    {
        private string CrearDataPruebas()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);

            var genero = new Genero() { Nombre = "Genero 1" };

            var peliculas = new List<Pelicula>()
            {
                new Pelicula(){ Titulo = "Pelicula 1", FechaEstreno = new DateTime(2010,1,1), EnCines = false },
                new Pelicula(){ Titulo = "Pelicula 2", FechaEstreno = DateTime.Today.AddDays(1), EnCines = false },
                new Pelicula(){ Titulo = "Pelicula 3", FechaEstreno = DateTime.Today.AddDays(-1), EnCines = true },
            };

            var peliculaconGenero = new Pelicula()
            {
                Titulo = "Pelicula con Genero",
                FechaEstreno = new DateTime(2010, 1, 1),
                EnCines = false
            };
            peliculas.Add(peliculaconGenero);
            contexto.Add(genero);
            contexto.AddRange(peliculas);
            contexto.SaveChangesAsync();

            var peliculasGenero = new PeliculasGeneros()
            {
                GeneroId = genero.Id,
                PeliculaId = peliculaconGenero.Id
            };
            contexto.Add(peliculasGenero);
            contexto.SaveChangesAsync();

            return nombreBD;
        }

        [TestMethod]
        public async Task FiltrarPorTitulo()
        {
            //preparacion
            var nombreBD = CrearDataPruebas();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            //prueba

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            var tituloPelicula = "Pelicula 1";
            var filtroDTO = new FiltroPeliculasDTO()
            {
                Titulo = tituloPelicula,
                CantidadRegistrosPorPagina = 10
            };
            var respuesta = await controller.Filtrar(filtroDTO);

            //verificación

            var resultado = respuesta.Value;
            Assert.AreEqual(1, resultado.Count);
            Assert.AreEqual(tituloPelicula, resultado[0].Titulo);
        }

        [TestMethod]
        public async Task FiltrarEnCines()
        {
            //preparacion
            var nombreBD = CrearDataPruebas();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            //prueba

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();
            
            var filtroDTO = new FiltroPeliculasDTO()
            {
                EnCines = true,
                CantidadRegistrosPorPagina = 10
            };
            var respuesta = await controller.Filtrar(filtroDTO);

            //verificación

            var resultado = respuesta.Value;
            Assert.AreEqual(1, resultado.Count);
            Assert.AreEqual("Pelicula 3", resultado[0].Titulo);

        }

        [TestMethod]
        public async Task FiltrarProximosEstrenos()
        {
            //preparacion
            var nombreBD = CrearDataPruebas();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            //prueba

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var filtroDTO = new FiltroPeliculasDTO()
            {
                ProximosEstrenos = true,
                CantidadRegistrosPorPagina = 10
            };
            var respuesta = await controller.Filtrar(filtroDTO);

            //verificación

            var resultado = respuesta.Value;
            Assert.AreEqual(1, resultado.Count);
            Assert.AreEqual("Pelicula 2", resultado[0].Titulo);
        }

        [TestMethod]
        public async Task FiltrarPorGenero()
        {
            //preparacion
            var nombreBD = CrearDataPruebas();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            //prueba

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var generoId = contexto.Generos.Select(x => x.Id ).First();

            var filtroDTO = new FiltroPeliculasDTO()
            {
                GeneroId = generoId,
                CampoOrdenar = "titulo"
            };
            var respuesta = await controller.Filtrar(filtroDTO);

            //verificación

            var resultado = respuesta.Value;
            
            Assert.AreEqual(1, resultado.Count);
            Assert.AreEqual("Pelicula con Genero", resultado[0].Titulo);
        }

        [TestMethod]
        public async Task FiltrarOrdenarTituloAscendente()
        {
            //preparacion
            var nombreBD = CrearDataPruebas();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            //prueba

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var generoId = contexto.Generos.Select(x => x.Id).First();

            var filtroDTO = new FiltroPeliculasDTO()
            {
                OrdenAscendente = true,
                CampoOrdenar = "titulo"
            };
            var respuesta = await controller.Filtrar(filtroDTO);

            //verificación

            var resultado = respuesta.Value;

            var contexto2 = ConstruirContext(nombreBD);
            var peliculasDB = contexto2.Peliculas.OrderBy(x => x.Titulo).ToList();

            Assert.AreEqual(peliculasDB.Count, resultado.Count);

            for (int i = 0; i < peliculasDB.Count; i++)
            {
                var peliculaDelControlador = resultado[i];
                var peliculaDB = peliculasDB[i];

                Assert.AreEqual(peliculaDelControlador.Id, peliculaDB.Id);
            }
        }

        [TestMethod]
        public async Task FiltrarOrdenarTituloDescendente()
        {
            //preparacion
            var nombreBD = CrearDataPruebas();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            //prueba

            var controller = new PeliculasController(contexto, mapper, null, null);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var generoId = contexto.Generos.Select(x => x.Id).First();

            var filtroDTO = new FiltroPeliculasDTO()
            {
                OrdenAscendente = false,
                CampoOrdenar = "titulo"
            };
            var respuesta = await controller.Filtrar(filtroDTO);

            //verificación

            var resultado = respuesta.Value;

            var contexto2 = ConstruirContext(nombreBD);
            var peliculasDB = contexto2.Peliculas.OrderByDescending(x => x.Titulo).ToList();

            Assert.AreEqual(peliculasDB.Count, resultado.Count);

            for (int i = 0; i < peliculasDB.Count; i++)
            {
                var peliculaDelControlador = resultado[i];
                var peliculaDB = peliculasDB[i];

                Assert.AreEqual(peliculaDelControlador.Id, peliculaDB.Id);
            }
        }

        [TestMethod]
        public async Task FiltrarCampoNoExistente()
        {
            //preparacion
            var nombreBD = CrearDataPruebas();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            //prueba

            var mock = new Mock<ILogger<PeliculasController>>();

            var controller = new PeliculasController(contexto, mapper, null, mock.Object);
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            var generoId = contexto.Generos.Select(x => x.Id).First();

            var filtroDTO = new FiltroPeliculasDTO()
            {
                CampoOrdenar = "titulo2"
            };
            var respuesta = await controller.Filtrar(filtroDTO);

            //verificación

            var resultado = respuesta.Value;

            var contexto2 = ConstruirContext(nombreBD);
            var peliculasDB = contexto2.Peliculas.ToList();

            Assert.AreEqual(peliculasDB.Count, resultado.Count);
            Assert.AreEqual(1, mock.Invocations.Count);


        }



    }
}
