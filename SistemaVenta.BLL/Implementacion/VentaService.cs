using Microsoft.EntityFrameworkCore;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;
using System.Globalization;

namespace SistemaVenta.BLL.Implementacion
{
    public class VentaService : IVentaService
    {
        private readonly IGenericRepository<Producto> _repositoryProducto;
        private readonly IVentaRepository _ventaRepository;

        public VentaService(IGenericRepository<Producto> repositoryProducto, IVentaRepository ventaRepository)
        {
            _repositoryProducto = repositoryProducto;
            _ventaRepository = ventaRepository;
        }

        public async Task<List<Producto>> ObtenerProductos(string busqueda)
        {
            IQueryable<Producto> query = await _repositoryProducto.Consultar(p =>
           p.EsActivo == true &&
           p.Stock > 0 &&
           string.Concat(p.CodigoBarra, p.Marca, p.Descripcion).Contains(busqueda)
               );

            return query.Include(c => c.IdCategoriaNavigation).ToList();
        }

        public async Task<Venta> Registrar(Venta entidad)
        {
            try
            {
                return await _ventaRepository.Registrar(entidad);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Venta>> Historial(string numeroVenta, string fechaInicio, string fechaFin)
        {
            IQueryable<Venta> query = await _ventaRepository.Consultar();
            fechaInicio = fechaInicio is null ? "" : fechaInicio;
            fechaFin = fechaFin is null ? "" : fechaFin;

            if (fechaInicio != "" && fechaFin != "")
            {
                DateTime fecha_inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-DO"));
                DateTime fecha_fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-DO"));

                return query.Where(v =>v.FechaRegistro.Value.Date >= fecha_inicio.Date &&
                                        v.FechaRegistro.Value.Date <= fecha_fin.Date)
                .Include(tdv => tdv.IdTipoDocumentoVentaNavigation)
                .Include(u => u.IdUsuarioNavigation)
                .Include(dv => dv.DetalleVenta)
                .ToList();

            }
            else
            {
                return query.Where(v => v.NumeroVenta == numeroVenta
                )
                    .Include(tdl => tdl.IdTipoDocumentoVentaNavigation)
                    .Include(u => u.IdUsuarioNavigation)
                    .Include(ld => ld.DetalleVenta)
                    .ToList();
            }
        }

        public async Task<Venta> Detalle(string numeroVenta)
        {

            IQueryable<Venta> query = await _ventaRepository.Consultar(v => v.NumeroVenta == numeroVenta);

            return query
                    .Include(tdl => tdl.IdTipoDocumentoVentaNavigation)
                    .Include(u => u.IdUsuarioNavigation)
                    .Include(ld => ld.DetalleVenta)
                    .First();
        }

        public async Task<List<DetalleVenta>> Reporte(string fechaInicio, string fechaFin)
        {
            DateTime fecha_inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-DO"));
            DateTime fecha_fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-DO"));

            List<DetalleVenta> lista = await _ventaRepository.Reporte(fecha_inicio, fecha_fin);

            return lista;
        }
    }
}