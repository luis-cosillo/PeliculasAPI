using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
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
    public class ReviewsControllerTests: BasePruebas
    {
        private void CrearPeliculas(string nombreDB)
        {
            var contexto = ConstruirContext(nombreDB);

            contexto.Peliculas.Add(new Pelicula() { Titulo = "pelicula 1" });

            contexto.SaveChanges();
        }

        [TestMethod]
        public async Task UsuarioNoPuedeCrearReviewParaMismaPelicula()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            CrearPeliculas(nombreBD);

            var peliculaId = contexto.Peliculas.Select(x => x.Id).First();
            var review1 = new Review()
            {
                PeliculaId = peliculaId,
                UsuarioId = usuarioPorDefectoId,
                Puntuacion = 5,
                Comentario = "comentario 1"
            };
            contexto.Add(review1);
            await contexto.SaveChangesAsync();

            var contexto2 = ConstruirContext(nombreBD);

            var controller = new ReviewController(contexto2, mapper);
            controller.ControllerContext = ConstruirControllerContext();

            var reviewCreacionDTO = new ReviewCreacionDTO() { Puntuacion = 5, Comentario = "Comentario 2" };
            var respuesta = await controller.Post(peliculaId, reviewCreacionDTO);

            var valor = respuesta as IStatusCodeActionResult;

            Assert.AreEqual(400, valor.StatusCode.Value);

        }

        [TestMethod]
        public async Task CrearReview()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var contexto = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            CrearPeliculas(nombreBD);

            var peliculaId = contexto.Peliculas.Select(x => x.Id).First();
            var contexto2 = ConstruirContext(nombreBD);

            var controller = new ReviewController(contexto2, mapper);
            controller.ControllerContext = ConstruirControllerContext();

            var reviewCreacionDTO = new ReviewCreacionDTO() { Puntuacion = 5, Comentario = "Comentario 1" };
            var respuesta = await controller.Post(peliculaId, reviewCreacionDTO);

            var valor = respuesta as NoContentResult;

            Assert.IsNotNull(valor);

            var contexto3 = ConstruirContext(nombreBD);
            var reviewDB = contexto3.Reviews.First();

            Assert.AreEqual(usuarioPorDefectoId, reviewDB.UsuarioId);
        }


    }
}
