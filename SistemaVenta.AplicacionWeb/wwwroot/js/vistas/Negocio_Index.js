$(document).ready(function () {
    $(".card-body").LoadingOverlay("show")

    fetch("/Negocio/Obtener")
        .then(response => {
            $(".card-body").LoadingOverlay("hide")
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                const d = responseJson.objeto

                $("#txtNumeroDocumento").val(d.numeroDocumento)
                $("#txtRazonSocial").val(d.nombre)
                $("#txtCorreo").val(d.correo)
                $("#txtDireccion").val(d.direccion)
                $("#txTelefono").val(d.telefono)
                $("#txtImpuesto").val(d.porcentajeImpuesto)
                $("#txtSimboloMoneda").val(d.simboloMoneda)
                $("#imgLogo").attr("src", d.urlLogo)
            } else {
                swal("Lo siento!", responseJson.mensaje, "error")

            }
        })
})

$("#btnGuardarCambios").click(function () {

    const inputs = $("input.input-validar").serializeArray();
    const inputs_without_data = inputs.filter((item) => item.value.trim() == "")

    if (inputs_without_data.length > 0) {
        const msg = `Debe completar el campo : "${inputs_without_data[0].name}"`;
        toastr.warning("", msg)
        $(`input[name = "${inputs_without_data[0].name}"]`).focus()
        return;
    }

    const model = {
        numeroDocumento : $("#txtNumeroDocumento").val(),
        nombre : $("#txtRazonSocial").val(),
        correo : $("#txtCorreo").val(),
        direccion : $("#txtDireccion").val(),
        telefono : $("#txTelefono").val(),
        porcentajeImpuesto : $("#txtImpuesto").val(),
        simboloMoneda : $("#txtSimboloMoneda").val()
    }

    const inputLogo = document.getElementById("txtLogo")
    const formData = new FormData()

    formData.append("logo", inputLogo.files[0])
    formData.append("model", JSON.stringify(model))

    $(".card-body").LoadingOverlay("show")

    fetch("/Negocio/GuardarCambios",{
        method: "POST",
        body: formData
    })
        .then(response => {
            $(".card-body").LoadingOverlay("hide")
            return response.ok ? response.json() : Promise.reject(response);
        })
        .then(responseJson => {
            if (responseJson.estado) {
                const d = responseJson.objeto

                $("#imgLogo").attr("src", d.urlLogo)

            } else {
                swal("Lo siento!", responseJson.mensaje, "error")
            }
        })
})