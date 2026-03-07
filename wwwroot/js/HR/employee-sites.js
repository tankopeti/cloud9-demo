// wwwroot/js/HR/employee-sites.js
(() => {
  "use strict";

  console.log("[employee-sites] loaded v=2026-03-05-TOMSELECT-SINGLE-2");

  const API = {
    searchSites: (q) =>
      `/api/SitesIndex?pageNumber=1&pageSize=50&search=${encodeURIComponent(q || "")}`,
    siteById: (id) => `/api/SitesIndex/${id}`,

    // backend: GET/PUT /api/employee/{id}/sites
    employeeSites: (id) => `/api/employee/${id}/sites`
  };

  let modalEl;
  let modalInstance;

  let pickerEl;
  let addBtn;
  let listEl;
  let emptyEl;
  let saveBtn;
  let errorEl;
  let nameEl;

  let tom = null;
  let currentEmployeeId = null;

  // id -> name
  const selected = new Map();

  function byId(id) { return document.getElementById(id); }

  function showError(msg) {
    if (!errorEl) return;
    if (!msg) {
      errorEl.classList.add("d-none");
      errorEl.textContent = "";
      return;
    }
    errorEl.textContent = msg;
    errorEl.classList.remove("d-none");
  }

  async function fetchJson(url, options = {}) {
    const res = await fetch(url, options);
    if (!res.ok) throw new Error(`${res.status} ${res.statusText}`);
    return await res.json();
  }

  function renderList() {
    if (!listEl || !emptyEl) return;

    listEl.innerHTML = "";

    if (selected.size === 0) {
      emptyEl.classList.remove("d-none");
      return;
    }

    emptyEl.classList.add("d-none");

    [...selected.entries()]
      .map(([id, name]) => ({ id, name }))
      .sort((a, b) => (a.name || "").localeCompare(b.name || "", "hu"))
      .forEach(({ id, name }) => {
        const li = document.createElement("li");
        li.className = "list-group-item d-flex justify-content-between align-items-center";

        const label = document.createElement("span");
        label.textContent = name || `#${id}`;

        const btn = document.createElement("button");
        btn.type = "button";
        btn.className = "btn btn-sm btn-outline-danger";
        btn.title = "Eltávolítás";
        btn.innerHTML = `<i class="bi bi-x"></i>`;
        btn.addEventListener("click", () => {
          selected.delete(id);
          renderList();
        });

        li.appendChild(label);
        li.appendChild(btn);
        listEl.appendChild(li);
      });
  }

  function addSite() {
    showError(null);

    if (!tom) {
      showError("A telephely kereső nincs inicializálva.");
      return;
    }

    const value = tom.getValue();
    if (!value) {
      showError("Válassz telephelyet.");
      return;
    }

    const siteId = parseInt(value, 10);
    if (!Number.isFinite(siteId)) {
      showError("Érvénytelen telephely azonosító.");
      return;
    }

    if (selected.has(siteId)) {
      showError("Ez a telephely már hozzá van adva.");
      return;
    }

    const siteName = tom.options?.[value]?.text || `#${siteId}`;

    selected.set(siteId, siteName);
    renderList();

    // ✅ ne nyíljon újra:
    tom.clear(true);
    tom.close();
  }

  async function loadEmployeeSites(employeeId) {
    selected.clear();

    const data = await fetchJson(API.employeeSites(employeeId));
    const ids = Array.isArray(data?.siteIds)
      ? data.siteIds
      : (Array.isArray(data) ? data : []);

    // pár darab -> oké a per-id lekérés
    for (const id of ids) {
      try {
        const s = await fetchJson(API.siteById(id));
        selected.set(parseInt(s.siteId, 10), s.siteName || `#${id}`);
      } catch {
        selected.set(parseInt(id, 10), `#${id}`);
      }
    }

    renderList();
  }

async function saveEmployeeSites() {
  try {
    showError(null);

    const ids = [...selected.keys()];

    await fetchJson(API.employeeSites(currentEmployeeId), {
      method: "PUT",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ siteIds: ids })
    });

    modalInstance?.hide();

    // ✅ AJAX frissítés (nem full reload)
    window.reloadEmployeesIndex?.();
  } catch (e) {
    console.error(e);
    showError("Mentés sikertelen.");
  }
}

  function initTomSelect() {
    if (typeof TomSelect === "undefined") {
      showError("TomSelect nincs betöltve (JS/CSS).");
      return;
    }

    // ha újranyitáskor újrainit jönne, ne duplikáljunk
    if (tom) return;

    tom = new TomSelect(pickerEl, {
      placeholder: "Telephely keresése...",
      valueField: "value",
      labelField: "text",
      searchField: "text",
      create: false,
      preload: false,
      maxOptions: 50,
      loadThrottle: 250,

      load: async (query, callback) => {
        try {
          const data = await fetchJson(API.searchSites(query));
          const items = (data || []).map(s => ({
            value: String(s.siteId),
            text: String(s.siteName ?? "")
          }));
          callback(items);
        } catch (err) {
          console.error(err);
          callback();
        }
      }
    });

    // Enter -> Hozzáadás (kényelmes UX)
    tom.on("keydown", (e) => {
      if (e.key === "Enter") {
        e.preventDefault();
        addSite();
      }
    });
  }

  function initDom() {
    modalEl = byId("employeeSitesModal");

    pickerEl = byId("employee-site-picker");
    addBtn = byId("employee-site-add");
    listEl = byId("employee-sites-list");
    emptyEl = byId("employee-sites-empty");
    saveBtn = byId("employee-sites-save");
    errorEl = byId("employee-sites-error");
    nameEl = byId("employee-sites-employee-name");

    if (!modalEl || !pickerEl || !addBtn || !listEl || !emptyEl || !saveBtn) {
      console.warn("[employee-sites] missing modal elements");
      return false;
    }

    initTomSelect();

    addBtn.addEventListener("click", (e) => {
      e.preventDefault();
      addSite();
    });

    saveBtn.addEventListener("click", (e) => {
      e.preventDefault();
      saveEmployeeSites();
    });

    modalEl.addEventListener("hidden.bs.modal", () => {
      selected.clear();
      renderList();
      showError(null);
      try {
        tom?.clear(true);
        tom?.close();
      } catch { /* ignore */ }
      currentEmployeeId = null;
    });

    return true;
  }

  // ✅ global entry point: employeesIndex.js hívja
  window.openEmployeeSitesModal = async (employeeId, employeeName) => {
    if (!modalEl) {
      const ok = initDom();
      if (!ok) return;
    }

    currentEmployeeId = employeeId;
    if (nameEl) nameEl.textContent = employeeName || "";

    modalInstance = bootstrap.Modal.getOrCreateInstance(modalEl, { focus: false });
    modalInstance.show();

    try {
      await loadEmployeeSites(employeeId);
    } catch (e) {
      console.error(e);
      showError("Betöltés sikertelen.");
    }
  };
})();