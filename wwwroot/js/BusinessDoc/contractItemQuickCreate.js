// wwwroot/js/BusinessDoc/contractItemQuickCreate.js
(function () {

  document.addEventListener('DOMContentLoaded', () => {
    const form = document.getElementById('createContractForm');
    if (!form) return;

    const tenantId = Number(form.querySelector('input[name="TenantId"]')?.value || 1);

    const itemSelectEl = document.getElementById('lineItemSelect');
    if (!itemSelectEl) return;

    // ---- Quick create modal refs
    const qcModalEl = document.getElementById('quickCreateItemModal');
    const qcModal = qcModalEl ? bootstrap.Modal.getOrCreateInstance(qcModalEl) : null;

    const qcTenantId = document.getElementById('qcTenantId');
    const qcCode = document.getElementById('qcCode');
    const qcName = document.getElementById('qcName');
    const qcDescription = document.getElementById('qcDescription');
    const qcItemTypeId = document.getElementById('qcItemTypeId');
    const qcSalesPrice = document.getElementById('qcSalesPrice');
    const qcIsStockManaged = document.getElementById('qcIsStockManaged');
    const qcIsManufactured = document.getElementById('qcIsManufactured');
    const qcSaveBtn = document.getElementById('qcSaveItemBtn');

    if (!qcModal || !qcSaveBtn) {
      console.warn("QuickCreateItem modal not found.");
      return;
    }

    qcTenantId.value = tenantId;

    // ---- init TomSelect on item dropdown
    // elvárás: TomSelect már be van húzva a layoutban
    const ts = new TomSelect(itemSelectEl, {
      valueField: 'itemId',
      labelField: 'label',
      searchField: ['code', 'name'],
      preload: true,
      create: (input) => {
        // "create new" flow: nyisd meg a modalt, előtöltéssel
        openQuickCreate(input);
        // TomSelect create callback-nak vissza kell adni valamit, de mi async vagyunk,
        // ezért null-t adunk és majd később manuálisan adjuk hozzá.
        return null;
      },
      load: async (query, callback) => {
        try {
          const url = `/api/items/lookup?tenantId=${tenantId}&q=${encodeURIComponent(query || '')}`;
          const res = await fetch(url, { headers: { 'Accept': 'application/json' } });
          if (!res.ok) return callback();

          const data = await res.json();
          const items = (data.items || []).map(it => ({
            itemId: it.itemId,
            code: it.code,
            name: it.name,
            label: `${it.code} - ${it.name}`,
            vatRate: it.vatRate ?? null
          }));

          callback(items);
        } catch (e) {
          console.error(e);
          callback();
        }
      },
      render: {
        option: (item, escape) => {
          return `<div>
            <div class="fw-medium">${escape(item.code)} - ${escape(item.name)}</div>
          </div>`;
        }
      },
      onItemAdd: (value, item) => {
        // ha akarod: beállíthatod a VAT%-ot az itemhez kötötten
        const vatInput = document.getElementById('lineVatRate');
        const opt = ts.options[value];
        if (vatInput && opt?.vatRate != null) {
          vatInput.value = opt.vatRate;
        }
      }
    });

    // ---- Quick create open
    function openQuickCreate(typedText) {
      // nagyon egyszerű előtöltés:
      qcCode.value = (typedText || '').trim().toUpperCase().slice(0, 20);
      qcName.value = (typedText || '').trim().slice(0, 200);
      qcDescription.value = '';
      qcSalesPrice.value = '';
      qcIsStockManaged.checked = true;
      qcIsManufactured.checked = false;

      // ItemType lista: ha üres, próbáljuk betölteni
      if (!qcItemTypeId.options.length) {
        loadItemTypes().finally(() => qcModal.show());
      } else {
        qcModal.show();
      }
    }

    async function loadItemTypes() {
      // készíts egy endpointot, vagy tedd viewdata-ból a selectbe
      const res = await fetch(`/api/itemtypes/lookup?tenantId=${tenantId}`, { headers: { 'Accept': 'application/json' } });
      if (!res.ok) return;

      const data = await res.json();
      qcItemTypeId.innerHTML = '';
      (data.itemTypes || []).forEach(t => {
        const opt = document.createElement('option');
        opt.value = t.itemTypeId;
        opt.textContent = t.name;
        qcItemTypeId.appendChild(opt);
      });
    }

    // ---- Save quick created item
    qcSaveBtn.addEventListener('click', async () => {

      const dto = {
        tenantId: tenantId,
        code: (qcCode.value || '').trim(),
        name: (qcName.value || '').trim(),
        description: (qcDescription.value || '').trim() || null,
        itemTypeId: parseInt(qcItemTypeId.value || '0', 10),
        isStockManaged: !!qcIsStockManaged.checked,
        isManufactured: !!qcIsManufactured.checked,
        defaultSalesPrice: qcSalesPrice.value ? parseFloat(qcSalesPrice.value) : null,
        isActive: true
      };

      if (!dto.code || !dto.name || !dto.itemTypeId) {
        alert('Kód, név és típus kötelező.');
        return;
      }

      setBtnLoading(qcSaveBtn, true);

      try {
        const res = await fetch('/api/items/create', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': getAntiForgeryToken()
          },
          body: JSON.stringify(dto)
        });

        if (!res.ok) {
          const err = await res.text();
          console.error(err);
          alert('Nem sikerült létrehozni az Itemet.');
          setBtnLoading(qcSaveBtn, false);
          return;
        }

        const created = await res.json(); // { itemId, code, name, vatRate? }

        // 1) add to TomSelect options
        ts.addOption({
          itemId: created.itemId,
          code: created.code,
          name: created.name,
          label: `${created.code} - ${created.name}`,
          vatRate: created.vatRate ?? null
        });

        // 2) select it
        ts.setValue(created.itemId);

        // 3) close modal
        qcModal.hide();

        // 4) optionally clear fields
        qcCode.value = '';
        qcName.value = '';
        qcDescription.value = '';
        qcSalesPrice.value = '';

      } catch (e) {
        console.error(e);
        alert('Váratlan hiba az Item létrehozásakor.');
      }

      setBtnLoading(qcSaveBtn, false);
    });

    function setBtnLoading(btn, on) {
      if (!btn) return;
      if (on) {
        btn.dataset.originalText = btn.innerHTML;
        btn.innerHTML = `<span class="spinner-border spinner-border-sm me-2"></span>Mentés...`;
        btn.disabled = true;
      } else {
        btn.innerHTML = btn.dataset.originalText || 'Mentés';
        btn.disabled = false;
      }
    }

    function getAntiForgeryToken() {
      return document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    }
  const openQuickCreateBtn = document.getElementById('openQuickCreateItemBtn');

if (openQuickCreateBtn && qcModal) {
    openQuickCreateBtn.addEventListener('click', () => {
        // ha van már beírt szöveg a selectben, átadhatod
        const typed = itemSelectEl?.tomselect?.input?.value || '';
        openQuickCreate(typed);
    });
}

  });

})();