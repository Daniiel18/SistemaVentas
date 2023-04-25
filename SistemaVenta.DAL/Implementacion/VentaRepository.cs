using Microsoft.EntityFrameworkCore;
using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.DAL.Implementacion
{
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {
        private readonly DBContext.ITventoryContext _dbContext;

        public VentaRepository(DBContext.ITventoryContext dbContext) : base (dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Venta> Registrar(Venta entity)
        {
            Venta ventaGenerada = new Venta();

            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (DetalleVenta dv in entity.DetalleVenta)
                    {
                        Producto producto_encontrado = _dbContext.Productos.Where(p => p.IdProducto == dv.IdProducto).First();

                        // Validamos que la cantidad a vender no supere el stock disponible
                        if (dv.Cantidad > producto_encontrado.Stock)
                        {
                            throw new Exception("La cantidad a vender supera el stock disponible para el " + producto_encontrado.Descripcion);
                        }

                        producto_encontrado.Stock = producto_encontrado.Stock - dv.Cantidad;
                        _dbContext.Productos.Update(producto_encontrado);
                    }

                    await _dbContext.SaveChangesAsync();
                    //Obtenemos el correlativo
                    NumeroCorrelativo correlativo = _dbContext.NumeroCorrelativos.Where(n => n.Gestion == "Venta").First();
                    //Aumentamos el ultimo digito del numero correlativo
                    correlativo.UltimoNumero = correlativo.UltimoNumero + 1;
                    correlativo.FechaActualizacion = DateTime.Now;

                    _dbContext.NumeroCorrelativos.Update(correlativo);
                    await _dbContext.SaveChangesAsync();

                    string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value));
                    string numeroVenta = ceros + correlativo.UltimoNumero.ToString();
                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - correlativo.CantidadDigitos.Value, correlativo.CantidadDigitos.Value);

                    entity.NumeroVenta = numeroVenta;


                    await _dbContext.Venta.AddAsync(entity);
                    await _dbContext.SaveChangesAsync();

                    ventaGenerada = entity;

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
            return ventaGenerada;
        }

        public async Task<List<DetalleVenta>> Reporte(DateTime FechaInicio, DateTime FechaFin)
        {
            List<DetalleVenta> detalleVentas = await _dbContext.DetalleVenta
                .Include(l => l.IdVentaNavigation)
                .ThenInclude(u => u.IdUsuarioNavigation)
                .Include(l => l.IdVentaNavigation)
                .ThenInclude(tdl => tdl.IdTipoDocumentoVentaNavigation)
                .Where(dl => dl.IdVentaNavigation.FechaRegistro.Value.Date >= FechaInicio.Date &&
                dl.IdVentaNavigation.FechaRegistro.Value.Date <= FechaFin.Date).ToListAsync();

            return detalleVentas;
        }
    }
}