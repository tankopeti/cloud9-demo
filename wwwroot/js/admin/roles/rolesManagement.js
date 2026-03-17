document.addEventListener('DOMContentLoaded', function () {
    const rolesTableBody = document.querySelector('#rolesTable tbody');

    const addRoleForm = document.getElementById('addRoleForm');
    const editRoleForm = document.getElementById('editRoleForm');
    const deleteRoleForm = document.getElementById('deleteRoleForm');

    const addRoleModalEl = document.getElementById('addRoleModal');
    const editRoleModalEl = document.getElementById('editRoleModal');
    const deleteRoleModalEl = document.getElementById('deleteRoleModal');

    const addRoleModal = addRoleModalEl ? bootstrap.Modal.getOrCreateInstance(addRoleModalEl) : null;
    const editRoleModal = editRoleModalEl ? bootstrap.Modal.getOrCreateInstance(editRoleModalEl) : null;
    const deleteRoleModal = deleteRoleModalEl ? bootstrap.Modal.getOrCreateInstance(deleteRoleModalEl) : null;

    function escapeHtml(value) {
        return String(value ?? '')
            .replaceAll('&', '&amp;')
            .replaceAll('<', '&lt;')
            .replaceAll('>', '&gt;')
            .replaceAll('"', '&quot;')
            .replaceAll("'", '&#39;');
    }

    function showError(elementId, message) {
        const box = document.getElementById(elementId);
        if (!box) return;
        box.textContent = message || 'Váratlan hiba történt.';
        box.classList.remove('d-none');
    }

    function hideError(elementId) {
        const box = document.getElementById(elementId);
        if (!box) return;
        box.textContent = '';
        box.classList.add('d-none');
    }

    function removeEmptyRolesRow() {
        const emptyRow = document.getElementById('emptyRolesRow');
        if (emptyRow) {
            emptyRow.remove();
        }
    }

    function ensureEmptyRolesRow() {
        if (!rolesTableBody) return;

        const rows = rolesTableBody.querySelectorAll('tr[data-role-id]');
        let emptyRow = document.getElementById('emptyRolesRow');

        if (rows.length === 0) {
            if (!emptyRow) {
                emptyRow = document.createElement('tr');
                emptyRow.id = 'emptyRolesRow';
                emptyRow.innerHTML = `
                    <td colspan="4" class="text-center text-muted py-4">Nincs szerepkör.</td>
                `;
                rolesTableBody.appendChild(emptyRow);
            }
        } else if (emptyRow) {
            emptyRow.remove();
        }
    }

    function buildRoleRow(role) {
        const tr = document.createElement('tr');
        tr.setAttribute('data-role-id', role.id);
        tr.innerHTML = `
            <td class="ps-4 text-truncate" style="max-width: 220px;">${escapeHtml(role.id)}</td>
            <td class="role-name">${escapeHtml(role.name)}</td>
            <td class="role-normalized-name">${escapeHtml(role.normalizedName || '')}</td>
            <td class="pe-4 text-end">
                <div class="btn-group btn-group-sm" role="group" aria-label="Role actions">
                    <button type="button"
                            class="btn btn-outline-primary edit-role"
                            data-bs-toggle="modal"
                            data-bs-target="#editRoleModal"
                            data-role-id="${escapeHtml(role.id)}"
                            data-role-name="${escapeHtml(role.name)}"
                            title="Edit role">
                        <i class="bi bi-pencil-square"></i>
                    </button>

                    <button type="button"
                            class="btn btn-outline-danger delete-role"
                            data-bs-toggle="modal"
                            data-bs-target="#deleteRoleModal"
                            data-role-id="${escapeHtml(role.id)}"
                            data-role-name="${escapeHtml(role.name)}"
                            title="Delete role">
                        <i class="bi bi-trash"></i>
                    </button>
                </div>
            </td>
        `;
        return tr;
    }

    function updateRoleRow(role) {
        if (!rolesTableBody) return;

        const row = rolesTableBody.querySelector(`tr[data-role-id="${CSS.escape(role.id)}"]`);
        if (!row) return;

        const nameCell = row.querySelector('.role-name');
        const normalizedCell = row.querySelector('.role-normalized-name');
        const editBtn = row.querySelector('.edit-role');
        const deleteBtn = row.querySelector('.delete-role');

        if (nameCell) {
            nameCell.textContent = role.name;
        }

        if (normalizedCell) {
            normalizedCell.textContent = role.normalizedName || '';
        }

        if (editBtn) {
            editBtn.setAttribute('data-role-id', role.id);
            editBtn.setAttribute('data-role-name', role.name);
        }

        if (deleteBtn) {
            deleteBtn.setAttribute('data-role-id', role.id);
            deleteBtn.setAttribute('data-role-name', role.name);
        }
    }

    async function postForm(url, form) {
        const formData = new URLSearchParams(new FormData(form));

        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8'
            },
            body: formData.toString()
        });

        let payload = null;
        try {
            payload = await response.json();
        } catch {
            payload = null;
        }

        return { response, payload };
    }

    if (addRoleForm) {
        addRoleForm.addEventListener('submit', async function (e) {
            e.preventDefault();
            hideError('addRoleError');

            const roleNameInput = document.getElementById('roleName');
            const roleName = roleNameInput?.value?.trim() || '';

            if (!roleName) {
                roleNameInput?.classList.add('is-invalid');
                showError('addRoleError', 'A role neve kötelező.');
                return;
            }

            roleNameInput?.classList.remove('is-invalid');

            try {
                const { response, payload } = await postForm('?handler=AddRole', addRoleForm);

                if (!response.ok || !payload?.success || !payload?.role) {
                    showError('addRoleError', payload?.message || 'A szerepkör létrehozása sikertelen.');
                    return;
                }

                removeEmptyRolesRow();
                const row = buildRoleRow(payload.role);
                rolesTableBody.appendChild(row);

                addRoleForm.reset();
                hideError('addRoleError');
                addRoleModal?.hide();
                ensureEmptyRolesRow();
            } catch (error) {
                console.error('Add role failed:', error);
                showError('addRoleError', 'Váratlan hiba történt a szerepkör létrehozása közben.');
            }
        });
    }

    if (editRoleForm) {
        editRoleForm.addEventListener('submit', async function (e) {
            e.preventDefault();
            hideError('editRoleError');

            const roleNameInput = document.getElementById('editRoleName');
            const roleName = roleNameInput?.value?.trim() || '';

            if (!roleName) {
                roleNameInput?.classList.add('is-invalid');
                showError('editRoleError', 'A role neve kötelező.');
                return;
            }

            roleNameInput?.classList.remove('is-invalid');

            try {
                const { response, payload } = await postForm('?handler=UpdateRole', editRoleForm);

                if (!response.ok || !payload?.success || !payload?.role) {
                    showError('editRoleError', payload?.message || 'A szerepkör módosítása sikertelen.');
                    return;
                }

                updateRoleRow(payload.role);
                hideError('editRoleError');
                editRoleModal?.hide();
            } catch (error) {
                console.error('Update role failed:', error);
                showError('editRoleError', 'Váratlan hiba történt a szerepkör módosítása közben.');
            }
        });
    }

    if (deleteRoleForm) {
        deleteRoleForm.addEventListener('submit', async function (e) {
            e.preventDefault();
            hideError('deleteRoleError');

            try {
                const { response, payload } = await postForm('?handler=DeleteRole', deleteRoleForm);

                if (!response.ok || !payload?.success) {
                    showError('deleteRoleError', payload?.message || 'A szerepkör törlése sikertelen.');
                    return;
                }

                const row = rolesTableBody.querySelector(`tr[data-role-id="${CSS.escape(payload.deletedId)}"]`);
                if (row) {
                    row.remove();
                }

                deleteRoleForm.reset();
                const deleteRoleName = document.getElementById('deleteRoleName');
                if (deleteRoleName) {
                    deleteRoleName.textContent = '';
                }

                hideError('deleteRoleError');
                deleteRoleModal?.hide();
                ensureEmptyRolesRow();
            } catch (error) {
                console.error('Delete role failed:', error);
                showError('deleteRoleError', 'Váratlan hiba történt a szerepkör törlése közben.');
            }
        });
    }

    if (rolesTableBody) {
        rolesTableBody.addEventListener('click', function (e) {
            const editBtn = e.target.closest('.edit-role');
            if (editBtn) {
                hideError('editRoleError');

                const roleId = editBtn.getAttribute('data-role-id') || '';
                const roleName = editBtn.getAttribute('data-role-name') || '';

                const editRoleId = document.getElementById('editRoleId');
                const editRoleName = document.getElementById('editRoleName');

                if (editRoleId) {
                    editRoleId.value = roleId;
                }

                if (editRoleName) {
                    editRoleName.value = roleName;
                    editRoleName.classList.remove('is-invalid');
                }

                return;
            }

            const deleteBtn = e.target.closest('.delete-role');
            if (deleteBtn) {
                hideError('deleteRoleError');

                const roleId = deleteBtn.getAttribute('data-role-id') || '';
                const roleName = deleteBtn.getAttribute('data-role-name') || '';

                const deleteRoleId = document.getElementById('deleteRoleId');
                const deleteRoleName = document.getElementById('deleteRoleName');

                if (deleteRoleId) {
                    deleteRoleId.value = roleId;
                }

                if (deleteRoleName) {
                    deleteRoleName.textContent = roleName;
                }
            }
        });
    }
});