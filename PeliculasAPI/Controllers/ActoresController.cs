using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PeliculasAPI.Dtos;
using PeliculasAPI.Entidades;
using PeliculasAPI.Helpers;
using PeliculasAPI.Servicios;

namespace PeliculasAPI.Controllers
{
    [ApiController]
    [Route("api/actores")]
    public class ActoresController: CustomBaseController
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IAlmacenadorArchivos almacenadorArchivos;
        private readonly string contenedor = "actores";
        public ActoresController( ApplicationDbContext context,
                                  IMapper mapper,
                                  IAlmacenadorArchivos almacenadorArchivos)
            :base( context, mapper)
        {
            this.context = context;
            this.mapper = mapper;
            this.almacenadorArchivos = almacenadorArchivos;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            //var queryable = context.Actores.AsQueryable();
            //await HttpContext.InsertarParametrosPaginacion(queryable, paginacionDTO.CantidadRegistrosPorPagina);
            //var entidades = await queryable.Paginar(paginacionDTO).ToListAsync();
            //return mapper.Map<List<ActorDTO>>(entidades); 

            return await Get<Actor, ActorDTO>(paginacionDTO);
        }

        [HttpGet("{id:int}",Name = "obtenerActor")]
        public async Task<ActionResult<ActorDTO>> Get( int id )
        {
            //var entidad = await context.Actores.FirstOrDefaultAsync( x => x.Id == id);

            //if( entidad == null)
            //{
            //    return NotFound();
            //}

            //var dto = mapper.Map<ActorDTO>(entidad);
            //return dto;

            return await Get<Actor, ActorDTO>(id);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            var entidad = mapper.Map<Actor>(actorCreacionDTO);

            if ( actorCreacionDTO.Foto != null)
            {
                using ( var memoryStream = new MemoryStream())
                {
                    await actorCreacionDTO.Foto.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);

                    //Para grabar an AZURE
                    entidad.Foto = await almacenadorArchivos.GuardarArchivo(contenido,
                        extension, contenedor, actorCreacionDTO.Foto.ContentType);
                }
            }

            context.Add(entidad);
            await context.SaveChangesAsync();

            var dto = mapper.Map<ActorDTO>(entidad);
            return new CreatedAtRouteResult("obtenerActor", new { id = entidad.Id }, dto);
        }

        [HttpPut("id:int", Name = "actualizarActor")]
        public async Task<ActionResult> Put(int id, [FromForm] ActorCreacionDTO actorCreacionDTO)
        {
            //Aqui lo manda todo a guardar 
            //var entidad = mapper.Map<Actor>(actorCreacionDTO);
            //entidad.Id = id;
            //context.Entry(entidad).State = EntityState.Modified;

            var actorDB = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if ( actorDB == null) { return NotFound(); }

            actorDB = mapper.Map(actorCreacionDTO, actorDB);

            if (actorCreacionDTO.Foto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await actorCreacionDTO.Foto.CopyToAsync(memoryStream);
                    var contenido = memoryStream.ToArray();
                    var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);
                  
                    //Para grabar an AZURE
                    actorDB.Foto = await almacenadorArchivos.EditarArchivo(contenido,
                        extension, contenedor, actorDB.Foto, actorCreacionDTO.Foto.ContentType);
                }
            }


            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "actualizaPachActor")]
        public async Task<ActionResult> Patch( int id, [FromBody ] JsonPatchDocument<ActorPathDTO> patchDocument)
        {
            //if ( patchDocument == null)
            //{
            //    return BadRequest();
            //}

            //var entidadDB = await context.Actores.FirstOrDefaultAsync( x => x.Id == id);    
            //if ( entidadDB == null)
            //{
            //    return NotFound();
            //}

            //var entidadDTO = mapper.Map<ActorPathDTO>( entidadDB );

            //patchDocument.ApplyTo(entidadDTO, ModelState);

            //var esValido = TryValidateModel(entidadDTO);

            //if (!esValido)
            //{
            //    return BadRequest(ModelState);
            //}

            //mapper.Map(entidadDTO, entidadDB);
            //await context.SaveChangesAsync();

            //return NoContent();

            return await Patch<Actor, ActorPathDTO>(id, patchDocument);
        }

        [HttpDelete("{id:int}", Name = "eliminarActor")]
        public async Task<ActionResult> Delete(int id)
        {
            //var existe = await context.Actores.AnyAsync(x => x.Id == id);

            //if (!existe)
            //{
            //    return NotFound();
            //}

            //context.Remove(new Actor() { Id = id });
            //await context.SaveChangesAsync();

            //return NoContent();

            return await Delete<Actor>(id);
        }
    }
}
