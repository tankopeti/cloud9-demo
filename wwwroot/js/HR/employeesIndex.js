// wwwroot/js/HR/employeesIndex.js
// Load-more + search + quick filter (AJAX) for HR/Employees/Index
// Endpoint: GET /api/employee/index?page=1&pageSize=50&searchTerm=...&quickFilter=all

(() => {
  "use strict";

  const state = {
    page: 1,
    pageSize: 30,
    totalRecords: 0,
    loadedCount: 0,
    searchTerm: "",
    quickFilter: "all",
    isLoading: false,
    reachedEnd: false
  };

  const els = {
    tbody: null,
    loadMoreBtn: null,
    loadMoreContainer: null,
    searchInput: null,
    quickFilterToggle: null
  };

  function qs(sel) { return document.querySelector(sel); }

  function setLoading(isLoading) {
    state.isLoading = isLoading;
    if (els.loadMoreBtn) els.loadMoreBtn.disabled = isLoading;
    if (els.loadMoreBtn) els.loadMoreBtn.innerHTML = isLoading
      ? `<span class="spinner-border spinner-border-sm me-2"></span>Betöltés...`
      : `Több betöltése`;
  }

  // ------------------------------------------------------------
  // ROW ACTIONS
  // ------------------------------------------------------------

  let rowActionsWired = false;

  function wireRowActions() {

    if (rowActionsWired) return;
    rowActionsWired = true;

    document.addEventListener("click", (e) => {

      const editBtn = e.target.closest(".editEmployeeBtn");
      if (editBtn) {
        e.preventDefault();
        const id = parseInt(editBtn.dataset.employeeId, 10);
        if (Number.isNaN(id)) return;
        window.openEditEmployee?.(id, null);
        return;
      }

      const viewBtn = e.target.closest(".viewEmployeeBtn");
      if (viewBtn) {
        e.preventDefault();
        const id = parseInt(viewBtn.dataset.employeeId, 10);
        if (Number.isNaN(id)) return;
        window.openViewEmployee?.(id);
        return;
      }

      // FILES
      const filesBtn = e.target.closest(".employeeFilesBtn");
      if (filesBtn) {
        e.preventDefault();

        const employeeId = parseInt(filesBtn.dataset.employeeId, 10);
        if (Number.isNaN(employeeId)) return;

        const employeeName = filesBtn.dataset.employeeName || "";

        if (!window.openEmployeeFilesModal) {
          console.error("employee-files.js nincs betöltve.");
          alert("A dokumentum modul nincs betöltve.");
          return;
        }

        window.openEmployeeFilesModal(employeeId, employeeName);
        return;
      }

      const historyBtn = e.target.closest(".employeeHistoryBtn");
      if (historyBtn) {
        e.preventDefault();

        const id = parseInt(historyBtn.dataset.employeeId, 10);
        if (Number.isNaN(id)) return;

        const name = historyBtn.dataset.employeeName || "";

        const nameEl = document.querySelector("#history-employee-name");
        if (nameEl) nameEl.textContent = name;

        const modalEl = document.getElementById("employeeHistoryModal");
        if (modalEl && window.bootstrap) {
          window.bootstrap.Modal.getOrCreateInstance(modalEl).show();
        }

        window.loadEmployeeHistory?.(id);
        return;
      }

      const assignSitesBtn = e.target.closest(".assignEmployeeSitesBtn");
      if (assignSitesBtn) {
        e.preventDefault();

        const id = parseInt(assignSitesBtn.dataset.employeeId, 10);
        if (Number.isNaN(id)) return;

        const name = assignSitesBtn.dataset.employeeName || "";
        window.openEmployeeSitesModal?.(id, name);
        return;
      }

      const delBtn = e.target.closest(".deleteEmployeeBtn");
      if (delBtn) {
        e.preventDefault();
        const id = parseInt(delBtn.dataset.employeeId || delBtn.dataset.id, 10);
        if (Number.isNaN(id)) return;
        window.deleteEmployee?.(id);
        return;
      }

    }, true);
  }

  // ------------------------------------------------------------
  // HELPERS
  // ------------------------------------------------------------

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

  function badge(text, cls) {
    const t = escapeHtml(text);
    return `<span class="badge ${cls} me-1 mb-1">${t}</span>`;
  }

  function renderBadges(list, emptyText = "-") {
    if (!Array.isArray(list) || list.length === 0)
      return `<span class="text-muted">${emptyText}</span>`;

    return list.slice(0, 6).map(x => badge(x, "bg-secondary")).join("") +
      (list.length > 6 ? badge(`+${list.length - 6}`, "bg-light text-dark") : "");
  }

  // ------------------------------------------------------------
  // TABLE ROW
  // ------------------------------------------------------------

  function renderRow(emp) {

    const fullName = escapeHtml(emp.fullName ?? "");

    const email = emp.email
      ? `<a href="mailto:${escapeHtml(emp.email)}">${escapeHtml(emp.email)}</a>`
      : `<span class="text-muted">-</span>`;

    const phone = emp.phoneNumber
      ? escapeHtml(emp.phoneNumber)
      : `<span class="text-muted">-</span>`;

    const workerType = emp.workerTypeName
      ? badge(emp.workerTypeName, emp.workerTypeId === 2
        ? "bg-warning text-dark"
        : "bg-info text-dark")
      : `<span class="text-muted">-</span>`;

    const partner = emp.partnerName
      ? escapeHtml(emp.partnerName)
      : `<span class="text-muted">-</span>`;

    const jobTitle = emp.jobTitleName
      ? escapeHtml(emp.jobTitleName)
      : `<span class="text-muted">-</span>`;

    const statuses = renderBadges(emp.statusNames, "-");
    const sites = renderBadges(emp.siteNames, "-");

    const isActive = emp.isActive
      ? `<span class="badge bg-success">Aktív</span>`
      : `<span class="badge bg-secondary">Inaktív</span>`;

    return `
<tr data-employee-id="${emp.employeeId}">
<td>${fullName}</td>
<td>${email}</td>
<td>${phone}</td>
<td>${workerType}</td>
<td>${partner}</td>
<td>${jobTitle}</td>
<td>${statuses}</td>
<td>${sites}</td>
<td>${isActive}</td>

<td class="text-center">

<div class="btn-group btn-group-sm">

<button type="button"
class="btn btn-outline-secondary viewEmployeeBtn"
data-employee-id="${emp.employeeId}"
title="Megnéz">
<i class="bi bi-eye"></i>
</button>

<div class="dropdown">

<button class="btn btn-outline-secondary dropdown-toggle btn-sm"
data-bs-toggle="dropdown">
<i class="bi bi-three-dots-vertical"></i>
</button>

<ul class="dropdown-menu dropdown-menu-end">

<li>
<a class="dropdown-item employeeHistoryBtn"
data-employee-id="${emp.employeeId}"
data-employee-name="${escapeHtml(emp.fullName ?? "")}">
<i class="bi bi-clock-history me-2"></i>
Előzmények
</a>
</li>

<li>
<a class="dropdown-item assignEmployeeSitesBtn"
data-employee-id="${emp.employeeId}"
data-employee-name="${escapeHtml(emp.fullName ?? "")}">
<i class="bi bi-diagram-3 me-2"></i>
Telephelyek összerendelése
</a>
</li>

<li>
<a class="dropdown-item employeeFilesBtn"
data-employee-id="${emp.employeeId}"
data-employee-name="${escapeHtml(emp.fullName ?? "")}">
<i class="bi bi-paperclip me-2"></i>
Fájlok kezelése
</a>
</li>

<li><hr class="dropdown-divider"></li>

<li>
<a class="dropdown-item editEmployeeBtn"
data-employee-id="${emp.employeeId}">
<i class="bi bi-pencil-square me-2"></i>
Szerkesztés
</a>
</li>

<li><hr class="dropdown-divider"></li>

<li>
<a class="dropdown-item text-danger deleteEmployeeBtn"
data-employee-id="${emp.employeeId}"
data-employee-name="${escapeHtml(emp.fullName ?? "")}">
<i class="bi bi-trash me-2"></i>
Törlés
</a>
</li>

</ul>
</div>
</div>
</td>
</tr>
`;
  }

  // ------------------------------------------------------------
  // FETCH
  // ------------------------------------------------------------

  function buildUrl() {
    const params = new URLSearchParams();

    params.set("page", state.page);
    params.set("pageSize", state.pageSize);

    if (state.searchTerm)
      params.set("searchTerm", state.searchTerm);

    if (state.quickFilter)
      params.set("quickFilter", state.quickFilter);

    const adv = (window.getEmployeeAdvancedFilterParams?.() || "");

    return `/api/employee/index?${params.toString()}${adv}`;
  }

  async function fetchPage() {

    if (state.isLoading || state.reachedEnd) return;

    setLoading(true);

    try {

      const url = buildUrl();

      const res = await fetch(url, {
        headers: { "Accept": "application/json" }
      });

      if (!res.ok) {
        console.error("Employees index fetch failed", res.status);
        return;
      }

      const data = await res.json();
      const items = data.items || [];

      state.totalRecords = data.totalRecords ?? 0;

      if (state.page === 1) {
        els.tbody.innerHTML = "";
        state.loadedCount = 0;
      }

      if (items.length === 0 && state.page === 1) {
        els.tbody.innerHTML =
          `<tr>
<td colspan="10" class="text-center text-muted py-5">
Nincs találat
</td>
</tr>`;
        return;
      }

      const rowsHtml = items.map(renderRow).join("");

      els.tbody.insertAdjacentHTML("beforeend", rowsHtml);

      state.loadedCount += items.length;

      if (items.length < state.pageSize ||
        state.loadedCount >= state.totalRecords) {

        state.reachedEnd = true;
      }

    } catch (err) {

      console.error("Employees index fetch exception", err);

    } finally {

      setLoading(false);

    }
  }

  // ------------------------------------------------------------
  // LOAD MORE
  // ------------------------------------------------------------

  function wireLoadMore() {

    if (!els.loadMoreBtn) return;

    els.loadMoreBtn.addEventListener("click", async () => {

      if (state.isLoading || state.reachedEnd) return;

      state.page += 1;

      await fetchPage();
    });
  }

  // ------------------------------------------------------------
  // SEARCH
  // ------------------------------------------------------------

function wireSearchForm() {

  const form = qs("#employeeSearchForm");
  if (!form) return;

  form.addEventListener("submit", (e) => {

    e.preventDefault();

    const input = qs("#employeeSearchInput");

    state.searchTerm = (input?.value || "").trim();

    resetAndLoad();
  });
}

function wireSearchInputLive() {
  if (!els.searchInput) return;

  let t = null;

  els.searchInput.addEventListener("input", () => {
    clearTimeout(t);

    t = setTimeout(() => {
      state.searchTerm = (els.searchInput.value || "").trim();
      resetAndLoad();
    }, 300);
  });
}

  // ------------------------------------------------------------
  // QUICK FILTER
  // ------------------------------------------------------------

  function wireQuickFilter() {

    document.querySelectorAll('.dropdown-menu a.dropdown-item[data-filter]')
      .forEach(a => {

        a.addEventListener("click", (e) => {

          e.preventDefault();

          const f = a.getAttribute("data-filter") || "all";

          state.quickFilter = f;

          if (els.quickFilterToggle) {

            const map = {
              all: "Szűrő",
              active: "Aktív",
              internal: "Belsős",
              external: "Külsős"
            };

            els.quickFilterToggle.innerHTML =
              `<i class="bi bi-funnel me-1"></i>${map[f] ?? "Szűrő"}`;
          }

          resetAndLoad();
        });
      });
  }

  // ------------------------------------------------------------
  // RESET
  // ------------------------------------------------------------

  function resetAndLoad() {

    state.page = 1;
    state.loadedCount = 0;
    state.totalRecords = 0;
    state.reachedEnd = false;

    fetchPage();
  }

  // ------------------------------------------------------------
  // INIT
  // ------------------------------------------------------------

  function init() {

    els.tbody = qs("#employeesTableBody");
    els.loadMoreBtn = qs("#loadMoreEmployeesBtn");
    els.loadMoreContainer = qs("#loadMoreEmployeesContainer");
    els.searchInput = qs("#employeeSearchInput");
    els.quickFilterToggle = qs("#employeeQuickFilterToggle");

    if (!els.tbody) {
      console.warn("employeesIndex.js: #employeesTableBody not found");
      return;
    }

    const hiddenPageSize = qs('input[name="pageSize"]');

    if (hiddenPageSize && hiddenPageSize.value) {

      const ps = parseInt(hiddenPageSize.value, 10);

      if (!Number.isNaN(ps) && ps > 0)
        state.pageSize = ps;
    }

    wireSearchForm();
    wireSearchInputLive();
    wireQuickFilter();
    wireLoadMore();
    wireRowActions();

    resetAndLoad();

    window.reloadEmployeesIndex = function () {

      if (!els.tbody) {
        init();
        return;
      }

      resetAndLoad();
    };
  }

  document.addEventListener("DOMContentLoaded", init);

})();