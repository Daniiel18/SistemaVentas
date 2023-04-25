const modeloBase = {
    idUsuario: 0,
    nombre: "",
    correo: "",
    telefono: "",
    idRol: 0,
    esActivo: 1,
    urlFoto: ""
}

let tablaData;

//Obteniendo lista de usuarios
$(document).ready(function () {
    $(".card-body").LoadingOverlay("show")

    fetch("/Usuario/ListaRoles")
        .then(response => {
            $(".card-body").LoadingOverlay("hide")

            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.length > 0) {
                responseJson.forEach((item) => {
                    $("#cboRol").append(
                        $("<option>").val(item.idRol).text(item.descripcion)
                    )
                })
            }
        })



    tablaData = $('#tbdata').DataTable({
        responsive: true,
        "ajax": {
            "url": '/Usuario/Lista',
            "type": "GET",
            "datatype": "json"
        },
        "columns": [
            { "data": "idUsuario", "visible": false, "searchable": false },
            {
                "data": "urlFoto", render: function (data) {
                    return `<img style="height:60px" src=${data} class="rounded mx-auto d-block"/>`
                }
            },
            { "data": "nombre" },
            { "data": "correo" },
            { "data": "telefono" },
            { "data": "nombreRol" },
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
                filename: 'Reporte Usuarios',
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
    $("#txtId").val(model.idUsuario)
    $("#txtNombre").val(model.nombre)
    $("#txtCorreo").val(model.correo)
    $("#txtTelefono").val(model.telefono)
    $("#cboRol").val(model.idRol == 0 ? $("#cboRol option:first").val() : model.idRol)
    $("#cboEstado").val(model.esActivo)
    $("#txtFoto").val("")
    $("#imgUsuario").attr("src",model.urlFoto)

    $("#modalData").modal("show")

    const emailInput = document.getElementById('txtCorreo');
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    const validationMessage = document.getElementById('email-validation-message');

    emailInput.addEventListener('blur', function () {
        if (!emailRegex.test(emailInput.value)) {
            emailInput.classList.add('invalid');
            validationMessage.textContent = 'Ingrese una dirección de correo válida';
            validationMessage.style.color = 'red';
        } else {
            emailInput.classList.remove('invalid');
            validationMessage.textContent = 'Dirección de correo válida';
            validationMessage.style.color = 'green';
        }
    });
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
    model["idUsuario"] = parseInt($("#txtId").val())
    model["nombre"] = $("#txtNombre").val()
    model["correo"] = $("#txtCorreo").val()
    model["telefono"] = $("#txtTelefono").val()
    model["idRol"] = $("#cboRol").val()
    model["esActivo"] = $("#cboEstado").val()

    const inputFoto = document.getElementById("txtFoto")

    const formData = new FormData();

    formData.append("foto", inputFoto.files[0])
    formData.append("model", JSON.stringify(model))

    $("#modalData").find("div.modal-content").LoadingOverlay("show")
    if (model.idUsuario == 0) {
        fetch("/Usuario/Crear", {
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
                    swal("Listo!", "El usuario fue creado", "success")
                } else {
                    swal("Lo siento!", responseJson.mensaje, "error")
                }
            })
    } else {
        fetch("/Usuario/Editar", {
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
                    swal("Listo!", "El usuario ha sido modificado", "success")
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
        text: `Eliminar al usuario "${dt.nombre}"`,
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

                fetch(`/Usuario/Eliminar?idUsuario=${dt.idUsuario}`, {
                    method: "DELETE"
                })
                    .then(response => {
                        $(".showSweetAlert").LoadingOverlay("hide");
                        return response.ok ? response.json() : Promise.reject(response);
                    })
                    .then(responseJson => {

                        if (responseJson.estado) {

                            tablaData.row(fila).remove().draw()

                            swal("Listo!", "El usuario fue eliminado", "success")

                        } else {

                            swal("Lo siento!", responseJson.mensaje, "error")
                        }
                    })
            }
        }
    )
})