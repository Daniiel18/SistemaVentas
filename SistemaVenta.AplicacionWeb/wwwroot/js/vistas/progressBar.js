// Seleccionamos la barra de progreso
const progressBar = document.querySelector('.progress-bar');

// Definimos la función que aumenta el tamaño de la barra
function aumentarBarra() {
    // Obtenemos el ancho actual de la barra
    const currentWidth = parseInt(progressBar.style.width) || 0;

    // Definimos el ancho que deseamos agregar en cada intervalo
    const increment = 1;

    // Definimos el ancho máximo que deseamos para la barra
    const maxWidth = 100;

    // Verificamos que el ancho actual no supere el máximo
    if (currentWidth < maxWidth) {
        // Aumentamos el ancho de la barra por el incremento definido
        progressBar.style.width = `${currentWidth + increment}%`;
    }
}

// Configuramos el intervalo para llamar a la función cada 5 segundos
setInterval(aumentarBarra, 1000);
