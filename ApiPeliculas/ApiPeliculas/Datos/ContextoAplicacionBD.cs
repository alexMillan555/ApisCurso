using ApiPeliculas.Modelos;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Datos
{
    public class ContextoAplicacionBD : DbContext
    {
        public ContextoAplicacionBD(DbContextOptions<ContextoAplicacionBD> opciones) : base(opciones)
        {
            
        }

        //AQUÍ SE DEBEN DE PASAR TODOS LOS MODELOS PARA QUE LA MIGRACIÓN EN DB ESTÉ DISPONIBLE
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Pelicula> Pelicula { get; set; }
        public DbSet<Usuario> Usuario {  get; set; }

    }
}
    