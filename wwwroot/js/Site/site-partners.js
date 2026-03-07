// wwwroot/js/Site/site-partners.js
(function () {
    'use strict';

    const SELECTORS = {
        modal: '#sitePartnersModal',
        siteName: '#site-partners-site-name',
        partnerPicker: '#site-partner-picker',
        partnerTypePicker: '#site-partner-type-picker',
        addBtn: '#site-partner-add',
        list: '#site-partners-list',
        empty: '#site-partners-empty',
        error: '#site-partners-error',
        saveBtn: '#site-partners-save'
    };

    const ENDPOINTS = {
        getSitePartners: (siteId) => `/api/sites/${siteId}/partners`,
        putSitePartners: (siteId) => `/api/sites/${siteId}/partners`,
        partnerSearch: `/api/partners/select`,
        partnerTypesMeta: `/api/sites/meta/partner-types`
    };

    const state = {
        siteId: null,
        siteName: '',
        modalInstance: null,
        partnerTom: null,
        partnerTypes: [],
        assignedPartners: [] // [{ partnerId, partnerName, partnerTypeId, partnerTypeName }]
    };

    document.addEventListener('DOMContentLoaded', init);

    function init() {
        const modalEl = document.querySelector(SELECTORS.modal);
        if (!modalEl) return;

        state.modalInstance = bootstrap.Modal.getOrCreateInstance(modalEl);

        wireOpenButtons();
        wireActions();
        initPartnerTomSelect();

        modalEl.addEventListener('hidden.bs.modal', resetModalState);
    }

    function wireOpenButtons() {
        document.addEventListener('click', async function (e) {
            const btn = e.target.closest('.assign-site-partners-btn');
            if (!btn) return;

            e.preventDefault();

            const siteId = btn.dataset.siteId;
            const siteName = btn.dataset.siteName || 'Telephely';

            if (!siteId) return;

            try {
                clearError();
                setBusy(true);

                state.siteId = siteId;
                state.siteName = siteName;

                setSiteName(siteName);
                resetWorkingData();

                await loadPartnerTypes();
                await loadAssignedPartners(siteId);

                renderAssignedPartners();
                state.modalInstance.show();
            } catch (err) {
                console.error('Hiba a partner hozzárendelés modal megnyitásakor:', err);
                showError(getErrorMessage(err, 'Nem sikerült betölteni a partner hozzárendelést.'));
            } finally {
                setBusy(false);
            }
        });
    }

    function wireActions() {
        const addBtn = document.querySelector(SELECTORS.addBtn);
        const saveBtn = document.querySelector(SELECTORS.saveBtn);

        if (addBtn) {
            addBtn.addEventListener('click', onAddPartner);
        }

        if (saveBtn) {
            saveBtn.addEventListener('click', onSavePartners);
        }

        const list = document.querySelector(SELECTORS.list);
        if (list) {
            list.addEventListener('click', function (e) {
                const removeBtn = e.target.closest('.remove-site-partner-btn');
                if (!removeBtn) return;

                const partnerId = removeBtn.dataset.partnerId;
                if (!partnerId) return;

                state.assignedPartners = state.assignedPartners.filter(x => String(x.partnerId) !== String(partnerId));
                renderAssignedPartners();
            });
        }
    }

    function initPartnerTomSelect() {
        const el = document.querySelector(SELECTORS.partnerPicker);
        if (!el) return;

        state.partnerTom = new TomSelect(el, {
            valueField: 'partnerId',
            labelField: 'partnerName',
            searchField: ['partnerName', 'partnerCode', 'email', 'city'],
            create: false,
            preload: false,
            maxOptions: 20,
            placeholder: 'Partner keresése...',
load: async function (query, callback) {
    try {
        const url = buildPartnerSearchUrl(query);
        console.log('🔎 partnerSearch url:', url);

        const res = await fetch(url, {
            headers: { 'X-Requested-With': 'XMLHttpRequest' }
        });

        console.log('📡 partnerSearch status:', res.status);

        if (!res.ok) {
            const raw = await res.text();
            console.error('❌ partnerSearch raw:', raw);
            callback();
            return;
        }

        const json = await res.json();
        console.log('✅ partnerSearch json:', json);

        const items = normalizePartnerSearchItems(json);
        console.log('✅ normalized partners:', items);

        callback(items);
    } catch (err) {
        console.error('Partner keresés hiba:', err);
        callback();
    }
},
            render: {
                option: function (item, escape) {
                    const sub = [
                        item.partnerCode ? escape(item.partnerCode) : null,
                        item.partnerTypeName ? escape(item.partnerTypeName) : null,
                        item.city ? escape(item.city) : null
                    ].filter(Boolean).join(' • ');

                    return `
                        <div>
                            <div>${escape(item.partnerName || '')}</div>
                            ${sub ? `<div class="small text-muted">${sub}</div>` : ''}
                        </div>
                    `;
                },
                item: function (item, escape) {
                    return `<div>${escape(item.partnerName || '')}</div>`;
                }
            }
        });
    }

function buildPartnerSearchUrl(query) {
    const url = new URL(ENDPOINTS.partnerSearch, window.location.origin);

    if (query && query.trim()) {
        url.searchParams.set('search', query.trim());
    }

    return url.toString();
}

    function normalizePartnerSearchItems(json) {
        if (!json) return [];

        const items =
            json.items ||
            json.data ||
            json.results ||
            (Array.isArray(json) ? json : []);

        return (items || []).map(x => ({
            partnerId: x.partnerId ?? x.id ?? x.value,
            partnerName: x.partnerName ?? x.name ?? x.text ?? '',
            partnerCode: x.partnerCode ?? '',
            partnerTypeName: x.partnerTypeName ?? '',
            city: x.city ?? ''
        })).filter(x => x.partnerId && x.partnerName);
    }

async function loadPartnerTypes() {
    const select = document.querySelector(SELECTORS.partnerTypePicker);
    if (!select) return;

    select.innerHTML = `<option value="">Válassz partner típust...</option>`;

    console.log('🔎 partnerTypesMeta url:', ENDPOINTS.partnerTypesMeta);

    const res = await fetch(ENDPOINTS.partnerTypesMeta, {
        headers: { 'X-Requested-With': 'XMLHttpRequest' }
    });

    console.log('📡 partnerTypesMeta status:', res.status);

    if (!res.ok) {
        const raw = await res.text();
        console.error('❌ partnerTypesMeta raw:', raw);
        throw new Error(`Nem sikerült betölteni a partner típusokat. HTTP ${res.status}`);
    }

    const json = await res.json();
    console.log('✅ partnerTypesMeta json:', json);

    const items = normalizePartnerTypeItems(json);
    console.log('✅ normalized partner types:', items);

    state.partnerTypes = items;

    for (const item of items) {
        const opt = document.createElement('option');
        opt.value = item.partnerTypeId;
        opt.textContent = item.partnerTypeName;
        select.appendChild(opt);
    }
}

    function normalizePartnerTypeItems(json) {
        const items =
            json?.items ||
            json?.data ||
            json?.results ||
            (Array.isArray(json) ? json : []);

        return (items || []).map(x => ({
            partnerTypeId: x.partnerTypeId ?? x.id ?? x.value,
            partnerTypeName: x.partnerTypeName ?? x.name ?? x.text ?? ''
        })).filter(x => x.partnerTypeId && x.partnerTypeName);
    }

function setBusy(isBusy) {
    const addBtn = document.querySelector(SELECTORS.addBtn);
    const saveBtn = document.querySelector(SELECTORS.saveBtn);
    const typeSelect = document.querySelector(SELECTORS.partnerTypePicker);

    if (addBtn) addBtn.disabled = isBusy;
    if (saveBtn) saveBtn.disabled = isBusy;
    if (typeSelect) typeSelect.disabled = isBusy;

    if (state.partnerTom) {
        if (isBusy) state.partnerTom.lock();
        else state.partnerTom.unlock();
    }
}

async function loadAssignedPartners(siteId) {
    const url = ENDPOINTS.getSitePartners(siteId);
    console.log('🔎 getSitePartners url:', url);

    const res = await fetch(url, {
        headers: { 'X-Requested-With': 'XMLHttpRequest' }
    });

    console.log('📡 getSitePartners status:', res.status);

    if (!res.ok) {
        const raw = await res.text();
        console.error('❌ getSitePartners raw:', raw);
        throw new Error(`Nem sikerült betölteni a hozzárendelt partnereket. HTTP ${res.status}`);
    }

    const json = await res.json();
    console.log('✅ getSitePartners json:', json);

    state.assignedPartners = normalizeAssignedPartners(json);
}

    function normalizeAssignedPartners(json) {
        const items =
            json?.items ||
            json?.data ||
            json?.results ||
            (Array.isArray(json) ? json : []);

        return (items || []).map(x => ({
            partnerId: x.partnerId,
            partnerName: x.partnerName ?? x.name ?? '',
            partnerTypeId: x.partnerTypeId ?? '',
            partnerTypeName: x.partnerTypeName ?? ''
        })).filter(x => x.partnerId && x.partnerName);
    }

    function onAddPartner() {
        clearError();

        const partner = getSelectedPartner();
        const partnerType = getSelectedPartnerType();

        if (!partner) {
            showError('Válassz partnert.');
            return;
        }

        if (!partnerType) {
            showError('Válassz partner típust.');
            return;
        }

        const exists = state.assignedPartners.some(x => String(x.partnerId) === String(partner.partnerId));
        if (exists) {
            showError('Ez a partner már hozzá van rendelve a telephelyhez.');
            return;
        }

        state.assignedPartners.push({
            partnerId: partner.partnerId,
            partnerName: partner.partnerName,
            partnerTypeId: partnerType.partnerTypeId,
            partnerTypeName: partnerType.partnerTypeName
        });

        clearPartnerSelection();
        renderAssignedPartners();
    }

    async function onSavePartners() {
        if (!state.siteId) return;

        const saveBtn = document.querySelector(SELECTORS.saveBtn);

        try {
            clearError();
            setButtonBusy(saveBtn, true);

            const payload = {
                siteId: state.siteId,
                partners: state.assignedPartners.map(x => ({
                    partnerId: x.partnerId,
                    partnerTypeId: x.partnerTypeId
                }))
            };

            const res = await fetch(ENDPOINTS.putSitePartners(state.siteId), {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'X-Requested-With': 'XMLHttpRequest'
                },
                body: JSON.stringify(payload)
            });

console.log('📤 saveSitePartners payload:', payload);
console.log('📡 saveSitePartners status:', res.status);

if (!res.ok) {
    const raw = await res.text();
    console.error('❌ saveSitePartners raw:', raw);
    throw new Error(raw || `Nem sikerült menteni a partner hozzárendeléseket. HTTP ${res.status}`);
}

            state.modalInstance.hide();

            if (window.Sites && typeof window.Sites.reload === 'function') {
                window.Sites.reload();
            }
        } catch (err) {
            console.error('Partner hozzárendelés mentési hiba:', err);
            showError(getErrorMessage(err, 'Nem sikerült menteni a partner hozzárendeléseket.'));
        } finally {
            setButtonBusy(saveBtn, false);
        }
    }

    function getSelectedPartner() {
        if (!state.partnerTom) return null;

        const value = state.partnerTom.getValue();
        if (!value) return null;

        const option = state.partnerTom.options[value];
        if (!option) return null;

        return {
            partnerId: option.partnerId ?? value,
            partnerName: option.partnerName ?? option.text ?? ''
        };
    }

    function getSelectedPartnerType() {
        const select = document.querySelector(SELECTORS.partnerTypePicker);
        if (!select || !select.value) return null;

        const item = state.partnerTypes.find(x => String(x.partnerTypeId) === String(select.value));
        if (!item) return null;

        return item;
    }

    function clearPartnerSelection() {
        if (state.partnerTom) {
            state.partnerTom.clear(true);
        }

        const typeSelect = document.querySelector(SELECTORS.partnerTypePicker);
        if (typeSelect) {
            typeSelect.value = '';
        }
    }

    function renderAssignedPartners() {
        const list = document.querySelector(SELECTORS.list);
        const empty = document.querySelector(SELECTORS.empty);

        if (!list || !empty) return;

        list.innerHTML = '';

        if (!state.assignedPartners.length) {
            empty.classList.remove('d-none');
            return;
        }

        empty.classList.add('d-none');

        for (const item of state.assignedPartners) {
            const li = document.createElement('li');
            li.className = 'list-group-item d-flex justify-content-between align-items-center';

            li.innerHTML = `
                <div>
                    <div class="fw-semibold">${escapeHtml(item.partnerName)}</div>
                    <div class="small text-muted">
                        Típus: ${escapeHtml(item.partnerTypeName || '-')}
                    </div>
                </div>

                <button type="button"
                        class="btn btn-sm btn-outline-danger remove-site-partner-btn"
                        data-partner-id="${escapeAttr(item.partnerId)}"
                        title="Eltávolítás">
                    <i class="bi bi-x-lg"></i>
                </button>
            `;

            list.appendChild(li);
        }
    }

    function setSiteName(name) {
        const el = document.querySelector(SELECTORS.siteName);
        if (el) el.textContent = name || '';
    }

    function resetWorkingData() {
        state.assignedPartners = [];
        clearPartnerSelection();
        renderAssignedPartners();
        clearError();
    }

    function resetModalState() {
        state.siteId = null;
        state.siteName = '';
        resetWorkingData();
        setSiteName('');
    }

    function showError(message) {
        const el = document.querySelector(SELECTORS.error);
        if (!el) return;

        el.textContent = message || 'Hiba történt.';
        el.classList.remove('d-none');
    }

    function clearError() {
        const el = document.querySelector(SELECTORS.error);
        if (!el) return;

        el.textContent = '';
        el.classList.add('d-none');
    }

    function setBusy(isBusy) {
        const addBtn = document.querySelector(SELECTORS.addBtn);
        const saveBtn = document.querySelector(SELECTORS.saveBtn);
        const typeSelect = document.querySelector(SELECTORS.partnerTypePicker);

        if (addBtn) addBtn.disabled = isBusy;
        if (saveBtn) saveBtn.disabled = isBusy;
        if (typeSelect) typeSelect.disabled = isBusy;
        if (state.partnerTom) state.partnerTom.lock();

        if (!isBusy && state.partnerTom) {
            state.partnerTom.unlock();
        }
    }

    function setButtonBusy(btn, isBusy) {
        if (!btn) return;

        if (isBusy) {
            btn.dataset.originalText = btn.innerHTML;
            btn.disabled = true;
            btn.innerHTML = `<span class="spinner-border spinner-border-sm me-2"></span>Mentés...`;
        } else {
            btn.disabled = false;
            if (btn.dataset.originalText) {
                btn.innerHTML = btn.dataset.originalText;
            }
        }
    }

    async function tryReadError(response) {
        try {
            const text = await response.text();
            if (!text) return null;

            try {
                const json = JSON.parse(text);
                return json.message || json.error || text;
            } catch {
                return text;
            }
        } catch {
            return null;
        }
    }

    function getErrorMessage(err, fallback) {
        if (!err) return fallback;
        if (typeof err === 'string') return err;
        if (err.message) return err.message;
        return fallback;
    }

    function escapeHtml(value) {
        return String(value ?? '')
            .replaceAll('&', '&amp;')
            .replaceAll('<', '&lt;')
            .replaceAll('>', '&gt;')
            .replaceAll('"', '&quot;')
            .replaceAll("'", '&#39;');
    }

    function escapeAttr(value) {
        return escapeHtml(value);
    }

    window.SitePartners = {
        open: async function (siteId, siteName) {
            try {
                clearError();
                setBusy(true);

                state.siteId = siteId;
                state.siteName = siteName || 'Telephely';

                setSiteName(state.siteName);
                resetWorkingData();

                await loadPartnerTypes();
                await loadAssignedPartners(siteId);

                renderAssignedPartners();
                state.modalInstance.show();
            } catch (err) {
                console.error(err);
                showError(getErrorMessage(err, 'Nem sikerült megnyitni a partner hozzárendelést.'));
            } finally {
                setBusy(false);
            }
        }
    };
})();