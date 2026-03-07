// wwwroot/js/HR/employeesView.js

(function () {
  // --- modal ---
  const modalEl = document.getElementById("viewEmployeeModal");
  if (!modalEl) {
    console.warn("[employeesView] #viewEmployeeModal not found");
    return;
  }
  const modal = bootstrap.Modal.getOrCreateInstance(modalEl);

  const el = (id) => document.getElementById(id);

  function safeText(v) {
    return (v === null || v === undefined || v === "") ? "–" : String(v);
  }

  function setText(id, value) {
    const node = el(id);
    if (!node) return;
    node.textContent = safeText(value);
  }

  function setWrap(wrapperId, valueId, value) {
    const w = el(wrapperId);
    const vEl = el(valueId);
    if (!w || !vEl) return;

    if (value === null || value === undefined || value === "") {
      w.style.display = "none";
      vEl.textContent = "";
      return;
    }

    w.style.display = "block";
    vEl.textContent = String(value);
  }

  function formatDate(v) {
    if (!v) return "–";
    const d = new Date(v);
    if (isNaN(d.getTime())) return safeText(v);
    return d.toLocaleDateString("hu-HU");
  }

  function formatDateTime(v) {
    if (!v) return "–";
    const d = new Date(v);
    if (isNaN(d.getTime())) return safeText(v);
    return d.toLocaleString("hu-HU");
  }

  function resetTables() {
    const statusRows = el("ve_statusRows");
    if (statusRows) statusRows.innerHTML = `<tr><td colspan="2" class="text-muted">–</td></tr>`;
    const siteRows = el("ve_siteRows");
    if (siteRows) siteRows.innerHTML = `<tr><td colspan="2" class="text-muted">–</td></tr>`;
    setText("ve_statusCount", "–");
    setText("ve_siteCount", "–");
  }

  function resetAll() {
    // header/top
    setText("viewEmployeeModalTitle", "Dolgozó");
    setText("ve_fullName", "–");
    setText("ve_employeeId", "–");
    setText("ve_isActiveText", "–");

    const wtBadge = el("ve_workerTypeBadge");
    if (wtBadge) wtBadge.textContent = "–";

    const pBadge = el("ve_partnerBadge");
    if (pBadge) {
      pBadge.textContent = "–";
      pBadge.style.display = "none";
    }

    // basic
    setText("ve_email", "–");
    setWrap("ve_email2_wrap", "ve_email2", null);

    setText("ve_phoneNumber", "–");
    setWrap("ve_phone2_wrap", "ve_phoneNumber2", null);

    setText("ve_dateOfBirth", "–");
    setText("ve_address", "–");
    setText("ve_hireDate", "–");
    setText("ve_employmentEndDate", "–");
    setText("ve_department", "–");
    setText("ve_permanentAddress", "–");
    setText("ve_mailingAddress", "–");
    setText("ve_bankAccountIban", "–");

    // work
    setText("ve_workerTypeName", "–");
    setText("ve_partnerName", "–");
    setText("ve_jobTitleName", "–");
    setText("ve_defaultSiteName", "–");
    setText("ve_workingTime", "–");
    setText("ve_isContracted", "–");
    setText("ve_familyData", "–");
    setText("ve_vacationDays", "–");
    setText("ve_fullVacationDays", "–");

    // ids
    setText("ve_taxId", "–");
    setText("ve_tajNumber", "–");
    setText("ve_feorCode", "–");
    setText("ve_birthName", "–");
    setText("ve_motherBirthName", "–");
    setText("ve_birthPlace", "–");
    setText("ve_nationalityCode", "–");

    // notes
    setText("ve_comment1", "–");
    setText("ve_comment2", "–");

    resetTables();

    const editBtn = el("editEmployeeBtn");
    if (editBtn) editBtn.style.display = "none";
  }

  function fillStatuses(statuses) {
    const tbody = el("ve_statusRows");
    if (!tbody) return;

    tbody.innerHTML = "";

    if (!statuses || statuses.length === 0) {
      tbody.innerHTML = `<tr><td colspan="2" class="text-muted">Nincs státusz</td></tr>`;
      setText("ve_statusCount", "0 db");
      return;
    }

    statuses.forEach(s => {
      const tr = document.createElement("tr");

      const tdName = document.createElement("td");
      tdName.textContent = safeText(s.statusName);

      const tdAt = document.createElement("td");
      tdAt.className = "text-nowrap text-muted";
      tdAt.textContent = formatDateTime(s.assignedAt);

      tr.appendChild(tdName);
      tr.appendChild(tdAt);
      tbody.appendChild(tr);
    });

    setText("ve_statusCount", `${statuses.length} db`);
  }

  function fillSites(sites) {
    const tbody = el("ve_siteRows");
    if (!tbody) return;

    tbody.innerHTML = "";

    if (!sites || sites.length === 0) {
      tbody.innerHTML = `<tr><td colspan="2" class="text-muted">Nincs telephely</td></tr>`;
      setText("ve_siteCount", "0 db");
      return;
    }

    sites.forEach(s => {
      const tr = document.createElement("tr");

      const tdName = document.createElement("td");
      tdName.textContent = safeText(s.siteName);

      const tdPrimary = document.createElement("td");
      tdPrimary.className = "text-nowrap";
      tdPrimary.innerHTML = s.isPrimary
        ? `<span class="badge bg-success">Alap</span>`
        : `<span class="badge bg-light text-muted border">–</span>`;

      tr.appendChild(tdName);
      tr.appendChild(tdPrimary);
      tbody.appendChild(tr);
    });

    setText("ve_siteCount", `${sites.length} db`);
  }

  async function openViewEmployee(employeeId) {
    resetAll();
    setText("viewEmployeeModalTitle", `Dolgozó #${employeeId}`);

    try {
      const resp = await fetch(`/api/employee/${employeeId}`, {
        headers: { "Accept": "application/json" }
      });

      if (!resp.ok) throw new Error(`Hiba a dolgozó lekérésnél (HTTP ${resp.status})`);

      const data = await resp.json();

      // ---- top ----
      const fullName =
        data.fullName ||
        [data.lastName, data.firstName].filter(Boolean).join(" ").trim();

      setText("ve_employeeId", data.employeeId ?? employeeId);
      setText("ve_fullName", fullName || "–");
      setText("ve_isActiveText", data.isActive ? "Igen" : "Nem");

      const wtBadge = el("ve_workerTypeBadge");
      if (wtBadge) wtBadge.textContent = safeText(data.workerTypeName);

      const partnerBadge = el("ve_partnerBadge");
      if (partnerBadge) {
        if (data.partnerName) {
          partnerBadge.textContent = data.partnerName;
          partnerBadge.style.display = "inline-block";
        } else {
          partnerBadge.style.display = "none";
        }
      }

      // ---- basic ----
      setText("ve_email", data.email);
      setWrap("ve_email2_wrap", "ve_email2", data.email2);

      setText("ve_phoneNumber", data.phoneNumber);
      setWrap("ve_phone2_wrap", "ve_phoneNumber2", data.phoneNumber2);

      setText("ve_dateOfBirth", formatDate(data.dateOfBirth));
      setText("ve_address", data.address);
      setText("ve_hireDate", formatDate(data.hireDate));
      setText("ve_employmentEndDate", formatDate(data.employmentEndDate));

      setText("ve_department", data.departmentName ?? (data.departmentId ? `#${data.departmentId}` : null));
      setText("ve_permanentAddress", data.permanentAddress);
      setText("ve_mailingAddress", data.mailingAddress);
      setText("ve_bankAccountIban", data.bankAccountIban);

      // ---- work ----
      setText("ve_workerTypeName", data.workerTypeName);
      setText("ve_partnerName", data.partnerName ?? "–");
      setText("ve_jobTitleName", data.jobTitleName ?? "–");
      setText("ve_defaultSiteName", data.defaultSiteName ?? "–");

      setText("ve_workingTime",
        (data.workingTime === null || data.workingTime === undefined) ? "–" : `${data.workingTime} óra/nap`
      );

      if (data.isContracted === null || data.isContracted === undefined) {
        setText("ve_isContracted", "–");
      } else {
        setText("ve_isContracted", Number(data.isContracted) === 1 ? "Igen" : "Nem");
      }

      setText("ve_familyData", data.familyData);
      setText("ve_vacationDays", data.vacationDays);
      setText("ve_fullVacationDays", data.fullVacationDays);

      // ---- status & sites ----
      fillStatuses(data.statuses || []);
      fillSites(data.sites || []);

      // ---- ids ----
      setText("ve_taxId", data.taxId);
      setText("ve_tajNumber", data.tajNumber);
      setText("ve_feorCode", data.feorCode);
      setText("ve_birthName", data.birthName);
      setText("ve_motherBirthName", data.motherBirthName);
      setText("ve_birthPlace", data.birthPlace);
      setText("ve_nationalityCode", data.nationalityCode);

      // ---- notes ----
      setText("ve_comment1", data.comment1);
      setText("ve_comment2", data.comment2);

      // edit button
      const editBtn = el("editEmployeeBtn");
      if (editBtn) {
        editBtn.style.display = "inline-block";
        editBtn.onclick = () => console.log("[employeesView] edit", employeeId);
      }

      modal.show();
    } catch (err) {
      console.error(err);
      alert(err.message || "Ismeretlen hiba történt.");
    }
  }

  // Delegated click handler (works for AJAX rendered rows too)
  document.addEventListener("click", function (e) {
    const btn = e.target.closest(".viewEmployeeBtn");
    if (!btn) return;

    e.preventDefault();

    const id = btn.getAttribute("data-employee-id");
    if (!id) {
      console.warn("[employeesView] Missing data-employee-id on view button", btn);
      return;
    }

    const employeeId = parseInt(id, 10);
    if (Number.isNaN(employeeId)) {
      console.warn("[employeesView] Invalid employeeId:", id);
      return;
    }

    openViewEmployee(employeeId);
  }, true);

  // debug helper
  window.openViewEmployee = openViewEmployee;

  console.log("[employeesView] initialized");
})();