// wwwroot/js/HR/employeesEdit.js
(() => {
  "use strict";

  // -------------------------
  // State
  // -------------------------
  let editModal;
  let currentEditEmployeeId = null;

  // TomSelect instances
  let tomWorkerType = null;
  let tomPartner = null;
  let tomJobTitle = null;
  let tomDefaultSite = null;
  let tomStatuses = null;
  let tomSites = null;

  let lookupsLoaded = false;

  // -------------------------
  // Helpers
  // -------------------------
  const el = (id) => document.getElementById(id);

  function toDateInputValue(dateStr) {
    if (!dateStr) return "";
    const s = String(dateStr);
    return s.length >= 10 ? s.substring(0, 10) : s;
  }

  function toIntOrNull(v) {
    if (v === null || v === undefined) return null;
    const s = String(v).trim();
    if (!s) return null;
    const n = parseInt(s, 10);
    return Number.isNaN(n) ? null : n;
  }

  function toNumberOrNull(v) {
    if (v === null || v === undefined) return null;
    const s = String(v).trim();
    if (!s) return null;
    const n = Number(s);
    return Number.isNaN(n) ? null : n;
  }

  async function fetchJson(url, options) {
    const res = await fetch(url, {
      headers: { "Accept": "application/json", ...(options?.headers || {}) },
      ...options,
    });

    // Nem OK -> olvassuk ki a szöveget, hogy lásd a backend hibát (HTML/Exception/ModelState)
    if (!res.ok) {
      const txt = await res.text().catch(() => "");
      throw new Error(`HTTP ${res.status} ${url}\n${txt}`);
    }

    // Üres body (pl. 204, vagy 200 de üres) -> null
    const txt = await res.text().catch(() => "");
    if (!txt) return null;

    try {
      return JSON.parse(txt);
    } catch {
      throw new Error(`Nem JSON válasz: ${url}\n${txt.substring(0, 300)}`);
    }
  }

  function ensureModal() {
    const modalEl = el("editEmployeeModal");
    if (!modalEl) {
      // ha az oldalon nincs edit modal markup, ne dobjunk
      console.warn("[employeesEdit] #editEmployeeModal not found on page");
      return null;
    }
    if (!window.bootstrap?.Modal) {
      console.error("[employeesEdit] Bootstrap Modal not found (bootstrap.js missing?)");
      return null;
    }

    if (!editModal) {
      editModal = bootstrap.Modal.getOrCreateInstance(modalEl);
    }
    return editModal;
  }

  // -------------------------
  // TomSelect init
  // -------------------------
  function initTomSelectsIfNeeded() {
    if (tomWorkerType) return;

    if (!window.TomSelect) {
      console.error("[employeesEdit] TomSelect not found (tom-select.js missing?)");
      return;
    }

    tomWorkerType = new TomSelect("#editWorkerTypeId", {
      create: false,
      allowEmptyOption: true,
      sortField: { field: "text", direction: "asc" },
    });

    tomPartner = new TomSelect("#editPartnerId", {
      create: false,
      allowEmptyOption: true,
      sortField: { field: "text", direction: "asc" },
    });

    tomJobTitle = new TomSelect("#editJobTitleId", {
      create: false,
      allowEmptyOption: true,
      sortField: { field: "text", direction: "asc" },
    });

    tomDefaultSite = new TomSelect("#editDefaultSiteId", {
      create: false,
      allowEmptyOption: true,
      sortField: { field: "text", direction: "asc" },
    });

    tomStatuses = new TomSelect("#editEmploymentStatusIds", {
      plugins: ["remove_button"],
      create: false,
      sortField: { field: "text", direction: "asc" },
    });

    tomSites = new TomSelect("#editSiteIds", {
      plugins: ["remove_button"],
      create: false,
      sortField: { field: "text", direction: "asc" },
    });

    tomWorkerType.on("change", () => {
      applyWorkerTypePartnerRules();
    });
  }

  function setTomOptions(tom, items, valueField = "id", textField = "name") {
    if (!tom) return;
    tom.clearOptions();

    (items ?? []).forEach((x) => {
      tom.addOption({
        value: String(x[valueField]),
        text: x[textField] ?? x.name ?? x.text ?? String(x[valueField]),
      });
    });

    tom.refreshOptions(false);
  }

  // -------------------------
  // Lookups loading
  // -------------------------
  async function ensureEditLookupsLoaded() {
    if (lookupsLoaded) return;

    initTomSelectsIfNeeded();
    if (!tomWorkerType) {
      // TomSelect nincs, ne menjünk tovább
      throw new Error("TomSelect nincs inicializálva (ellenőrizd a script include-okat).");
    }

    const [workerTypes, partners, jobTitles, sites, statuses] = await Promise.all([
    fetchJson("/api/employee/lookups/workertypes"),
    fetchJson("/api/employee/lookups/partners"),
    fetchJson("/api/employee/lookups/jobtitles"),
    fetchJson("/api/employee/lookups/sites"),
    fetchJson("/api/employee/lookups/employmentstatus"),
    ]);

  setTomOptions(tomPartner, partners ?? [], "id", "name");
  setTomOptions(tomJobTitle, jobTitles ?? [], "id", "name");
  setTomOptions(tomDefaultSite, sites ?? [], "id", "name");
  setTomOptions(tomSites, sites ?? [], "id", "name");
  setTomOptions(tomStatuses, statuses ?? [], "id", "name");
  setTomOptions(tomWorkerType, workerTypes ?? [], "id", "name");

    lookupsLoaded = true;
  }

  // -------------------------
  // Form reset + fill
  // -------------------------
  function resetEditForm() {
    const form = el("editEmployeeForm");
    if (form) form.reset();

    currentEditEmployeeId = null;
    const hid = el("editEmployeeId");
    if (hid) hid.value = "";

    initTomSelectsIfNeeded();

    tomWorkerType?.clear(true);
    tomPartner?.clear(true);
    tomJobTitle?.clear(true);
    tomDefaultSite?.clear(true);
    tomStatuses?.clear(true);
    tomSites?.clear(true);

    const partnerContainer = el("editPartnerContainer");
    if (partnerContainer) partnerContainer.style.display = "none";
  }

  function fillEditFormFromDetails(data) {
    currentEditEmployeeId = data?.employeeId ?? data?.id ?? null;
    el("editEmployeeId").value = currentEditEmployeeId ?? "";

    el("editLastName").value = data.lastName ?? "";
    el("editFirstName").value = data.firstName ?? "";
    el("editEmail").value = data.email ?? "";
    el("editEmail2").value = data.email2 ?? "";
    el("editPhoneNumber").value = data.phoneNumber ?? data.phone ?? "";
    el("editPhoneNumber2").value = data.phoneNumber2 ?? "";
    el("editDateOfBirth").value = toDateInputValue(data.dateOfBirth);
    el("editHireDate").value = toDateInputValue(data.hireDate);
    el("editEmployeeIsActive").checked = !!data.isActive;

    el("editDepartmentId").value = data.departmentId ?? "";
    el("editWorkingTime").value = data.workingTime ?? "";
    el("editIsContracted").value = (data.isContracted ? 1 : 0).toString();
    el("editEmploymentEndDate").value = toDateInputValue(data.employmentEndDate);

    el("editVacationDays").value = data.vacationDays ?? "";
    el("editFullVacationDays").value = data.fullVacationDays ?? "";

    el("editTaxId").value = data.taxId ?? "";
    el("editTajNumber").value = data.tajNumber ?? "";
    el("editNationalityCode").value = data.nationalityCode ?? "";

    el("editBirthName").value = data.birthName ?? "";
    el("editMotherBirthName").value = data.motherBirthName ?? "";
    el("editBirthPlace").value = data.birthPlace ?? "";
    el("editFeorCode").value = data.feorCode ?? "";

    el("editPermanentAddress").value = data.permanentAddress ?? "";
    el("editMailingAddress").value = data.mailingAddress ?? "";
    el("editAddress").value = data.address ?? "";

    el("editBankAccountIban").value = data.bankAccountIban ?? "";
    el("editFamilyData").value = data.familyData ?? "";
    el("editComment1").value = data.comment1 ?? "";
    el("editComment2").value = data.comment2 ?? "";

    // Selectek (TomSelect)
    if (data.workerTypeId != null) tomWorkerType.setValue(String(data.workerTypeId), true);
    else tomWorkerType.clear(true);

    if (data.partnerId != null) tomPartner.setValue(String(data.partnerId), true);
    else tomPartner.clear(true);

    if (data.jobTitleId != null) tomJobTitle.setValue(String(data.jobTitleId), true);
    else tomJobTitle.clear(true);

    if (data.defaultSiteId != null) tomDefaultSite.setValue(String(data.defaultSiteId), true);
    else tomDefaultSite.clear(true);

    tomStatuses.setValue((data.statusIds ?? []).map(String), true);
    tomSites.setValue((data.siteIds ?? []).map(String), true);
  }

  // -------------------------
  // WorkerType -> Partner rules
  // -------------------------
  function applyWorkerTypePartnerRules() {
    const workerTypeVal = tomWorkerType?.getValue?.() ?? "";
    const container = el("editPartnerContainer");
    if (!container) return;

    // TODO: igazítsd a saját logikádra (pl. workerTypeVal === "2" -> külsős)
    const shouldShowPartner = !!workerTypeVal;
    container.style.display = shouldShowPartner ? "block" : "none";

    if (!shouldShowPartner) tomPartner?.clear(true);
  }

  // -------------------------
  // Payload build + Save (PUT)
  // -------------------------
  function buildPayloadFromEditForm() {
    return {
      employeeId: currentEditEmployeeId,

      firstName: el("editFirstName").value.trim(),
      lastName: el("editLastName").value.trim(),
      email: el("editEmail").value.trim(),
      email2: el("editEmail2").value.trim(),
      phoneNumber: el("editPhoneNumber").value.trim(),
      phoneNumber2: el("editPhoneNumber2").value.trim(),

      dateOfBirth: el("editDateOfBirth").value || null,
      hireDate: el("editHireDate").value || null,
      isActive: el("editEmployeeIsActive").checked,

      workerTypeId: toIntOrNull(tomWorkerType.getValue()),
      partnerId: toIntOrNull(tomPartner.getValue()),

      departmentId: toIntOrNull(el("editDepartmentId").value),
      jobTitleId: toIntOrNull(tomJobTitle.getValue()),
      defaultSiteId: toIntOrNull(tomDefaultSite.getValue()),

      workingTime: toNumberOrNull(el("editWorkingTime").value),
      isContracted: el("editIsContracted").value ? parseInt(el("editIsContracted").value, 10) : null,
      employmentEndDate: el("editEmploymentEndDate").value || null,

      vacationDays: toIntOrNull(el("editVacationDays").value),
      fullVacationDays: toIntOrNull(el("editFullVacationDays").value),

      statusIds: (tomStatuses.getValue() ?? [])
        .map((x) => parseInt(x, 10))
        .filter((n) => !Number.isNaN(n)),
      siteIds: (tomSites.getValue() ?? [])
        .map((x) => parseInt(x, 10))
        .filter((n) => !Number.isNaN(n)),

      taxId: el("editTaxId").value.trim(),
      tajNumber: el("editTajNumber").value.trim(),
      nationalityCode: el("editNationalityCode").value.trim(),

      birthName: el("editBirthName").value.trim(),
      motherBirthName: el("editMotherBirthName").value.trim(),
      birthPlace: el("editBirthPlace").value.trim(),
      feorCode: el("editFeorCode").value.trim(),

      permanentAddress: el("editPermanentAddress").value.trim(),
      mailingAddress: el("editMailingAddress").value.trim(),
      address: el("editAddress").value.trim(),

      bankAccountIban: el("editBankAccountIban").value.trim(),
      familyData: el("editFamilyData").value.trim(),
      comment1: el("editComment1").value.trim(),
      comment2: el("editComment2").value.trim(),
    };
  }

  async function saveEdit() {
    if (!currentEditEmployeeId) throw new Error("Hiányzó employeeId (Edit state).");

    const payload = buildPayloadFromEditForm();

    const resp = await fetch(`/api/employee/${currentEditEmployeeId}`, {
      method: "PUT",
      headers: { "Content-Type": "application/json", "Accept": "application/json" },
      body: JSON.stringify(payload), 
    });

    if (!resp.ok) {
      const txt = await resp.text().catch(() => "");
      throw new Error(txt || "Mentés sikertelen");
    }

    // ha a backend visszaad JSON-t, itt ki tudod olvasni:
    // const updated = await resp.json().catch(() => null);
  }

  // -------------------------
  // Public entrypoint: openEditEmployee
  // -------------------------
  window.openEditEmployee = async function (employeeId, detailsDto) {
    try {
      const m = ensureModal();
      if (!m) return;

      resetEditForm();
      await ensureEditLookupsLoaded();

      const data = detailsDto ?? await fetchJson(`/api/employee/${employeeId}`);
      if (!data) throw new Error("Az API üres employee details választ adott.");

      fillEditFormFromDetails(data);
      applyWorkerTypePartnerRules();

      editModal.show();
    } catch (err) {
      console.error("[employeesEdit] openEditEmployee failed", err);
      alert(err?.message || "Nem sikerült megnyitni az Edit modalt.");
    }
  };

  // -------------------------
  // Wire up form submit
  // -------------------------
  document.addEventListener("DOMContentLoaded", () => {
    // Ha ezen az oldalon nincs edit modal, ne csináljunk semmit
    if (!el("editEmployeeModal")) return;

    // Csak akkor initeljünk, ha megvannak a függőségek
    if (!window.bootstrap?.Modal) {
      console.error("[employeesEdit] Bootstrap Modal missing");
      return;
    }
    if (!window.TomSelect) {
      console.error("[employeesEdit] TomSelect missing");
      return;
    }

    ensureModal();
    initTomSelectsIfNeeded();

    const form = el("editEmployeeForm");
    if (!form) return;

    form.addEventListener("submit", async (e) => {
      e.preventDefault();
      try {
        await saveEdit();
        editModal?.hide();
        window.reloadEmployeesIndex?.();

        // opcionális: friss view újranyitás
        // window.openViewEmployee?.(currentEditEmployeeId);

      } catch (err) {
        console.error("[employeesEdit] save failed", err);
        alert(err?.message || "Hiba történt mentés közben.");
      }
    });
  });

})();