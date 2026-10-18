using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;
using SistemaVenta.AplicacionWeb.Utilidades.Reportes;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class VentaController : Controller
    {
        private readonly ITipoDocumentoVentaService _tipoDocumentoVentaService;
        private readonly IVentaService _ventaService;
        private readonly IMapper _mapper;
        private readonly INegocioService _negocioService;
        private readonly IHttpClientFactory _httpClientFactory;

        public VentaController(ITipoDocumentoVentaService tipoDocumentoVentaService,
            IVentaService ventaService, IMapper mapper,
            INegocioService negocioService, IHttpClientFactory httpClientFactory)
        {
            _tipoDocumentoVentaService = tipoDocumentoVentaService;
            _ventaService = ventaService;
            _mapper = mapper;
            _negocioService = negocioService;
            _httpClientFactory = httpClientFactory;

        }

        public IActionResult NuevaVenta()
        {
            return View();
        }
        public IActionResult HistorialVenta()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ListaTipoDocumentoVenta()
        {
            List<VMTipoDocumentoVenta> vmListaTipoDocumentos = _mapper.Map<List<VMTipoDocumentoVenta>>(await _tipoDocumentoVentaService.Lista());

            return StatusCode(StatusCodes.Status200OK, vmListaTipoDocumentos);
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerProductos(string busqueda)
        {
            List<VMProducto> vmListaProductos = _mapper.Map<List<VMProducto>>(await _ventaService.ObtenerProductos(busqueda));
            return StatusCode(StatusCodes.Status200OK, vmListaProductos);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarVenta([FromBody] VMVenta model)
        {
            GenericResponse<VMVenta> genericResponse = new GenericResponse<VMVenta>();

            try
            {
                ClaimsPrincipal claimUsuario = HttpContext.User;

                string idUsuario = claimUsuario.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault();

                model.IdUsuario = int.Parse(idUsuario);

                Venta ventaCreada = await _ventaService.Registrar(_mapper.Map<Venta>(model));
                model = _mapper.Map<VMVenta>(ventaCreada);

                genericResponse.Estado = true;
                genericResponse.Objeto = model;

            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;

                if (ex.Message.Contains("La cantidad a vender supera el stock disponible"))
                {
                    genericResponse.Mensaje = ex.Message;
                    return StatusCode(StatusCodes.Status200OK, genericResponse);
                }

                genericResponse.Mensaje = "Error al registrar la venta: " + ex.Message;
                return StatusCode(StatusCodes.Status200OK, genericResponse);
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }


        [HttpGet]
        public async Task<IActionResult> Historial(string numeroVenta, string fechaInicio, string fechaFin)
        {
            List<VMVenta> vmHistorialVenta = _mapper.Map<List<VMVenta>>(await _ventaService.Historial(numeroVenta, fechaInicio, fechaFin));
            return StatusCode(StatusCodes.Status200OK, vmHistorialVenta);
        }

        public async Task<IActionResult> MostrarPDFVenta(string numeroVenta)
        {
            VMVenta venta = _mapper.Map<VMVenta>(await _ventaService.Detalle(numeroVenta));
            VMNegocio negocio = _mapper.Map<VMNegocio>(await _negocioService.Obtener());

            byte[]? logo = null;
            if (!string.IsNullOrWhiteSpace(negocio.UrlLogo))
            {
                try
                {
                    logo = await _httpClientFactory.CreateClient().GetByteArrayAsync(negocio.UrlLogo);
                }
                catch (Exception)
                {
                    // The receipt is still valid without the logo
                }
            }

            byte[] archivoPDF = ReciboVenta.Generar(venta, negocio, logo);

            return File(archivoPDF, "application/pdf");
        }
    }
}