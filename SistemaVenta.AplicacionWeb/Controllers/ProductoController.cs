using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.AplicacionWeb.Utilidades.Response;
using SistemaVenta.BLL.Interfaces;
using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    [Authorize]
    public class ProductoController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IProductoService _productoService;

        public ProductoController(IMapper mapper, IProductoService productoService)
        {
            _mapper = mapper;
            _productoService = productoService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMProducto> vmListaProducto = _mapper.Map<List<VMProducto>>(await _productoService.Lista());

            return StatusCode(StatusCodes.Status200OK, new { data = vmListaProducto });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile imagen, [FromForm] string model)
        {
            GenericResponse<VMProducto> genericResponse = new GenericResponse<VMProducto>();
            try
            {
                VMProducto vmProducto = JsonConvert.DeserializeObject<VMProducto>(model);

                string nombreImagen = "";
                Stream imagenStrem = null;

                if (imagen != null)
                {
                    string nombreEnCodigo = Guid.NewGuid().ToString("N");
                    string ext = Path.GetExtension(imagen.FileName);
                    nombreImagen = string.Concat(nombreEnCodigo, ext);
                    imagenStrem = imagen.OpenReadStream();
                }

                Producto productoCreado = await _productoService.Crear(_mapper.Map<Producto>(vmProducto), imagenStrem, nombreImagen);

                vmProducto = _mapper.Map<VMProducto>(productoCreado);

                genericResponse.Estado = true;
                genericResponse.Objeto = vmProducto;


            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile imagen, [FromForm] string model)
        {
            GenericResponse<VMProducto> genericResponse = new GenericResponse<VMProducto>();
            try
            {
                VMProducto vmProducto = JsonConvert.DeserializeObject<VMProducto>(model);

                string nombreImagen = "";
                Stream imageStrem = null;

                if (imagen != null)
                {
                    string nombreEnCodigo = Guid.NewGuid().ToString("N");
                    string ext = Path.GetExtension(imagen.FileName);
                    nombreImagen = string.Concat(nombreEnCodigo, ext);
                    imageStrem = imagen.OpenReadStream();
                }

                Producto productoEditado = await _productoService.Editar(_mapper.Map<Producto>(vmProducto), imageStrem, nombreImagen);

                vmProducto = _mapper.Map<VMProducto>(productoEditado);

                genericResponse.Estado = true;
                genericResponse.Objeto = vmProducto;
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idProducto)
        {
            GenericResponse<string> genericResponse = new GenericResponse<string>();

            try
            {
                genericResponse.Estado = await _productoService.Eliminar(idProducto);
            }
            catch (Exception ex)
            {
                genericResponse.Estado = false;
                genericResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, genericResponse);
        }
    }
}