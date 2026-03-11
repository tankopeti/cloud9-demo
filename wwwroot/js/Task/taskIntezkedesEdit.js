// wwwroot/js/Task/taskBejelentesEdit.js
(function () {
  'use strict';

  console.log('[taskBejelentesEdit] loaded');

  document.addEventListener('DOMContentLoaded', function () {
    console.log('[taskBejelentesEdit] DOM loaded');

    var DISPLAY_TYPE = 2;
    var MODAL_ID = 'editTaskModal';

    var API = {
      assignees: '/api/tasks/assignees/select',
      commMethods: '/api/tasks/taskpm-communication-methods/select',
      taskTypes: function (displayType) {
        return '/api/tasks/tasktypes/select?displayType=' + encodeURIComponent(displayType);
      },
      taskStatuses: function (displayType) {
        return '/api/tasks/taskstatuses/select?displayType=' + encodeURIComponent(displayType);
      },
      taskById: function (id) {
        return '/api/tasks/' + encodeURIComponent(id);
      },
      updateTask: function (id) {
        return '/api/tasks/' + encodeURIComponent(id);
      },
      taskAudit: function (id) {
        return '/api/tasks/' + encodeURIComponent(id) + '/audit?_=' + Date.now();
      }
    };

    var modalEl = document.getElementById(MODAL_ID);
    if (!modalEl) {
      console.warn('[taskBejelentesEdit] #editTaskModal not found -> skip');
      return;
    }

    var formEl = modalEl.querySelector('form');
    if (!formEl) {
      console.warn('[taskBejelentesEdit] form not found in modal -> skip');
      return;
    }

    var submitBtn = AppForms.qs(formEl, 'button[type="submit"]');

    var el = {
      id: AppForms.qs(formEl, '#EditId, [name="Id"]'),
      title: AppForms.qs(formEl, '#EditTitle, [name="Title"]'),
      desc: AppForms.qs(formEl, '#EditDescription, [name="Description"]'),

      site: AppForms.qs(formEl, '#EditSiteId, [name="SiteId"]'),
      relatedPartner: AppForms.qs(formEl, '#EditRelatedPartnerId, [name="RelatedPartnerId"]'),

      taskType: AppForms.qs(formEl, '#EditTaskTypePMId, [name="TaskTypePMId"]'),
      status: AppForms.qs(formEl, '#EditTaskStatusPMId, [name="TaskStatusPMId"]'),
      priority: AppForms.qs(formEl, '#EditTaskPriorityPMId, [name="TaskPriorityPMId"]'),

      assignedTo: AppForms.qs(formEl, '#EditAssignedToId, [name="AssignedToId"]'),

      commMethod: AppForms.qs(formEl, '#EditTaskPMcomMethodID, [name="TaskPMcomMethodID"]'),
      commDesc: AppForms.qs(formEl, '#EditCommunicationDescription, [name="CommunicationDescription"]'),

      scheduledDate: AppForms.qs(formEl, '#EditScheduledDate, [name="ScheduledDate"]'),
      partnerHidden: AppForms.qs(formEl, '#editAutoPartnerId, [name="PartnerId"]')
    };

    var historyEl = {
      title: document.getElementById('editHistoryTaskTitle'),
      loading: document.getElementById('editHistoryLoading'),
      content: document.getElementById('editHistoryContent'),
      empty: document.getElementById('editHistoryEmpty'),
      list: document.getElementById('editHistoryList'),
      count: document.getElementById('editHistoryCountBadge')
    };

    var hasHistoryPanel =
      historyEl.title &&
      historyEl.loading &&
      historyEl.content &&
      historyEl.empty &&
      historyEl.list &&
      historyEl.count;

    var state = {
      currentId: null,
      isSubmitting: false,
      isOpening: false
    };

    var selectCache = Object.create(null);

    function toast(message, type) {
      if (window.AppToast && typeof window.AppToast.show === 'function') {
        window.AppToast.show(message, type || 'info');
        return;
      }

      console.log('[toast fallback]', type || 'info', message);
    }

    function esc(value) {
      var div = document.createElement('div');
      div.textContent = value == null ? '' : String(value);
      return div.innerHTML;
    }

    function pick(obj, keys) {
      for (var i = 0; i < keys.length; i++) {
        var key = keys[i];
        if (obj && obj[key] !== undefined && obj[key] !== null) return obj[key];
      }
      return null;
    }

    function setVisible(node, visible) {
      if (!node) return;
      node.classList.toggle('d-none', !visible);
    }

    function formatHuDateTime(value) {
      if (!value) return '';
      try {
        var d = new Date(value);
        if (isNaN(d.getTime())) return String(value);
        return d.toLocaleString('hu-HU');
      } catch (e) {
        return String(value);
      }
    }

    function fmtForInputDate(iso) {
      if (!iso) return '';
      try {
        var d = new Date(iso);
        if (isNaN(d.getTime())) return '';
        var pad = function (n) { return String(n).padStart(2, '0'); };
        return d.getFullYear() + '-' + pad(d.getMonth() + 1) + '-' + pad(d.getDate());
      } catch (e) {
        return '';
      }
    }

    function sleep(ms) {
      return new Promise(function (resolve) {
        setTimeout(resolve, ms);
      });
    }

    async function waitForTomSelect(selectEl, timeoutMs) {
      timeoutMs = timeoutMs || 3500;
      var step = 50;
      var tries = Math.ceil(timeoutMs / step);

      for (var i = 0; i < tries; i++) {
        if (selectEl && selectEl.tomselect) return selectEl.tomselect;
        await sleep(step);
      }

      return selectEl && selectEl.tomselect ? selectEl.tomselect : null;
    }

    async function waitAndSetSelectValue(selectEl, value) {
      if (!selectEl) return;

      var v = value == null ? '' : String(value);
      if (!v) {
        selectEl.value = '';
        try { selectEl.dispatchEvent(new Event('change', { bubbles: true })); } catch (e) {}
        return;
      }

      for (var i = 0; i < 60; i++) {
        var hasOption = Array.from(selectEl.options || []).some(function (o) {
          return String(o.value) === v;
        });

        if (hasOption) {
          selectEl.value = v;
          try { selectEl.dispatchEvent(new Event('change', { bubbles: true })); } catch (e) {}
          return;
        }

        await sleep(50);
      }

      selectEl.value = v;
      try { selectEl.dispatchEvent(new Event('change', { bubbles: true })); } catch (e) {}
    }

    async function fetchSelectCached(url) {
      if (selectCache[url]) return selectCache[url];

      selectCache[url] = AppApi.get(url)
        .then(function (data) {
          return Array.isArray(data) ? data : [];
        })
        .catch(function (err) {
          delete selectCache[url];
          throw err;
        });

      return selectCache[url];
    }

    async function loadSimpleSelect(selectEl, url, selectedId, placeholder) {
      if (!selectEl) return;

      selectEl.disabled = true;
      selectEl.innerHTML = '<option value="">Betöltés...</option>';

      try {
        var items = await fetchSelectCached(url);

        selectEl.innerHTML =
          '<option value="">' + String(placeholder || '-- Válasszon --') + '</option>' +
          items.map(function (x) {
            return '<option value="' + String(x.id) + '">' + String(x.text) + '</option>';
          }).join('');

        await waitAndSetSelectValue(selectEl, selectedId);
      } catch (err) {
        console.error('[taskBejelentesEdit] loadSimpleSelect failed', url, err);
        selectEl.innerHTML = '<option value="">-- Nem sikerült betölteni --</option>';
      } finally {
        selectEl.disabled = false;
      }
    }

    async function loadTomOrSimpleSelect(selectEl, url, selectedId, placeholder) {
      if (!selectEl) return;

      var ts = await waitForTomSelect(selectEl, 2500);

      if (!ts) {
        return loadSimpleSelect(selectEl, url, selectedId, placeholder);
      }

      ts.disable();
      ts.clearOptions();
      ts.addOption({ id: '', text: 'Betöltés...' });
      ts.refreshOptions(false);

      try {
        var items = await fetchSelectCached(url);
        var options = items.map(function (x) {
          return { id: String(x.id), text: String(x.text) };
        });

        ts.clearOptions();
        ts.addOption({ id: '', text: String(placeholder || '-- Válasszon --') });
        ts.addOptions(options);
        ts.setValue(selectedId != null ? String(selectedId) : '', true);
        ts.refreshOptions(false);
        ts.enable();
      } catch (err) {
        console.error('[taskBejelentesEdit] loadTomOrSimpleSelect failed', url, err);
        ts.clearOptions();
        ts.addOption({ id: '', text: '-- Nem sikerült betölteni --' });
        ts.setValue('', true);
        ts.enable();
      }
    }

    async function loadTask(id) {
      return AppApi.get(API.taskById(id));
    }

    async function loadTaskAudit(id) {
      return AppApi.get(API.taskAudit(id));
    }

    function rowElById(id) {
      return document.querySelector('tr[data-task-id="' + CSS.escape(String(id)) + '"]');
    }

    function updateRowFromTask(task) {
      var id = pick(task, ['id', 'Id']);
      if (id == null) return;

      var tr = rowElById(id);
      if (!tr) return;

      var tds = tr.querySelectorAll('td');
      if (!tds || tds.length < 12) return;

      tds[4].textContent = pick(task, ['title', 'Title']) || '';

      var prioBadge =
        tds[5].querySelector('.clickable-priority-badge') ||
        tds[5].querySelector('.badge');

      if (prioBadge) {
        prioBadge.textContent = pick(task, ['taskPriorityPMName', 'TaskPriorityPMName']) || '';
        prioBadge.style.backgroundColor = pick(task, ['priorityColorCode', 'PriorityColorCode']) || '#6c757d';
        prioBadge.dataset.priorityId = pick(task, ['taskPriorityPMId', 'TaskPriorityPMId']) || '';
      }

      tds[6].textContent = formatHuDateTime(pick(task, ['dueDate', 'DueDate']));

      var statusBadge =
        tds[7].querySelector('.clickable-status-badge') ||
        tds[7].querySelector('.badge');

      if (statusBadge) {
        statusBadge.textContent = pick(task, ['taskStatusPMName', 'TaskStatusPMName']) || '';
        statusBadge.style.backgroundColor = pick(task, ['colorCode', 'ColorCode']) || '#6c757d';
        statusBadge.dataset.statusId = pick(task, ['taskStatusPMId', 'TaskStatusPMId']) || '';
      }

      tds[9].textContent = formatHuDateTime(pick(task, ['updatedDate', 'UpdatedDate']));

      var assignedEmail = pick(task, ['assignedToEmail', 'AssignedToEmail']) || '';
      var assignedName = pick(task, ['assignedToName', 'AssignedToName']) || '';

      if (assignedEmail) {
        tds[10].innerHTML =
          '<a class="js-assigned-mail" href="mailto:' + assignedEmail + '">' + assignedName + '</a>';
      } else {
        tds[10].textContent = assignedName;
      }
    }

    async function refreshRow(id) {
      try {
        var fresh = await loadTask(id);
        updateRowFromTask(fresh);
      } catch (err) {
        console.warn('[taskBejelentesEdit] refreshRow failed', err);
      }
    }

    function translateAuditField(fieldName) {
      var map = {
        Id: 'Azonosító',
        Title: 'Tárgy',
        Description: 'Leírás',
        PartnerId: 'Partner',
        RelatedPartnerId: 'Kapcsolt partner',
        SiteId: 'Telephely',
        TaskTypePMId: 'Feladat típusa',
        TaskPriorityPMId: 'Prioritás',
        TaskStatusPMId: 'Státusz',
        AssignedToId: 'Felelős',
        TaskPMcomMethodID: 'Kommunikáció módja',
        CommunicationDescription: 'Név / elérhetőség / részletek',
        ScheduledDate: 'Beütemezve',
        DueDate: 'Határidő',
        RelatedEmployeeId: 'Kapcsolt személy',
        CreatedDate: 'Létrehozva',
        UpdatedDate: 'Módosítva'
      };

      return map[fieldName] || fieldName;
    }

    function formatAuditValue(fieldName, value) {
      if (value == null || value === '' || String(value).toLowerCase() === 'null') {
        return '–';
      }

      if (
        fieldName === 'ScheduledDate' ||
        fieldName === 'DueDate' ||
        fieldName === 'CreatedDate' ||
        fieldName === 'UpdatedDate'
      ) {
        return formatHuDateTime(value);
      }

      return String(value);
    }

    function localizeAuditChanges(changesText) {
      if (!changesText) return '–';

      var parts = String(changesText)
        .split(';')
        .map(function (x) { return x.trim(); })
        .filter(Boolean);

      var localized = parts.map(function (part) {
        var match = part.match(/^([A-Za-z0-9_]+)\s*:\s*(.*?)\s*→\s*(.*)$/);
        if (!match) return esc(part);

        var fieldName = match[1];
        var oldValue = match[2];
        var newValue = match[3];

        return esc(translateAuditField(fieldName)) +
          ': ' +
          esc(formatAuditValue(fieldName, oldValue)) +
          ' → ' +
          esc(formatAuditValue(fieldName, newValue));
      });

      return localized.join('\n');
    }

    function historyMetaForAction(action) {
      var a = String(action || '').toLowerCase();

      if (a === 'created') {
        return { icon: 'bi-plus-circle-fill', badge: 'bg-success', label: 'Létrehozva' };
      }
      if (a === 'updated') {
        return { icon: 'bi-pencil-square', badge: 'bg-primary', label: 'Módosítva' };
      }
      if (a === 'deleted') {
        return { icon: 'bi-trash-fill', badge: 'bg-danger', label: 'Törölve' };
      }
      if (a === 'statuschanged' || a === 'status_change' || a === 'status') {
        return { icon: 'bi-arrow-repeat', badge: 'bg-warning text-dark', label: 'Státusz módosítva' };
      }
      if (a === 'assignedchanged' || a === 'assigned_to' || a === 'assigneechanged') {
        return { icon: 'bi-person-check-fill', badge: 'bg-info text-dark', label: 'Felelős módosítva' };
      }

      return { icon: 'bi-info-circle-fill', badge: 'bg-secondary', label: action || 'Esemény' };
    }

    function resetHistoryPanel() {
      if (!hasHistoryPanel) return;

      historyEl.title.textContent = 'Betöltés...';
      historyEl.list.innerHTML = '';
      historyEl.count.textContent = '0';

      setVisible(historyEl.loading, true);
      setVisible(historyEl.content, false);
      setVisible(historyEl.empty, false);
    }

    function showHistoryLoading(taskId) {
      if (!hasHistoryPanel) return;

      historyEl.title.textContent = taskId ? ('Intézkedés #' + taskId) : 'Betöltés...';
      historyEl.list.innerHTML = '';
      historyEl.count.textContent = '0';

      setVisible(historyEl.loading, true);
      setVisible(historyEl.content, false);
      setVisible(historyEl.empty, false);
    }

    function showHistoryContent() {
      if (!hasHistoryPanel) return;

      setVisible(historyEl.loading, false);
      setVisible(historyEl.content, true);
    }

    function renderHistoryError(message) {
      if (!hasHistoryPanel) return;

      historyEl.count.textContent = '0';
      historyEl.list.innerHTML =
        '<div class="alert alert-danger mb-0">' +
        '  <div class="fw-semibold">Nem sikerült betölteni az előzményeket.</div>' +
        '  <div class="small mt-1">' + esc(message || 'Ismeretlen hiba') + '</div>' +
        '</div>';

      setVisible(historyEl.empty, false);
    }

    function renderHistory(items, task) {
      if (!hasHistoryPanel) return;

      var taskId = pick(task, ['id', 'Id']) || state.currentId;
      var taskTitle = pick(task, ['title', 'Title']) || '';

      historyEl.title.textContent = taskId
        ? ('Intézkedés #' + taskId + (taskTitle ? ' – ' + taskTitle : ''))
        : (taskTitle || 'Előzmények');

      if (!Array.isArray(items) || items.length === 0) {
        historyEl.count.textContent = '0';
        historyEl.list.innerHTML = '';
        setVisible(historyEl.empty, true);
        return;
      }

      historyEl.count.textContent = String(items.length);
      setVisible(historyEl.empty, false);

      historyEl.list.innerHTML = items.map(function (x) {
        var meta = historyMetaForAction(x.action);
        var when = formatHuDateTime(x.changedAt) || '—';
        var who = x.changedByName ? esc(x.changedByName) : '—';
        var changes = x.changes ? localizeAuditChanges(x.changes) : '—';

        return '' +
          '<div class="border-start border-3 ps-3 ms-2 mb-4 position-relative">' +
          '  <div class="position-absolute top-0 start-0 translate-middle rounded-circle bg-success" style="width:12px;height:12px;"></div>' +
          '  <div class="d-flex flex-wrap align-items-center gap-2 mb-1">' +
          '    <span class="badge ' + meta.badge + '">' +
          '      <i class="bi ' + meta.icon + ' me-1"></i>' + esc(meta.label) +
          '    </span>' +
          '    <span class="small text-muted">' + esc(when) + '</span>' +
          '  </div>' +
          '  <div class="fw-semibold">' + who + '</div>' +
          '  <div class="small text-body mt-1" style="white-space: pre-wrap;">' + changes + '</div>' +
          '</div>';
      }).join('');
    }

    async function loadHistoryPanel(taskId, task) {
      if (!hasHistoryPanel) return;

      showHistoryLoading(taskId);

      try {
        var items = await loadTaskAudit(taskId);
        showHistoryContent();
        renderHistory(items, task);
      } catch (err) {
        console.error('[taskBejelentesEdit] history load failed', err);
        showHistoryContent();
        renderHistoryError(err && err.message ? err.message : 'Nem sikerült betölteni az előzményeket.');
      }
    }

    function resetFormState() {
      state.isSubmitting = false;
      AppForms.setSubmitting(submitBtn, false);
      state.currentId = null;

      try { formEl.reset(); } catch (e) {}
      formEl.classList.remove('was-validated');

      resetHistoryPanel();
    }

    async function openEditModal(id) {
      if (state.isOpening) return;
      state.isOpening = true;
      state.currentId = id;

      formEl.classList.remove('was-validated');
      resetHistoryPanel();

      AppModal.show(MODAL_ID);
      AppForms.setSubmitting(submitBtn, true);

      try {
        var task = await loadTask(id);
        var taskIdVal = pick(task, ['id', 'Id']) != null ? pick(task, ['id', 'Id']) : id;

        var data = {
          title: pick(task, ['title', 'Title']) || '',
          desc: pick(task, ['description', 'Description']) || '',
          statusId: pick(task, ['taskStatusPMId', 'TaskStatusPMId']),
          priorityId: pick(task, ['taskPriorityPMId', 'TaskPriorityPMId']),
          assignedToId: pick(task, ['assignedToId', 'AssignedToId']) || '',
          scheduledDate: pick(task, ['scheduledDate', 'ScheduledDate']),
          partnerId: pick(task, ['partnerId', 'PartnerId']),
          relatedPartnerId: pick(task, ['relatedPartnerId', 'RelatedPartnerId']),
          siteId: pick(task, ['siteId', 'SiteId']),
          siteName: pick(task, ['siteName', 'SiteName']),
          partnerName: pick(task, ['partnerName', 'PartnerName']),
          taskTypeId: pick(task, ['taskTypePMId', 'TaskTypePMId']),
          commMethodId: pick(task, ['taskPMcomMethodID', 'TaskPMcomMethodID']),
          commDesc: pick(task, ['communicationDescription', 'CommunicationDescription']) || ''
        };

        if (el.id) el.id.value = String(taskIdVal);
        if (el.title) el.title.value = data.title;
        if (el.desc) el.desc.value = data.desc;
        if (el.scheduledDate) el.scheduledDate.value = fmtForInputDate(data.scheduledDate);
        if (el.partnerHidden) el.partnerHidden.value = data.partnerId != null ? String(data.partnerId) : '';

        if (el.relatedPartner) {
          el.relatedPartner.value = data.relatedPartnerId != null ? String(data.relatedPartnerId) : '';
          try { el.relatedPartner.dispatchEvent(new Event('change', { bubbles: true })); } catch (e) {}
        }

        await Promise.all([
          loadTomOrSimpleSelect(el.taskType, API.taskTypes(DISPLAY_TYPE), data.taskTypeId, '-- Válasszon --'),
          loadTomOrSimpleSelect(el.status, API.taskStatuses(DISPLAY_TYPE), data.statusId, '-- Válasszon --'),
          loadSimpleSelect(el.assignedTo, API.assignees, data.assignedToId, '-- Válasszon --'),
          loadSimpleSelect(el.commMethod, API.commMethods, data.commMethodId, '-- Válasszon --'),
          loadHistoryPanel(taskIdVal, task)
        ]);

        if (el.priority) {
          el.priority.value = data.priorityId != null ? String(data.priorityId) : '';
          try { el.priority.dispatchEvent(new Event('change', { bubbles: true })); } catch (e) {}
        }

        if (el.commDesc) el.commDesc.value = String(data.commDesc || '');

        if (el.site && data.siteId != null) {
          var siteIdStr = String(data.siteId);
          var ts = await waitForTomSelect(el.site, 5000);

          if (ts) {
            ts.addOption({
              id: siteIdStr,
              text: data.siteName || ('#' + siteIdStr),
              partnerId: data.partnerId || null,
              partnerName: data.partnerName || '',
              partnerDetails: data.partnerName || ''
            });

            if (el.partnerHidden && data.partnerId != null) {
              el.partnerHidden.value = String(data.partnerId);
            }

            ts.setValue(siteIdStr, true);
            ts.refreshOptions(false);
          } else {
            el.site.value = siteIdStr;
            try { el.site.dispatchEvent(new Event('change', { bubbles: true })); } catch (e) {}
          }
        }
      } catch (err) {
        console.error('[taskBejelentesEdit] open failed', err);
        toast('Nem sikerült betölteni a feladatot (Task ID: ' + id + ').', 'danger');
        AppModal.hide(MODAL_ID);
      } finally {
        AppForms.setSubmitting(submitBtn, false);
        state.isOpening = false;
      }
    }

    AppModal.onHidden(MODAL_ID, function () {
      resetFormState();
    });

    Promise.allSettled([
      fetchSelectCached(API.assignees),
      fetchSelectCached(API.commMethods),
      fetchSelectCached(API.taskTypes(DISPLAY_TYPE)),
      fetchSelectCached(API.taskStatuses(DISPLAY_TYPE))
    ]);

    window.addEventListener('tasks:openEdit', function (e) {
      var id = AppForms.toInt(e && e.detail && e.detail.id);
      if (!id) return;
      openEditModal(id);
    });

    formEl.addEventListener('submit', async function (e) {
      e.preventDefault();
      if (state.isSubmitting) return;

      if (!formEl.checkValidity()) {
        formEl.classList.add('was-validated');
        toast('Kérlek töltsd ki a kötelező mezőket.', 'warning');
        return;
      }

      var fd = new FormData(formEl);

      var id = AppForms.toInt(fd.get('Id')) || state.currentId;
      if (!id) {
        toast('Hiányzik a Task ID a szerkesztéshez.', 'danger');
        return;
      }

      var partnerId = AppForms.toInt(fd.get('PartnerId'));
      if (!partnerId && el.partnerHidden) {
        partnerId = AppForms.toInt(el.partnerHidden.value);
      }

      var siteId = AppForms.toInt(fd.get('SiteId'));
      if (!siteId && el.site && el.site.tomselect) {
        siteId = AppForms.toInt(el.site.tomselect.getValue());
      }

      var taskTypeId = AppForms.toInt(fd.get('TaskTypePMId'));
      if (!taskTypeId && el.taskType && el.taskType.tomselect) {
        taskTypeId = AppForms.toInt(el.taskType.tomselect.getValue());
      }

      var sd = fd.get('ScheduledDate') ? String(fd.get('ScheduledDate')) : null;
      if (sd) sd = sd + 'T00:00:00';

      var payload = {
        Id: id,
        Title: String(fd.get('Title') || '').trim(),
        Description: String(fd.get('Description') || '').trim() || null,
        PartnerId: partnerId,
        RelatedPartnerId: AppForms.toInt(fd.get('RelatedPartnerId')),
        SiteId: siteId,
        TaskTypePMId: taskTypeId,
        TaskPriorityPMId: AppForms.toInt(fd.get('TaskPriorityPMId')),
        TaskStatusPMId: AppForms.toInt(fd.get('TaskStatusPMId')),
        AssignedToId: String(fd.get('AssignedToId') || '').trim() || null,
        TaskPMcomMethodID: AppForms.toInt(fd.get('TaskPMcomMethodID')),
        CommunicationDescription: String(fd.get('CommunicationDescription') || '').trim() || null,
        ScheduledDate: sd
      };

      if (!payload.Title) {
        toast('A tárgy (cím) megadása kötelező!', 'danger');
        return;
      }

      if (!payload.SiteId) {
        toast('A telephely kiválasztása kötelező!', 'danger');
        return;
      }

      if (!payload.TaskTypePMId) {
        toast('A feladat típusa kötelező!', 'danger');
        return;
      }

      if (!payload.PartnerId) {
        toast('Telephely kiválasztásakor Partner kötelező (Site választásból jön).', 'danger');
        return;
      }

      state.isSubmitting = true;
      AppForms.setSubmitting(submitBtn, true);

      try {
        var updated = await AppApi.put(API.updateTask(id), payload);

        toast('Intézkedés frissítve!', 'success');

        if (updated && (updated.id || updated.Id)) {
          try {
            updateRowFromTask(updated);
          } catch (_) {
            await refreshRow(id);
          }
        } else {
          await refreshRow(id);
        }

        try {
          await loadHistoryPanel(id, updated || { Id: id, Title: payload.Title });
        } catch (_) {}

        AppModal.hide(MODAL_ID);
      } catch (err) {
        console.error('[taskBejelentesEdit] update exception', err);
        toast('Nem sikerült a mentés.', 'danger');
      } finally {
        state.isSubmitting = false;
        AppForms.setSubmitting(submitBtn, false);
      }
    });
  });
})();