namespace SistemaVenta.BLL.Interfaces
{
    public interface IFirebBseService
    {
        Task<string> SubirStorage(Stream StreamArchivo, string CarpetaDestino, string NombreArchivo);
        Task<bool> EliminarStorage(string CarpetaDestino, string NombreArchivo);
    }
}