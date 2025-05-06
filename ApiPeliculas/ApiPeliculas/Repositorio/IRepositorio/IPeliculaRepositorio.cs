using ApiPeliculas.Modelos;

namespace ApiPeliculas.Repositorio.IRepositorio
{
    public interface IPeliculaRepositorio
    {
        //V1
        //ICollection<Pelicula> GetPeliculas();

        //Paginación
        //V2
        ICollection<Pelicula> GetPeliculas(int numeroPagina, int tamanioPagina);
        //Obtener total de páginas
        int GetTotalPeliculas();
        ICollection<Pelicula> GetPeliculasEnCategoria(int CategoriaId);
        IEnumerable<Pelicula> BuscarPelicula(string Nombre);
        Pelicula GetPelicula(int PeliculaId);

        bool ExistePelicula(int Id);
        bool ExistePelicula(string Nombre);
        bool CrearPelicula(Pelicula Pelicula);
        bool ActualizarPelicula(Pelicula Pelicula);
        bool BorrarPelicula(Pelicula Pelicula);
        bool Guardar();
    }
}
