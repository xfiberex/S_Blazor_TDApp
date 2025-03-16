
// Funcion para detectar el cambio de tamaño de la ventana del navegador
window.addResizeListener = function (dotnetHelper) {
    window.addEventListener('resize', () => {
        // Invoca el método OnResize en el componente Blazor y le pasa el ancho actual de la ventana
        dotnetHelper.invokeMethodAsync('OnResize', window.innerWidth);
    });
}