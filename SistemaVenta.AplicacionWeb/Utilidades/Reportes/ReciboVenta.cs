using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SistemaVenta.AplicacionWeb.Models.ViewModels;

namespace SistemaVenta.AplicacionWeb.Utilidades.Reportes
{
    // Sale receipt as a PDF, with the same layout as the original HTML template
    public static class ReciboVenta
    {
        private const string ColorPrincipal = "#03A99F";
        private const string ColorTexto = "#757575";
        private const string ColorSuave = "#858585";
        private const string ColorFila = "#EDF6F9";
        private const string ColorBorde = "#E8E8E8";

        public static byte[] Generar(VMVenta venta, VMNegocio negocio, byte[]? logo)
        {
            string moneda = negocio.SimboloMoneda ?? "";
            string Importe(decimal? valor) => $"{moneda} {valor ?? 0:0.00}";

            return Document.Create(documento =>
            {
                documento.Page(pagina =>
                {
                    pagina.Size(PageSizes.Letter);
                    pagina.Margin(40);
                    pagina.DefaultTextStyle(estilo => estilo.FontSize(11).FontColor(ColorTexto));

                    pagina.Content().Column(columna =>
                    {
                        columna.Spacing(18);

                        // Logo and sale number
                        columna.Item().Row(fila =>
                        {
                            if (logo != null)
                                fila.ConstantItem(90).Height(90).Image(logo).FitArea();
                            else
                                fila.ConstantItem(90);

                            fila.RelativeItem().AlignRight().Column(c =>
                            {
                                c.Item().AlignRight().Text("NÚMERO VENTA").Bold().FontSize(16).FontColor(ColorPrincipal);
                                c.Item().AlignRight().Text(venta.NumeroVenta ?? "").FontSize(13).FontColor(Colors.Black);
                                c.Item().AlignRight().Text(venta.TipoDocumentoVenta ?? "").FontColor(ColorSuave);
                                if (venta.FechaRegistro != null)
                                    c.Item().AlignRight().Text($"{venta.FechaRegistro:dd/MM/yyyy HH:mm}").FontColor(ColorSuave);
                            });
                        });

                        // Business and customer
                        columna.Item().Row(fila =>
                        {
                            fila.RelativeItem().Column(c =>
                            {
                                c.Item().Text((negocio.Nombre ?? "").ToUpper()).Bold().FontColor(Colors.Black);
                                c.Item().Text($"RUC: {negocio.NumeroDocumento}").FontColor(ColorSuave);
                                c.Item().Text($"Dirección: {negocio.Direccion}").FontColor(ColorSuave);
                                c.Item().Text($"Correo: {negocio.Correo}").FontColor(ColorSuave);
                            });

                            fila.RelativeItem().AlignRight().Column(c =>
                            {
                                c.Item().AlignRight().Text("CLIENTE").Bold().FontColor(Colors.Black);
                                c.Item().AlignRight().Text(venta.NombreCliente ?? "").FontColor(ColorSuave);
                                c.Item().AlignRight().Text(venta.DocumentoCliente ?? "").FontColor(ColorSuave);
                            });
                        });

                        // Products
                        columna.Item().Table(tabla =>
                        {
                            tabla.ColumnsDefinition(c =>
                            {
                                c.RelativeColumn();
                                c.ConstantColumn(80);
                                c.ConstantColumn(95);
                                c.ConstantColumn(95);
                            });

                            tabla.Header(cabecera =>
                            {
                                foreach (string titulo in new[] { "Producto", "Cantidad", "Precio", "Total" })
                                    cabecera.Cell().Background(ColorPrincipal).Padding(8)
                                        .Text(titulo).Bold().FontColor(Colors.White);
                            });

                            foreach (VMDetalleVenta item in venta.DetalleVenta ?? new List<VMDetalleVenta>())
                            {
                                tabla.Cell().BorderBottom(2).BorderColor(ColorBorde).Padding(8).Text(item.DescripcionProducto ?? "");
                                tabla.Cell().BorderBottom(2).BorderColor(ColorBorde).Padding(8).Text($"{item.Cantidad}");
                                tabla.Cell().BorderBottom(2).BorderColor(ColorBorde).Padding(8).Text(Importe(item.Precio));
                                tabla.Cell().Background(ColorFila).Padding(8).Text(Importe(item.Total));
                            }
                        });

                        // Totals
                        columna.Item().AlignRight().Width(190).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(95);
                                c.ConstantColumn(95);
                            });

                            void Linea(string etiqueta, decimal? valor, bool resaltada)
                            {
                                var celda = tabla.Cell().Padding(2);
                                (resaltada ? celda.Background(ColorPrincipal) : celda.BorderBottom(2).BorderColor(ColorBorde))
                                    .Padding(6).Text(etiqueta).Bold().FontColor(resaltada ? Colors.White : ColorTexto);
                                tabla.Cell().Padding(2).Background(ColorPrincipal).Padding(6)
                                    .Text(Importe(valor)).Bold().FontColor(Colors.White);
                            }

                            Linea("Sub Total", venta.SubTotal, false);
                            Linea("IGV", venta.ImpuestoTotal, false);
                            Linea("Total", venta.Total, true);
                        });
                    });

                    pagina.Footer().AlignCenter().Text("Gracias por su compra").FontSize(9).FontColor(ColorSuave);
                });
            }).GeneratePdf();
        }
    }
}
