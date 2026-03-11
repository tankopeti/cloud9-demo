window.AppToast = {

  show(message, type = 'info') {

    let container = document.getElementById('toastContainer');

    if (!container) {
      container = document.createElement('div');
      container.id = 'toastContainer';
      container.className = 'position-fixed bottom-0 end-0 p-3';
      container.style.zIndex = '1100';
      document.body.appendChild(container);
    }

    const el = document.createElement('div');
    el.className = 'toast text-white bg-' + type;

    el.innerHTML =
      '<div class="d-flex">' +
      '<div class="toast-body">' + message + '</div>' +
      '<button class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>' +
      '</div>';

    container.appendChild(el);

    new bootstrap.Toast(el, { delay: 3500 }).show();
  }

};