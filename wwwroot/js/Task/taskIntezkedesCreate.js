// wwwroot/js/Task/taskIntezkedesCreate.js
(function () {
  'use strict';

  console.log('[taskIntezkedesCreate] loaded');

  document.addEventListener('DOMContentLoaded', function () {
    console.log('[taskIntezkedesCreate] DOM loaded');

    var modalEl = document.getElementById('newTaskModal');
    if (!modalEl) return;

    var formEl = modalEl.querySelector('form');
    if (!formEl) return;

    var submitBtn = AppForms.qs(formEl, 'button[type="submit"]');

    var el = {
      site: AppForms.qs(formEl, '#SiteId, [name="SiteId"]'),
      assignedTo: AppForms.qs(formEl, '#AssignedToId, [name="AssignedToId"]'),
      relatedEmployee: AppForms.qs(formEl, '#RelatedEmployeeId, [name="RelatedEmployeeId"]'),
      commMethod: AppForms.qs(formEl, '#TaskPMcomMethodID, [name="TaskPMcomMethodID"]'),
      taskType: AppForms.qs(formEl, '#TaskTypePMId, [name="TaskTypePMId"]'),
      taskStatus: AppForms.qs(formEl, '#TaskStatusPMId, [name="TaskStatusPMId"]')
    };

    var API = {
      create: '/api/tasks',
      assignees: '/api/tasks/assignees/select',
      commMethods: '/api/tasks/taskpm-communication-methods/select',
      taskTypes: function (displayType) {
        return '/api/tasks/tasktypes/select?displayType=' + encodeURIComponent(displayType);
      },
      taskStatuses: function (displayType) {
        return '/api/tasks/taskstatuses/select?displayType=' + encodeURIComponent(displayType);
      },
      siteEmployees: function (siteId) {
        return '/api/sites/' + encodeURIComponent(siteId) + '/employees';
      }
    };

    var DISPLAY_TYPE = AppForms.toInt(modalEl.getAttribute('data-display-type')) || 2;
    var isSubmitting = false;

    function toast(message, type) {
      if (window.AppToast && typeof AppToast.show === 'function') {
        AppToast.show(message, type || 'info');
        return;
      }

      console.log('[toast fallback]', type || 'info', message);
    }

    function resetRelatedEmployeeSelect(message) {
      if (!el.relatedEmployee) return;

      el.relatedEmployee.innerHTML =
        '<option value="">' + String(message || '-- Előbb válassz telephelyet --') + '</option>';
      el.relatedEmployee.value = '';
      el.relatedEmployee.disabled = true;
    }

    async function loadSiteEmployeesSelect(selectEl, siteId, selectedId) {
      if (!selectEl) return;

      if (!siteId) {
        resetRelatedEmployeeSelect('-- Előbb válassz telephelyet --');
        return;
      }

      selectEl.disabled = true;
      selectEl.innerHTML = '<option value="">Betöltés...</option>';

      try {
        var data = await AppApi.get(API.siteEmployees(siteId));
        var items = Array.isArray(data && data.items) ? data.items : [];

        if (items.length === 0) {
          selectEl.innerHTML = '<option value="">-- Nincs a telephelyhez kapcsolt személy --</option>';
          selectEl.value = '';
          return;
        }

        selectEl.innerHTML =
          '<option value="">-- Válasszon --</option>' +
          items.map(function (x) {
            var text = x.fullName || x.employeeName || 'Névtelen dolgozó';
            if (x.partnerName) text += ' (' + x.partnerName + ')';

            return '<option value="' + String(x.employeeId) + '">' + text + '</option>';
          }).join('');

        selectEl.value = selectedId != null ? String(selectedId) : '';
        try { selectEl.dispatchEvent(new Event('change', { bubbles: true })); } catch (e) {}
      } catch (e) {
        console.error('[taskIntezkedesCreate] site employees load failed', e);
        selectEl.innerHTML = '<option value="">-- Nem sikerült betölteni a kapcsolt személyeket --</option>';
      } finally {
        selectEl.disabled = false;
      }
    }

    if (el.site && el.relatedEmployee) {
      if (!el.site.value) {
        resetRelatedEmployeeSelect('-- Előbb válassz telephelyet --');
      }

      el.site.addEventListener('change', async function () {
        var siteId = AppForms.toInt(el.site.value);
        console.log('[taskIntezkedesCreate] site changed:', el.site.value, siteId);

        el.relatedEmployee.value = '';
        await loadSiteEmployeesSelect(el.relatedEmployee, siteId, '');
      });
    } else if (el.relatedEmployee) {
      resetRelatedEmployeeSelect('-- Telephely mező nem található --');
    }

    AppModal.onHidden('newTaskModal', function () {
      isSubmitting = false;
      AppForms.setSubmitting(submitBtn, false);
      AppForms.reset(formEl);
      resetRelatedEmployeeSelect('-- Előbb válassz telephelyet --');
    });

    AppModal.onShown('newTaskModal', async function () {
      await Promise.allSettled([
        AppSelects.load(el.taskType, API.taskTypes(DISPLAY_TYPE), el.taskType ? el.taskType.value || '' : '', '-- Válasszon --'),
        AppSelects.load(el.taskStatus, API.taskStatuses(DISPLAY_TYPE), el.taskStatus ? el.taskStatus.value || '' : '', '-- Válasszon --'),
        AppSelects.load(el.assignedTo, API.assignees, el.assignedTo ? el.assignedTo.value || '' : '', '-- Válasszon --'),
        AppSelects.load(el.commMethod, API.commMethods, el.commMethod ? el.commMethod.value || '' : '', '-- Válasszon --')
      ]);

      if (el.relatedEmployee) {
        var currentSiteId = el.site ? AppForms.toInt(el.site.value) : null;
        await loadSiteEmployeesSelect(el.relatedEmployee, currentSiteId, el.relatedEmployee.value || '');
      }
    });

    formEl.addEventListener('submit', async function (e) {
      e.preventDefault();
      if (isSubmitting) return;

      if (!formEl.checkValidity()) {
        formEl.classList.add('was-validated');
        toast('Kérlek töltsd ki a kötelező mezőket.', 'warning');
        return;
      }

      var fd = new FormData(formEl);
      var scheduledIso = AppForms.combineDateTime(fd.get('ScheduledDate'), fd.get('ScheduledTime'));

      var payload = {
        Title: String(fd.get('Title') || '').trim(),
        Description: String(fd.get('Description') || '').trim() || null,

        TaskPMcomMethodID: AppForms.toInt(fd.get('TaskPMcomMethodID')),
        CommunicationDescription: String(fd.get('CommunicationDescription') || '').trim() || null,

        PartnerId: AppForms.toInt(fd.get('PartnerId')),
        RelatedPartnerId: AppForms.toInt(fd.get('RelatedPartnerId')) || null,
        SiteId: AppForms.toInt(fd.get('SiteId')),
        TaskTypePMId: AppForms.toInt(fd.get('TaskTypePMId')),

        TaskPriorityPMId: AppForms.toInt(fd.get('TaskPriorityPMId')),
        TaskStatusPMId: AppForms.toInt(fd.get('TaskStatusPMId')),
        AssignedToId: String(fd.get('AssignedToId') || '').trim() || null,
        RelatedEmployeeId: AppForms.toInt(fd.get('RelatedEmployeeId')),

        ScheduledDate: scheduledIso
      };

      if (!payload.Title) {
        toast('A tárgy megadása kötelező!', 'danger');
        return;
      }

      if (!payload.SiteId) {
        toast('A telephely kiválasztása kötelező!', 'danger');
        return;
      }

      if (!payload.PartnerId) {
        toast('A partner kiválasztása kötelező!', 'danger');
        return;
      }

      if (!payload.TaskTypePMId) {
        toast('A feladat típusa kötelező!', 'danger');
        return;
      }

      console.log('[taskIntezkedesCreate] payload', payload);

      isSubmitting = true;
      AppForms.setSubmitting(submitBtn, true);

      try {
        var created = await AppApi.post(API.create, payload);

        toast('Intézkedés létrehozva!', 'success');
        AppModal.hide('newTaskModal');

        window.dispatchEvent(new CustomEvent('tasks:reload', {
          detail: { created: created }
        }));
      } catch (err) {
        console.error('[taskIntezkedesCreate] CREATE EXCEPTION', err);
        toast('Nem sikerült a mentés.', 'danger');
      } finally {
        isSubmitting = false;
        AppForms.setSubmitting(submitBtn, false);
      }
    });
  });
})();