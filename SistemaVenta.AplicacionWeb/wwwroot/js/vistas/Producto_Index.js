const modeloBase = {
    idProducto: 0,
    codigoBarra: "",
    marca: "",
    nombre: "",
    idCategoria: 0,
    urlImagen: "",
    stock: 0,
    precio: 0,
    esActivo: 1
}

let tablaData;

//Obteniendo lista de categorias
$(document).ready(function () {
    $(".card-body").LoadingOverlay("show")
    fetch("/Categoria/Lista")
        .then(response => {
            $(".card-body").LoadingOverlay("hide")

            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.data.length > 0) {
                responseJson.data.forEach((item) => {
                    $("#cboCategoria").append(
                        $("<option>").val(item.idCategoria).text(item.descripcion)
                    )
                })
            }
        })



    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Producto/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idProducto", "visible": false, "searchable": false },
            {
                "data": "urlImagen", render: function (data) {
                    return `<img style="height:60px" src=${data} class="rounded mx-auto d-block"/>`
                }
            },
            { "data": "codigoBarra" },
            { "data": "marca" },
            { "data": "descripcion" },
            { "data": "nombreCategoria" },
            { "data": "stock" },
            { "data": "precio" },
            {
                "data": "esActivo", render: function (data) {
                    if (data == 1)
                        return '<span class="badge badge-info">Activo</span>';
                    else {
                        return '<span class="badge badge-danger">No Activo</span>';
                    }
                }
            },
            {
                "defaultContent": '<button class="btn btn-primary btn-editar btn-sm mr-2"><i class="fas fa-pencil-alt"></i></button>' +
                    '<button class="btn btn-danger btn-eliminar btn-sm"><i class="fas fa-trash-alt"></i></button>',
                "orderable": false,
                "searchable": false,
                "width": "80px"
            }
        ],
        order: [[0, "desc"]],
        dom: "Bfrtip",
        buttons: [
            {
                text: 'Exportar Excel',
                extend: 'excelHtml5',
                title: '',
                filename: 'Reporte Productos',
                exportOptions: {
                    columns: [2, 3, 4, 5, 6]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

})

function showModal(model = modeloBase) {
    $("#txtId").val(model.idProducto)
    $("#txtCodigoBarra").val(model.codigoBarra)
    $("#txtMarca").val(model.marca)
    $("#txtDescripcion").val(model.descripcion)
    $("#cboCategoria").val(model.idCategoria == 0 ? $("#cboCategoria option:first").val() : model.idCategoria)
    $("#cboEstado").val(model.esActivo)
    $("#txtStock").val(model.stock)
    $("#txtPrecio").val(model.precio)
    $("#txtImagen").val("")
    $("#imgProducto").attr("src", model.urlImagen)

    $("#modalData").modal("show")

}

$("#btnNuevo").click(function () {
    showModal()
})

$("#btnGuardar").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const inputs_without_data = inputs.filter((item) => item.value.trim() == "")

    if (inputs_without_data.length > 0) {
        const msg = `Debe completar el campo : "${inputs_without_data[0].name}"`;
        toastr.warning("", msg)
        $(`input[name = "${inputs_without_data[0].name}"]`).focus()
        return;
    }

    const model = structuredClone(modeloBase);
    model["idProducto"] = parseInt($("#txtId").val())
    model["codigoBarra"] = $("#txtCodigoBarra").val()
    model["marca"] = $("#txtMarca").val()
    model["descripcion"] = $("#txtDescripcion").val()
    model["idCategoria"] = $("#cboCategoria").val()
    model["esActivo"] = $("#cboEstado").val()
    model["stock"] = $("#txtStock").val()
    model["precio"] = $("#txtPrecio").val()

    const inputFoto = document.getElementById("txtImagen")

    const formData = new FormData();

    formData.append("imagen", inputFoto.files[0])
    formData.append("model", JSON.stringify(model))

    $("#modalData").find("div.modal-content").LoadingOverlay("show")
    if (model.idProducto == 0) {
        fetch("/Producto/Crear", {
            method: "POST",
            body: formData
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide")
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false)
                    $("#modalData").modal("hide")
                    swal("Listo!", "El producto fue creado", "success")
                } else {
                    swal("Lo siento!", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/Producto/Editar", {
            method: "PUT",
            body: formData
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide")
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row(selectRow).data(responseJson.objeto).draw(false);
                    selectRow = null;
                    $("#modalData").modal("hide")
                    swal("Listo!", "El producto ha sido modificado", "success")
                } else {
                    swal("Lo siento!", responseJson.mensaje, "error")
                }
            })
    }
})

let selectRow;
$("#tbdata tbody").on("click", ".btn-editar", function () {

    if ($(this).closest("tr").hasClass("child")) {
        selectRow = $(this).closest("tr").prev();
    } else {
        selectRow = $(this).closest("tr");
    }

    const data = tablaData.row(selectRow).data();
    showModal(data);
})

$("#tbdata tbody").on("click", ".btn-eliminar", function () {
    let fila;

    if ($(this).closest("tr").hasClass("child")) {
        fila = $(this).closest("tr").prev();
    } else {
        fila = $(this).closest("tr");
    }

    const dt = tablaData.row(fila).data();
    swal({
        title: "¿Esás seguro?",
        text: `Eliminar al usuario "${dt.descripcion}"`,
        type: "warning",
        showCancelButton: true,
        confirmButtonClass: "btn-danger",
        confirmButtonText: "Si, Eliminar",
        cancelButtonText: "No",
        closeOnConfirm: false,
        closeOnCancel: true
    },
        function (answer) {

            if (answer) {
                $(".showSweetAlert").LoadingOverlay("show");

                fetch(`/Producto/Eliminar?idProducto=${dt.idProducto}`, {
                    method: "DELETE"
                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {

                        if (responseJson.estado) {

                            tablaData.row(fila).remove().draw()

                            swal("Listo!", "El producto fue eliminado", "success")

                        } else {

                            swal("Lo siento!", responseJson.mensaje, "error")
                        }
                    })
            }
        }
    )
})