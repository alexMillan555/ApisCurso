using Microsoft.AspNetCore.Identity;

namespace ApiPeliculas.Modelos
{
    public class AppUsuario : IdentityUser
    {
        //AQUÍ ES DONDE SE CREAN CAMPOS NUEVOS, EN CASO DE QUE SE REQUIERAN
        public string Nombre { get; set; }
        
    }
}
