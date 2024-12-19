using System.ComponentModel.DataAnnotations;

namespace ApiPeliculas.Modelos
{
    public class Categoria
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; }//Si pones el caracter '?' en el tipo de dato, se da chance de aceptar valores que no sean obligatorios
        [Required]
        [Display(Name = "Fecha de creación")] //Así se mostraría en una vista con el método 'Display', escribiéndolo así como está
        public DateTime FechaCreacion { get; set; }
    }
}
