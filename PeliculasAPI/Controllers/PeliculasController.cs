using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.Dtos;
using PeliculasAPI.Entidades;
using PeliculasAPI.Helpers;
using PeliculasAPI.Servicios;
using System.Linq.Dynamic.Core;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/peliculas")]
    public class PeliculasController: CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly ILogger<PeliculasController> logger;
        private readonly string contenedor = "peliculas";
        public PeliculasController(ApplicationDbContext context,
                                  IMapper mapper,
                                  IAlmacenadorArchivos almacenadorArchivos,
                                  ILogger<PeliculasController> logger) 
                                  : base( context, mapper )
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
            this.logger = logger;
            this.logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<PeliculaDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            //var queryable = context.Peliculas.AsQueryable();
            //await HttpContext.InsertarParametrosPaginacion(queryable, paginacionDTO.CantidadRegistrosPorPagina);
            //var entidades = await queryable.Paginar(paginacionDTO).ToListAsync();
            //return mapper.Map<List<PeliculaDTO>>(entidades);

            return await Get<Pelicula, PeliculaDTO>(paginacionDTO);

        }

        [HttpGet("PeliculasFiltradasConPagineo")]
        public async Task<ActionResult<PeliculasIndexDTO>> PeliculasFiltradasConPagineo([FromQuery] PaginacionDTO paginacionDTO)
        {
            var top = 5;
            var hoy = DateTime.Today;

            var qrPximosEstrenos = context.Peliculas.AsQueryable();
            qrPximosEstrenos = qrPximosEstrenos
                                  .Where(x => x.FechaEstreno > hoy)
                                  .OrderBy(x => x.FechaEstreno)
                                  .Take(top);
            await HttpContext.InsertarParametrosPaginacion(qrPximosEstrenos, "cantidadPaginasEstrenos", paginacionDTO.CantidadRegistrosPorPagina);
            var proximosEstrenos = await qrPximosEstrenos.Paginar(paginacionDTO).ToListAsync();

            var qrEnCines = context.Peliculas.AsQueryable();
            qrEnCines = qrEnCines
                            .Where(x => x.EnCines)
                            .OrderBy(x => x.FechaEstreno)
                            .Take(top);
            await HttpContext.InsertarParametrosPaginacion(qrEnCines, "cantidadPaginasEnCine", paginacionDTO.CantidadRegistrosPorPagina);
            var soloEnCines = await qrEnCines.Paginar(paginacionDTO).ToListAsync();

            var resultado = new PeliculasIndexDTO();
            resultado.FuturosEstrenos = mapper.Map<List<PeliculaDTO>>(proximosEstrenos);
            resultado.EnCines = mapper.Map<List<PeliculaDTO>>(soloEnCines);

            return resultado;
        }

        [HttpGet("filtro")]
        public async Task<ActionResult<List<PeliculaDTO>>> Filtrar([FromQuery] FiltroPeliculasDTO filtroPeliculasDTO)
        {

            var peliculasQueryable = context.Peliculas.AsQueryable();

            if (!string.IsNullOrEmpty(filtroPeliculasDTO.Titulo))
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.Titulo.Contains(filtroPeliculasDTO.Titulo));
            }

            if (filtroPeliculasDTO.EnCines)
            {
                peliculasQueryable = peliculasQueryable.Where(x => x.EnCines == filtroPeliculasDTO.EnCines);
            }

            if (filtroPeliculasDTO.ProximosEstrenos)
            {
                var hoy = DateTime.Today;
                peliculasQueryable = peliculasQueryable.Where(x => x.FechaEstreno > hoy );
            }

            if (filtroPeliculasDTO.GeneroId != 0)
            {
                peliculasQueryable = peliculasQueryable
                    .Where(x => x.PeliculasGeneros.Select( y => y.GeneroId)
                    .Contains(filtroPeliculasDTO.GeneroId));
            }

            if (!string.IsNullOrEmpty(filtroPeliculasDTO.CampoOrdenar))
            {
                var tipoOrden = filtroPeliculasDTO.OrdenAscendente ? " asc" : "desc";

                try
                {
                    peliculasQueryable = peliculasQueryable.OrderBy($"{ filtroPeliculasDTO.CampoOrdenar } {tipoOrden}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message, ex);
                }

                //if ( filtroPeliculasDTO.CampoOrdenar == "titulo")
                //{
                //    if ( filtroPeliculasDTO.OrdenAscendente)
                //    {
                //        peliculasQueryable = peliculasQueryable.OrderBy(x => x.Titulo);
                //    } else
                //    {
                //        peliculasQueryable = peliculasQueryable.OrderByDescending(x => x.Titulo);
                //    }
                //}
            }

            await HttpContext.InsertarParametrosPaginacion(peliculasQueryable, filtroPeliculasDTO.CantidadRegistrosPorPagina);

            var peliculas = await peliculasQueryable.Paginar(filtroPeliculasDTO.Paginacion).ToListAsync();

            return mapper.Map<List<PeliculaDTO>>(peliculas);
        }

        [HttpGet("GetPeliculasFiltradas")]
        public async Task<ActionResult<PeliculasIndexDTO>> GetPeliculasFiltradas()
        {
            var top = 5;
            var hoy = DateTime.Today;

            var proximosEstrenos = await context.Peliculas
                                                .Where(x => x.FechaEstreno > hoy)
                                                .OrderBy(x => x.FechaEstreno)
                                                .Take(top)
                                                .ToListAsync();


            var enCines = await context.Peliculas
                                       .Where(x => x.EnCines)
                                       .Take(top)
                                       .ToListAsync();

            var resultado = new PeliculasIndexDTO();
            resultado.FuturosEstrenos = mapper.Map<List<PeliculaDTO>>(proximosEstrenos);
            resultado.EnCines = mapper.Map<List<PeliculaDTO>>(enCines);

            return resultado; 
        }

        [HttpGet("obtenerPeliculaRelacionada/{id:int}", Name = "obtenerPeliculaRelacionada")]
        public async Task<ActionResult<PeliculaRelacionadaDTO>> GetInfoRelacionada(int id)
        {
            var pelicula = await context.Peliculas
                                        .Include( x => x.PeliculasActores)
                                        .ThenInclude( x => x.Actor )
                                        .Include( x => x.PeliculasGeneros )
                                        .ThenInclude( x => x.Genero )
                                        .FirstOrDefaultAsync(x => x.Id == id);

            if (pelicula == null)
            {
                return NotFound();
            }

            pelicula.PeliculasActores = pelicula.PeliculasActores.OrderBy(x => x.Orden).ToList();

            var dto = mapper.Map<PeliculaRelacionadaDTO>(pelicula);
            return dto;
        }

        [HttpGet("{id:int}", Name = "obtenerPelicula")]
        public async Task<ActionResult<PeliculaDTO>> Get(int id)
        {
            //var pelicula = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);

            //if (pelicula == null)
            //{
            //    return NotFound();
            //}

            //var dto = mapper.Map<PeliculaDTO>(pelicula);
            //return dto;

            return await Get<Pelicula, PeliculaDTO>(id);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var pelicula = mapper.Map<Pelicula>(peliculaCreacionDTO);

            if (peliculaCreacionDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await peliculaCreacionDTO.Poster.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName);

                    pelicula.Poster = await almacenadorArchivos.GuardarArchivo(contenido,
                        extension, contenedor, peliculaCreacionDTO.Poster.ContentType);
                }
            }

            AsignarOrdenActores(pelicula);
            context.Add(pelicula);
            await context.SaveChangesAsync();

            var peliculaDTO = mapper.Map<PeliculaDTO>(pelicula);
            return new CreatedAtRouteResult("obtenerPelicula", new { id = pelicula.Id }, peliculaDTO);
        }

        private void AsignarOrdenActores( Pelicula pelicula)
        {
            if ( pelicula.PeliculasActores != null)
            {

                for ( int i= 0; i < pelicula.PeliculasActores.Count; i++)
                {
                    pelicula.PeliculasActores[i].Orden = i;
                }


            }
        }

        [HttpPut("id:int", Name = "actualizarPelicula")]
        public async Task<ActionResult> Put(int id, [FromForm] PeliculaCreacionDTO peliculaCreacionDTO)
        {
            var peliculaDB = await context.Peliculas
                                          .Include( x => x.PeliculasActores )
                                          .Include( x => x.PeliculasGeneros )
                                          .FirstOrDefaultAsync(x => x.Id == id);

            if (peliculaDB == null) { return NotFound(); }

            peliculaDB = mapper.Map(peliculaCreacionDTO, peliculaDB);

            if (peliculaCreacionDTO.Poster != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await peliculaCreacionDTO.Poster.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(peliculaCreacionDTO.Poster.FileName);

                    //Para grabar an AZURE
                    peliculaDB.Poster = await almacenadorArchivos.EditarArchivo(contenido,
                        extension, contenedor, peliculaDB.Poster, peliculaCreacionDTO.Poster.ContentType);
                }
            }
            AsignarOrdenActores(peliculaDB);
            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "actualizaPachPelicula")]
        public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<PeliculaPathDTO> patchDocument)
        {
            //if (patchDocument == null)
            //{
            //    return BadRequest();
            //}

            //var peliculaDB = await context.Peliculas.FirstOrDefaultAsync(x => x.Id == id);
            //if (peliculaDB == null)
            //{
            //    return NotFound();
            //}

            //var peliculaDTO = mapper.Map<PeliculaPathDTO>(peliculaDB);

            //patchDocument.ApplyTo(peliculaDTO, ModelState);

            //var esValido = TryValidateModel(peliculaDTO);

            //if (!esValido)
            //{
            //    return BadRequest(ModelState);
            //}

            //mapper.Map(peliculaDTO, peliculaDB);
            //await context.SaveChangesAsync();

            //return NoContent();

            return await Patch<Pelicula, PeliculaPathDTO>(id, patchDocument);
        }

        [HttpDelete("{id:int}", Name = "eliminarPelicula")]
        public async Task<ActionResult> Delete(int id)
        {
            //var existe = await context.Peliculas.AnyAsync(x => x.Id == id);

            //if (!existe)
            //{
            //    return NotFound();
            //}

            //context.Remove(new Pelicula() { Id = id });
            //await context.SaveChangesAsync();

            //return NoContent();
            return await Delete<Pelicula>(id);
        }
    }
}

