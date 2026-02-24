(function () {

document.addEventListener('DOMContentLoaded', function () {

  const form = document.getElementById('createContractForm');
  if (!form) return;

  const itemSelect = document.getElementById('lineItemSelect');
  const qtyInput = document.getElementById('lineQty');
  const unitPriceInput = document.getElementById('lineUnitPrice');
  const vatInput = document.getElementById('lineVatRate');
  const addBtn = document.getElementById('addLineBtn');

  const tbody = document.getElementById('contractLinesTbody');
  const hidden = document.getElementById('contractLinesHidden');

  const netTotal = form.querySelector('[name="NetTotal"]');
  const taxTotal = form.querySelector('[name="TaxTotal"]');
  const grossTotal = form.querySelector('[name="GrossTotal"]');

  const noteModal = new bootstrap.Modal(document.getElementById('lineNoteModal'));
  const noteTextarea = document.getElementById('lineNoteTextarea');
  const openNoteBtn = document.getElementById('openLineNoteBtn');
  const saveNoteBtn = document.getElementById('saveLineNoteBtn');

  let lines = [];
  let currentNote = '';

  openNoteBtn.addEventListener('click', () => {
    noteTextarea.value = currentNote;
    noteModal.show();
  });

  saveNoteBtn.addEventListener('click', () => {
    currentNote = noteTextarea.value.trim();
    noteModal.hide();
  });

  function calculate(line) {
    line.net = line.quantity * line.unitPrice;
    line.tax = line.net * (line.vatRate / 100);
    line.gross = line.net + line.tax;
  }

  function render() {

    tbody.innerHTML = '';

    if (lines.length === 0) {
      tbody.innerHTML = `<tr class="text-muted"><td colspan="9" class="text-center">Nincs tétel.</td></tr>`;
    }

    lines.forEach((l, i) => {

      const desc = l.description
        ? `<div class="small text-muted mt-1">${l.description}</div>`
        : '';

      tbody.innerHTML += `
        <tr>
          <td>${i+1}</td>
          <td>
            <div class="fw-medium">${l.name}</div>
            ${desc}
          </td>
          <td class="text-end">${l.quantity}</td>
          <td class="text-end">${l.unitPrice.toFixed(2)}</td>
          <td class="text-end">${l.vatRate}</td>
          <td class="text-end">${l.net.toFixed(2)}</td>
          <td class="text-end">${l.tax.toFixed(2)}</td>
          <td class="text-end">${l.gross.toFixed(2)}</td>
          <td><button class="btn btn-sm btn-outline-danger" data-i="${i}">X</button></td>
        </tr>
      `;
    });

    hidden.innerHTML = '';

    lines.forEach((l, i) => {
      hidden.innerHTML += `
        <input type="hidden" name="Lines[${i}].ItemId" value="${l.itemId}">
        <input type="hidden" name="Lines[${i}].Quantity" value="${l.quantity}">
        <input type="hidden" name="Lines[${i}].UnitPrice" value="${l.unitPrice}">
        <input type="hidden" name="Lines[${i}].VatRate" value="${l.vatRate}">
        <input type="hidden" name="Lines[${i}].Description" value="${l.description || ''}">
        <input type="hidden" name="Lines[${i}].DiscountAmount" value="0">
      `;
    });

    const net = lines.reduce((s,l)=>s+l.net,0);
    const tax = lines.reduce((s,l)=>s+l.tax,0);
    const gross = lines.reduce((s,l)=>s+l.gross,0);

    netTotal.value = net.toFixed(2);
    taxTotal.value = tax.toFixed(2);
    grossTotal.value = gross.toFixed(2);
  }

  addBtn.addEventListener('click', () => {

    if (!itemSelect.value) {
      alert("Válassz terméket.");
      return;
    }

    const line = {
      itemId: parseInt(itemSelect.value),
      name: itemSelect.options[itemSelect.selectedIndex].text,
      quantity: parseFloat(qtyInput.value || 1),
      unitPrice: parseFloat(unitPriceInput.value || 0),
      vatRate: parseFloat(vatInput.value || 0),
      description: currentNote
    };

    calculate(line);
    lines.push(line);

    currentNote = '';
    noteTextarea.value = '';
    itemSelect.value = '';
    unitPriceInput.value = '';

    render();
  });

  tbody.addEventListener('click', function(e){
    const btn = e.target.closest('[data-i]');
    if (!btn) return;
    lines.splice(btn.dataset.i,1);
    render();
  });

});

})();