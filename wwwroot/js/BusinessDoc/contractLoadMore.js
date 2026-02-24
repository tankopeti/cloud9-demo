// wwwroot/js/BusinessDoc/contractLoadMore.js
(function () {
  'use strict';

  document.addEventListener('DOMContentLoaded', function () {
    console.log('[contractLoadMore] init');

    var wrap = document.getElementById('contractsTableWrap');
    var tbody = document.getElementById('contractsTableBody');
    var btn = document.getElementById('loadMoreContractsBtn');
    var info = document.getElementById('contractsLoadInfo');

    if (!wrap || !tbody || !btn) {
      console.warn('[contractLoadMore] missing elements');
      return;
    }

    // --- basic params from dataset ---
    var pageSize = parseInt(wrap.dataset.pageSize || '20', 10) || 20;
    var sortBy = (wrap.dataset.sortBy || wrap.dataset.sort || 'date_desc').trim() || 'date_desc';
    var search = (wrap.dataset.search || '').trim();

    // pagination
    var page = 0; // nálunk skip/take lesz, ezért page 0-ról indul
    var isLoading = false;
    var renderedIds = new Set();
    var reachedEnd = false;
    var totalCount = null; // ha a backend visszaadja egyszer

    // Advanced filters (modalból / queryből)
    var activeFilters = {}; // pl: { statusId:"2", partnerId:"10", issueFrom:"2026-01-01", ... }
    window.__CONTRACTS_FILTER_AJAX__ = true;

    // --------------------------------------------------
    // Helpers
    // --------------------------------------------------
    function esc(s) {
      var d = document.createElement('div');
      d.textContent = s == null ? '' : String(s);
      return d.innerHTML;
    }

    function formatDateHu(v) {
      if (!v) return '–';
      var d = new Date(v);
      if (isNaN(d.getTime())) return '–';
      return d.toLocaleDateString('hu-HU', { year: 'numeric', month: '2-digit', day: '2-digit' });
    }

    function setInfo(text) {
      if (!info) return;
      info.textContent = text || '';
    }

    function setInfoLoaded() {
      if (!info) return;
      var loaded = renderedIds.size;
      if (typeof totalCount === 'number') {
        info.textContent = 'Betöltve: ' + Math.min(loaded, totalCount) + ' / ' + totalCount;
      } else {
        info.textContent = 'Betöltve: ' + loaded;
      }
    }

    function setButtonLoading(loading) {
      btn.disabled = !!loading;
      btn.textContent = loading ? 'Betöltés...' : (reachedEnd ? 'Nincs több' : 'Több betöltése');
    }

    function setNoMore() {
      reachedEnd = true;
      btn.disabled = true;
      btn.textContent = 'Nincs több';
    }

    // Bizonylat DTO kompatibilitás (camelCase / PascalCase)
    function pick(obj, keys, fallback) {
      for (var i = 0; i < keys.length; i++) {
        var k = keys[i];
        if (obj && obj[k] != null) return obj[k];
      }
      return fallback;
    }

    // --------------------------------------------------
    // Render row
    // --------------------------------------------------
    function renderRow(d) {
      // id
      var id = Number(pick(d, ['businessDocumentId', 'BusinessDocumentId'], 0));
      if (!Number.isFinite(id) || id <= 0) id = 0;

      var docNo = pick(d, ['documentNo', 'DocumentNo'], '') || '';
      var subject = pick(d, ['subject', 'Subject'], '') || '';

      // Dátum (IssueDate)
      var issueDate = pick(d, ['issueDate', 'IssueDate'], null);

      // Type/Status: a list DTO-ban nálunk csak ID van, name nincs.
      // Itt ezért fallback: TypeId/StatusId.
      var typeId = pick(d, ['businessDocumentTypeId', 'BusinessDocumentTypeId'], '');
      var statusId = pick(d, ['businessDocumentStatusId', 'BusinessDocumentStatusId'], '');

      // CRM-es oszlopok nálad (megbízó/telephely/partner/megbízott)
      // Ezeket tipikusan Parties-ből kellene összerakni, de a list DTO-ban nincs Parties.
      // Ha a backend ad külön mezőket (pl. BuyerName, SiteName, etc.), akkor itt felvesszük:
      var principal = pick(d, ['principalName', 'PrincipalName', 'buyerName', 'BuyerName'], '–');
      var site = pick(d, ['siteName', 'SiteName'], '–');
      var partner = pick(d, ['partnerName', 'PartnerName'], '–');
      var assignee = pick(d, ['assigneeName', 'AssigneeName', 'sellerName', 'SellerName'], '–');

      // status megjelenítés: ha a backend ad statusName/color, azt használjuk
      var statusName = pick(d, ['statusName', 'StatusName', 'businessDocumentStatusName', 'BusinessDocumentStatusName'], '');
      var statusColor = (pick(d, ['statusColor', 'StatusColor', 'color', 'Color'], '') || '').trim() || '#6c757d';

      var statusText = statusName || ('#' + esc(statusId));

      var tr = document.createElement('tr');
      tr.setAttribute('data-doc-id', String(id));

      tr.innerHTML = `
        <td>${esc(typeId)}</td>
        <td>${esc(principal)}</td>
        <td>${esc(site)}</td>
        <td>${esc(partner)}</td>
        <td>${esc(assignee)}</td>
        <td>${esc(subject || docNo)}</td>
        <td>${esc(formatDateHu(issueDate))}</td>
        <td class="text-center">
          <span class="badge text-white" style="background:${esc(statusColor)}">${statusText}</span>
        </td>
        <td class="text-center">
          <div class="btn-group btn-group-sm" role="group">
            <button type="button"
                    class="btn btn-outline-info js-view-contract"
                    data-doc-id="${esc(id)}"
                    title="Megtekintés">
              <i class="bi bi-eye"></i>
            </button>

            <div class="dropdown">
              <button class="btn btn-outline-secondary dropdown-toggle btn-sm"
                      type="button"
                      data-bs-toggle="dropdown"
                      aria-expanded="false">
                <i class="bi bi-three-dots-vertical"></i>
              </button>

              <ul class="dropdown-menu dropdown-menu-end">
                <li>
                  <a class="dropdown-item js-edit-contract" href="#" data-doc-id="${esc(id)}">
                    <i class="bi bi-pencil-square me-2"></i> Szerkesztés
                  </a>
                </li>
                <li>
                  <a class="dropdown-item js-contract-attachments" href="#" data-doc-id="${esc(id)}">
                    <i class="bi bi-paperclip me-2"></i> Csatolmányok
                  </a>
                </li>
                <li>
                  <a class="dropdown-item js-contract-history" href="#" data-doc-id="${esc(id)}">
                    <i class="bi bi-clock-history me-2"></i> Előzmények
                  </a>
                </li>
                <li><hr class="dropdown-divider"></li>
                <li>
                  <a class="dropdown-item text-danger js-delete-contract" href="#" data-doc-id="${esc(id)}">
                    <i class="bi bi-trash me-2"></i> Törlés
                  </a>
                </li>
              </ul>
            </div>
          </div>
        </td>
      `;

      return tr;
    }

    // --------------------------------------------------
    // Build query for backend
    // --------------------------------------------------
    function buildSearchParams(skip, take) {
      var qs = new URLSearchParams();

      // paging
      qs.set('skip', String(skip));
      qs.set('take', String(take));

      // search
      if (search) qs.set('docNo', search); // vagy Subject/DocumentNo külön; nálad SearchTerm lehet

      // sortBy -> query (itt a te controllered paraméterneveit használjuk)
      // A te controllered: typeId, statusId, docNo, subject, issueFrom, issueTo, buyerPartnerId, buyerRoleId, skip, take
      // sortot a backend még nem fogad – ha később bevezeted, itt hozzáadhatod.

      // Advanced filters
      if (activeFilters && typeof activeFilters === 'object') {
        Object.keys(activeFilters).forEach(function (k) {
          var v = activeFilters[k];
          if (v == null) return;
          v = String(v).trim();
          if (!v) return;

          // védelem: paging/search paramokat ne lehessen felülírni
          if (k === 'skip' || k === 'take' || k === 'docNo' || k === 'subject') return;

          qs.set(k, v);
        });
      }

      // ✅ Fix: csak Contract típus (ha tudod a typeId-t)
      // ha van wrap.dataset.contractTypeId, automatikusan ráteszi
      var contractTypeId = (wrap.dataset.contractTypeId || '').trim();
      if (contractTypeId) qs.set('typeId', contractTypeId);

      return qs;
    }

    async function fetchPage() {
      var skip = page * pageSize;
      var take = pageSize;

      var qs = buildSearchParams(skip, take);
      var url = '/api/business-documents?' + qs.toString();

      var res = await fetch(url, { headers: { 'Accept': 'application/json' } });
      if (!res.ok) {
        var txt = await res.text().catch(function () { return ''; });
        throw new Error('HTTP ' + res.status + ' :: ' + txt);
      }

      var json = await res.json();

      // controllered jelenleg List<BusinessDocumentDto>-t ad vissza -> ez array.
      // ha később {items,totalCount} lesz, ezt már kezeli.
      var items = Array.isArray(json) ? json : (json.items || json.Items || []);
      var tc = Array.isArray(json) ? null : (json.totalCount ?? json.TotalCount ?? null);

      return { items: items, totalCount: tc };
    }

    async function loadMore() {
      if (isLoading || reachedEnd) return;

      isLoading = true;
      setButtonLoading(true);
      setInfo('Betöltés...');

      try {
        var result = await fetchPage();
        var items = result.items || [];

        if (result.totalCount != null) {
          var n = parseInt(String(result.totalCount), 10);
          if (Number.isFinite(n)) totalCount = n;
        }

        if (!items.length) {
          setInfoLoaded();
          setNoMore();
          return;
        }

        items.forEach(function (d) {
          var id = Number(pick(d, ['businessDocumentId', 'BusinessDocumentId'], 0));
          if (!Number.isFinite(id) || id <= 0) return;
          if (renderedIds.has(id)) return;

          renderedIds.add(id);
          tbody.appendChild(renderRow(d));
        });

        // ha kevesebb jött, mint pageSize, valószínű vége
        if (items.length < pageSize) {
          if (typeof totalCount !== 'number') setNoMore();
          else page += 1;
        } else {
          page += 1;
        }

        setInfoLoaded();

        if (typeof totalCount === 'number' && renderedIds.size >= totalCount) {
          setNoMore();
        }

      } catch (e) {
        console.error('[contractLoadMore] load failed', e);
        setInfo('Hiba a betöltéskor (nézd meg a konzolt).');
      } finally {
        isLoading = false;
        setButtonLoading(false);
      }
    }

    // --------------------------------------------------
    // Click delegation
    // --------------------------------------------------
    if (wrap._delegationAbortController) {
      try { wrap._delegationAbortController.abort(); } catch (e) { }
    }
    var ac = new AbortController();
    wrap._delegationAbortController = ac;

    wrap.addEventListener('click', function (e) {
      var view = e.target.closest('.js-view-contract');
      if (view) {
        e.preventDefault();
        var id = parseInt(view.dataset.docId, 10);
        if (!Number.isFinite(id)) return;

        window.dispatchEvent(new CustomEvent('contracts:view', { detail: { id: id } }));
        return;
      }

      var edit = e.target.closest('.js-edit-contract');
      if (edit) {
        e.preventDefault();
        var id2 = parseInt(edit.dataset.docId, 10);
        if (!Number.isFinite(id2)) return;

        window.dispatchEvent(new CustomEvent('contracts:edit', { detail: { id: id2 } }));
        return;
      }

      var att = e.target.closest('.js-contract-attachments');
      if (att) {
        e.preventDefault();
        var id3 = parseInt(att.dataset.docId, 10);
        if (!Number.isFinite(id3)) return;

        window.dispatchEvent(new CustomEvent('contracts:attachments', { detail: { id: id3 } }));
        return;
      }

      var hist = e.target.closest('.js-contract-history');
      if (hist) {
        e.preventDefault();
        var id4 = parseInt(hist.dataset.docId, 10);
        if (!Number.isFinite(id4)) return;

        window.dispatchEvent(new CustomEvent('contracts:history', { detail: { id: id4 } }));
        return;
      }

      var del = e.target.closest('.js-delete-contract');
      if (del) {
        e.preventDefault();
        var id5 = parseInt(del.dataset.docId, 10);
        if (!Number.isFinite(id5)) return;

        window.dispatchEvent(new CustomEvent('contracts:delete', { detail: { id: id5 } }));
        return;
      }
    }, { signal: ac.signal });

    btn.addEventListener('click', function (e) {
      e.preventDefault();
      loadMore();
    });

    // --------------------------------------------------
    // Optional: external filter apply/reset events (ha lesz filter modal JS)
    // --------------------------------------------------
    window.addEventListener('contracts:applyFilters', function (e) {
      var params = e && e.detail && (e.detail.params || e.detail);
      if (!params || typeof params !== 'object') return;

      var cleaned = {};
      Object.keys(params).forEach(function (k) {
        var v = params[k];
        if (v == null) return;
        v = String(v).trim();
        if (!v) return;

        if (k === 'skip' || k === 'take' || k === 'docNo' || k === 'subject') return;
        cleaned[k] = v;
      });

      activeFilters = cleaned;

      page = 0;
      renderedIds.clear();
      reachedEnd = false;
      totalCount = null;

      tbody.innerHTML = '';
      btn.disabled = false;
      btn.textContent = 'Több betöltése';
      setInfo('');

      loadMore();
    });

    window.addEventListener('contracts:clearFilters', function () {
      activeFilters = {};

      page = 0;
      renderedIds.clear();
      reachedEnd = false;
      totalCount = null;

      tbody.innerHTML = '';
      btn.disabled = false;
      btn.textContent = 'Több betöltése';
      setInfo('');

      loadMore();
    });

    window.addEventListener('contracts:reload', function () {
      page = 0;
      renderedIds.clear();
      reachedEnd = false;
      totalCount = null;

      tbody.innerHTML = '';
      btn.disabled = false;
      btn.textContent = 'Több betöltése';
      setInfo('');

      loadMore();
    });

    // init
    loadMore();
  });
})();
