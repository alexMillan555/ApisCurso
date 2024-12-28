using ApiPeliculas.Datos;
using ApiPeliculas.Modelos;
using ApiPeliculas.Repositorio.IRepositorio;

namespace ApiPeliculas.Repositorio
{
    public class CategoriaRepositorio : ICategoriaRepositorio 
    {
        private readonly ContextoAplicacionBD _bd;

        public CategoriaRepositorio(ContextoAplicacionBD bd)
        {
            _bd = bd;
        }

        public bool ActualizarCategoria(Categoria Categoria)
        {
            Categoria.FechaCreacion = DateTime.Now;

            //ARREGLO PROBLEMA PUT
            var categoriaExistente = _bd.Categoria.Find(Categoria.Id);
            if (categoriaExistente != null)
                _bd.Entry(categoriaExistente).CurrentValues.SetValues(Categoria);
            else
                _bd.Categoria.Update(Categoria);
            
            return Guardar();
        }

        public bool BorrarCategoria(Categoria Categoria)
        {
            _bd.Categoria.Remove(Categoria);
            return Guardar();
        }

        public bool CrearCategoria(Categoria Categoria)
        {
            Categoria.FechaCreacion = DateTime.Now;
            _bd.Categoria.Add(Categoria);
            return Guardar();
        }

        public bool ExisteCategoria(int Id)
        {
            return _bd.Categoria.Any(c => c.Id == Id);
        }

        public bool ExisteCategoria(string Nombre)
        {
            bool valor = _bd.Categoria.Any(c => c.Nombre.ToLower().Trim() == Nombre.ToLower().Trim());
            return valor;
        }

        public Categoria GetCategoria(int CategoriaId)
        {
            return _bd.Categoria.FirstOrDefault(c => c.Id == CategoriaId);
        }

        public ICollection<Categoria> GetCategorias()
        {
            return _bd.Categoria.OrderBy(c => c.Nombre).ToList();
        }

        public bool Guardar()
        {
            return _bd.SaveChanges() >= 0 ? true : false;
        }
    }
}
