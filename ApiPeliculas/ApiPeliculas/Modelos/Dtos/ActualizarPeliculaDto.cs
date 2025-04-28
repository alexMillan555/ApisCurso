using System.ComponentModel.DataAnnotations.Schema;

namespace ApiPeliculas.Modelos.Dtos
{
    public class ActualizarPeliculaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Duracion { get; set; }
        public string? RutaImagen { get; set; } //Al ponerle el signo de interrogación, esto ya puede ser nulo por defecto
        public string? RutaLocalImagen { get; set; }
        public IFormFile Imagen { get; set; }
        public enum TipoClasificacion { Siete, Trece, Dieciseis, Dieciocho }
        public TipoClasificacion Clasificacion { get; set; }
        public DateTime? FechaCreacion { get; set; }

        //RELACIÓN TABLA CATEGORÍA
        public int CategoriaId { get; set; }
    }
}
