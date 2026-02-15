// /wwwroot/js/Site/siteFilter.js

document.addEventListener('DOMContentLoaded', () => {
  // ✅ dupla init elleni védelem (pl. partial reload / duplán behúzott script esetén)
  if (window.__sitesFilterInitialized) {
    console.warn('siteFilter already initialized, skipping');
    return;
  }
  window.__sitesFilterInitialized = true;

  const tbody = document.getElementById('sitesTableBody');
  const loadMoreBtn = document.getElementById('loadMoreBtn');
  const loadMoreSpinner = document.getElementById('loadMoreSpinner');
  const loadMoreContainer = document.getElementById('loadMoreContainer');

  const searchInput = document.getElementById('siteSearchInput');
  const searchForm = document.getElementById('siteSearchForm');
  const clearBtn = document.getElementById('clearSiteSearchBtn');
  const countText = document.getElementById('sitesCountText');
  const tableWrap = document.querySelector('.table-dynamic-height');


  if (!tbody) {
    console.error('Hiányzik a sitesTableBody');
    return;
  }

  console.log('✅ siteFilter.js betöltve', {
    hasLoadMoreBtn: !!loadMoreBtn,
    hasSearchInput: !!searchInput,
    hasSearchForm: !!searchForm,
    hasClearBtn: !!clearBtn
  });

  let currentPage = 1;

  // ✅ ÁLLÍTSD ITT: 25 vagy 50
  const pageSize = parseInt(tableWrap?.getAttribute('data-page-size') || '20', 10);

  let isLoading = false;
  let hasMore = true;
  let totalCount = 0;

  let filters = {
    search: '',
    filter: 'all' // all | primary
  };

  function buildUrl(page) {
    const p = new URLSearchParams({
      pageNumber: String(page),
      pageSize: String(pageSize),
      search: filters.search || '',
      filter: filters.filter === 'primary' ? 'primary' : ''
    });

    const url = `/api/SitesIndex?${p.toString()}`;
    console.log('🔎 buildUrl:', url);
    return url;
  }

  function setLoadMoreText() {
    if (!loadMoreBtn) return;
    const loaded = tbody.querySelectorAll('tr[data-site-id]').length;
    const total = totalCount || loaded;

    loadMoreBtn.innerHTML =
      `<span class="me-2">Betöltve <strong>${loaded}</strong> / <strong>${total}</strong></span> ` +
      `<span class="opacity-75">Több betöltése</span>`;

    if (countText) {
      countText.textContent = `Betöltve ${loaded} / ${total} telephely`;
    }
  }

  async function loadSites(reset = false) {
    if (isLoading) return;
    isLoading = true;

    loadMoreSpinner?.classList.remove('d-none');

    if (reset) {
      currentPage = 1;
      hasMore = true;
      totalCount = 0;
      tbody.innerHTML = `
        <tr>
          <td colspan="13" class="text-center py-5 text-muted">Betöltés...</td>
        </tr>`;
    }

    try {
      const res = await fetch(buildUrl(currentPage), {
        credentials: 'same-origin',
        headers: {
          'Accept': 'application/json',
          // ✅ dev-ben segít elkerülni "furcsa" cache-t
          'Cache-Control': 'no-cache',
          'Pragma': 'no-cache'
        }
      });

      console.log('📡 /api/SitesIndex status:', res.status);

      if (!res.ok) {
        const raw = await res.text();
        console.error('❌ API error raw:', raw);
        throw new Error(`HTTP ${res.status}`);
      }

      totalCount = parseInt(res.headers.get('X-Total-Count') || '0', 10);
      const data = await res.json();

      console.log('✅ Rows received:', Array.isArray(data) ? data.length : data, {
        totalCount,
        currentPage,
        pageSize,
        search: filters.search,
        filter: filters.filter
      });

      if (reset) tbody.innerHTML = '';
      (data || []).forEach(addRow);

      const loaded = tbody.querySelectorAll('tr[data-site-id]').length;
      hasMore = loaded < totalCount;

      loadMoreContainer?.classList.toggle('d-none', !hasMore);
      setLoadMoreText();

      if (loaded === 0) {
        tbody.innerHTML = `
          <tr>
            <td colspan="13" class="text-center py-5 text-muted">Nincs találat</td>
          </tr>`;
        if (countText) countText.textContent = 'Nincs találat';
      }
    } catch (err) {
      console.error('Site load error:', err);
      tbody.innerHTML = `
        <tr>
          <td colspan="13" class="text-center text-danger py-5">Hiba a telephelyek betöltésekor</td>
        </tr>`;
      loadMoreContainer?.classList.add('d-none');
      if (countText) countText.textContent = 'Hiba a telephelyek betöltésekor';
    } finally {
      isLoading = false;
      loadMoreSpinner?.classList.add('d-none');
    }
  }

  function buildRowHtml(s) {
    const statusColor = s.status?.color || '#6c757d';
    const statusText = s.status?.name || '—';
    const partnerText = s.partnerName || '—';

    return `
<tr data-site-id="${escapeAttr(s.siteId)}">
  <td class="text-nowrap"><i class="bi bi-building me-1"></i>${escapeHtml(s.siteName || '—')}</td>
  <td class="text-nowrap">${escapeHtml(partnerText)}</td>
  <td class="text-nowrap">${escapeHtml(s.addressLine1 || '—')}</td>
  <td class="text-nowrap">${escapeHtml(s.addressLine2 || '—')}</td>
  <td class="text-nowrap">${escapeHtml(s.city || '—')}</td>
  <td class="text-nowrap">${escapeHtml(s.postalCode || '—')}</td>
  <td class="text-nowrap">${escapeHtml(s.contactPerson1 || '—')}</td>
  <td class="text-nowrap">${escapeHtml(s.contactPerson2 || '—')}</td>
  <td class="text-nowrap">${escapeHtml(s.contactPerson3 || '—')}</td>
  <td class="text-nowrap">${escapeHtml(s.siteType?.name || '—')}</td>
  <td class="text-nowrap">
    <span class="badge" style="background:${escapeAttr(statusColor)};color:white">${escapeHtml(statusText)}</span>
  </td>
  <td class="text-nowrap">
    ${s.isPrimary ? `<span class="badge bg-primary">Elsődleges</span>` : '—'}
  </td>
  <td class="text-center">
    <div class="btn-group btn-group-sm" role="group">
      <button type="button" class="btn btn-outline-info view-site-btn" data-site-id="${escapeAttr(s.siteId)}">
        <i class="bi bi-eye"></i>
      </button>

      <div class="dropdown">
        <button class="btn btn-outline-secondary dropdown-toggle btn-sm" type="button" data-bs-toggle="dropdown">
          <i class="bi bi-three-dots-vertical"></i>
        </button>

        <ul class="dropdown-menu dropdown-menu-end">
          <li>
            <a class="dropdown-item edit-site-btn" href="#" data-site-id="${escapeAttr(s.siteId)}">
              <i class="bi bi-pencil-square me-2"></i>Szerkesztés
            </a>
          </li>
          <li><hr class="dropdown-divider"></li>
          <li>
            <a class="dropdown-item text-danger delete-site-btn" href="#" data-site-id="${escapeAttr(s.siteId)}" data-site-name="${escapeAttr(s.siteName || 'Telephely')}">
              <i class="bi bi-trash me-2"></i>Törlés
            </a>
          </li>
        </ul>
      </div>
    </div>
  </td>
</tr>`;
  }

  function addRow(s) {
    // ✅ duplikált sorok elleni védelem (ha valamiért kétszer érkezne ugyanaz)
    const id = String(s.siteId);
    if (tbody.querySelector(`tr[data-site-id="${CSS.escape(id)}"]`)) return;

    tbody.insertAdjacentHTML('beforeend', buildRowHtml(s));
  }

  // ✅ Public API: create/edit/delete után frissítés oldalújratöltés nélkül
  window.Sites = window.Sites || {};
  window.Sites.reload = () => loadSites(true);

  // opcionális: create után azonnali beszúrás (ha akarod reload helyett)
  window.Sites.prependRow = (row) => {
    if (!row) return;

    const first = tbody.querySelector('tr');
    if (first && first.querySelector('td')?.textContent?.includes('Nincs találat')) {
      tbody.innerHTML = '';
    }

    const id = String(row.siteId);
    if (tbody.querySelector(`tr[data-site-id="${CSS.escape(id)}"]`)) return;

    tbody.insertAdjacentHTML('afterbegin', buildRowHtml(row));
    setLoadMoreText();
  };

  // ✅ kereső (debounce) + clear gomb állapota
  if (searchInput) {
    searchInput.addEventListener('input', debounce((e) => {
      filters.search = (e.target.value || '').trim();
      clearBtn?.classList.toggle('d-none', !filters.search);
      console.log('⌨️ search changed:', filters.search);
      loadSites(true);
    }, 300));
  }

  // ✅ Enter / kereső gomb: ne reloadoljon
  if (searchForm) {
    searchForm.addEventListener('submit', (e) => {
      e.preventDefault();
      filters.search = (searchInput?.value || '').trim();
      clearBtn?.classList.toggle('d-none', !filters.search);
      console.log('🟨 form submit search=', filters.search);
      loadSites(true);
    });
  }

  // ✅ X gomb: törlés + reload
  clearBtn?.addEventListener('click', () => {
    if (searchInput) searchInput.value = '';
    filters.search = '';
    clearBtn.classList.add('d-none');
    loadSites(true);
  });

  // filter dropdown (all/primary)
  document.querySelectorAll('[data-filter]').forEach(a => {
    a.addEventListener('click', e => {
      e.preventDefault();
      filters.filter = a.getAttribute('data-filter') || 'all';
      console.log('🧰 filter changed:', filters.filter);
      loadSites(true);
    });
  });

  loadMoreBtn?.addEventListener('click', () => {
    if (!hasMore) return;
    currentPage++;
    loadSites(false);
  });

  // init
  clearBtn?.classList.toggle('d-none', !(filters.search || '').trim());
  loadSites(true);

  function debounce(fn, delay) {
    let t;
    return (...args) => {
      clearTimeout(t);
      t = setTimeout(() => fn(...args), delay);
    };
  }

  function escapeHtml(str) {
    return String(str ?? '')
      .replaceAll('&', '&amp;')
      .replaceAll('<', '&lt;')
      .replaceAll('>', '&gt;')
      .replaceAll('"', '&quot;')
      .replaceAll("'", '&#039;');
  }

  function escapeAttr(str) {
    return escapeHtml(str).replaceAll('`', '&#096;');
  }
});
