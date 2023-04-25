using AutoMapper;
using SistemaVenta.AplicacionWeb.Models.ViewModels;
using SistemaVenta.Entity;
using System.Globalization;

namespace SistemaVenta.AplicacionWeb.Utilidades.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region Rol
            CreateMap<Rol, VMRol>().ReverseMap();
            #endregion Rol

            #region Usuario
            CreateMap<Usuario, VMUsuario>()
                .ForMember(destin =>
                destin.EsActivo,
                opt => opt.MapFrom(origin => origin.EsActivo == true ? 1 : 0)
                )
                .ForMember(destin =>
                destin.NombreRol,
                opt => opt.MapFrom(origin => origin.IdRolNavigation.Descripcion)
            );

            CreateMap<VMUsuario, Usuario>()
                .ForMember(destin =>
                destin.EsActivo,
                opt => opt.MapFrom(origin => origin.EsActivo == 1 ? true : false)
                )
                .ForMember(destin =>
                destin.IdRolNavigation,
                opt => opt.Ignore()
                );
            #endregion User

            #region Negocio
            CreateMap<Negocio, VMNegocio>()
                .ForMember(dest => dest.PorcentajeImpuesto, opt => opt.MapFrom(src => src.PorcentajeImpuesto));

            CreateMap<VMNegocio, Negocio>()
                .ForMember(dest => dest.PorcentajeImpuesto, opt => opt.MapFrom(src => src.PorcentajeImpuesto));


            #endregion

            #region Categoria
            CreateMap<Categoria, VMCategoria>()
                .ForMember(destin =>
                destin.EsActivo,
                opt => opt.MapFrom(origin => origin.EsActivo == true ? 1 : 0)
                );

            CreateMap<VMCategoria, Categoria>()
                .ForMember(destin =>
                destin.EsActivo,
                opt => opt.MapFrom(origin => origin.EsActivo == 1 ? true : false)
                );
            #endregion Category
            
            #region Producto
            CreateMap<Producto, VMProducto>()
                .ForMember(destin =>
                destin.EsActivo,
                opt => opt.MapFrom(origin => origin.EsActivo == true ? 1 : 0)
                )
                .ForMember(destin =>
                destin.NombreCategoria,
                opt => opt.MapFrom(origin => origin.IdCategoriaNavigation.Descripcion)
                )
                .ForMember(destin =>
                destin.Precio,
                opt => opt.MapFrom(origin => Convert.ToString(origin.Precio.Value, new CultureInfo("es-DO")))
                );

            CreateMap<VMProducto, Producto>()
                .ForMember(destin =>
                destin.EsActivo,
                opt => opt.MapFrom(origin => origin.EsActivo == 1 ? true : false)
                )
                .ForMember(destin =>
                destin.IdCategoriaNavigation,
                opt => opt.Ignore()
                )
                .ForMember(destin =>
                destin.Precio,
                opt => opt.MapFrom(origin => Convert.ToString(origin.Precio.Value, new CultureInfo("es-DO")))
                );
            #endregion Product

            #region TipoDocumentoVenta
            CreateMap<TipoDocumentoVenta, VMTipoDocumentoVenta>().ReverseMap();

            #endregion TipoDocumentoVenta

            #region Venta
            CreateMap<Venta, VMVenta>()
                .ForMember(destin =>
                destin.TipoDocumentoVenta,
                opt => opt.MapFrom(origin => origin.IdTipoDocumentoVentaNavigation.Descripcion)
                )
                .ForMember(destin =>
                destin.Usuario,
                opt => opt.MapFrom(origin => origin.IdUsuarioNavigation.Nombre)
                )
                .ForMember(destin =>
                destin.SubTotal,
                opt => opt.MapFrom(origin => Convert.ToString(origin.SubTotal.Value, new CultureInfo("es-DO")))
                )
                .ForMember(destin =>
                destin.ImpuestoTotal,
                opt => opt.MapFrom(origin => Convert.ToString(origin.ImpuestoTotal.Value, new CultureInfo("es-DO")))
                )
                .ForMember(destin =>
                destin.Total,
                opt => opt.MapFrom(origin => Convert.ToString(origin.Total.Value, new CultureInfo("es-DO")))
                )
                .ForMember(destin =>
                destin.FechaRegistro,
                opt => opt.MapFrom(origin => origin.FechaRegistro.Value.ToString("dd/MM/yyyy hh:mm:ss"))
                );


            CreateMap<VMVenta, Venta>()
                .ForMember(destin =>
                destin.SubTotal,
                opt => opt.MapFrom(origin => Convert.ToDecimal(origin.SubTotal, new CultureInfo("es-DO")))
                )
                .ForMember(destin =>
                destin.ImpuestoTotal,
                opt => opt.MapFrom(origin => Convert.ToDecimal(origin.ImpuestoTotal, new CultureInfo("es-DO")))
                )
                .ForMember(destin =>
                destin.Total,
                opt => opt.MapFrom(origin => Convert.ToDecimal(origin.Total, new CultureInfo("es-DO")))
                );
            #endregion Venta

            #region Detalle Venta
            CreateMap<DetalleVenta, VMDetalleVenta>()
                .ForMember(destin =>
                destin.Precio,
                opt => opt.MapFrom(origin => Convert.ToString(origin.Precio.Value, new CultureInfo("es-DO")))
                )
                .ForMember(destin =>
                destin.Total,
                opt => opt.MapFrom(origin => Convert.ToString(origin.Total.Value, new CultureInfo("es-DO")))
                );

            CreateMap<VMDetalleVenta, DetalleVenta>()
            .ForMember(destin =>
            destin.Precio,
            opt => opt.MapFrom(origin => Convert.ToDecimal(origin.Precio, new CultureInfo("es-DO")))
            )
            .ForMember(destin =>
            destin.Total,
            opt => opt.MapFrom(origin => Convert.ToDecimal(origin.Total, new CultureInfo("es-DO")))
            );

            CreateMap<DetalleVenta, VMReporteVenta>()
            .ForMember(destin =>
            destin.FechaRegistro,
            opt => opt.MapFrom(origin => origin.IdVentaNavigation.FechaRegistro.Value.ToString("dd/MM/yyyy hh:mm:ss"))
            )
            .ForMember(destin =>
            destin.NumeroVenta,
            opt => opt.MapFrom(origin => origin.IdVentaNavigation.NumeroVenta)
            )
            .ForMember(destin =>
            destin.TipoDocumento,
            opt => opt.MapFrom(origin => origin.IdVentaNavigation.IdTipoDocumentoVentaNavigation.Descripcion)
            )
            .ForMember(destin =>
            destin.DocumentoCliente,
            opt => opt.MapFrom(origin => origin.IdVentaNavigation.DocumentoCliente)
            )
            .ForMember(destin =>
            destin.NombreCliente,
            opt => opt.MapFrom(origin => origin.IdVentaNavigation.NombreCliente)
            )
            .ForMember(destin =>
            destin.SubTotalVenta,
            opt => opt.MapFrom(origin => Convert.ToString(origin.IdVentaNavigation.SubTotal.Value, new CultureInfo("es-DO")))
            )
            .ForMember(destin =>
            destin.ImpuestoTotalVenta,
            opt => opt.MapFrom(origin => Convert.ToString(origin.IdVentaNavigation.ImpuestoTotal.Value, new CultureInfo("es-DO")))
            )
            .ForMember(destin =>
            destin.TotalVenta,
            opt => opt.MapFrom(origin => Convert.ToString(origin.IdVentaNavigation.Total.Value, new CultureInfo("es-DO")))
            )
            .ForMember(destin =>
            destin.Producto,
            opt => opt.MapFrom(origin => origin.DescripcionProducto)
            )
            .ForMember(destin =>
            destin.Precio,
            opt => opt.MapFrom(origin => Convert.ToString(origin.Precio.Value, new CultureInfo("es-DO")))
            )
            .ForMember(destin =>
            destin.Total,
            opt => opt.MapFrom(origin => Convert.ToString(origin.Total.Value, new CultureInfo("es-DO")))
            );
            #endregion

            #region Menu
            CreateMap<Menu, VMMenu>()
                .ForMember(destin =>
                destin.SubMenus,
                opt => opt.MapFrom(origin => origin.InverseIdMenuPadreNavigation)
                );
            #endregion
        }
    }
}