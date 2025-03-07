// Para registrar procesos utilizando modales.

window.showModal = (modalId) => {
    const modalElement = document.getElementById(modalId);
    const modal = new bootstrap.Modal(modalElement);
    modal.show();
};

window.hideModal = (modalId) => {
    const modalElement = document.getElementById(modalId);
    const modal = bootstrap.Modal.getInstance(modalElement);
    if (modal) {
        modal.hide();
    }
};