using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class CategoriaService : ICategoriaService
    {
        readonly private IGenericRepository<Categoria> _repository;

        public CategoriaService(IGenericRepository<Categoria> repository)
        {
            _repository = repository;
        }
        public async Task<List<Categoria>> Lista()
        {
            IQueryable<Categoria> query = await _repository.Consultar();
            return query.ToList();
        }
        public async Task<Categoria> Crear(Categoria entidad)
        {
            try
            {
                Categoria categoriaCreada = await _repository.Crear(entidad);
                if (categoriaCreada.IdCategoria == 0)
                    throw new TaskCanceledException("No fue posible crear esta categoria");

                return categoriaCreada;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Categoria> Editar(Categoria entidad)
        {
            try
            {
                Categoria categoriaEncontrada = await _repository.Obtener(c => c.IdCategoria == entidad.IdCategoria);
                categoriaEncontrada.Descripcion = entidad.Descripcion;
                categoriaEncontrada.EsActivo = entidad.EsActivo;
                bool respuesta = await _repository.Editar(categoriaEncontrada);

                if (!respuesta)
                    throw new TaskCanceledException("No fue posible modificar esta categoria");

                return categoriaEncontrada;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Eliminar(int idCategoria)
        {
            try
            {
                Categoria categoriaEncontrada = await _repository.Obtener(c => c.IdCategoria == idCategoria);

                if (categoriaEncontrada == null)
                    throw new TaskCanceledException("La categoria no existe!");

                bool respuesta = await _repository.Eliminar(categoriaEncontrada);

                return respuesta;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
