// ================================
// Contract CREATE (AJAX)
// Nem tölt újra oldalt
// ================================

(function () {

    document.addEventListener('DOMContentLoaded', function () {

        const form = document.getElementById('createContractForm');
        if (!form) return;

        const modalEl = document.getElementById('createContractModal');
        const modal = bootstrap.Modal.getOrCreateInstance(modalEl);

        form.addEventListener('submit', async function (e) {
            e.preventDefault();

            if (!validateLines()) {
                alert("Adj hozzá legalább egy tételt.");
                return;
            }

            const submitBtn = form.querySelector('button[type="submit"]');
            setLoading(submitBtn, true);

            try {

                const dto = buildCreateDto(form);

                const response = await fetch('/api/contracts/create', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'RequestVerificationToken': getAntiForgeryToken()
                    },
                    body: JSON.stringify(dto)
                });

                if (!response.ok) {
                    const err = await response.text();
                    console.error(err);
                    alert("Hiba történt mentés közben.");
                    setLoading(submitBtn, false);
                    return;
                }

                const result = await response.json();

                // Modal bezárása
                modal.hide();

                // UI reset
                resetCreateForm(form);

                // Ha van lista reload függvényed
                if (typeof reloadContractsList === "function") {
                    reloadContractsList();
                }

                // Siker toast (ha van saját rendszered)
                if (typeof showSuccessToast === "function") {
                    showSuccessToast("Szerződés sikeresen létrehozva.");
                }

            } catch (err) {
                console.error(err);
                alert("Váratlan hiba történt.");
            }

            setLoading(submitBtn, false);
        });

    });

    // ================================
    // DTO összeállítás
    // ================================
    function buildCreateDto(form) {

        const get = name => form.querySelector(`[name="${name}"]`)?.value ?? null;

        return {
            tenantId: toInt(get('TenantId')),
            businessDocumentTypeId: toInt(get('BusinessDocumentTypeId')),
            businessDocumentStatusId: toInt(get('BusinessDocumentStatusId')),
            documentNo: emptyToNull(get('DocumentNo')),
            currencyId: toIntOrNull(get('CurrencyId')),
            issueDate: emptyToNull(get('IssueDate')),
            fulfillmentDate: emptyToNull(get('FulfillmentDate')),
            dueDate: emptyToNull(get('DueDate')),
            subject: emptyToNull(get('Subject')),
            exchangeRate: toFloatOrNull(get('ExchangeRate')),
            notes: emptyToNull(get('Notes')),

            buyerPartnerId: toIntOrNull(get('BuyerPartnerId')),
            sellerPartnerId: toIntOrNull(get('SellerPartnerId')),

            // ÚJ: telephely a partnerhez
            buyerSiteId: toIntOrNull(get('BuyerSiteId')),

            // Totals mezők read-only, de elküldhetjük vagy hagyhatjuk null-on
            // Itt elküldjük, mert már a line JS kitölti:
            netTotal: toFloatOrNull(get('NetTotal')),
            taxTotal: toFloatOrNull(get('TaxTotal')),
            grossTotal: toFloatOrNull(get('GrossTotal')),

            lines: buildLinesArray()
        };
    }

    // ================================
    // Lines JSON tömb
    // ================================
    function buildLinesArray() {

        const inputs = document.querySelectorAll('#contractLinesHidden input');
        const lines = {};

        inputs.forEach(input => {

            const match = input.name.match(/Lines\[(\d+)\]\.(.+)/);
            if (!match) return;

            const index = match[1];
            const prop = match[2];

            if (!lines[index]) lines[index] = {};
            lines[index][prop] = input.value;
        });

        return Object.values(lines).map(l => ({
            tenantId: toInt(l.TenantId),
            businessDocumentId: 0,
            lineNo: toIntOrDefault(l.LineNo, 1),

            itemId: toInt(l.ItemId),
            quantity: toFloatOrDefault(l.Quantity, 1),

            // override ár (ha a line JS kitölti, megy vele)
            unitPrice: toFloatOrNull(l.UnitPrice),

            // marad a DTO-ban, Contractnál most 0 (ha nincs hidden input, is 0 lesz)
            discountAmount: toFloatOrDefault(l.DiscountAmount, 0),

            // tételes áfa (új)
            vatRate: toFloatOrNull(l.VatRate),

            taxCodeId: toIntOrNull(l.TaxCodeId),
            warehouseId: toIntOrNull(l.WarehouseId),
            description: emptyToNull(l.Description)
        }));
    }

    // ================================
    // Validáció
    // ================================
    function validateLines() {
        return document.querySelectorAll('#contractLinesHidden input[name^="Lines["]').length > 0;
    }

    // ================================
    // UI Reset
    // ================================
    function resetCreateForm(form) {

        form.reset();

        // reset lines (ha van line manager globálisan)
        if (typeof window.__contractLinesReset === "function") {
            window.__contractLinesReset();
        }

        const hidden = document.getElementById('contractLinesHidden');
        if (hidden) hidden.innerHTML = '';

        const tbody = document.getElementById('contractLinesTbody');
        if (tbody) {
            tbody.innerHTML = `
                <tr class="text-muted" id="noLinesRow">
                    <td colspan="9" class="py-3 text-center">
                        Nincs tétel. Adj hozzá legalább egy terméket/szolgáltatást.
                    </td>
                </tr>`;
        }
    }

    // ================================
    // Loading állapot
    // ================================
    function setLoading(button, isLoading) {
        if (!button) return;

        if (isLoading) {
            button.dataset.originalText = button.innerHTML;
            button.innerHTML = `<span class="spinner-border spinner-border-sm me-2"></span> Mentés...`;
            button.disabled = true;
        } else {
            button.innerHTML = button.dataset.originalText;
            button.disabled = false;
        }
    }

    // ================================
    // AntiForgery
    // ================================
    function getAntiForgeryToken() {
        return document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    }

    // ================================
    // Helpers
    // ================================
    function emptyToNull(v) {
        if (v == null) return null;
        const s = String(v).trim();
        return s === '' ? null : s;
    }

    function toInt(v) {
        const n = parseInt(String(v), 10);
        return isNaN(n) ? 0 : n;
    }

    function toIntOrNull(v) {
        if (v == null) return null;
        const s = String(v).trim();
        if (s === '') return null;
        const n = parseInt(s, 10);
        return isNaN(n) ? null : n;
    }

    function toIntOrDefault(v, def) {
        const n = parseInt(String(v), 10);
        return isNaN(n) ? def : n;
    }

    function toFloatOrNull(v) {
        if (v == null) return null;
        const s = String(v).trim().replace(',', '.');
        if (s === '') return null;
        const n = parseFloat(s);
        return isNaN(n) ? null : n;
    }

    function toFloatOrDefault(v, def) {
        if (v == null) return def;
        const s = String(v).trim().replace(',', '.');
        if (s === '') return def;
        const n = parseFloat(s);
        return isNaN(n) ? def : n;
    }

})();