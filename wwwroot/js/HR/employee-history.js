// wwwroot/js/HR/employee-history.js
(() => {
  "use strict";

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

  function fmtDate(dt) {
    if (!dt) return "—";
    const d = new Date(dt);
    if (Number.isNaN(d.getTime())) return escapeHtml(String(dt));
    return new Intl.DateTimeFormat("hu-HU", {
      year: "numeric", month: "2-digit", day: "2-digit",
      hour: "2-digit", minute: "2-digit"
    }).format(d);
  }

  function actionUi(action) {
    const a = (action || "").toLowerCase();
    if (a.includes("create")) return { icon: "bi-plus-lg", badge: "bg-success", label: "Létrehozva" };
    if (a.includes("update") || a.includes("modify")) return { icon: "bi-pencil-square", badge: "bg-primary", label: "Módosítva" };
    if (a.includes("delete")) return { icon: "bi-trash", badge: "bg-danger", label: "Törölve" };
    return { icon: "bi-activity", badge: "bg-secondary", label: action || "Esemény" };
  }

  function normalizeItems(data) {
    // Támogatott formák:
    // 1) { items: [...] }
    // 2) [...]
    const items = Array.isArray(data) ? data : (data?.items || []);
    return items.map((x, idx) => ({
      id: x.auditLogId ?? x.id ?? idx,
      action: x.action ?? x.Action ?? "",
      changedAt: x.changedAt ?? x.ChangedAt ?? x.timestamp ?? x.Timestamp ?? null,
      changedByName: x.changedByName ?? x.ChangedByName ?? x.userName ?? x.UserName ?? "—",
      changes: x.changes ?? x.Changes ?? x.message ?? x.Message ?? ""
    }));
  }

  function renderTimeline(items) {
    // Bootstrap collapse id-k
    const lis = items.map((it, i) => {
      const ui = actionUi(it.action);
      const when = fmtDate(it.changedAt);
      const who = escapeHtml(it.changedByName || "—");
      const title = ui.label;
      const changesText = (it.changes || "").trim();

      const collapseId = `empHistCollapse_${it.id}_${i}`;
      const hasDetails = changesText.length > 0;

      return `
        <li class="emp-timeline-item">
          <div class="d-flex justify-content-center">
            <div class="emp-timeline-dot">
              <i class="bi ${ui.icon}"></i>
            </div>
          </div>

          <div class="emp-timeline-card">
            <div class="d-flex justify-content-between align-items-start gap-2 flex-wrap">
              <p class="emp-timeline-title mb-1">
                <span class="badge ${ui.badge}">${escapeHtml(title)}</span>
                <span class="text-muted emp-timeline-meta">
                  ${escapeHtml(when)} • ${who}
                </span>
              </p>

              ${hasDetails ? `
                <button class="btn btn-sm btn-outline-secondary"
                        type="button"
                        data-bs-toggle="collapse"
                        data-bs-target="#${collapseId}"
                        aria-expanded="false"
                        aria-controls="${collapseId}">
                  Részletek
                </button>` : ``}
            </div>

            ${hasDetails ? `
              <div class="collapse mt-2" id="${collapseId}">
                <div class="emp-timeline-changes small">
                  ${escapeHtml(changesText)}
                </div>
              </div>` : `
              <div class="small text-muted mt-2">Nincs részletes változás információ.</div>`}
          </div>
        </li>
      `;
    }).join("");

    return `<ul class="emp-timeline">${lis}</ul>`;
  }

  async function loadEmployeeHistory(employeeId) {
    const loading = qs("#employee-history-loading");
    const content = qs("#employee-history-content");
    const empty = qs("#employee-history-empty");
    const list = qs("#employee-history-list");

    if (!list) return;

    // UI state
    if (loading) loading.classList.remove("d-none");
    if (content) content.classList.add("d-none");
    if (empty) empty.classList.add("d-none");
    list.innerHTML = "";

    try {
      // ✅ Endpoint – állítsd be a backendedhez
      // Ha máshol van, egyszerűen írd felül:
      // window.employeeHistoryEndpoint = (id) => `/api/audit/employee/${id}`;
      const endpointBuilder = window.employeeHistoryEndpoint || ((id) => `/api/employee/${id}/history`);
      const url = endpointBuilder(employeeId);

      const res = await fetch(url, { headers: { "Accept": "application/json" } });
      if (!res.ok) throw new Error(`HTTP ${res.status}`);

      const data = await res.json();
      const items = normalizeItems(data);

      if (loading) loading.classList.add("d-none");
      if (content) content.classList.remove("d-none");

      if (!items.length) {
        if (empty) empty.classList.remove("d-none");
        return;
      }

      // legújabb felül (ha a backend nem így adja)
      items.sort((a, b) => new Date(b.changedAt || 0) - new Date(a.changedAt || 0));

      list.innerHTML = renderTimeline(items);
    } catch (err) {
      console.error("Employee history load failed", err);

      if (loading) loading.classList.add("d-none");
      if (content) content.classList.remove("d-none");

      list.innerHTML = `
        <div class="alert alert-danger mb-0">
          Nem sikerült betölteni a történetet.
          <div class="small mt-1 text-muted">${escapeHtml(err?.message || "Ismeretlen hiba")}</div>
        </div>`;
    }
  }

  // Expose global
  window.loadEmployeeHistory = loadEmployeeHistory;
})();