// wwwroot/js/Site/site-employees.js
// Modal: #siteEmployeesModal
// Purpose: assign Employees to a Site
//
// Expected API (recommended):
//   GET  /api/site/{siteId}/employees        -> { employeeIds: number[] }
//   PUT  /api/site/{siteId}/employees        body: { employeeIds: number[] }
//
// TomSelect remote search endpoint (you can adjust):
//   GET /api/employee/index?page=1&pageSize=20&searchTerm=...&quickFilter=all
//   -> expects { items: [{ employeeId, fullName, ...}] }

(() => {
  "use strict";

  const els = {
    modal: null,
    siteName: null,
    picker: null,
    addBtn: null,
    list: null,
    empty: null,
    error: null,
    saveBtn: null
  };

  const state = {
    siteId: null,
    siteName: "",
    isLoading: false,
    isSaving: false,
    selected: [] // [{ employeeId, fullName, partnerName }]
  };

  let pickerTs = null;

  // -----------------------------
  // Helpers
  // -----------------------------
  function qs(sel) { return document.querySelector(sel); }

  function escapeHtml(str) {
    if (str === null || str === undefined) return "";
    return String(str).replace(/[&<>"']/g, (m) => ({
      "&": "&amp;",
      "<": "&lt;",
      ">": "&gt;",
      '"': "&quot;",
      "'": "&#39;"
    }[m]));
  }

  function setError(msg) {
    if (!els.error) return;
    if (!msg) {
      els.error.classList.add("d-none");
      els.error.textContent = "";
      return;
    }
    els.error.classList.remove("d-none");
    els.error.textContent = msg;
  }

  function setSaving(isSaving) {
    state.isSaving = isSaving;
    if (els.saveBtn) els.saveBtn.disabled = isSaving;
    if (els.addBtn) els.addBtn.disabled = isSaving;
    if (pickerTs) pickerTs.disable();
    if (!isSaving && pickerTs) pickerTs.enable();
  }

  function updateEmpty() {
    if (!els.empty) return;
    els.empty.classList.toggle("d-none", state.selected.length > 0);
  }

  function renderList() {
    if (!els.list) return;

    if (state.selected.length === 0) {
      els.list.innerHTML = "";
      updateEmpty();
      return;
    }

    const html = state.selected
      .slice()
      .sort((a, b) => (a.fullName || "").localeCompare(b.fullName || ""))
.map(x => `
  <li class="list-group-item d-flex justify-content-between align-items-center">
    <div>
      <i class="bi bi-person me-2 text-muted"></i>
      <strong>${escapeHtml(x.fullName || ("#" + x.employeeId))}</strong>
      <span class="text-muted ms-2">#${x.employeeId}</span>
      ${x.partnerName ? `<div class="small text-muted ms-4"><i class="bi bi-building me-1"></i>${escapeHtml(x.partnerName)}</div>` : ""}
    </div>
          <button type="button"
                  class="btn btn-outline-danger btn-sm site-employee-remove"
                  data-employee-id="${x.employeeId}">
            <i class="bi bi-x-lg"></i>
          </button>
        </li>
      `).join("");

    els.list.innerHTML = html;
    updateEmpty();
  }

  function selectedHas(id) {
    return state.selected.some(x => x.employeeId === id);
  }

  function addSelected(emp) {
    if (!emp || !emp.employeeId) return;
    const id = parseInt(emp.employeeId, 10);
    if (Number.isNaN(id) || id <= 0) return;
    if (selectedHas(id)) return;

state.selected.push({
  employeeId: id,
  fullName: emp.fullName || emp.text || emp.name || "",
  partnerName: emp.partnerName || ""
});

    renderList();
  }

  function removeSelected(employeeId) {
    const id = parseInt(employeeId, 10);
    if (Number.isNaN(id)) return;
    state.selected = state.selected.filter(x => x.employeeId !== id);
    renderList();
  }

  // -----------------------------
  // API
  // -----------------------------
  async function apiGetAssignedEmployees(siteId) {
    const res = await fetch(`/api/sites/${siteId}/employees`, {
      headers: { "Accept": "application/json" }
    });

    if (!res.ok) {
      throw new Error(`GET /api/site/${siteId}/employees failed (${res.status})`);
    }

    return await res.json(); // { employeeIds: [...] } (recommended)
  }

  async function apiPutAssignedEmployees(siteId, employeeIds) {
    const res = await fetch(`/api/sites/${siteId}/employees`, {
      method: "PUT",
      headers: {
        "Accept": "application/json",
        "Content-Type": "application/json"
      },
      body: JSON.stringify({ employeeIds })
    });

    if (!res.ok) {
      let msg = `Mentés sikertelen (${res.status})`;
      try {
        const j = await res.json();
        if (j?.message) msg = j.message;
      } catch { /* ignore */ }
      throw new Error(msg);
    }

    return await res.json().catch(() => ({}));
  }

  async function apiLookupEmployeeById(employeeId) {
    // so the list can show names even if GET returns only IDs
    const res = await fetch(`/api/employee/${employeeId}`, {
      headers: { "Accept": "application/json" }
    });
    if (!res.ok) return { employeeId, fullName: `#${employeeId}`, partnerName: "" };
    const dto = await res.json();
return {
  employeeId: dto.employeeId ?? employeeId,
  fullName: dto.fullName ?? `${dto.lastName ?? ""} ${dto.firstName ?? ""}`.trim(),
  partnerName:
    dto.partnerName ??
    dto.companyName ??
    dto.partner?.companyName ??
    dto.partner?.name ??
    ""
};
  }

  // -----------------------------
  // TomSelect (remote search)
  // -----------------------------
  function ensureTomSelect() {
    if (!els.picker) return;

    // Already initialized
    if (pickerTs) return;

    if (!window.TomSelect) {
      console.warn("TomSelect not found. Include TomSelect before this script.");
      return;
    }

    pickerTs = new TomSelect(els.picker, {
      valueField: "employeeId",
      labelField: "fullName",
      searchField: ["fullName", "partnerName"],
      create: false,
      maxItems: 1,
      preload: false,
      placeholder: "Dolgozó keresése név/email/telefon/TAJ/adószám alapján...",
      load: async (query, callback) => {
        try {
          const q = (query || "").trim();
          if (!q || q.length < 2) return callback();

          const url = `/api/employee/index?page=1&pageSize=20&quickFilter=all&searchTerm=${encodeURIComponent(q)}`;
          const res = await fetch(url, { headers: { "Accept": "application/json" } });
          if (!res.ok) return callback();

          const data = await res.json();
const items = (data.items || []).map(x => ({
  employeeId: x.employeeId,
  fullName: x.fullName || "",
  partnerName:
    x.partnerName ??
    x.companyName ??
    x.partner?.companyName ??
    x.partner?.name ??
    ""
}));

          callback(items);
        } catch (e) {
          console.error("TomSelect load failed", e);
          callback();
        }
      },
      render: {
option: (item, escape) =>
  `<div>
     <div><strong>${escape(item.fullName || "")}</strong></div>
     <div class="text-muted small">
       #${escape(String(item.employeeId))}
       ${item.partnerName ? ` · <i class="bi bi-building me-1"></i>${escape(item.partnerName)}` : ""}
     </div>
   </div>`,
item: (item, escape) =>
  `<div>
     ${escape(item.fullName || "")}
     ${item.partnerName ? ` <span class="text-muted">(${escape(item.partnerName)})</span>` : ""}
   </div>`
      }
    });
  }

  function getPickedEmployee() {
    if (!pickerTs) return null;
    const id = parseInt(pickerTs.getValue(), 10);
    if (Number.isNaN(id) || id <= 0) return null;

    // TomSelect stores options
    const opt = pickerTs.options?.[id];
return {
  employeeId: id,
  fullName: opt?.fullName || opt?.text || "",
  partnerName: opt?.partnerName || ""
};
  }

  function clearPicker() {
    if (!pickerTs) return;
    pickerTs.clear(true);
  }

  // -----------------------------
  // Wiring
  // -----------------------------
  function wireListRemove() {
    if (!els.list) return;

    els.list.addEventListener("click", (e) => {
      const btn = e.target.closest(".site-employee-remove");
      if (!btn) return;
      e.preventDefault();
      removeSelected(btn.dataset.employeeId);
    });
  }

  function wireAdd() {
    if (!els.addBtn) return;

    els.addBtn.addEventListener("click", (e) => {
      e.preventDefault();
      setError(null);

      const emp = getPickedEmployee();
      if (!emp) {
        setError("Válassz ki egy dolgozót a listából.");
        return;
      }

      addSelected(emp);
      clearPicker();
    });
  }

  function wireSave() {
    if (!els.saveBtn) return;

    els.saveBtn.addEventListener("click", async (e) => {
      e.preventDefault();
      setError(null);

      if (!state.siteId) {
        setError("Hiányzik a telephely azonosító.");
        return;
      }

      try {
        setSaving(true);

        const ids = state.selected.map(x => x.employeeId);

        await apiPutAssignedEmployees(state.siteId, ids);

        // optional: reload your sites index / employees index if you want
        window.reloadSitesIndex?.();
        window.reloadEmployeesIndex?.();

        // close modal
        const modal = window.bootstrap?.Modal.getOrCreateInstance(els.modal);
        modal?.hide();
      } catch (err) {
        console.error(err);
        setError(err?.message || "Mentés sikertelen.");
      } finally {
        setSaving(false);
      }
    });
  }

  function wireModalResetOnHide() {
    if (!els.modal) return;

    els.modal.addEventListener("hidden.bs.modal", () => {
      setError(null);
      state.siteId = null;
      state.siteName = "";
      state.selected = [];
      renderList();
      if (els.siteName) els.siteName.textContent = "";
      if (pickerTs) clearPicker();
    });
  }

  // -----------------------------
  // Public API: open modal
  // -----------------------------
  async function openSiteEmployeesModal(siteId, siteName) {
    state.siteId = parseInt(siteId, 10);
    state.siteName = siteName || "";

    if (Number.isNaN(state.siteId) || state.siteId <= 0) {
      alert("Hibás telephely ID.");
      return;
    }

    setError(null);
    state.selected = [];
    renderList();

    if (els.siteName) els.siteName.textContent = state.siteName || `#${state.siteId}`;

    ensureTomSelect();

    // show modal first (fast UI), then load data
    const modal = window.bootstrap?.Modal.getOrCreateInstance(els.modal);
    modal?.show();

    try {
      setSaving(true);

      const data = await apiGetAssignedEmployees(state.siteId);

      // Support both shapes:
      //  - { employeeIds: [...] }
      //  - { items: [{ employeeId, fullName }] }   (if you implement it that way)
      if (Array.isArray(data?.items)) {
state.selected = data.items.map(x => ({
  employeeId: x.employeeId,
  fullName: x.fullName || "",
  partnerName: x.partnerName || ""
}));
        renderList();
        return;
      }

      const ids = Array.isArray(data?.employeeIds) ? data.employeeIds : [];
      if (ids.length === 0) {
        state.selected = [];
        renderList();
        return;
      }

      // Load names so the list is nice
      const lookedUp = await Promise.all(ids.map(apiLookupEmployeeById));
state.selected = lookedUp.map(x => ({
  employeeId: x.employeeId,
  fullName: x.fullName || `#${x.employeeId}`,
  partnerName: x.partnerName || ""
}));

      renderList();
    } catch (err) {
      console.error(err);
      setError("Nem sikerült betölteni a telephely dolgozóit.");
    } finally {
      setSaving(false);
    }
  }

  // -----------------------------
  // Init
  // -----------------------------
  function init() {
    els.modal = document.getElementById("siteEmployeesModal");
    if (!els.modal) return;

    els.siteName = qs("#site-employees-site-name");
    els.picker = qs("#site-employee-picker");
    els.addBtn = qs("#site-employee-add");
    els.list = qs("#site-employees-list");
    els.empty = qs("#site-employees-empty");
    els.error = qs("#site-employees-error");
    els.saveBtn = qs("#site-employees-save");

    wireListRemove();
    wireAdd();
    wireSave();
    wireModalResetOnHide();

    // Expose to window for dropdown click handler
    window.openSiteEmployeesModal = openSiteEmployeesModal;
  }

  document.addEventListener("DOMContentLoaded", init);
})();