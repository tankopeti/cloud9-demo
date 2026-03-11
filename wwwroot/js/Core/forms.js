window.AppForms = {

  setSubmitting(button, submitting) {

    if (!button) return;

    button.disabled = submitting;

    button.dataset._origText =
      button.dataset._origText || button.innerHTML;

    button.innerHTML = submitting
      ? '<span class="spinner-border spinner-border-sm me-2"></span>Mentés...'
      : button.dataset._origText;
  },

  toInt(value) {
    const n = parseInt(value);
    return isFinite(n) ? n : null;
  },

  getCsrfToken(formEl) {
    const el = formEl?.querySelector('input[name="__RequestVerificationToken"]');
    return el ? el.value : '';
  },

  getFormData(formEl) {
    const fd = new FormData(formEl);
    const obj = {};

    fd.forEach((value, key) => {
      obj[key] = value;
    });

    return obj;
  },

  reset(formEl) {
    if (!formEl) return;

    try {
      formEl.reset();
    } catch {}

    formEl.classList.remove('was-validated');
  },

  qs(root, selector) {
    return root ? root.querySelector(selector) : null;
  },

  qsa(root, selector) {
    return root ? Array.from(root.querySelectorAll(selector)) : [];
  },

  combineDateTime(dateStr, timeStr) {

    const d = String(dateStr || '').trim();
    if (!d) return null;

    const t = String(timeStr || '').trim();
    if (!t) return d;

    if (/^\d{2}:\d{2}$/.test(t))
      return d + 'T' + t + ':00';

    if (/^\d{2}:\d{2}:\d{2}$/.test(t))
      return d + 'T' + t;

    return d;
  }

};