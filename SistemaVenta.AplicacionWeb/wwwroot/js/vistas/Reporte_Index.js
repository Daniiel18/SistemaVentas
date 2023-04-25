let tablaData;
$(document).ready(function () {

    $.datepicker.setDefaults($.datepicker.regional["es"])

    $("#txtFechaInicio").datepicker({ dateFormat: "dd/mm/yy" })
    $("#txtFechaFin").datepicker({ dateFormat: "dd/mm/yy" })

    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Reporte/ReporteVenta?fechaInicio=01/01/1991&fechaFin=01/01/1991',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            {
                "data": "fechaRegistro",
                //En este código, primero convertimos la fecha de entrada a un objeto moment 
                //y le agregamos 12 horas.Luego, obtenemos la hora y el período AM / PM de la 
                //fecha y la devolvemos en el formato deseado.Este código supone que el formato 
                //de entrada de la fecha es "DD/MM/YYYY HH:mm".
                "render": function (data, type, row) {
                    var fecha = moment(data, "DD/MM/YYYY HH:mm").add(12, "hours");
                    var hora = fecha.format("h:mm");
                    var am_pm = fecha.format("A");
                    return fecha.format("DD/MM/YYYY") + " " + hora + " " + am_pm;
                }
            },
            { "data": "numeroVenta" },
            { "data": "tipoDocumento" },
            { "data": "documentoCliente" },
            { "data": "nombreCliente" },
            { "data": "subTotalVenta" },
            { "data": "impuestoTotalVenta" },
            { "data": "totalVenta" },
            { "data": "producto" },
            { "data": "cantidad" },
            { "data": "precio" },
            { "data": "total" },
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Ventas'
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

})

$("#btnBuscar").click(function () {

  if ($("#txtFechaInicio").val().trim() == "" || $("#txtFechaFin").val().trim() == "") {
      toastr.warning("", "Debes ingresar una fecha de inicio y de fin")
      return;
  }

    let fechaInicio = $("#txtFechaInicio").val().trim();
    let fechaFin = $("#txtFechaFin").val().trim();

    let nuevaUrl = `/Reporte/ReporteVenta?fechaInicio=${fechaInicio}&fechaFin=${fechaFin}`;

    tablaData.ajax.url(nuevaUrl).load();


})