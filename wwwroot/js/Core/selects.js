window.AppSelects = {

  normalize(items) {
    return (items || []).map(x => ({
      id: String(
        x.id ??
        x.value ??
        x.siteId ??
        x.partnerId ??
        x.employeeId ??
        ''
      ),
      text: String(
        x.text ??
        x.name ??
        x.label ??
        x.siteName ??
        x.fullName ??
        ''
      )
    })).filter(x => x.id);
  },

  async load(selectEl, url, selectedId = '', placeholder = '-- Válasszon --') {

    if (!selectEl) return;

    selectEl.disabled = true;
    selectEl.innerHTML = '<option>Betöltés...</option>';

    try {

      const data = await AppApi.get(url);
      const items = this.normalize(data);

      selectEl.innerHTML =
        `<option value="">${placeholder}</option>` +
        items.map(x =>
          `<option value="${x.id}">${x.text}</option>`
        ).join('');

      selectEl.value = selectedId || '';

    } catch (err) {

      console.error('[AppSelects.load]', err);

      selectEl.innerHTML =
        '<option value="">Nem sikerült betölteni</option>';

    }

    selectEl.disabled = false;

  }

};