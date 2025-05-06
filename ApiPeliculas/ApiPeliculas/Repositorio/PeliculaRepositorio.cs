using ApiPeliculas.Datos;
using ApiPeliculas.Modelos;
using ApiPeliculas.Repositorio.IRepositorio;
using Microsoft.EntityFrameworkCore;

namespace ApiPeliculas.Repositorio
{
    public class PeliculaRepositorio : IPeliculaRepositorio 
    {
        private readonly ContextoAplicacionBD _bd;

        public PeliculaRepositorio(ContextoAplicacionBD bd)
        {
            _bd = bd;
        }

        public bool ActualizarPelicula(Pelicula Pelicula)
        {
            Pelicula.FechaCreacion = DateTime.Now;
            var peliculaExistente = _bd.Pelicula.Find(Pelicula.Id);
            //ARREGLO DE PROBLEMA EN MÉTODO PATCH
            if(peliculaExistente != null)
                _bd.Entry(peliculaExistente).CurrentValues.SetValues(Pelicula);
            else
                _bd.Pelicula.Update(Pelicula);
            return Guardar();
        }

        public bool BorrarPelicula(Pelicula Pelicula)
        {
            _bd.Pelicula.Remove(Pelicula);
            return Guardar();
        }

        public IEnumerable<Pelicula> BuscarPelicula(string Nombre)
        {
            IQueryable<Pelicula> query = _bd.Pelicula;
            if(!string.IsNullOrEmpty(Nombre))            
                query = query.Where(e => e.Nombre.Contains(Nombre) || e.Descripcion.Contains(Nombre));            
            return query.ToList();
        }

        public bool CrearPelicula(Pelicula Pelicula)
        {
            Pelicula.FechaCreacion = DateTime.Now;
            _bd.Pelicula.Add(Pelicula);
            return Guardar();
        }

        public bool ExistePelicula(int Id)
        {
            return _bd.Pelicula.Any(c => c.Id == Id);
        }

        public bool ExistePelicula(string Nombre)
        {
            bool valor = _bd.Pelicula.Any(c => c.Nombre.ToLower().Trim() == Nombre.ToLower().Trim());
            return valor;
        }

        public Pelicula GetPelicula(int PeliculaId)
        {
            return _bd.Pelicula.FirstOrDefault(c => c.Id == PeliculaId);
        }

        public ICollection<Pelicula> GetPelicula()
        {
            return _bd.Pelicula.OrderBy(c => c.Nombre).ToList();
        }

        //V1
        //public ICollection<Pelicula> GetPeliculas()
        //{
        //    return _bd.Pelicula.OrderBy(c => c.Nombre).ToList();
        //}

        //V2
        //Habilitar paginación
        public ICollection<Pelicula> GetPeliculas(int numeroPagina, int tamanioPagina)
        {
            return _bd.Pelicula.OrderBy(c => c.Nombre).Skip((numeroPagina - 1) * tamanioPagina).Take(tamanioPagina).ToList();
        }

        public int GetTotalPeliculas()
        {
            return _bd.Pelicula.Count();
        }

        public ICollection<Pelicula> GetPeliculasEnCategoria(int CategoriaId)
        {
            return _bd.Pelicula.Include(ca => ca.Categoria).Where(ca => ca.CategoriaId == CategoriaId).ToList();
        }        

        public bool Guardar()
        {
            return _bd.SaveChanges() >= 0 ? true : false;
        }
    }
}
