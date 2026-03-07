// wwwroot/js/HR/employee-files.js
(() => {
  "use strict";

  const modalEl = document.getElementById("employeeDocumentsModal");
  if (!modalEl) return;

  const employeeNameEl = document.getElementById("employee-documents-employee-name");
  const listEl = document.getElementById("employee-documents-list");
  const emptyEl = document.getElementById("employee-documents-empty");
  const errorEl = document.getElementById("employee-documents-error");

  const fileInput = document.getElementById("employee-document-file");
  const docNameInput = document.getElementById("employee-document-name");
  const uploadBtn = document.getElementById("employee-document-upload");
  const refreshBtn = document.getElementById("employee-documents-refresh");

  let currentEmployeeId = null;
  const bsModal = window.bootstrap?.Modal?.getOrCreateInstance(modalEl);

  function setError(msg) {
    if (!errorEl) return;

    if (!msg) {
      errorEl.classList.add("d-none");
      errorEl.textContent = "";
      return;
    }
    errorEl.textContent = msg;
    errorEl.classList.remove("d-none");
  }

  function setEmptyState(isEmpty) {
    if (!emptyEl) return;
    emptyEl.style.display = isEmpty ? "block" : "none";
  }

  function escapeHtml(str) {
    const s = String(str ?? "");
    return s
      .replaceAll("&", "&amp;")
      .replaceAll("<", "&lt;")
      .replaceAll(">", "&gt;")
      .replaceAll('"', "&quot;")
      .replaceAll("'", "&#039;");
  }

  function formatBytes(bytes) {
    if (bytes == null) return "";
    const b = Number(bytes);
    if (Number.isNaN(b) || b <= 0) return "0 B";
    const units = ["B", "KB", "MB", "GB"];
    let i = 0;
    let v = b;
    while (v >= 1024 && i < units.length - 1) {
      v /= 1024;
      i++;
    }
    return `${v.toFixed(i === 0 ? 0 : 1)} ${units[i]}`;
  }

  async function loadDocuments() {
    setError(null);
    listEl.innerHTML = "";
    setEmptyState(false);

    if (!currentEmployeeId) {
      setEmptyState(true);
      return;
    }

    try {
      const res = await fetch(`/api/employee/${currentEmployeeId}/documents?includeInactive=false&skip=0&take=200`, {
        headers: { "Accept": "application/json" }
      });

      if (!res.ok) {
        const txt = await res.text();
        throw new Error(`Lista betöltés sikertelen (${res.status}): ${txt}`);
      }

      const items = await res.json();

      if (!Array.isArray(items) || items.length === 0) {
        setEmptyState(true);
        return;
      }

      for (const d of items) {
        const displayName = (d.documentName && String(d.documentName).trim())
          ? d.documentName
          : (d.originalFileName || d.fileName || `#${d.documentId}`);

        const meta = [
          d.uploadDate ? new Date(d.uploadDate).toLocaleString() : null,
          d.fileSizeBytes != null ? formatBytes(d.fileSizeBytes) : null,
          d.uploadedBy ? d.uploadedBy : null
        ].filter(Boolean).join(" • ");

        const li = document.createElement("li");
        li.className = "list-group-item d-flex justify-content-between align-items-start";

        li.innerHTML = `
          <div class="me-2">
            <div class="fw-semibold">${escapeHtml(displayName)}</div>
            <div class="small text-muted">${escapeHtml(meta)}</div>
          </div>

          <div class="btn-group btn-group-sm" role="group">
            <a class="btn btn-outline-secondary"
               href="/api/documents/${d.documentId}/download"
               target="_blank"
               rel="noopener"
               title="Letöltés">
              <i class="bi bi-download"></i>
            </a>

            <button class="btn btn-outline-danger employee-doc-delete"
                    type="button"
                    data-document-id="${d.documentId}"
                    title="Törlés">
              <i class="bi bi-trash"></i>
            </button>
          </div>
        `;

        listEl.appendChild(li);
      }
    } catch (err) {
      setEmptyState(true);
      setError(err?.message ?? "Ismeretlen hiba történt.");
    }
  }

  async function uploadDocument() {
    setError(null);

    if (!currentEmployeeId) {
      setError("Nincs kiválasztott dolgozó.");
      return;
    }

    const file = fileInput?.files?.[0];
    if (!file) {
      setError("Válassz ki egy fájlt.");
      return;
    }

    uploadBtn.disabled = true;
    uploadBtn.innerHTML = `<span class="spinner-border spinner-border-sm me-2"></span>Feltöltés...`;

    try {
      const payload = {
        fileName: file.name,
        status: "Beérkezett",
        documentName: (docNameInput?.value || "").trim() || null,
        documentDescription: null,
        documentTypeId: null,
        partnerId: null,
        siteId: null,
        contactId: null,
        contentType: file.type || null,
        customMetadata: []
      };

      const fd = new FormData();
      fd.append("file", file);
      fd.append("payloadJson", JSON.stringify(payload));

      const res = await fetch(`/api/employee/${currentEmployeeId}/documents`, {
        method: "POST",
        body: fd
      });

      if (!res.ok) {
        const txt = await res.text();
        throw new Error(`Feltöltés sikertelen (${res.status}): ${txt}`);
      }

      fileInput.value = "";
      docNameInput.value = "";

      await loadDocuments();
    } catch (err) {
      setError(err?.message ?? "Feltöltés hiba.");
    } finally {
      uploadBtn.disabled = false;
      uploadBtn.innerHTML = `<i class="bi bi-cloud-upload me-1"></i> Hozzáadás / Feltöltés`;
    }
  }

  async function deleteDocument(documentId) {
    setError(null);
    if (!documentId) return;

    const ok = confirm("Biztosan törlöd a dokumentumot?");
    if (!ok) return;

    try {
      const res = await fetch(`/api/documents/${documentId}`, { method: "DELETE" });
      if (!res.ok) {
        const txt = await res.text();
        throw new Error(`Törlés sikertelen (${res.status}): ${txt}`);
      }

      await loadDocuments();
    } catch (err) {
      setError(err?.message ?? "Törlés hiba.");
    }
  }

  uploadBtn?.addEventListener("click", (e) => {
    e.preventDefault();
    uploadDocument();
  });

  refreshBtn?.addEventListener("click", (e) => {
    e.preventDefault();
    loadDocuments();
  });

  listEl?.addEventListener("click", (e) => {
    const btn = e.target.closest(".employee-doc-delete");
    if (!btn) return;
    e.preventDefault();
    const id = Number(btn.dataset.documentId);
    deleteDocument(id);
  });

  // PUBLIC: ezt hívja a wireRowActions() a dropdownból
  window.openEmployeeFilesModal = async function (employeeId, employeeName) {
    currentEmployeeId = employeeId;
    if (employeeNameEl) employeeNameEl.textContent = employeeName || "";
    setError(null);

    listEl.innerHTML = "";
    setEmptyState(false);

    fileInput.value = "";
    docNameInput.value = "";

    bsModal?.show();
    await loadDocuments();
  };
})();