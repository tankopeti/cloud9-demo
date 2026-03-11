window.AppModal = {
  get(id) {
    return document.getElementById(id);
  },

  instance(id) {
    const el = this.get(id);
    if (!el) return null;

    return bootstrap.Modal.getOrCreateInstance(el);
  },

  show(id) {
    const inst = this.instance(id);
    if (inst) inst.show();
  },

  hide(id) {
    const inst = this.instance(id);
    if (inst) inst.hide();
  },

  onShown(id, handler) {
    const el = this.get(id);
    if (!el) return;

    el.addEventListener('shown.bs.modal', handler);
  },

  onHidden(id, handler) {
    const el = this.get(id);
    if (!el) return;

    el.addEventListener('hidden.bs.modal', handler);
  }
};