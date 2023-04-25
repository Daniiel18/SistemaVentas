﻿const modeloBase = {
    idCategoria: 0,
    descripcion: "",
    esActivo: 1
}

let tablaData;

//Obteniendo lista de usuarios
$(document).ready(function () {

    tablaData = $('#tbdata').DataTable({

        responsive: true,
        "ajax": {
            "url": '/Categoria/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idCategoria", "visible": false, "searchable": false },
            { "data": "descripcion" },
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
                filename: 'Reporte Categorias',
                exportOptions: {
                    columns: [2, 3]
                }
            }, 'pageLength'
        ],
        language: {
            url: "https://cdn.datatables.net/plug-ins/1.11.5/i18n/es-ES.json"
        },
    });

})

function showModal(model = modeloBase) {
    $("#txtId").val(model.idCategoria)
    $("#txtDescripcion").val(model.descripcion)
    $("#cboEstado").val(model.esActivo)

    $("#modalData").modal("show")

}

$("#btnNuevo").click(function () {
    showModal()
})

$("#btnGuardar").click(function () {

    if ($("#txtDescripcion").val().trim() == "") {
        toastr.warning("", "Debe completar el campo : descripcion")
        $("#txtDescripcion").focus()

        return;
    }

    const model = structuredClone(modeloBase);
    model["idCategoria"] = parseInt($("#txtId").val())
    model["descripcion"] = $("#txtDescripcion").val()
    model["esActivo"] = $("#cboEstado").val()

    $("#modalData").find("div.modal-content").LoadingOverlay("show")
    if (model.idCategoria == 0) {
        fetch("/Categoria/Crear", {
            method: "POST",
            headers: { "Content-Type": "application/json;charset=utf-8" },
            body: JSON.stringify(model)
        })
            .then(response => {
                $("#modalData").find("div.modal-content").LoadingOverlay("hide")
                return response.ok ? response.json() : Promise.reject(response);
            })
            .then(responseJson => {
                if (responseJson.estado) {
                    tablaData.row.add(responseJson.objeto).draw(false)
                    $("#modalData").modal("hide")
                    swal("Listo!", "La categoria fue creada", "success")
                } else {
                    swal("Lo siento!", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/Categoria/Editar", {
            method: "PUT",
            headers: { "Content-Type": "application/json;charset=utf-8" },
            body: JSON.stringify(model)
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
                    swal("Listo!", "La categoria ha sido modificada", "success")
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

//Funcion para eliminar
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
        text: `Eliminar la categoria "${dt.descripcion}"`,
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

                fetch(`/Categoria/Eliminar?idCategoria=${dt.idCategoria}`, {
                    method: "DELETE"
                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {

                        if (responseJson.estado) {

                            tablaData.row(fila).remove().draw()

                            swal("Listo!", "La categoria fue eliminada", "success")

                        } else {

                            swal("Lo siento!", responseJson.mensaje, "error")
                        }
                    })
            }
        }
    )
})