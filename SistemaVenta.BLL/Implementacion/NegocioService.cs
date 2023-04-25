using SistemaVenta.BLL.Interfaces;
using SistemaVenta.DAL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.BLL.Implementacion
{
    public class NegocioService : INegocioService
    {
        private readonly IGenericRepository<Negocio> _repositorio;
        private readonly IFirebBseService _firebBseService;

        public NegocioService(IGenericRepository<Negocio> repositorio, IFirebBseService firebBseService)
        {
            _repositorio = repositorio;
            _firebBseService = firebBseService;
        }

        public async Task<Negocio> Obtener()
        {
            try
            {
                Negocio negocioEncontrado = await _repositorio.Obtener(n => n.IdNegocio == 1);
                return negocioEncontrado;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<Negocio> GuardarCambios(Negocio entidad, Stream Logo = null, string NombreLogo = "")
        {
            try
            {
                Negocio negocioEncontrado = await _repositorio.Obtener(n => n.IdNegocio == 1);

                negocioEncontrado.NumeroDocumento = entidad.NumeroDocumento;
                negocioEncontrado.Nombre = entidad.Nombre;
                negocioEncontrado.Correo = entidad.Correo;
                negocioEncontrado.Direccion = entidad.Direccion;
                negocioEncontrado.Telefono = entidad.Telefono;
                negocioEncontrado.PorcentajeImpuesto = entidad.PorcentajeImpuesto;
                negocioEncontrado.SimboloMoneda = entidad.SimboloMoneda;

                negocioEncontrado.NombreLogo = negocioEncontrado.NombreLogo == "" ? NombreLogo : negocioEncontrado.NombreLogo;

                if (Logo != null)
                {
                    string urlLogo = await _firebBseService.SubirStorage(Logo, "carpeta_logo", negocioEncontrado.NombreLogo);
                    negocioEncontrado.UrlLogo = urlLogo;
                }

                await _repositorio.Editar(negocioEncontrado);
                return negocioEncontrado;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
