using ApiPeliculas.Modelos;

namespace ApiPeliculas.Repositorio.IRepositorio
{
    public interface ICategoriaRepositorio
    {
        ICollection<Categoria> GetCategorias();

        Categoria GetCategoria(int CategoriaId);

        bool ExisteCategoria(int Id);
        bool ExisteCategoria(string Nombre);
        bool CrearCategoria(Categoria Categoria);
        bool ActualizarCategoria(Categoria Categoria);
        bool BorrarCategoria(Categoria Categoria);
        bool Guardar();
    }
}
