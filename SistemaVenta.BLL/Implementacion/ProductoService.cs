using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using static System.Net.Mime.MediaTypeNames;

namespace SistemaVenta.BLL.Implementacion
{
    public class ProductoService : IProductoService
    {
        private readonly IGenericRepository<Producto> _repository;
        private readonly IFirebBseService _fireBseService;

        public ProductoService(IGenericRepository<Producto> repository, IFirebBseService firebBseService)
        {
            _repository = repository;
            _fireBseService = firebBseService;
        }
        public async Task<List<Producto>> Lista()
        {
            IQueryable<Producto> query = await _repository.Consultar();
            return query.Include(c => c.IdCategoriaNavigation).ToList();
        }

        public async Task<Producto> Crear(Producto entidad, Stream imagen = null, string nombreImagen = "")
        {
            Producto existeProducto = await _repository.Obtener(p => p.CodigoBarra == entidad.CodigoBarra);

            if (existeProducto != null)
                throw new TaskCanceledException("Ya existe un equipo con este serial!!");

            try
            {
                entidad.NombreImagen = nombreImagen;
                if (imagen != null)
                {
                    string urlImagen = await _fireBseService.SubirStorage(imagen, "carpeta_producto", nombreImagen);
                    entidad.UrlImagen = urlImagen;

                }

                Producto productoCreado = await _repository.Crear(entidad);
                if (productoCreado.IdProducto == 0)
                    throw new TaskCanceledException("No se pudo crear este equipo!!");

                IQueryable<Producto> query = await _repository.Consultar(p => p.IdProducto == productoCreado.IdProducto);

                productoCreado = query.Include(c => c.IdCategoriaNavigation).First();

                return productoCreado;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Producto> Editar(Producto entidad, Stream imagen = null, string nombreImagen = "")
        {
            Producto existeProducto = await _repository.Obtener(p => p.CodigoBarra == entidad.CodigoBarra && p.IdProducto != entidad.IdProducto);

            if (existeProducto != null)
                throw new TaskCanceledException("Este serial ya esta asignado a otro equipo!!");

            try
            {
                IQueryable<Producto> queryProduct = await _repository.Consultar(p => p.IdProducto == entidad.IdProducto);

                Producto productoParaEditar = queryProduct.First();

                productoParaEditar.CodigoBarra = entidad.CodigoBarra;
                productoParaEditar.Marca = entidad.Marca;
                productoParaEditar.Descripcion = entidad.Descripcion;
                productoParaEditar.IdCategoria = entidad.IdCategoria;
                productoParaEditar.Stock = entidad.Stock;
                productoParaEditar.Precio = entidad.Precio;
                productoParaEditar.EsActivo = entidad.EsActivo;

                if(productoParaEditar.NombreImagen == "")
                {
                    productoParaEditar.NombreImagen = nombreImagen;
                }

                if (imagen != null)
                {
                    string urlImage = await _fireBseService.SubirStorage(imagen, "carpeta_producto", productoParaEditar.NombreImagen);

                    productoParaEditar.UrlImagen = urlImage;
                }

                bool respuesta = await _repository.Editar(productoParaEditar);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo editar este equipo!!");

                Producto productoEditado = queryProduct.Include(c => c.IdCategoriaNavigation).First();

                return productoEditado;

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<bool> Eliminar(int idProducto)
        {
            try
            {
                Producto productoEncontrado = await _repository.Obtener(p => p.IdProducto == idProducto);

                if (productoEncontrado == null)
                    throw new TaskCanceledException("Este equipo no existe!!");

                string nombreImagen = productoEncontrado.NombreImagen;

                bool answer = await _repository.Eliminar(productoEncontrado);

                if (answer)
                    await _fireBseService.EliminarStorage("carpeta_producto", nombreImagen);
                return true;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
