using System.ComponentModel.DataAnnotations;

namespace PeliculasAPI.Dtos
{
    public class SalaDeCineCercanoFiltroDTO
    {
        [Range(-90, 90)]
        public double Latitud { get; set; }
        [Range(-180, 180)]
        public double Longitud { get; set; }

        private int distanciaEnKms { get; set; } = 10;
        private int distanciaMaximaKms = 50;

        public int DistanciaEnKms
        {
            get => distanciaEnKms;
            set
            {
                distanciaEnKms = (value > distanciaMaximaKms) ? distanciaMaximaKms : value;
            }
        }
    }
}
