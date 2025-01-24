using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; } 
        public string NombreUsuario { get; set; }
        public string Nombre {  get; set; } 
        public string Contraseña { get; set; }
        public string Rol { get; set; } 
    }
}
