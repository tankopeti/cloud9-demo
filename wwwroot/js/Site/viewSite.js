// /js/Site/viewSite.js
(function () {
  'use strict';

  document.addEventListener('DOMContentLoaded', () => {
    document.addEventListener('click', async (e) => {
      const btn = e.target.closest('.view-site-btn');
      if (!btn) return;

      const siteId = btn.dataset.siteId;
      if (!siteId) return;

      const modalEl = document.getElementById('viewSiteModal');
      const contentEl = document.getElementById('viewSiteContent');
      if (!modalEl || !contentEl) return;

      bootstrap.Modal.getOrCreateInstance(modalEl).show();
      contentEl.innerHTML = loadingHtml('Adatok betöltése...');

      try {
        const res = await fetch(`/api/SitesIndex/${encodeURIComponent(siteId)}`, {
          credentials: 'same-origin',
          headers: { 'Accept': 'application/json' }
        });

        if (!res.ok) {
          throw new Error(res.status === 404 ? 'Telephely nem található' : `HTTP ${res.status}`);
        }

        const d = await res.json();

        // ---- normalize ----
        const id = d.siteId ?? d.SiteId ?? siteId;
        const siteName = d.siteName ?? d.SiteName ?? 'Telephely';

        const partnerName =
          d.partnerName ??
          d.PartnerName ??
          d.partner?.companyName ??
          d.partner?.name ??
          d.Partner?.CompanyName ??
          d.Partner?.Name ??
          '—';

        const partnerIdVal = d.partnerId ?? d.PartnerId ?? '—';

        const status = d.status ?? d.Status ?? null;
        const isPrimary = (d.isPrimary ?? d.IsPrimary) === true;
        const isActive = (d.isActive ?? d.IsActive) !== false;

        const addr1 = d.addressLine1 ?? d.AddressLine1;
        const addr2 = d.addressLine2 ?? d.AddressLine2;
        const city = d.city ?? d.City;
        const state = d.state ?? d.State;
        const postal = d.postalCode ?? d.PostalCode;
        const country = d.country ?? d.Country;

        const cp1 = d.contactPerson1 ?? d.ContactPerson1;
        const cp2 = d.contactPerson2 ?? d.ContactPerson2;
        const cp3 = d.contactPerson3 ?? d.ContactPerson3;

        const phone1 = d.phone1 ?? d.Phone1;
        const phone2 = d.phone2 ?? d.Phone2;
        const phone3 = d.phone3 ?? d.Phone3;

        const mobile1 = d.mobilePhone1 ?? d.MobilePhone1;
        const mobile2 = d.mobilePhone2 ?? d.MobilePhone2;
        const mobile3 = d.mobilePhone3 ?? d.MobilePhone3;

        const msg1 = d.messagingApp1 ?? d.MessagingApp1;
        const msg2 = d.messagingApp2 ?? d.MessagingApp2;
        const msg3 = d.messagingApp3 ?? d.MessagingApp3;

        const mail1 = d.eMail1 ?? d.EMail1 ?? d.Email1;
        const mail2 = d.eMail2 ?? d.EMail2 ?? d.Email2;

        const c1 = d.comment1 ?? d.Comment1;
        const c2 = d.comment2 ?? d.Comment2;

        const defaultCommunicationTypeName =
          d.defaultCommunicationTypeName ??
          d.DefaultCommunicationTypeName ??
          '—';

        // ✅ site type
        const siteTypeName =
          d.siteType?.name ??
          d.SiteType?.Name ??
          d.siteTypeName ??
          d.SiteTypeName ??
          '—';

        const linkedPartners = d.linkedPartners ?? d.LinkedPartners ?? [];
        const linkedEmployees = d.linkedEmployees ?? d.LinkedEmployees ?? [];

        // ---- render ----
        contentEl.innerHTML = `
          <div class="d-flex justify-content-between align-items-start gap-3 mb-3">
            <div>
              <h4 class="fw-bold mb-1">${esc(siteName)}</h4>
              <div class="text-muted small">ID: <strong>#${esc(id)}</strong></div>
            </div>
            <div class="text-end">
              <div>${renderStatus(status)}</div>
              <div class="mt-2">
                ${isActive ? `<span class="badge bg-success">Aktív</span>` : `<span class="badge bg-secondary">Inaktív</span>`}
                ${isPrimary ? `<span class="badge bg-primary ms-1">Elsődleges</span>` : ``}
              </div>
            </div>
          </div>

<div class="row g-4">

  <div class="col-lg-6">

    ${card('Alapadatok', `
      <div class="d-flex justify-content-between">
        <strong>Telephely neve:</strong>
        <span class="ms-3 text-end">${esc(siteName)}</span>
      </div>
      <div class="d-flex justify-content-between">
        <strong>Partner:</strong>
        <span class="ms-3 text-end">${esc(partnerName)}</span>
      </div>
      <div class="d-flex justify-content-between">
        <strong>PartnerId:</strong>
        <span class="ms-3 text-end">${esc(partnerIdVal)}</span>
      </div>
      <div class="d-flex justify-content-between">
        <strong>Telephely típusa:</strong>
        <span class="ms-3 text-end">${mutedIfEmpty(siteTypeName)}</span>
      </div>
      <hr/>
      <div class="d-flex justify-content-between">
        <strong>Státusz:</strong>
        <span class="ms-3 text-end">${renderStatus(status)}</span>
      </div>
      <div class="d-flex justify-content-between">
        <strong>Alapértelmezett kommunikáció:</strong>
        <span class="ms-3 text-end">${mutedIfEmpty(defaultCommunicationTypeName)}</span>
      </div>
      <div class="d-flex justify-content-between">
        <strong>Elsődleges:</strong>
        <span class="ms-3 text-end">${isPrimary ? 'Igen' : 'Nem'}</span>
      </div>
      <div class="d-flex justify-content-between">
        <strong>Aktív:</strong>
        <span class="ms-3 text-end">${isActive ? 'Igen' : 'Nem'}</span>
      </div>
    `, 'text-info')}

    ${card('Cím adatok', `
      <div><strong>Cím 1:</strong> ${mutedIfEmpty(addr1)}</div>
      <div><strong>Cím 2:</strong> ${mutedIfEmpty(addr2)}</div>
      <hr/>
      <div><strong>Város:</strong> ${mutedIfEmpty(city)}</div>
      <div><strong>Megye:</strong> ${mutedIfEmpty(state)}</div>
      <div><strong>Irányítószám:</strong> ${mutedIfEmpty(postal)}</div>
      <div><strong>Ország:</strong> ${mutedIfEmpty(country)}</div>
    `, 'text-info')}

    ${card('Kapcsolt személyek', renderLinkedEmployees(linkedEmployees), 'text-info')}
    ${card('Kapcsolt partnerek', renderLinkedPartners(linkedPartners), 'text-info')}

  </div>

  <div class="col-lg-6">

    ${card('Kapcsolattartók', `
      <div><strong>Kapcsolattartó 1:</strong> ${mutedIfEmpty(cp1)}</div>
      <div><strong>Kapcsolattartó 2:</strong> ${mutedIfEmpty(cp2)}</div>
      <div><strong>Kapcsolattartó 3:</strong> ${mutedIfEmpty(cp3)}</div>
    `, 'text-info')}

    ${card('Elérhetőségek', `
      <div class="row g-2">
        <div class="col-md-6"><div class="text-muted small">Vezetékes (1)</div><div>${mutedIfEmpty(phone1)}</div></div>
        <div class="col-md-6"><div class="text-muted small">Vezetékes (2)</div><div>${mutedIfEmpty(phone2)}</div></div>
        <div class="col-md-6"><div class="text-muted small">Vezetékes (3)</div><div>${mutedIfEmpty(phone3)}</div></div>

        <div class="col-md-6"><div class="text-muted small">Mobil (1)</div><div>${mutedIfEmpty(mobile1)}</div></div>
        <div class="col-md-6"><div class="text-muted small">Mobil (2)</div><div>${mutedIfEmpty(mobile2)}</div></div>
        <div class="col-md-6"><div class="text-muted small">Mobil (3)</div><div>${mutedIfEmpty(mobile3)}</div></div>

        <div class="col-md-6"><div class="text-muted small">Üzenető app 1</div><div>${mutedIfEmpty(msg1)}</div></div>
        <div class="col-md-6"><div class="text-muted small">Üzenető app 2</div><div>${mutedIfEmpty(msg2)}</div></div>
        <div class="col-md-6"><div class="text-muted small">Üzenető app 3</div><div>${mutedIfEmpty(msg3)}</div></div>

        <div class="col-md-6"><div class="text-muted small">e-Mail (1)</div><div>${mailHtml(mail1)}</div></div>
        <div class="col-md-6"><div class="text-muted small">e-Mail (2)</div><div>${mailHtml(mail2)}</div></div>
      </div>
    `, 'text-info')}

    ${card('Megjegyzések', `
      <div class="mb-3">
        <div class="text-muted small">Megjegyzés 1</div>
        <div class="p-2 bg-body-tertiary rounded">${nlOrDash(c1)}</div>
      </div>
      <div>
        <div class="text-muted small">Megjegyzés 2</div>
        <div class="p-2 bg-body-tertiary rounded">${nlOrDash(c2)}</div>
      </div>
    `, 'text-info')}

  </div>

</div>
        `;
      } catch (err) {
        console.error(err);
        contentEl.innerHTML = `<div class="alert alert-danger m-2"><strong>Hiba:</strong> ${esc(err.message || 'Nem sikerült betölteni')}</div>`;
        window.c92?.showToast?.('error', 'Hiba a telephely betöltésekor');
      }
    });

    // helpers
    function loadingHtml(text) {
      return `<div class="text-center py-5">
          <div class="spinner-border text-info"></div>
          <p class="mt-3 text-muted">${esc(text)}</p>
        </div>`;
    }

    function card(title, bodyHtml, titleClass) {
      return `<div class="p-3 border rounded bg-light">
        <h6 class="${titleClass}">${esc(title)}</h6>
        ${bodyHtml}
      </div><div class="mt-3"></div>`;
    }

    function renderStatus(st) {
      const name = st?.name || st?.Name || '—';
      const color = st?.color || st?.Color || '#6c757d';
      return `<span class="badge text-white" style="background:${esc(color)}">${esc(name)}</span>`;
    }

    function mutedIfEmpty(v) {
      return v ? esc(v) : `<span class="text-muted">—</span>`;
    }

    function mailHtml(v) {
      return v ? `<a href="mailto:${esc(v)}">${esc(v)}</a>` : `<span class="text-muted">—</span>`;
    }

    function nlOrDash(v) {
      return v ? esc(v).replace(/\n/g, '<br>') : `<span class="text-muted">—</span>`;
    }

    function esc(v) {
      const d = document.createElement('div');
      d.textContent = v ?? '';
      return d.innerHTML;
    }

    function escAttr(v) {
      return esc(v).replaceAll('`', '&#096;');
    }

    function renderLinkedPartners(items) {
      if (!items?.length) return `<div class="text-muted">Nincs kapcsolt partner.</div>`;
      return items.map(x => `<div>${esc(x.partnerName)}</div>`).join('');
    }

    function renderLinkedEmployees(items) {
      if (!items?.length) return `<div class="text-muted">Nincs kapcsolt személy.</div>`;
      return items.map(x => `<div>${esc(x.employeeName)}</div>`).join('');
    }

  });
})();