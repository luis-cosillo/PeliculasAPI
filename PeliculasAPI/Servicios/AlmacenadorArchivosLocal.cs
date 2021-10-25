namespace PeliculasAPI.Servicios
{
    public class AlmacenadorArchivosLocal : IAlmacenadorArchivos
    {
        private readonly IWebHostEnvironment env;
        private readonly IHttpContextAccessor httpContextAccessor;

        public AlmacenadorArchivosLocal(IWebHostEnvironment env,
                                        IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }

        public Task BorrarArchivo(string ruta, string contenedor)
        {
            if ( ruta != null)
            {
                var archivoNombre = Path.GetFileName(ruta);
                string directorioArchivo = Path.Combine(env.WebRootPath, contenedor, archivoNombre);

                if ( File.Exists(directorioArchivo))
                {
                    File.Delete(directorioArchivo);
                }

            }
            return Task.FromResult(0);
        }

        public async Task<string> EditarArchivo(byte[] contenido, string extension, 
            string contenedor, string ruta, string contenType)
        {
            await BorrarArchivo(ruta, contenedor);
            return await GuardarArchivo(contenido, extension, contenedor, contenType);
        }

        public async Task<string> GuardarArchivo(byte[] contenido, string extension, 
            string contenedor, string contenType)
        {
            var archivoNombre = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(env.WebRootPath, contenedor);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string ruta = Path.Combine(folder, archivoNombre);
            await File.WriteAllBytesAsync(ruta, contenido);

            var urlActual = $"{httpContextAccessor.HttpContext?.Request.Scheme}://{httpContextAccessor.HttpContext?.Request.Host}";
            var urlParaDB = Path.Combine(urlActual, contenedor, archivoNombre).Replace("\\", "/");

            return urlParaDB;
        }
    }
}
