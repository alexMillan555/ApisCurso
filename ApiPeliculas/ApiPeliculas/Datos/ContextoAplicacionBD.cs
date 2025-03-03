using ApiPeliculas.Modelos;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Datos
{
    public class ContextoAplicacionBD : IdentityDbContext<AppUsuario>
    {
        public ContextoAplicacionBD(DbContextOptions<ContextoAplicacionBD> opciones) : base(opciones)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        //AQUÍ SE DEBEN DE PASAR TODOS LOS MODELOS PARA QUE LA MIGRACIÓN EN DB ESTÉ DISPONIBLE
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Pelicula> Pelicula { get; set; }
        public DbSet<Usuario> Usuario {  get; set; }
        public DbSet<AppUsuario> AppUsuario {  get; set; }

    }
}
    