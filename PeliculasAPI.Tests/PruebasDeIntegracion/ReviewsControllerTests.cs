﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using PeliculasAPI.Dtos;
using PeliculasAPI.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeliculasAPI.Tests.PruebasDeIntegracion
{
    [TestClass]
    public class ReviewsControllerTests: BasePruebas
    {
        private static readonly string url = "api/peliculas/1/reviews";

        [TestMethod]
        public async Task ObtenerReviewsDevuelve404PeliculaInexistente()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);

            var cliente = factory.CreateClient();
            var respuesta = await cliente.GetAsync(url);
            Assert.AreEqual(404, (int)respuesta.StatusCode);

        }

        [TestMethod]
        public async Task ObtenerReviewsDevuelveListadoVacio()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);
            var contexto = ConstruirContext(nombreDB);

            contexto.Peliculas.Add(new Pelicula() { Titulo = "Pelicula 1 " });
            await contexto.SaveChangesAsync();

            var cliente = factory.CreateClient();
            var respuesta = await cliente.GetAsync(url);

            respuesta.EnsureSuccessStatusCode();

            var reviews = JsonConvert
                .DeserializeObject<List<ReviewDTO>>(await respuesta.Content.ReadAsStringAsync());

            Assert.AreEqual(0, reviews.Count);
        }

    }
}
