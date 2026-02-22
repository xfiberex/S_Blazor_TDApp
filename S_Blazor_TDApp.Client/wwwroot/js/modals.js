// Para registrar procesos utilizando modales.

const cleanupModalArtifacts = () => {
    const hasVisibleModals = document.querySelectorAll('.modal.show').length > 0;
    if (!hasVisibleModals) {
        document.querySelectorAll('.modal-backdrop').forEach(backdrop => backdrop.remove());
        document.body.classList.remove('modal-open');
        document.body.style.removeProperty('padding-right');
        document.body.style.removeProperty('overflow');
    }
};

window.showModal = (modalId) => {
    const modalElement = document.getElementById(modalId);
    if (!modalElement) return;

    cleanupModalArtifacts();

    const modal = bootstrap.Modal.getOrCreateInstance(modalElement);
    modal.show();
};

window.hideModalAsync = (modalId) => {
    const modalElement = document.getElementById(modalId);
    if (!modalElement) {
        cleanupModalArtifacts();
        return Promise.resolve();
    }

    const modal = bootstrap.Modal.getOrCreateInstance(modalElement);

    return new Promise((resolve) => {
        let resolved = false;

        const finish = () => {
            if (resolved) return;
            resolved = true;
            cleanupModalArtifacts();
            resolve();
        };

        modalElement.addEventListener('hidden.bs.modal', finish, { once: true });
        modal.hide();

        setTimeout(finish, 500);
    });
};

window.hideModal = (modalId) => window.hideModalAsync(modalId);