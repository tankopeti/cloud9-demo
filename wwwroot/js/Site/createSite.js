// /wwwroot/js/Site/createSite.js
console.log("✅ createSite.js betöltve");

document.addEventListener("DOMContentLoaded", () => {
  const modalEl = document.getElementById("createSiteModal");
  const form = document.getElementById("createSiteForm");

  if (!modalEl || !form) {
    console.warn("createSiteModal vagy createSiteForm hiányzik");
    return;
  }

  const partnerSelectEl = document.getElementById("createPartnerId");
  const statusSelectEl = document.getElementById("createStatusId");
  const communicationTypeSelectEl = document.getElementById("createDefaultCommunicationTypeId");
  const siteTypeSelectEl = document.getElementById("createSiteTypeId");

  console.log("communicationTypeSelectEl:", communicationTypeSelectEl);
  console.log("siteTypeSelectEl:", siteTypeSelectEl);

  // CSRF
  const csrf =
    document.querySelector('meta[name="csrf-token"]')?.content ||
    document.querySelector('input[name="__RequestVerificationToken"]')?.value ||
    (document.cookie.match(/XSRF-TOKEN=([^;]+)/)?.[1]
      ? decodeURIComponent(document.cookie.match(/XSRF-TOKEN=([^;]+)/)?.[1])
      : "") ||
    "";

  function valByName(name) {
    return (form.querySelector(`[name="${name}"]`)?.value ?? "").trim();
  }

  async function loadStatusesIntoSelect(selectEl, selectedId) {
    const base = window.API_BASE || "";
    const res = await fetch(`${base}/api/SitesIndex/meta/statuses`, {
      credentials: "include",
      headers: { Accept: "application/json" }
    });

    if (!res.ok) throw new Error("Failed to load statuses");

    const data = await res.json();
    console.log("statuses data:", data);

    selectEl.innerHTML = '<option value="">— válassz státuszt —</option>';

    data.forEach(s => {
      const opt = document.createElement("option");
      opt.value = s.id;
      opt.textContent = s.name;
      if (selectedId && String(selectedId) === String(s.id)) {
        opt.selected = true;
      }
      selectEl.appendChild(opt);
    });
  }

  async function loadCommunicationTypesIntoSelect(selectEl, selectedId = null) {
    if (!selectEl) {
      console.warn("communication type select not found");
      return;
    }

    const base = window.API_BASE || "";
    const res = await fetch(`${base}/api/SitesIndex/meta/communication-types`, {
      credentials: "include",
      headers: { Accept: "application/json" }
    });

    console.log("communication types response status:", res.status);

    if (!res.ok) throw new Error("Failed to load communication types");

    const data = await res.json();
    console.log("communication types data:", data);

    selectEl.innerHTML = '<option value="">-- Válassz kommunikációs módot --</option>';

    data.forEach(x => {
      const opt = document.createElement("option");
      opt.value = x.id;
      opt.textContent = x.name ?? "";

      if (selectedId && String(selectedId) === String(opt.value)) {
        opt.selected = true;
      }

      selectEl.appendChild(opt);
    });

    console.log("communication type options after load:", selectEl.options.length);
  }

  async function loadSiteTypesIntoSelect(selectEl, selectedId = null) {
    if (!selectEl) {
      console.warn("site type select not found");
      return;
    }

    const base = window.API_BASE || "";
    const res = await fetch(`${base}/api/SitesIndex/meta/site-types`, {
      credentials: "include",
      headers: { Accept: "application/json" }
    });

    console.log("site types response status:", res.status);

    if (!res.ok) throw new Error("Failed to load site types");

    const data = await res.json();
    console.log("site types data:", data);

    selectEl.innerHTML = '<option value="">-- Válassz telephely típust --</option>';

    data.forEach(x => {
      const opt = document.createElement("option");
      opt.value = x.id;
      opt.textContent = x.name ?? "";

      if (selectedId && String(selectedId) === String(opt.value)) {
        opt.selected = true;
      }

      selectEl.appendChild(opt);
    });

    console.log("site type options after load:", selectEl.options.length);
  }

  function nullIfEmpty(v) {
    const s = (v ?? "").toString().trim();
    return s ? s : null;
  }

  function isCheckedById(id) {
    return document.getElementById(id)?.checked === true;
  }

  function resetForm() {
    form.classList.remove("was-validated");
    form.reset();

    // TomSelect partner clear
    const ts = partnerSelectEl?.tomselect;
    if (ts) ts.clear(true);

    // defaultok
    const country = form.querySelector('[name="country"]');
    if (country && !country.value) {
      country.value = "Magyarország";
    }

    const isActive = document.getElementById("createSiteIsActive");
    if (isActive) isActive.checked = true;

    if (communicationTypeSelectEl) {
      communicationTypeSelectEl.value = "";
    }

    if (siteTypeSelectEl) {
      siteTypeSelectEl.value = "";
    }
  }

  form.addEventListener("submit", async (e) => {
    e.preventDefault();

    if (!form.checkValidity()) {
      form.classList.add("was-validated");
      return;
    }

    const ts = partnerSelectEl?.tomselect;
    const partnerId = Number(ts ? ts.getValue() : (partnerSelectEl?.value || 0));

    if (!partnerId) {
      window.c92?.showToast?.("error", "Partner megadása kötelező");
      form.classList.add("was-validated");
      return;
    }

    const statusIdRaw = valByName("statusId");
    const defaultCommunicationTypeIdRaw = valByName("defaultCommunicationTypeId");
    const siteTypeIdRaw = valByName("siteTypeId");

    const dto = {
      siteId: 0,
      partnerId,

      siteName: nullIfEmpty(valByName("siteName")),
      siteTypeId: siteTypeIdRaw ? Number(siteTypeIdRaw) : null,

      addressLine1: nullIfEmpty(valByName("addressLine1")),
      addressLine2: nullIfEmpty(valByName("addressLine2")),
      city: nullIfEmpty(valByName("city")),
      state: nullIfEmpty(valByName("state")),
      postalCode: nullIfEmpty(valByName("postalCode")),
      country: nullIfEmpty(valByName("country")),

      contactPerson1: nullIfEmpty(valByName("contactPerson1")),
      contactPerson2: nullIfEmpty(valByName("contactPerson2")),
      contactPerson3: nullIfEmpty(valByName("contactPerson3")),

      phone1: nullIfEmpty(valByName("phone1")),
      phone2: nullIfEmpty(valByName("phone2")),
      phone3: nullIfEmpty(valByName("phone3")),

      mobilePhone1: nullIfEmpty(valByName("mobilePhone1")),
      mobilePhone2: nullIfEmpty(valByName("mobilePhone2")),
      mobilePhone3: nullIfEmpty(valByName("mobilePhone3")),

      messagingApp1: nullIfEmpty(valByName("messagingApp1")),
      messagingApp2: nullIfEmpty(valByName("messagingApp2")),
      messagingApp3: nullIfEmpty(valByName("messagingApp3")),

      eMail1: nullIfEmpty(valByName("eMail1")),
      eMail2: nullIfEmpty(valByName("eMail2")),

      comment1: nullIfEmpty(valByName("comment1")),
      comment2: nullIfEmpty(valByName("comment2")),

      statusId: statusIdRaw ? Number(statusIdRaw) : null,
      defaultCommunicationTypeId: defaultCommunicationTypeIdRaw
        ? Number(defaultCommunicationTypeIdRaw)
        : null,

      isPrimary: isCheckedById("createSiteIsPrimary"),
      isActive: isCheckedById("createSiteIsActive")
    };

    console.log("📦 create dto:", dto);

    const saveBtn = form.querySelector('button[type="submit"]');
    if (saveBtn) saveBtn.disabled = true;

    try {
      const res = await fetch("/api/SitesIndex", {
        method: "POST",
        credentials: "same-origin",
        headers: {
          "Content-Type": "application/json",
          Accept: "application/json",
          ...(csrf ? { RequestVerificationToken: csrf } : {})
        },
        body: JSON.stringify(dto)
      });

      console.log("POST /api/SitesIndex status:", res.status);

      if (!res.ok) {
        const raw = await res.text().catch(() => "");
        let err = {};

        try {
          err = raw ? JSON.parse(raw) : {};
        } catch {
          // ignore
        }

        window.c92?.showToast?.(
          "error",
          err?.errors?.PartnerId?.[0] ||
            err?.errors?.SiteName?.[0] ||
            err?.errors?.SiteTypeId?.[0] ||
            err?.errors?.DefaultCommunicationTypeId?.[0] ||
            err?.title ||
            err?.message ||
            raw ||
            `HTTP ${res.status}`
        );
        return;
      }

      const createdRow = await res.json().catch(() => null);
      console.log("✅ createdRow:", createdRow);

      window.c92?.showToast?.("success", "Telephely létrehozva!");

      bootstrap.Modal.getInstance(modalEl)?.hide();
      window.Sites?.reload?.();
    } catch (err) {
      console.error(err);
      window.c92?.showToast?.("error", "Hálózati hiba");
    } finally {
      if (saveBtn) saveBtn.disabled = false;
    }
  });

  modalEl.addEventListener("hidden.bs.modal", () => {
    resetForm();
  });

  modalEl.addEventListener("shown.bs.modal", async () => {
    console.log("modal shown");
    console.log("communicationTypeSelectEl options before load:", communicationTypeSelectEl?.options?.length);
    console.log("siteTypeSelectEl options before load:", siteTypeSelectEl?.options?.length);

    try {
      if (statusSelectEl && statusSelectEl.options.length <= 1) {
        await loadStatusesIntoSelect(statusSelectEl, 8);
      }

      if (communicationTypeSelectEl && communicationTypeSelectEl.options.length <= 1) {
        console.log("loading communication types...");
        await loadCommunicationTypesIntoSelect(communicationTypeSelectEl, null);
      } else {
        console.log("communication type select missing or already loaded");
      }

      if (siteTypeSelectEl && siteTypeSelectEl.options.length <= 1) {
        console.log("loading site types...");
        await loadSiteTypesIntoSelect(siteTypeSelectEl, null);
      } else {
        console.log("site type select missing or already loaded");
      }
    } catch (e) {
      console.error("Failed to load modal select data:", e);
    }
  });
});