namespace PeliculasAPI.Servicios
{
    public interface IAlmacenadorArchivos
    {

        Task<String> GuardarArchivo(byte[] contenido, string extension, 
            string contenedor, string contenType);
        Task<String> EditarArchivo(byte[] contenido, string extension, 
            string contenedor, string ruta, string contenType);
        Task BorrarArchivo(string ruta, string contenedor);

    }
}
