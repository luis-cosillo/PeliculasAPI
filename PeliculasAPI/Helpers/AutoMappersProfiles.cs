using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;
using PeliculasAPI.Dtos;
using PeliculasAPI.Entidades;

namespace PeliculasAPI.Helpers
{
    public class AutoMappersProfiles: Profile
    {
        public AutoMappersProfiles(GeometryFactory geometryFactory)
        {
            CreateMap<IdentityUser, UsuarioDTO>();

            CreateMap<Genero, GeneroDTO>().ReverseMap();
            CreateMap<GeneroCreacionDTO, Genero>();

            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<ActorCreacionDTO, Actor>()
                .ForMember( x => x.Foto, options => options.Ignore());
            CreateMap<ActorPathDTO, Actor>().ReverseMap();

            CreateMap<Pelicula, PeliculaDTO>().ReverseMap();
            CreateMap<PeliculaCreacionDTO, Pelicula>()
                .ForMember(x => x.Poster, options => options.Ignore())
                .ForMember(x => x.PeliculasGeneros, options => options.MapFrom(MapPeliculasGeneros))
                .ForMember(x => x.PeliculasActores, options => options.MapFrom(MapPeliculasActores));
            CreateMap<PeliculaPathDTO, Pelicula>().ReverseMap();

            CreateMap<Pelicula, PeliculaRelacionadaDTO>()
                .ForMember(x => x.Generos, options => options.MapFrom(MapPeliculasGeneros))
                .ForMember(x => x.Actores, options => options.MapFrom(MapPeliculasActores));


            CreateMap<SalaDeCine, SalaDeCineDTO>()
                .ForMember( x => x.Latitud, x => x.MapFrom( y => y.Ubicacion.Y))
                .ForMember( x => x.Longitud, x => x.MapFrom( y => y.Ubicacion.X));
            
            CreateMap<SalaDeCineDTO, SalaDeCine>()
                .ForMember( x => x.Ubicacion, x => x.MapFrom( y =>
                    geometryFactory.CreatePoint(new Coordinate(y.Longitud, y.Latitud))
                ));
            CreateMap<SalaDeCineCreacionDTO, SalaDeCine>()
                .ForMember(x => x.Ubicacion, x => x.MapFrom(y =>
                  geometryFactory.CreatePoint(new Coordinate(y.Longitud, y.Latitud))
               ));

            CreateMap<Review, ReviewDTO>()
                .ForMember(x => x.NombreUsuario, x => x.MapFrom(y => y.Usuario.UserName));
            CreateMap<ReviewDTO, Review>();
            CreateMap<ReviewCreacionDTO, Review>();
        }

        private List<ActorPeliculaDetalleDTO> MapPeliculasActores(Pelicula pelicula, PeliculaRelacionadaDTO peliculaRelacionadaDTO)
        {
            var resultado = new List<ActorPeliculaDetalleDTO>();
            if (pelicula.PeliculasActores == null) { return resultado; }

            foreach (var actor in pelicula.PeliculasActores)
            {
                resultado.Add(new ActorPeliculaDetalleDTO() {
                    ActorId = actor.ActorId, 
                    Personaje = actor.Personaje, 
                    NombrePersona = actor.Actor.Nombre 
                });
            }

            return resultado;
        }

        private List<GeneroDTO> MapPeliculasGeneros(Pelicula pelicula, PeliculaRelacionadaDTO peliculaRelacionadaDTO)
        {
            var resultado = new List<GeneroDTO>();
            if (pelicula.PeliculasGeneros == null) { return resultado; }

            foreach (var generoPelicula in pelicula.PeliculasGeneros)
            {
                resultado.Add(new GeneroDTO() { Id = generoPelicula.GeneroId, Nombre = generoPelicula.Genero.Nombre });
            }

            return resultado;
        }

        private List<PeliculasGeneros> MapPeliculasGeneros(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasGeneros>();
            if (peliculaCreacionDTO.GenerosIds == null) { return resultado; }

            foreach (var id in peliculaCreacionDTO.GenerosIds)
            {
                resultado.Add(new PeliculasGeneros() { GeneroId = id });
            }

            return resultado;
        }

        private List<PeliculasActores> MapPeliculasActores(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasActores>();
            if (peliculaCreacionDTO.Actores == null) { return resultado; }

            foreach (var actor in peliculaCreacionDTO.Actores)
            {
                resultado.Add(new PeliculasActores() { ActorId = actor.ActorId, Personaje = actor.Personaje });
            }

            return resultado;
        }

    }
}
