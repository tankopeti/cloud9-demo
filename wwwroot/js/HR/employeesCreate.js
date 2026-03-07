// wwwroot/js/HR/employeesCreate.js
// Cloud9_2 ERP - HR / Employees - Create modal JS
// Requires: Bootstrap 5, optional TomSelect

(function () {
  "use strict";

  // -----------------------------
  // Endpoints (EmployeeController lookups)
  // -----------------------------
  const endpoints = {
    createEmployee: "/api/employee",
    workerTypes: "/api/employee/lookups/workertypes",
    partners: "/api/employee/lookups/partners",
    jobTitles: "/api/employee/lookups/jobtitles",
    sites: "/api/employee/lookups/sites",
    employmentStatuses: "/api/employee/lookups/employmentstatus"
  };

  // -----------------------------
  // DOM
  // -----------------------------
  const modalEl = document.getElementById("createEmployeeModal");
  const formEl = document.getElementById("createEmployeeForm");
  const saveBtn = document.getElementById("saveNewEmployeeBtn");

  const workerTypeSelect = document.getElementById("createWorkerTypeId");
  const partnerContainer = document.getElementById("createPartnerContainer");
  const partnerSelect = document.getElementById("createPartnerId");

  const jobTitleSelect = document.getElementById("createJobTitleId");
  const defaultSiteSelect = document.getElementById("createDefaultSiteId");

  const statusSelect = document.getElementById("createEmploymentStatusIds");
  const siteMultiSelect = document.getElementById("createSiteIds"); // UI van, DTO-ban még nincs!

  if (!modalEl || !formEl || !saveBtn) return;

  // -----------------------------
  // TomSelect
  // -----------------------------
  let tsStatuses = null;
  let tsSites = null;

  function initTomSelect() {
    if (typeof TomSelect === "undefined") return;

    if (statusSelect && !tsStatuses) {
      tsStatuses = new TomSelect(statusSelect, {
        plugins: ["remove_button"],
        create: false,
        persist: false
      });
    }

    if (siteMultiSelect && !tsSites) {
      tsSites = new TomSelect(siteMultiSelect, {
        plugins: ["remove_button"],
        create: false,
        persist: false
      });
    }
  }

  function resetTomSelectValues() {
    tsStatuses?.clear(true);
    tsSites?.clear(true);
  }

  // -----------------------------
  // UI helpers
  // -----------------------------
  function setButtonLoading(btn, isLoading) {
    btn.disabled = isLoading;
    btn.dataset._origText ??= btn.innerHTML;
    btn.innerHTML = isLoading
      ? `<span class="spinner-border spinner-border-sm me-2"></span>Mentés...`
      : btn.dataset._origText;
  }

  function clearValidation() {
    formEl.querySelectorAll(".is-invalid").forEach(x => x.classList.remove("is-invalid"));
    formEl.querySelectorAll(".invalid-feedback.dynamic").forEach(x => x.remove());
    formEl.querySelectorAll(".alert.dynamic").forEach(x => x.remove());
  }

  function showTopAlert(message, type = "danger") {
    const alert = document.createElement("div");
    alert.className = `alert alert-${type} dynamic`;
    alert.role = "alert";
    alert.innerText = message;
    formEl.prepend(alert);
  }

  function addFieldError(fieldName, message) {
    const input = formEl.querySelector(`[name="${CSS.escape(fieldName)}"]`);
    if (!input) return;

    input.classList.add("is-invalid");

    const fb = document.createElement("div");
    fb.className = "invalid-feedback dynamic";
    fb.innerText = message;

    input.insertAdjacentElement("afterend", fb);
  }

  // -----------------------------
  // Fetch helpers
  // -----------------------------
  async function fetchJson(url) {
    const res = await fetch(url, { headers: { "Accept": "application/json" } });
    if (!res.ok) throw new Error(`HTTP ${res.status} - ${url}`);
    return await res.json();
  }

  function fillSelect(selectEl, items, placeholder = "Válasszon...") {
    selectEl.innerHTML = "";
    const opt0 = document.createElement("option");
    opt0.value = "";
    opt0.textContent = placeholder;
    selectEl.appendChild(opt0);

    for (const it of items || []) {
      const opt = document.createElement("option");
      opt.value = it.id ?? "";
      opt.textContent = it.name ?? "";
      selectEl.appendChild(opt);
    }
  }

  function fillMultiSelect(selectEl, items) {
    selectEl.innerHTML = "";
    for (const it of items || []) {
      const opt = document.createElement("option");
      opt.value = it.id ?? "";
      opt.textContent = it.name ?? "";
      selectEl.appendChild(opt);
    }
  }

  // -----------------------------
  // WorkerType / Partner logic
  // -----------------------------
  function getWorkerTypeId() {
    const v = workerTypeSelect.value;
    if (v === "") return null;
    const n = Number.parseInt(v, 10);
    return Number.isFinite(n) ? n : null;
  }

  function isExternal(workerTypeId) {
    return workerTypeId === 2; // 1=Internal, 2=External
  }

  function showPartnerForExternal(external) {
    partnerContainer.style.display = external ? "" : "none";
    partnerSelect.required = external;

    if (!external) partnerSelect.value = "";
  }

  // -----------------------------
  // Build DTO (PascalCase -> matches C# DTO!)
  // -----------------------------
  function parseIntOrNull(v) {
    if (v === "" || v === null || v === undefined) return null;
    const n = Number.parseInt(v, 10);
    return Number.isFinite(n) ? n : null;
  }

  function parseDecimalOrNull(v) {
    if (v === "" || v === null || v === undefined) return null;
    const n = Number.parseFloat(String(v).replace(",", "."));
    return Number.isFinite(n) ? n : null;
  }

  function getStringOrNull(v) {
    const s = (v ?? "").toString().trim();
    return s.length ? s : null;
  }

  function getDateOrNull(v) {
    const s = getStringOrNull(v);
    return s; // "YYYY-MM-DD" ok ASP.NET-nek
  }

  function getStatusIds() {
    const raw = tsStatuses
      ? tsStatuses.getValue()
      : Array.from(statusSelect.selectedOptions).map(o => o.value);

    const arr = Array.isArray(raw) ? raw : String(raw).split(",");
    return arr
      .filter(x => x !== "")
      .map(x => Number.parseInt(x, 10))
      .filter(Number.isFinite);
  }

  function buildCreateDto() {
    const wt = getWorkerTypeId();
    const external = isExternal(wt);

    return {
      FirstName: getStringOrNull(formEl.elements["FirstName"]?.value),
      LastName: getStringOrNull(formEl.elements["LastName"]?.value),
      Email: getStringOrNull(formEl.elements["Email"]?.value),
      Email2: getStringOrNull(formEl.elements["Email2"]?.value),
      PhoneNumber: getStringOrNull(formEl.elements["PhoneNumber"]?.value),
      PhoneNumber2: getStringOrNull(formEl.elements["PhoneNumber2"]?.value),

      DateOfBirth: getDateOrNull(formEl.elements["DateOfBirth"]?.value),
      Address: getStringOrNull(formEl.elements["Address"]?.value),
      HireDate: getDateOrNull(formEl.elements["HireDate"]?.value),

      DepartmentId: parseIntOrNull(formEl.elements["DepartmentId"]?.value),
      JobTitleId: parseIntOrNull(jobTitleSelect.value),
      DefaultSiteId: parseIntOrNull(defaultSiteSelect.value),

      IsActive: !!formEl.elements["IsActive"]?.checked,

      WorkingTime: parseDecimalOrNull(formEl.elements["WorkingTime"]?.value),
      IsContracted: (() => {
        const v = formEl.elements["IsContracted"]?.value ?? "0";
        const n = Number.parseInt(v, 10);
        return Number.isFinite(n) ? n : 0;
      })(),

      VacationDays: parseIntOrNull(formEl.elements["VacationDays"]?.value),
      FullVacationDays: parseIntOrNull(formEl.elements["FullVacationDays"]?.value),

      EmploymentEndDate: getDateOrNull(formEl.elements["EmploymentEndDate"]?.value),

      TaxId: getStringOrNull(formEl.elements["TaxId"]?.value),
      TajNumber: getStringOrNull(formEl.elements["TajNumber"]?.value),
      NationalityCode: getStringOrNull(formEl.elements["NationalityCode"]?.value),
      BirthName: getStringOrNull(formEl.elements["BirthName"]?.value),
      MotherBirthName: getStringOrNull(formEl.elements["MotherBirthName"]?.value),
      BirthPlace: getStringOrNull(formEl.elements["BirthPlace"]?.value),
      FeorCode: getStringOrNull(formEl.elements["FeorCode"]?.value),

      PermanentAddress: getStringOrNull(formEl.elements["PermanentAddress"]?.value),
      MailingAddress: getStringOrNull(formEl.elements["MailingAddress"]?.value),

      BankAccountIban: getStringOrNull(formEl.elements["BankAccountIban"]?.value),
      FamilyData: getStringOrNull(formEl.elements["FamilyData"]?.value),
      Comment1: getStringOrNull(formEl.elements["Comment1"]?.value),
      Comment2: getStringOrNull(formEl.elements["Comment2"]?.value),

      WorkerTypeId: wt ?? 1,
      PartnerId: external ? parseIntOrNull(partnerSelect.value) : null,

      StatusIds: getStatusIds()
    };
  }

  // -----------------------------
  // Client validation
  // -----------------------------
  function validateClient(dto) {
    if (!dto.LastName) addFieldError("LastName", "A vezetéknév megadása javasolt.");
    if (!dto.FirstName) addFieldError("FirstName", "A keresztnév megadása javasolt.");
    if (!dto.WorkerTypeId) addFieldError("WorkerTypeId", "A dolgozó típusa kötelező.");

    if (dto.WorkerTypeId === 2 && !dto.PartnerId) {
      addFieldError("PartnerId", "Külsős dolgozónál a partner kötelező.");
    }

    return formEl.querySelectorAll(".is-invalid").length === 0;
  }

  // -----------------------------
  // Server error parsing
  // -----------------------------
  async function parseErrorResponse(res) {
    try { return await res.json(); } catch { return null; }
  }

  function applyModelStateErrors(problem) {
    // Csak akkor kezeljük mezőhibának, ha van "errors" objektum
    if (!problem || !problem.errors || typeof problem.errors !== "object") return false;

    let appliedAny = false;
    for (const [field, msgs] of Object.entries(problem.errors)) {
      const msg = Array.isArray(msgs) ? (msgs[0] ?? "Hibás mező.") : String(msgs);
      addFieldError(field, msg);
      appliedAny = true;
    }

    return appliedAny;
  }

  // -----------------------------
  // Submit
  // -----------------------------
  async function submitCreate() {
    clearValidation();

    const dto = buildCreateDto();
    console.log("CREATE DTO:", dto); // DEBUG

    if (!validateClient(dto)) {
      showTopAlert("Kérlek javítsd a pirossal jelölt mezőket.", "warning");
      return;
    }

    setButtonLoading(saveBtn, true);

    try {
      console.log("POST URL:", endpoints.createEmployee);

      const res = await fetch(endpoints.createEmployee, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Accept": "application/json"
        },
        body: JSON.stringify(dto)
      });

      console.log("RESPONSE STATUS:", res.status);

      const contentType = res.headers.get("content-type") || "";
      console.log("CONTENT TYPE:", contentType);

      const rawText = await res.clone().text();
      console.log("RAW RESPONSE:", rawText);

      if (!res.ok) {
        const problem = await parseErrorResponse(res);

        const applied = applyModelStateErrors(problem);
        if (applied) {
          showTopAlert("Mentés sikertelen: kérlek javítsd a jelzett mezőket.", "danger");
        } else {
          const msg = problem?.message || problem?.title || `Mentés sikertelen (HTTP ${res.status}).`;
          showTopAlert(msg, "danger");
        }

        return;
      }

      console.log("EMPLOYEE CREATE SUCCESS");

      // close modal
      const bsModal = bootstrap.Modal.getInstance(modalEl) || new bootstrap.Modal(modalEl);
      bsModal.hide();

      // refresh index (IMPORTANT: ezt az employeesIndex.js-nek kell kitenni window-ra!)
      setTimeout(() => {
        if (typeof window.reloadEmployeesIndex === "function") {
          window.reloadEmployeesIndex(true);
        } else if (typeof window.loadEmployees === "function") {
          window.loadEmployees(1, true);
        } else {
          console.warn("No reload function found. Add window.reloadEmployeesIndex in employeesIndex.js");
        }
      }, 50);

    } catch (err) {
      console.error("CREATE ERROR:", err);
      showTopAlert("Hálózati hiba történt mentés közben. Próbáld újra.", "danger");
    } finally {
      setButtonLoading(saveBtn, false);
    }
  }

  // -----------------------------
  // Lookups loading
  // -----------------------------
  let lookupsLoaded = false;

  async function loadLookupsOnce() {
    if (lookupsLoaded) return;

    const [workerTypes, partners, jobTitles, sites, statuses] = await Promise.all([
      fetchJson(endpoints.workerTypes),
      fetchJson(endpoints.partners),
      fetchJson(endpoints.jobTitles),
      fetchJson(endpoints.sites),
      fetchJson(endpoints.employmentStatuses)
    ]);

    fillSelect(workerTypeSelect, workerTypes);
    fillSelect(partnerSelect, partners);
    fillSelect(jobTitleSelect, jobTitles);
    fillSelect(defaultSiteSelect, sites);

    fillMultiSelect(statusSelect, statuses);
    fillMultiSelect(siteMultiSelect, sites);

    initTomSelect();
    lookupsLoaded = true;
  }

  // -----------------------------
  // Events
  // -----------------------------
  workerTypeSelect.addEventListener("change", () => {
    const wt = getWorkerTypeId();
    showPartnerForExternal(isExternal(wt));
  });

  saveBtn.addEventListener("click", submitCreate);

  modalEl.addEventListener("show.bs.modal", async () => {
    clearValidation();
    try {
      await loadLookupsOnce();
      showPartnerForExternal(isExternal(getWorkerTypeId()));
    } catch (err) {
      console.error(err);
      showTopAlert("Nem sikerült betölteni a lenyíló listákat (lookups).", "danger");
    }
  });

  modalEl.addEventListener("hidden.bs.modal", () => {
    clearValidation();
    formEl.reset();
    resetTomSelectValues();
    showPartnerForExternal(false);
  });

})();