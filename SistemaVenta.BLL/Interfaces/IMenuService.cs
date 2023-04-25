using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Interfaces
{
    public interface IMenuService
    {
        Task<List<Menu>> ObtenerMenu(int idUsuario);
    }
}
