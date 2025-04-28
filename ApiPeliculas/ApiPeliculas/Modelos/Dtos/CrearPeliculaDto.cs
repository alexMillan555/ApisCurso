namespace ApiPeliculas.Modelos.Dtos
{
    public class CrearPeliculaDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int Duracion { get; set; }
        public string? RutaImagen { get; set; } //Al ponerle el signo de interrogación, esto ya puede ser nulo por defecto
        public IFormFile Imagen {get; set;}
        public enum CrearTipoClasificacion { Siete, Trece, Dieciseis, Dieciocho }
        public CrearTipoClasificacion Clasificacion { get; set; }

        //RELACIÓN TABLA CATEGORÍA
        public int CategoriaId { get; set; }
    }
}
