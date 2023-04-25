using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IRoService
    {
        Task<List<Rol>> Lista();
    }
}
