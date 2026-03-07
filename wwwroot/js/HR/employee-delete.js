// HR - Employee Delete Logic
// Handles delete modal + API call + index reload

(function () {

    let deleteEmployeeModal = null;

    document.addEventListener("DOMContentLoaded", function () {

        const modalElement = document.getElementById("deleteEmployeeModal");
        if (modalElement) {
            deleteEmployeeModal = new bootstrap.Modal(modalElement);
        }

    });


    // -------------------------------
    // OPEN DELETE MODAL
    // -------------------------------
    document.addEventListener("click", function (e) {

        const btn = e.target.closest(".deleteEmployeeBtn");
        if (!btn) return;

        const employeeId = btn.dataset.employeeId;
        const employeeName = btn.dataset.employeeName;

        const idInput = document.getElementById("deleteEmployeeId");
        const nameLabel = document.getElementById("deleteEmployeeName");

        if (idInput) {
            idInput.value = employeeId;
        }

        if (nameLabel) {
            nameLabel.textContent = employeeName || `#${employeeId}`;
        }

        if (deleteEmployeeModal) {
            deleteEmployeeModal.show();
        }

    });


    // -------------------------------
    // CONFIRM DELETE
    // -------------------------------
    document.addEventListener("click", async function (e) {

        if (!e.target.closest("#confirmDeleteEmployeeBtn")) return;

        const btn = document.getElementById("confirmDeleteEmployeeBtn");
        const id = document.getElementById("deleteEmployeeId").value;

        if (!id) return;

        btn.disabled = true;

        try {

            const response = await fetch(`/api/employee/${id}`, {
                method: "DELETE"
            });

            if (!response.ok) {
                throw new Error("Delete failed");
            }

            if (deleteEmployeeModal) {
                deleteEmployeeModal.hide();
            }

            if (window.reloadEmployeesIndex) {
                window.reloadEmployeesIndex();
            }

        }
        catch (err) {

            console.error("Employee delete error:", err);
            alert("A törlés nem sikerült.");

        }
        finally {

            btn.disabled = false;

        }

    });

})();