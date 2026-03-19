// wwwroot/js/Task/taskIntezkedesViewModal.js
(function () {
  'use strict';

  const MODAL_ID = 'taskViewModal';
  const DISPLAY_TYPE = 2;

  const API = {
    taskById: (id) => `/api/tasks/${encodeURIComponent(id)}`,
    taskAudit: (id) => `/api/tasks/${encodeURIComponent(id)}/audit?_=${Date.now()}`
  };

  const el = {};

  function qs(selector) {
    return AppForms.qs(document, selector);
  }

  function text(value) {
    return value == null || value === '' ? '–' : String(value);
  }

  function esc(value) {
    const div = document.createElement('div');
    div.textContent = value == null ? '' : String(value);
    return div.innerHTML;
  }

  function fmtDate(value) {
    if (!value) return '–';

    const d = new Date(value);
    if (isNaN(d.getTime())) return '–';

    return d.toLocaleString('hu-HU', {
      year: 'numeric',
      month: '2-digit',
      day: '2-digit',
      hour: '2-digit',
      minute: '2-digit'
    }).replace(',', '');
  }

  function badge(textValue, color) {
    const safeText = text(textValue);
    const safeColor = color && String(color).trim() ? String(color).trim() : '#6c757d';
    return `<span class="badge task-status-badge" style="background:${esc(safeColor)}">${esc(safeText)}</span>`;
  }

  function getValue(obj, ...keys) {
    for (const key of keys) {
      const value = obj?.[key];
      if (value !== undefined && value !== null) return value;
    }
    return null;
  }

  function setText(selector, value) {
    const node = qs(selector);
    if (!node) return;
    node.textContent = text(value);
  }

  function setHtml(selector, value) {
    const node = qs(selector);
    if (!node) return;
    node.innerHTML = value == null || value === '' ? '–' : String(value);
  }

  function setCompletedDate(value) {
    const node = qs('#viewCompletedDate');
    if (!node) return;

    node.classList.remove('text-muted', 'fw-semibold', 'text-success');

    if (value) {
      node.textContent = fmtDate(value);
      node.classList.add('fw-semibold', 'text-success');
      return;
    }

    node.textContent = 'Nincs lezárva';
    node.classList.add('text-muted');
  }

  function show(node) {
    if (node) node.classList.remove('d-none');
  }

  function hide(node) {
    if (node) node.classList.add('d-none');
  }

  function resetAttachments() {
    if (!el.viewAttachments) return;
    el.viewAttachments.innerHTML = '<div class="text-muted small">Nincs csatolt dokumentum.</div>';
  }

  function resetFields() {
    setText('#viewTitle', '–');
    setText('#viewDescription', '–');

    setText('#viewPartner', '–');
    setText('#viewRelatedPartner', '–');
    setText('#viewSite', '–');
    setText('#viewRelatedEmployee', '–');

    setText('#viewTaskType', '–');
    setText('#viewAssignedTo', '–');

    setHtml('#viewPriority', '–');
    setHtml('#viewStatus', '–');

    setText('#viewCommunicationMethod', '–');
    setText('#viewCommunicationDescription', '–');

    setText('#viewScheduledDate', '–');
    setText('#viewOptionalDate1', '–');
    setText('#viewOptionalDate2', '–');
    setText('#viewDueDate', '–');

    setText('#viewCreatedDate', '–');
    setText('#viewCreatedBy', '–');
    setText('#viewUpdatedDate', '–');
    setText('#viewUpdatedBy', '–');

    setCompletedDate(null);

    resetAttachments();
  }

  function resetTitle() {
    if (!el.title) return;
    el.title.innerHTML = '<i class="bi bi-eye-fill"></i> Intézkedés részletei';
  }

  function setTitle(taskId, title) {
    if (!el.title) return;
    el.title.innerHTML = `<i class="bi bi-eye-fill"></i> Intézkedés #${esc(taskId)} – ${esc(title)}`;
  }

  function setLoading() {
    hide(el.error);
    hide(el.content);
    show(el.loading);
  }

  function setError(message) {
    if (el.error) {
      el.error.textContent = message || 'Nem sikerült betölteni a feladatot.';
      show(el.error);
    }

    hide(el.loading);
    hide(el.content);
  }

  function setReady() {
    hide(el.loading);
    hide(el.error);
    show(el.content);
  }

  function resetHistory() {
    if (el.viewHistoryCountBadge) el.viewHistoryCountBadge.textContent = '0';
    if (el.viewHistoryTaskTitle) el.viewHistoryTaskTitle.textContent = 'Betöltés...';

    show(el.viewHistoryLoading);
    hide(el.viewHistoryContent);
    hide(el.viewHistoryEmpty);

    if (el.viewHistoryList) {
      el.viewHistoryList.innerHTML = '';
    }
  }

  function setHistoryLoading(taskId) {
    if (el.viewHistoryTaskTitle) {
      el.viewHistoryTaskTitle.textContent = taskId ? ('Intézkedés #' + taskId) : 'Betöltés...';
    }

    if (el.viewHistoryCountBadge) {
      el.viewHistoryCountBadge.textContent = '0';
    }

    if (el.viewHistoryList) {
      el.viewHistoryList.innerHTML = '';
    }

    show(el.viewHistoryLoading);
    hide(el.viewHistoryContent);
    hide(el.viewHistoryEmpty);
  }

  function setHistoryEmpty(taskTitle) {
    if (el.viewHistoryTaskTitle) {
      el.viewHistoryTaskTitle.textContent = text(taskTitle || 'Intézkedés');
    }

    if (el.viewHistoryCountBadge) {
      el.viewHistoryCountBadge.textContent = '0';
    }

    hide(el.viewHistoryLoading);
    show(el.viewHistoryContent);
    show(el.viewHistoryEmpty);

    if (el.viewHistoryList) {
      el.viewHistoryList.innerHTML = '';
    }
  }

  function setHistoryError(message) {
    if (el.viewHistoryTaskTitle) {
      el.viewHistoryTaskTitle.textContent = 'Előzmények nem tölthetők be';
    }

    if (el.viewHistoryCountBadge) {
      el.viewHistoryCountBadge.textContent = '0';
    }

    hide(el.viewHistoryLoading);
    show(el.viewHistoryContent);
    hide(el.viewHistoryEmpty);

    if (el.viewHistoryList) {
      el.viewHistoryList.innerHTML = `
        <div class="alert alert-danger mb-0">
          <div class="fw-semibold">Nem sikerült betölteni az előzményeket.</div>
          <div class="small mt-1">${esc(message || 'Ismeretlen hiba')}</div>
        </div>
      `;
    }
  }

  function getAuditItems(auditResponse) {
    if (Array.isArray(auditResponse)) return auditResponse;
    if (Array.isArray(auditResponse?.items)) return auditResponse.items;
    if (Array.isArray(auditResponse?.Items)) return auditResponse.Items;
    if (Array.isArray(auditResponse?.results)) return auditResponse.results;
    if (Array.isArray(auditResponse?.Results)) return auditResponse.Results;
    return [];
  }

  function translateAuditField(fieldName) {
    const map = {
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
      UpdatedDate: 'Módosítva',
      CompletedDate: 'Lezárva'
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
      fieldName === 'UpdatedDate' ||
      fieldName === 'CompletedDate'
    ) {
      return fmtDate(value);
    }

    return String(value);
  }

  function localizeAuditChanges(changesText) {
    if (!changesText) return '–';

    const parts = String(changesText)
      .split(';')
      .map(x => x.trim())
      .filter(Boolean);

    const localized = parts.map(part => {
      const match = part.match(/^([A-Za-z0-9_]+)\s*:\s*(.*?)\s*→\s*(.*)$/);
      if (!match) return esc(part);

      const fieldName = match[1];
      const oldValue = match[2];
      const newValue = match[3];

      return esc(translateAuditField(fieldName)) +
        ': ' +
        esc(formatAuditValue(fieldName, oldValue)) +
        ' → ' +
        esc(formatAuditValue(fieldName, newValue));
    });

    return localized.join('\n');
  }

  function historyMetaForAction(action) {
    const a = String(action || '').toLowerCase();

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
    if (a === 'closed' || a === 'completed' || a === 'taskclosed') {
      return { icon: 'bi-lock-fill', badge: 'bg-danger', label: 'Lezárva' };
    }
    if (a === 'reopened' || a === 'reopen' || a === 'taskreopened') {
      return { icon: 'bi-unlock-fill', badge: 'bg-success', label: 'Újranyitva' };
    }

    return { icon: 'bi-info-circle-fill', badge: 'bg-secondary', label: action || 'Esemény' };
  }

  function renderHistoryItems(items, task) {
    const taskId = getValue(task, 'id', 'Id');
    const taskTitle = getValue(task, 'title', 'Title') || '';

    if (el.viewHistoryTaskTitle) {
      el.viewHistoryTaskTitle.textContent = taskId
        ? ('Intézkedés #' + taskId + (taskTitle ? ' – ' + taskTitle : ''))
        : (taskTitle || 'Előzmények');
    }

    hide(el.viewHistoryLoading);
    show(el.viewHistoryContent);

    if (!Array.isArray(items) || items.length === 0) {
      if (el.viewHistoryCountBadge) {
        el.viewHistoryCountBadge.textContent = '0';
      }

      show(el.viewHistoryEmpty);

      if (el.viewHistoryList) {
        el.viewHistoryList.innerHTML = '';
      }

      return;
    }

    hide(el.viewHistoryEmpty);

    if (el.viewHistoryCountBadge) {
      el.viewHistoryCountBadge.textContent = String(items.length);
    }

    if (!el.viewHistoryList) return;

    el.viewHistoryList.innerHTML = items.map(function (x) {
      const action = getValue(x, 'action', 'Action');
      const changedAt = getValue(x, 'changedAt', 'ChangedAt');
      const changedByName = getValue(x, 'changedByName', 'ChangedByName');
      const changes = getValue(x, 'changes', 'Changes');

      const meta = historyMetaForAction(action);
      const when = fmtDate(changedAt) || '—';
      const who = changedByName ? esc(changedByName) : '—';
      const localizedChanges = changes ? localizeAuditChanges(changes) : '–';

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
        '  <div class="small text-body mt-1" style="white-space: pre-wrap;">' + localizedChanges + '</div>' +
        '</div>';
    }).join('');
  }

  function resetModal() {
    resetTitle();
    resetFields();
    resetHistory();

    hide(el.error);
    hide(el.content);
    hide(el.loading);

    if (el.editBtn) {
      el.editBtn.style.display = 'none';
      el.editBtn.dataset.taskId = '';
      el.editBtn.onclick = null;
    }
  }

  function fileLinkFor(att) {
    const docId = att.documentId ?? att.DocumentId;
    const filePath = att.filePath ?? att.FilePath;

    if (filePath) return String(filePath);
    return `/documents/download/${encodeURIComponent(docId)}`;
  }

  function renderAttachments(task) {
    if (!el.viewAttachments) return;

    const list = task.attachments ?? task.Attachments ?? [];

    if (!Array.isArray(list) || list.length === 0) {
      resetAttachments();
      return;
    }

    el.viewAttachments.innerHTML = list.map(att => {
      const docId = att.documentId ?? att.DocumentId;
      const name = att.fileName ?? att.FileName ?? (`Dokumentum #${docId}`);
      const linkedDate = att.linkedDate ?? att.LinkedDate;
      const linkedBy = att.linkedByName ?? att.LinkedByName;
      const note = att.note ?? att.Note;
      const href = fileLinkFor(att);

      return `
        <div class="d-flex justify-content-between align-items-start p-2 mb-2 border rounded task-view-attachment-item">
          <div class="me-2">
            <div>
              <i class="bi bi-paperclip me-2 text-success"></i>
              <a href="${esc(href)}" target="_blank" rel="noopener">
                <strong>${esc(name)}</strong>
              </a>
            </div>
            <div class="text-muted small mt-1">
              ${linkedDate ? esc(fmtDate(linkedDate)) : '–'}
              ${linkedBy ? ` • ${esc(linkedBy)}` : ''}
              ${note ? ` • ${esc(note)}` : ''}
            </div>
          </div>
          <span class="badge border task-view-attachment-badge">#${esc(docId)}</span>
        </div>
      `;
    }).join('');
  }

  function bindEditButton(id) {
    if (!el.editBtn) return;

    el.editBtn.style.display = 'inline-block';
    el.editBtn.dataset.taskId = String(id);
    el.editBtn.onclick = function () {
      window.dispatchEvent(new CustomEvent('tasks:openEdit', {
        detail: {
          id: Number(id),
          displayType: DISPLAY_TYPE
        }
      }));
    };
  }

  function renderTask(task) {
    const id = getValue(task, 'id', 'Id');
    const title = getValue(task, 'title', 'Title') || '';
    const description = getValue(task, 'description', 'Description');

    const communicationMethodName = getValue(
      task,
      'taskPMcomMethodName',
      'TaskPMcomMethodName',
      'communicationMethodName',
      'CommunicationMethodName'
    );

    const communicationDescription = getValue(
      task,
      'communicationDescription',
      'CommunicationDescription'
    );

    const partnerName = getValue(task, 'partnerName', 'PartnerName');
    const relatedPartnerName = getValue(task, 'relatedPartnerName', 'RelatedPartnerName');

    const siteName = getValue(task, 'siteName', 'SiteName');
    const city = getValue(task, 'city', 'City');
    const siteDisplay = city ? `${text(siteName)} (${text(city)})` : text(siteName);

    const taskTypeName = getValue(task, 'taskTypePMName', 'TaskTypePMName');
    const taskPriorityName = getValue(task, 'taskPriorityPMName', 'TaskPriorityPMName');
    const taskStatusName = getValue(task, 'taskStatusPMName', 'TaskStatusPMName');

    const statusColor = getValue(task, 'colorCode', 'ColorCode', 'StatusColorCode');
    const priorityColor = getValue(task, 'priorityColorCode', 'PriorityColorCode');

    const assignedToName = getValue(task, 'assignedToName', 'AssignedToName');
    const relatedEmployeeName = getValue(
      task,
      'relatedEmployeeName',
      'RelatedEmployeeName',
      'employeeName',
      'EmployeeName'
    );

    const scheduledDate = getValue(task, 'scheduledDate', 'ScheduledDate');
    const optionalDate1 = getValue(task, 'optionalDate1', 'OptionalDate1');
    const optionalDate2 = getValue(task, 'optionalDate2', 'OptionalDate2');
    const dueDate = getValue(task, 'dueDate', 'DueDate');
    const completedDate = getValue(task, 'completedDate', 'CompletedDate');

    const createdDate = getValue(task, 'createdDate', 'CreatedDate');
    const updatedDate = getValue(task, 'updatedDate', 'UpdatedDate');
    const createdByName = getValue(task, 'createdByName', 'CreatedByName');
    const updatedByName = getValue(task, 'updatedByName', 'UpdatedByName');

    setTitle(id, title);

    setText('#viewTitle', title);
    setText('#viewDescription', description);

    setText('#viewPartner', partnerName);
    setText('#viewRelatedPartner', relatedPartnerName);
    setText('#viewSite', siteDisplay);
    setText('#viewRelatedEmployee', relatedEmployeeName);

    setText('#viewTaskType', taskTypeName);
    setText('#viewAssignedTo', assignedToName);
    setHtml('#viewPriority', badge(taskPriorityName, priorityColor));
    setHtml('#viewStatus', badge(taskStatusName, statusColor));

    setText('#viewCommunicationMethod', communicationMethodName);
    setText('#viewCommunicationDescription', communicationDescription);

    setText('#viewScheduledDate', fmtDate(scheduledDate));
    setText('#viewOptionalDate1', fmtDate(optionalDate1));
    setText('#viewOptionalDate2', fmtDate(optionalDate2));
    
    setText('#viewDueDate', fmtDate(dueDate));

    setText('#viewCreatedDate', fmtDate(createdDate));
    setText('#viewCreatedBy', createdByName);
    setText('#viewUpdatedDate', fmtDate(updatedDate));
    setText('#viewUpdatedBy', updatedByName);
    setCompletedDate(completedDate);

    renderAttachments(task);
    bindEditButton(id);
    setReady();

    return {
      id: id,
      title: title
    };
  }

  async function fetchTask(taskId) {
    return AppApi.get(API.taskById(taskId));
  }

  async function fetchTaskAudit(taskId) {
    return AppApi.get(API.taskAudit(taskId));
  }

  async function openTaskView(taskId) {
    const idNum = AppForms.toInt(taskId);
    if (!idNum) return;

    resetFields();
    setLoading();
    setHistoryLoading(idNum);
    AppModal.show(MODAL_ID);

    try {
      const [task, audit] = await Promise.all([
        fetchTask(idNum),
        fetchTaskAudit(idNum)
      ]);

      renderTask(task);

      const items = getAuditItems(audit);

      if (!items.length) {
        setHistoryEmpty(getValue(task, 'title', 'Title'));
      } else {
        renderHistoryItems(items, task);
      }
    } catch (err) {
      console.error('[taskIntezkedesViewModal] load failed', err);
      setError('Nem sikerült betölteni a feladatot.');
      setHistoryError(err && err.message ? err.message : 'Nem sikerült betölteni az előzményeket.');
    }
  }

  function handleViewTrigger(target) {
    const btn = target.closest('.btn-view-task,[data-view-task],.js-view-task-btn');
    if (!btn) return;

    const id =
      btn.dataset.taskId ||
      btn.dataset.viewTask ||
      btn.getAttribute('data-task-id') ||
      btn.getAttribute('data-view-task');

    if (id) {
      openTaskView(id);
    }
  }

  function initElements() {
    el.modal = qs('#taskViewModal');
    el.title = qs('#taskModalTitle');
    el.editBtn = qs('#editTaskBtn');

    el.loading = qs('#taskViewLoading');
    el.error = qs('#taskViewError');
    el.content = qs('#taskViewContent');

    el.viewAttachments = qs('#viewAttachments');

    el.viewHistoryCountBadge = qs('#viewHistoryCountBadge');
    el.viewHistoryTaskTitle = qs('#viewHistoryTaskTitle');
    el.viewHistoryLoading = qs('#viewHistoryLoading');
    el.viewHistoryContent = qs('#viewHistoryContent');
    el.viewHistoryEmpty = qs('#viewHistoryEmpty');
    el.viewHistoryList = qs('#viewHistoryList');
  }

  function bindEvents() {
    document.addEventListener('click', function (e) {
      handleViewTrigger(e.target);
    });

    window.addEventListener('tasks:view', function (e) {
      const id = e?.detail?.id ?? e?.detail?.taskId;
      if (id) openTaskView(id);
    });

    AppModal.onHidden(MODAL_ID, function () {
      resetModal();
    });
  }

  function init() {
    initElements();

    if (!el.modal || !el.content || !el.loading || !el.error) {
      console.warn('[taskIntezkedesViewModal] missing modal elements', {
        modal: !!el.modal,
        content: !!el.content,
        loading: !!el.loading,
        error: !!el.error
      });
      return;
    }

    resetModal();
    bindEvents();

    window.Tasks = window.Tasks || {};
    window.Tasks.openViewModal = openTaskView;

    console.log('[taskIntezkedesViewModal] initialized');
  }

  document.addEventListener('DOMContentLoaded', init);
})();