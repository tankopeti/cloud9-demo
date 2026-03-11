// wwwroot/js/Task/taskIntezkedesLoadMore.js
(function () {
  'use strict';

  document.addEventListener('DOMContentLoaded', function () {
    console.log('[taskIntezkedesLoadMore] init');

    var wrap = document.getElementById('tasksTableWrap');
    var tbody = document.getElementById('tasksTbody');
    var btn = document.getElementById('btnLoadMoreTasks');
    var info = document.getElementById('tasksLoadInfo');

    if (!wrap || !tbody || !btn) {
      console.warn('[taskIntezkedesLoadMore] missing elements');
      return;
    }

    var state = {
      pageSize: parseInt(wrap.dataset.pageSize || '20', 10) || 20,
      sort: (wrap.dataset.sort || 'CreatedDate').trim() || 'CreatedDate',
      order: (wrap.dataset.order || 'desc').trim() || 'desc',
      search: (wrap.dataset.search || '').trim(),
      page: 1,
      isLoading: false,
      renderedIds: new Set(),
      reachedEnd: false,
      totalCount: null,
      activeFilters: {}
    };

    window.__TASKS_FILTER_AJAX__ = true;

    function esc(s) {
      var d = document.createElement('div');
      d.textContent = s == null ? '' : String(s);
      return d.innerHTML;
    }

    function formatDateHu(v) {
      if (!v) return '–';
      var d = new Date(v);
      if (isNaN(d.getTime())) return '–';
      return d.toLocaleString('hu-HU', {
        year: 'numeric', month: '2-digit', day: '2-digit',
        hour: '2-digit', minute: '2-digit'
      }).replace(',', '');
    }

    function setInfo(text) {
      if (info) info.textContent = text || '';
    }

    function setInfoLoaded() {
      if (!info) return;
      var loaded = state.renderedIds.size;
      info.textContent = typeof state.totalCount === 'number'
        ? 'Betöltve: ' + Math.min(loaded, state.totalCount) + ' / ' + state.totalCount
        : 'Betöltve: ' + loaded;
    }

    function setButtonState() {
      btn.disabled = !!state.isLoading || !!state.reachedEnd;
      btn.textContent = state.isLoading
        ? 'Betöltés...'
        : (state.reachedEnd ? 'Nincs több' : 'Több betöltése');
    }

    function setNoMore() {
      state.reachedEnd = true;
      setButtonState();
    }

    function resetList() {
      state.page = 1;
      state.renderedIds.clear();
      state.reachedEnd = false;
      state.totalCount = null;

      tbody.innerHTML = '';
      setInfo('');
      setButtonState();
    }

    function cleanFilterParams(params) {
      var cleaned = {};
      if (!params || typeof params !== 'object') return cleaned;

      Object.keys(params).forEach(function (k) {
        var v = params[k];
        if (v == null) return;

        v = String(v).trim();
        if (!v) return;

        if (k === 'page' || k === 'pageSize' || k === 'sort' || k === 'order' || k === 'search' || k === 'displayType') {
          return;
        }

        cleaned[k] = v;
      });

      return cleaned;
    }

    function buildUrl(page) {
      var qs = new URLSearchParams();
      qs.set('page', String(page));
      qs.set('pageSize', String(state.pageSize));
      qs.set('sort', state.sort);
      qs.set('order', state.order);
      qs.set('displayType', '2');

      if (state.search) qs.set('search', state.search);

      Object.keys(state.activeFilters).forEach(function (k) {
        qs.set(k, state.activeFilters[k]);
      });

      return '/api/tasks/paged?' + qs.toString();
    }

    async function fetchPage(page) {
      var json = await AppApi.get(buildUrl(page));

      var items = Array.isArray(json)
        ? json
        : (json.items || json.Items || json.results || json.Results || []);

      var totalCount = Array.isArray(json)
        ? null
        : (json.totalCount ?? json.TotalCount ?? json.totalRecords ?? json.TotalRecords ?? null);

      return { items: items, totalCount: totalCount };
    }

    function renderRow(t) {
      var id = Number(t && (t.id ?? t.Id)) || 0;
      var priorityColor = (t.priorityColorCode || t.PriorityColorCode || '').trim() || '#6c757d';
      var statusColor = (t.colorCode || t.ColorCode || '').trim() || '#6c757d';
      var assignedEmail = t.assignedToEmail || t.AssignedToEmail || '';
      var assignedName = t.assignedToName || t.AssignedToName || '';

      var assignedHtml = assignedEmail
        ? '<a class="js-assigned-mail" href="mailto:' + esc(assignedEmail) + '">' + esc(assignedName) + '</a>'
        : esc(assignedName);

      var tr = document.createElement('tr');
      tr.setAttribute('data-task-id', String(id));

      tr.innerHTML = `
        <td>${esc(id)}</td>
        <td>${esc(t.siteName || t.SiteName || '')}</td>
        <td>${esc(t.city || t.City || '')}</td>
        <td>${esc(t.partnerName || t.PartnerName || '')}</td>
        <td>${esc(t.title || t.Title || '')}</td>

        <td class="text-center">
          <span class="badge text-white" style="background:${esc(priorityColor)}">
            ${esc(t.taskPriorityPMName || t.TaskPriorityPMName || '')}
          </span>
        </td>

        <td>${esc(formatDateHu(t.dueDate || t.DueDate))}</td>

        <td class="text-center">
          <span class="badge text-white" style="background:${esc(statusColor)}">
            ${esc(t.taskStatusPMName || t.TaskStatusPMName || '')}
          </span>
        </td>

        <td>${esc(formatDateHu(t.createdDate || t.CreatedDate))}</td>
        <td>${esc(formatDateHu(t.updatedDate || t.UpdatedDate))}</td>
        <td>${assignedHtml}</td>

        <td>
          <div class="btn-group btn-group-sm" role="group">
            <button type="button"
                    class="btn btn-outline-info js-view-task-btn"
                    data-task-id="${esc(id)}"
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
                <li><a class="dropdown-item js-edit-task" href="#" data-task-id="${esc(id)}"><i class="bi bi-pencil-square me-2"></i> Szerkesztés</a></li>
                <li><a class="dropdown-item js-task-documents" href="#" data-task-id="${esc(id)}"><i class="bi bi-paperclip me-2"></i> Fájlok</a></li>
                <li><a class="dropdown-item btn-show-history" href="#" data-task-id="${esc(id)}"><i class="bi bi-clock-history me-2"></i> Előzmények</a></li>
                <li><hr class="dropdown-divider"></li>
                <li><a class="dropdown-item text-danger js-delete-task" href="#" data-task-id="${esc(id)}"><i class="bi bi-trash me-2"></i> Törlés</a></li>
              </ul>
            </div>
          </div>
        </td>
      `;

      return tr;
    }

    async function loadMore() {
      if (state.isLoading || state.reachedEnd) return;

      state.isLoading = true;
      setButtonState();
      setInfo('Betöltés...');

      try {
        var result = await fetchPage(state.page);
        var items = result.items || [];

        if (result.totalCount != null) {
          var n = parseInt(String(result.totalCount), 10);
          if (Number.isFinite(n)) state.totalCount = n;
        }

        if (!items.length) {
          state.page += 1;

          if (state.page > 200) {
            setInfoLoaded();
            setNoMore();
            return;
          }

          state.isLoading = false;
          setButtonState();
          setInfoLoaded();
          return loadMore();
        }

        items.forEach(function (t) {
          var id = Number(t && (t.id ?? t.Id));
          if (!Number.isFinite(id) || state.renderedIds.has(id)) return;

          state.renderedIds.add(id);
          tbody.appendChild(renderRow(t));
        });

        state.page += 1;

        if (items.length < state.pageSize && typeof state.totalCount !== 'number') {
          setNoMore();
        }

        if (typeof state.totalCount === 'number' && state.renderedIds.size >= state.totalCount) {
          setNoMore();
        }

        setInfoLoaded();
      } catch (e) {
        console.error('[taskIntezkedesLoadMore] load failed', e);
        setInfo('Hiba a betöltéskor (nézd meg a konzolt).');
      } finally {
        state.isLoading = false;
        setButtonState();
      }
    }

    function dispatchTaskEvent(name, detail) {
      window.dispatchEvent(new CustomEvent(name, { detail: detail }));
    }

    function getTaskId(target, selector) {
      var el = target.closest(selector);
      if (!el) return null;

      var id = parseInt(el.dataset.taskId, 10);
      return Number.isFinite(id) ? id : null;
    }

    if (wrap._delegationAbortController) {
      try { wrap._delegationAbortController.abort(); } catch (e) {}
    }

    var ac = new AbortController();
    wrap._delegationAbortController = ac;

    wrap.addEventListener('click', function (e) {
      if (e.target.closest('a[href^="mailto:"]')) return;

      var handlers = [
        {
          selector: '.js-view-task-btn',
          run: function (id) {
            if (window.Tasks && typeof window.Tasks.openViewModal === 'function') {
              window.Tasks.openViewModal(id);
            } else {
              dispatchTaskEvent('tasks:view', { id: id });
            }
          }
        },
        {
          selector: '.js-edit-task',
          run: function (id) { dispatchTaskEvent('tasks:openEdit', { id: id }); }
        },
        {
          selector: '.js-task-documents',
          run: function (id) { dispatchTaskEvent('tasks:openDocuments', { taskId: id }); }
        },
        {
          selector: '.btn-show-history',
          run: function (id) { dispatchTaskEvent('tasks:history', { id: id }); }
        },
        {
          selector: '.js-delete-task',
          run: function (id) { dispatchTaskEvent('tasks:openDelete', { id: id }); }
        }
      ];

      for (var i = 0; i < handlers.length; i++) {
        var id = getTaskId(e.target, handlers[i].selector);
        if (id == null) continue;

        e.preventDefault();
        handlers[i].run(id);
        return;
      }
    }, { signal: ac.signal });

    btn.addEventListener('click', function (e) {
      e.preventDefault();
      loadMore();
    });

    (function wireAdvancedFilterModal() {
      var modalEl = document.getElementById('advancedFilterModal');
      if (!modalEl) return;

      var form = document.getElementById('advancedFilterForm') || modalEl.querySelector('form');
      if (!form) return;

      function closeModal() {
        AppModal.hide('advancedFilterModal');
      }

      var partnerSel = modalEl.querySelector('#partnerFilterSelect');
      var siteSel = modalEl.querySelector('#siteFilterSelect');

      if (partnerSel && siteSel) {
        var allSiteOptions = Array.from(siteSel.options).map(function (o) {
          return {
            value: o.value,
            text: o.textContent,
            partnerId: o.dataset.partnerId || ''
          };
        });

        function rebuildSites(partnerId) {
          var current = siteSel.value;
          siteSel.innerHTML = '';

          var opt0 = document.createElement('option');
          opt0.value = '';
          opt0.textContent = '-- Minden --';
          siteSel.appendChild(opt0);

          allSiteOptions
            .filter(function (x) { return x.value !== ''; })
            .filter(function (x) { return !partnerId || x.partnerId === partnerId; })
            .forEach(function (x) {
              var opt = document.createElement('option');
              opt.value = x.value;
              opt.textContent = x.text;
              if (x.partnerId) opt.dataset.partnerId = x.partnerId;
              siteSel.appendChild(opt);
            });

          var stillExists = Array.from(siteSel.options).some(function (o) {
            return o.value === current;
          });

          siteSel.value = stillExists ? current : '';
        }

        rebuildSites(partnerSel.value);
        partnerSel.addEventListener('change', function () {
          rebuildSites(partnerSel.value);
        });
      }

      form.addEventListener('submit', function (e) {
        e.preventDefault();

        var fd = new FormData(form);
        var raw = {};
        fd.forEach(function (v, k) { raw[k] = v; });

        state.activeFilters = cleanFilterParams(raw);

        console.log('[taskIntezkedesLoadMore] apply filters', state.activeFilters);

        resetList();
        closeModal();
        loadMore();
      });
    })();

    window.addEventListener('tasks:applyFilters', function (e) {
      var params = e && e.detail && (e.detail.params || e.detail);
      state.activeFilters = cleanFilterParams(params);
      resetList();
      loadMore();
    });

    window.addEventListener('tasks:clearFilters', function () {
      state.activeFilters = {};
      resetList();
      loadMore();
    });

    window.addEventListener('tasks:reload', function () {
      resetList();
      loadMore();
    });

    window.addEventListener('tasks:deleted', function (e) {
      var id = parseInt(e && e.detail && e.detail.id, 10);
      if (!Number.isFinite(id)) return;

      if (state.renderedIds.has(id)) state.renderedIds.delete(id);
      if (typeof state.totalCount === 'number' && state.totalCount > 0) state.totalCount -= 1;

      setInfoLoaded();
      if (typeof state.totalCount === 'number' && state.renderedIds.size >= state.totalCount) {
        setNoMore();
      }
    });

    loadMore();
  });
})();