// wwwroot/js/Tasks/TaskCreate.js
(function () {
  'use strict';

  console.log('[TaskCreate] loaded file');

  document.addEventListener('DOMContentLoaded', () => {
    console.log('[TaskCreate] DOM loaded');

    if (typeof TomSelect === 'undefined') {
      console.error('[TaskCreate] TomSelect nincs betöltve');
      return;
    }

    const modalEl = document.getElementById('newTaskModal');
    if (!modalEl) {
      console.warn('[TaskCreate] #newTaskModal nincs meg');
      return;
    }

    const formEl = document.getElementById('createTaskForm') || modalEl.querySelector('form');
    if (!formEl) {
      console.warn('[TaskCreate] create form nincs meg');
      return;
    }

    const el = {
      type: AppForms.qs(formEl, '[name="TaskTypePMId"]'),
      status: AppForms.qs(formEl, '[name="TaskStatusPMId"]'),
      priority: AppForms.qs(formEl, '[name="TaskPriorityPMId"]'),
      assignedTo: AppForms.qs(formEl, '[name="AssignedToId"]'),
      partner: AppForms.qs(formEl, '[name="PartnerId"]'),
      site: AppForms.qs(formEl, '[name="SiteId"], #SiteId')
    };

    const API = {
      taskTypes: '/api/tasks/tasktypes/select',
      taskStatuses: '/api/tasks/taskstatuses/select',
      taskPriorities: '/api/tasks/taskpriorities/select',
      users: '/api/users/select',
      partners: (q) => '/api/partners/select?search=' + encodeURIComponent(q),
      sitesByPartner: (partnerId) => '/api/sites/by-partner/' + encodeURIComponent(partnerId) + '?search='
    };

    let currentPartnerId = '';

    let tsType = null;
    let tsStatus = null;
    let tsPriority = null;
    let tsAssignedTo = null;
    let tsPartner = null;
    let tsSite = null;

    function destroyTom(ts) {
      try {
        if (ts) ts.destroy();
      } catch (e) {
        console.warn('[TaskCreate] TomSelect destroy warning', e);
      }
    }

    function destroyAll() {
      destroyTom(tsType);
      destroyTom(tsStatus);
      destroyTom(tsPriority);
      destroyTom(tsAssignedTo);
      destroyTom(tsPartner);
      destroyTom(tsSite);

      tsType = null;
      tsStatus = null;
      tsPriority = null;
      tsAssignedTo = null;
      tsPartner = null;
      tsSite = null;

      currentPartnerId = '';
    }

    function normalizeItems(items) {
      return (items || []).map(x => ({
        id: String(
          x.id ??
          x.value ??
          x.siteId ??
          x.partnerId ??
          x.employeeId ??
          ''
        ),
        text: String(
          x.text ??
          x.name ??
          x.label ??
          x.siteName ??
          x.fullName ??
          ''
        )
      })).filter(x => x.id);
    }

    function initSimpleTom(selectEl) {
      if (!selectEl) return null;

      return new TomSelect(selectEl, {
        dropdownParent: 'body',
        openOnFocus: true,
        closeAfterSelect: true,
        allowEmptyOption: true
      });
    }

    function initPartnerTom() {
      if (!el.partner) return null;

      return new TomSelect(el.partner, {
        dropdownParent: 'body',
        valueField: 'id',
        labelField: 'text',
        searchField: 'text',
        loadThrottle: 300,
        preload: false,
        closeAfterSelect: true,
        openOnFocus: true,
        load: async function (query, callback) {
          try {
            if (!query || query.length < 2) {
              callback();
              return;
            }

            const data = await AppApi.get(API.partners(query));
            callback(normalizeItems(data));
          } catch (e) {
            console.error('[TaskCreate] partner load failed', e);
            callback();
          }
        }
      });
    }

    function initSiteTom() {
      if (!el.site) return null;

      const siteTom = new TomSelect(el.site, {
        dropdownParent: 'body',
        valueField: 'id',
        labelField: 'text',
        searchField: 'text',
        closeAfterSelect: true,
        openOnFocus: true
      });

      siteTom.disable();
      return siteTom;
    }

    async function reloadSitesByPartner(partnerId) {
      if (!tsSite) return;

      tsSite.clear(true);
      tsSite.clearOptions();

      if (!partnerId) {
        tsSite.disable();
        return;
      }

      tsSite.enable();

      try {
        const data = await AppApi.get(API.sitesByPartner(partnerId));
        const items = normalizeItems(data);

        tsSite.addOptions(items);
        tsSite.refreshOptions(false);
      } catch (e) {
        console.error('[TaskCreate] sites load failed', e);
      }
    }

    AppModal.onShown('newTaskModal', async () => {
      console.log('[TaskCreate] shown.bs.modal');

      destroyAll();

      await Promise.allSettled([
        AppSelects.load(el.type, API.taskTypes, '', '-- Válasszon típust --'),
        AppSelects.load(el.status, API.taskStatuses, '', '-- Válasszon státuszt --'),
        AppSelects.load(el.priority, API.taskPriorities, '', '-- Válasszon prioritást --'),
        AppSelects.load(el.assignedTo, API.users, '', '-- Nincs kijelölve --')
      ]);

      tsType = initSimpleTom(el.type);
      tsStatus = initSimpleTom(el.status);
      tsPriority = initSimpleTom(el.priority);
      tsAssignedTo = initSimpleTom(el.assignedTo);

      tsPartner = initPartnerTom();
      tsSite = initSiteTom();

      if (tsPartner) {
        tsPartner.on('change', async (value) => {
          currentPartnerId = value ? String(value) : '';
          console.log('[TaskCreate] partner changed', currentPartnerId);

          await reloadSitesByPartner(currentPartnerId);
        });
      }
    });

    AppModal.onHidden('newTaskModal', () => {
      console.log('[TaskCreate] hidden.bs.modal');
      destroyAll();
    });
  });
})();