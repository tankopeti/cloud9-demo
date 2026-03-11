// wwwroot/js/Task/taskIntezkedesViewModal.js
(function () {
  'use strict';

  const MODAL_ID = 'taskViewModal';
  const DISPLAY_TYPE = 2;

  const API = {
    taskById: (id) => `/api/tasks/${encodeURIComponent(id)}`,
    taskAudit: (id) => `/api/tasks/${encodeURIComponent(id)}/audit`
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
    setText('#viewDueDate', '–');

    setText('#viewCreatedDate', '–');
    setText('#viewCreatedBy', '–');
    setText('#viewUpdatedDate', '–');
    setText('#viewUpdatedBy', '–');

    resetAttachments();
  }

  function resetTitle() {
    if (!el.title) return;
    el.title.innerHTML = '<i class="bi bi-eye me-2"></i> Intézkedés részletei';
  }

  function setTitle(taskId, title) {
    if (!el.title) return;
    el.title.innerHTML = `<i class="bi bi-eye me-2"></i> Intézkedés #${esc(taskId)} – ${esc(title)}`;
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

  function setHistoryLoading() {
    resetHistory();
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

  function setHistoryError() {
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
        <div class="alert alert-warning mb-0">
          Nem sikerült betölteni az előzményeket.
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

function renderHistoryItems(items, taskTitle) {
  if (el.viewHistoryTaskTitle) {
    el.viewHistoryTaskTitle.textContent = text(taskTitle || 'Intézkedés');
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

  el.viewHistoryList.innerHTML = items.map(function (item) {
    const changedAt = getValue(
      item,
      'changedAt',
      'ChangedAt',
      'createdAt',
      'CreatedAt',
      'auditDate',
      'AuditDate',
      'timestamp',
      'Timestamp'
    );

    const changedBy = getValue(
      item,
      'changedByName',
      'ChangedByName',
      'userName',
      'UserName',
      'createdByName',
      'CreatedByName'
    ) || 'Ismeretlen';

    const action = getValue(
      item,
      'actionTypeName',
      'ActionTypeName',
      'actionType',
      'ActionType',
      'eventType',
      'EventType'
    ) || 'Módosítás';

    const detailsHtml = getValue(item, 'detailsHtml', 'DetailsHtml');

    const description = getValue(
      item,
      'description',
      'Description',
      'details',
      'Details',
      'message',
      'Message'
    );

    const renderedDetails = detailsHtml
      ? String(detailsHtml)
      : `<div class="small text-body-secondary">Nincs részletezés.</div>`;

    return `
      <div class="border-start ps-3 ms-1 mb-4">
        <div class="d-flex justify-content-between align-items-start gap-3">
          <div>
            <div class="fw-semibold">${esc(action)}</div>
            <div class="small text-body-secondary">${esc(changedBy)}</div>
          </div>
          <div class="small text-body-secondary text-nowrap">${esc(fmtDate(changedAt))}</div>
        </div>

        <div class="mt-2">
          ${detailsHtml ? renderedDetails : `<div class="small text-body-secondary">${esc(description || 'Nincs részletezés.')}</div>`}
        </div>
      </div>
    `;
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
    const dueDate = getValue(task, 'dueDate', 'DueDate');

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
    setText('#viewDueDate', fmtDate(dueDate));

    setText('#viewCreatedDate', fmtDate(createdDate));
    setText('#viewCreatedBy', createdByName);
    setText('#viewUpdatedDate', fmtDate(updatedDate));
    setText('#viewUpdatedBy', updatedByName);

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
    setHistoryLoading();
    AppModal.show(MODAL_ID);

    try {
      const [task, audit] = await Promise.all([
        fetchTask(idNum),
        fetchTaskAudit(idNum)
      ]);

      const vm = renderTask(task);
      const items = getAuditItems(audit);

      if (!items.length) {
        setHistoryEmpty(vm.title);
      } else {
        renderHistoryItems(items, vm.title);
      }
    } catch (err) {
      console.error('[taskIntezkedesViewModal] load failed', err);
      setError('Nem sikerült betölteni a feladatot.');
      setHistoryError();
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