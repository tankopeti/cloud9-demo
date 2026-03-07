// wwwroot/js/HR/employee-advanced-filter.js
(() => {
  "use strict";

  const API = {
    workerTypes: "/api/employee/lookups/workertypes",
    partners: "/api/employee/lookups/partners",
    statuses: "/api/employee/lookups/employmentstatus",
    sites: "/api/employee/lookups/sites",
  };

  const ids = {
    modal: "advancedEmployeeFilterModal",

    text: "empFilterText",
    phone: "empFilterPhone",
    workerTypeId: "empFilterWorkerTypeId",
    partnerId: "empFilterPartnerId",
    statusId: "empFilterStatusId",
    siteId: "empFilterSiteId",
    activeOnly: "empFilterActiveOnly",

    applyBtn: "applyEmployeeFilterBtn",
    clearBtn: "clearEmployeeFilterBtn",
  };

  function byId(id) { return document.getElementById(id); }

  function toIntOrNull(v) {
    const s = (v ?? "").toString().trim();
    if (!s) return null;
    const n = parseInt(s, 10);
    return Number.isFinite(n) ? n : null;
  }

  function getSelectValue(id) {
    const el = byId(id);
    if (!el) return "";
    // TomSelect kompatibilis
    if (el.tomselect) return el.tomselect.getValue();
    return el.value;
  }

  function setSelectValue(id, value) {
    const el = byId(id);
    if (!el) return;
    if (el.tomselect) el.tomselect.setValue(value ?? "", true);
    else el.value = value ?? "";
  }

  function fillSelect(selectEl, data) {
    if (!selectEl) return;

    // data: [{id, name}]
    const current = selectEl.value;

    selectEl.innerHTML = "";
    const opt0 = document.createElement("option");
    opt0.value = "";
    opt0.textContent = "-- Mindegy --";
    selectEl.appendChild(opt0);

    (data || []).forEach(x => {
      const opt = document.createElement("option");
      opt.value = String(x.id);
      opt.textContent = String(x.name);
      selectEl.appendChild(opt);
    });

    selectEl.value = current;
  }

  async function fetchJson(url) {
    const res = await fetch(url, { headers: { "Accept": "application/json" } });
    if (!res.ok) throw new Error(`GET ${url} failed (${res.status})`);
    return await res.json();
  }

  function initTomSelect() {
    if (typeof TomSelect === "undefined") return;

    const idsToInit = [ids.workerTypeId, ids.partnerId, ids.statusId, ids.siteId];
    idsToInit.forEach((id) => {
      const el = byId(id);
      if (!el) return;
      if (el.tomselect) return;
      new TomSelect(el, {
        create: false,
        allowEmptyOption: true,
        plugins: ["dropdown_input"]
      });
    });
  }

  // --------------------------------------
  // Advanced filter state
  // --------------------------------------
  window.employeeAdvancedFilterState = window.employeeAdvancedFilterState || {
    filterText: "",
    filterPhone: "",
    filterWorkerTypeId: null,
    filterPartnerId: null,
    filterStatusId: null,
    filterSiteId: null,
    filterActiveOnly: true
  };

  // Ezt fogja az employeesIndex.js meghívni a buildUrl-ban
  window.getEmployeeAdvancedFilterParams = function () {
    const s = window.employeeAdvancedFilterState;
    if (!s) return "";

    const p = new URLSearchParams();

    if (s.filterText) p.set("filterText", s.filterText);
    if (s.filterPhone) p.set("filterPhone", s.filterPhone);

    if (s.filterWorkerTypeId != null) p.set("filterWorkerTypeId", String(s.filterWorkerTypeId));
    if (s.filterPartnerId != null) p.set("filterPartnerId", String(s.filterPartnerId));
    if (s.filterStatusId != null) p.set("filterStatusId", String(s.filterStatusId));
    if (s.filterSiteId != null) p.set("filterSiteId", String(s.filterSiteId));

    // bool? -> csak ha explicit van értelme küldeni; mi mindig küldjük (true/false)
    if (s.filterActiveOnly != null) p.set("filterActiveOnly", String(!!s.filterActiveOnly));

    const qs = p.toString();
    return qs ? `&${qs}` : "";
  };

  function readModalToState() {
    const s = window.employeeAdvancedFilterState;

    s.filterText = (byId(ids.text)?.value ?? "").trim();
    s.filterPhone = (byId(ids.phone)?.value ?? "").trim();

    s.filterWorkerTypeId = toIntOrNull(getSelectValue(ids.workerTypeId));
    s.filterPartnerId = toIntOrNull(getSelectValue(ids.partnerId));
    s.filterStatusId = toIntOrNull(getSelectValue(ids.statusId));
    s.filterSiteId = toIntOrNull(getSelectValue(ids.siteId));

    const activeEl = byId(ids.activeOnly);
    s.filterActiveOnly = activeEl ? !!activeEl.checked : true;
  }

  function writeStateToModal() {
    const s = window.employeeAdvancedFilterState;

    if (byId(ids.text)) byId(ids.text).value = s.filterText ?? "";
    if (byId(ids.phone)) byId(ids.phone).value = s.filterPhone ?? "";

    setSelectValue(ids.workerTypeId, s.filterWorkerTypeId ?? "");
    setSelectValue(ids.partnerId, s.filterPartnerId ?? "");
    setSelectValue(ids.statusId, s.filterStatusId ?? "");
    setSelectValue(ids.siteId, s.filterSiteId ?? "");

    if (byId(ids.activeOnly)) byId(ids.activeOnly).checked = (s.filterActiveOnly !== false);
  }

  function clearStateAndModal() {
    const s = window.employeeAdvancedFilterState;

    s.filterText = "";
    s.filterPhone = "";
    s.filterWorkerTypeId = null;
    s.filterPartnerId = null;
    s.filterStatusId = null;
    s.filterSiteId = null;
    s.filterActiveOnly = true;

    writeStateToModal();
  }

  function closeModal() {
    const modalEl = byId(ids.modal);
    if (!modalEl) return;
    const instance = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
    instance.hide();
  }

  let lookupsLoaded = false;

  async function loadLookupsOnce() {
    if (lookupsLoaded) return;

    const [workerTypes, partners, statuses, sites] = await Promise.all([
      fetchJson(API.workerTypes).catch(() => []),
      fetchJson(API.partners).catch(() => []),
      fetchJson(API.statuses).catch(() => []),
      fetchJson(API.sites).catch(() => []),
    ]);

    fillSelect(byId(ids.workerTypeId), workerTypes);
    fillSelect(byId(ids.partnerId), partners);
    fillSelect(byId(ids.statusId), statuses);
    fillSelect(byId(ids.siteId), sites);

    initTomSelect();
    lookupsLoaded = true;
  }

  function wireModalOpen() {
    const modalEl = byId(ids.modal);
    if (!modalEl) return;

    modalEl.addEventListener("show.bs.modal", async () => {
      try {
        await loadLookupsOnce();
        writeStateToModal();
      } catch (e) {
        console.error("Employee advanced filter lookups failed:", e);
      }
    });
  }

  function wireButtons() {
    const applyBtn = byId(ids.applyBtn);
    const clearBtn = byId(ids.clearBtn);

    if (applyBtn) {
      applyBtn.addEventListener("click", () => {
        readModalToState();
        closeModal();
        window.reloadEmployeesIndex?.(); // a te meglévő reload
      });
    }

    if (clearBtn) {
      clearBtn.addEventListener("click", () => {
        clearStateAndModal();
        window.reloadEmployeesIndex?.();
      });
    }
  }

  document.addEventListener("DOMContentLoaded", () => {
    wireModalOpen();
    wireButtons();
  });

})();