document.addEventListener("DOMContentLoaded", function () {
    ["comboBox2", "IDNhanVien", "comboBox", "IDHTTT"].forEach(id => {
        const el = document.getElementById(id);
        if (el && el.dataset.value) {
            el.value = el.dataset.value;
        }
    });

    if (typeof _idcn !== "undefined") {
        const chiNhanhEl = document.getElementById("IDChiNhanh");
        if (chiNhanhEl) {
            chiNhanhEl.value = _idcn;
        }
    } else {
        console.warn("Biến _idcn chưa được định nghĩa (kiểm tra BienChung.js)");
    }
});

function changePage(newPage) {
    document.getElementById("page").value = newPage;
    document.getElementById("paginationForm").submit();
}

function changePageSize(newSize) {
    document.getElementById("pageSize").value = newSize;
    document.getElementById("page").value = 1; 
    document.getElementById("paginationForm").submit();
}

$(function () {
    var ngayBatDau = $('#myDate').data('ngay'); 
    $("#myDate").datepicker({
        format: 'dd-mm-yyyy',
        autoclose: true,
        todayHighlight: true,
        language: 'vi'
    }).datepicker('update', ngayBatDau);
    console.log("Giá trị ViewBag.NgayBatDau:", ngayBatDau);

    document.getElementById('myDate').addEventListener('input', function (e) {
        const input = e.target;
        const rawValue = input.value;
        const cursorPos = input.selectionStart;

        let digits = rawValue.replace(/\D/g, '').slice(0, 8);

        let formatted = '';
        if (digits.length >= 2) {
            formatted += digits.slice(0, 2) + '-';
        } else {
            formatted += digits;
        }
        if (digits.length >= 4) {
            formatted += digits.slice(2, 4) + '-';
        } else if (digits.length > 2) {
            formatted += digits.slice(2);
        }
        if (digits.length > 4) {
            formatted += digits.slice(4);
        }

        input.value = formatted;

        let newCursorPos = cursorPos;
        if (rawValue.length < formatted.length && formatted[cursorPos] === '-') {
            newCursorPos++;
        }
        input.setSelectionRange(newCursorPos, newCursorPos);
    });

    $('#datepicker-icon').on('click', function () {
        $('#myDate').datepicker('show');
    });

    var ngayKetThuc = $('#myDate2').data('ngay2'); 
    $("#myDate2").datepicker({
        format: 'dd-mm-yyyy',
        autoclose: true,
        todayHighlight: true,
        language: 'vi'
    }).datepicker('update', ngayKetThuc);

    document.getElementById('myDate2').addEventListener('input', function (e) {
        const input = e.target;
        const rawValue = input.value;
        const cursorPos = input.selectionStart;

        let digits = rawValue.replace(/\D/g, '').slice(0, 8);

        let formatted = '';
        if (digits.length >= 2) {
            formatted += digits.slice(0, 2) + '-';
        } else {
            formatted += digits;
        }
        if (digits.length >= 4) {
            formatted += digits.slice(2, 4) + '-';
        } else if (digits.length > 2) {
            formatted += digits.slice(2);
        }
        if (digits.length > 4) {
            formatted += digits.slice(4);
        }

        input.value = formatted;

        let newCursorPos = cursorPos;
        if (rawValue.length < formatted.length && formatted[cursorPos] === '-') {
            newCursorPos++;
        }
        input.setSelectionRange(newCursorPos, newCursorPos);
    });

    $('#datepicker-icon2').on('click', function () {
        $('#myDate2').datepicker('show');
    });
});

$('#selectGiaiDoan').change(function () {
    const selectedValue = $(this).val();
    const container = $('#selectContainer');
    container.empty(); 

    if (selectedValue === 'Nam' || selectedValue === 'Ngay') {
        container.css('justify-content', 'flex-start');
    }

    else if (selectedValue === 'Quy' || selectedValue === 'Thang') {
        container.css('justify-content', 'space-around');
    }

    const selectNam = `
			<div style="width: 48%; min-width: 80px;">
				<label class="form-label" style="display: block; margin-bottom: 5px; white-space: nowrap;">Năm</label>
				<select id="selectNam" class="form-select" style="width: 100%;">
					<option value="2023">2023</option>
					<option value="2024">2024</option>
				</select>
			</div>
		`;
    container.append(selectNam);

    if (selectedValue === 'Quy') {
        const selectQuy = `
				<div style="width: 50%; ">
					<label class="form-label" style="display: block; margin-bottom: 5px; white-space: nowrap;">Quý</label>
					<select id="selectQuy" class="form-select" style="width: 100%;">
						<option value="1"> 1</option>
						<option value="2"> 2</option>
						<option value="3"> 3</option>
						<option value="4"> 4</option>
					</select>
				</div>
			`;
        container.append(selectQuy);
    }
    else if (selectedValue === 'Thang') {
        const selectThang = `
				<div style="width: 50%;">
					<label class="form-label" style="display: block; margin-bottom: 5px; white-space: nowrap;">Tháng</label>
					<select id="selectThang" class="form-select" style="width: 100%;">
						${Array.from({ length: 12 }, (_, i) =>
            `<option value="${i + 1}"> ${i + 1}</option>`
        ).join('')}
					</select>
				</div>
			`;
        container.append(selectThang);
    }
    else if (selectedValue === "Ngay") {
        container.empty(); 
    }
});

document.addEventListener('DOMContentLoaded', () => {
    const select = document.getElementById('selectGiaiDoan');
    const selectContainer = document.getElementById('selectContainer');
    const fromInput = document.getElementById('myDate');
    const toInput = document.getElementById('myDate2');
    const fromItem = fromInput?.closest('.item');
    const toItem = toInput?.closest('.item');

    const ALLOWED = new Set(['Nam', 'Quy', 'Thang', 'Ngay']);
    const DEFAULT = 'Nam';

    const style = document.createElement('style');
    style.textContent = `
      #selectContainer {
        display: flex;
        flex-wrap: nowrap;
        align-items: flex-start;
        gap: 10px;
      }
      #selectContainer > div { flex: 1; min-width: 80px; }
      .date-locked {
        background: #f5f5f5;
        cursor: not-allowed;
      }
      .date-locked::-webkit-calendar-picker-indicator { opacity: 0.35; }
    `;
    document.head.appendChild(style);

    const blockEvt = (e) => { e.preventDefault(); e.stopImmediatePropagation(); };
    function setInteractable(input, enable) {
        if (!input) return;
        input.readOnly = !enable;
        input.classList.toggle('date-locked', !enable);
        if (!enable) {
            input.addEventListener('mousedown', blockEvt, true);
            input.addEventListener('touchstart', blockEvt, true);
            input.addEventListener('keydown', blockEvt, true);
            input.tabIndex = -1;
        } else {
            input.removeEventListener('mousedown', blockEvt, true);
            input.removeEventListener('touchstart', blockEvt, true);
            input.removeEventListener('keydown', blockEvt, true);
            input.tabIndex = 0;
        }
    }

    function updateUI() {
        const val = select.value;
        const isNgay = val === 'Ngay';

        if (fromItem) fromItem.style.display = '';
        if (toItem) toItem.style.display = '';

        if (selectContainer) {
            if (val === 'Nam' || val === 'Ngay') {
                selectContainer.style.justifyContent = 'flex-start';
            } else if (val === 'Quy' || val === 'Thang') {
                selectContainer.style.justifyContent = 'space-around';
            }
        }

        setInteractable(fromInput, isNgay);
        setInteractable(toInput, isNgay);
    }

    if (!sessionStorage.getItem('firstLoadDone')) {
        select.value = DEFAULT;
        localStorage.setItem('giaiDoan', DEFAULT);
        sessionStorage.setItem('firstLoadDone', 'true');
    } else {
        const saved = localStorage.getItem('giaiDoan');
        if (ALLOWED.has(saved)) {
            select.value = saved;
        } else {
            select.value = DEFAULT;
        }
    }

    updateUI();

    select.addEventListener('change', () => {
        updateUI();
        localStorage.setItem('giaiDoan', select.value);
    });
});

$(document).ready(function () {
    const container = $("#selectContainer");
    const $giaiDoan = $("#selectGiaiDoan");
    const currentYear = new Date().getFullYear();
    const currentMonth = new Date().getMonth() + 1;
    const currentQuarter = Math.floor((new Date().getMonth()) / 3) + 1;

    if (isFirstLoadFromServer) {
        localStorage.removeItem("filterState");
    }
    let saved = JSON.parse(localStorage.getItem("filterState") || "{}");

    let state = {
        giaiDoan: saved.giaiDoan || $giaiDoan.val() || "Nam",
        nam: saved.nam || currentYear,
        quy: saved.quy || currentQuarter,
        thang: saved.thang || currentMonth
    };

    const startDateVal = $('#myDate').datepicker('getDate');
    const endDateVal = $('#myDate2').datepicker('getDate');
    const hasDates = !!(startDateVal && endDateVal);
    const firstLoad = !saved.giaiDoan && !hasDates;

    if (firstLoad) {
        state.giaiDoan = "Nam";
        state.nam = currentYear;
        setDateRangeYear(state.nam);
    }

    $giaiDoan.val(state.giaiDoan);
    renderControls(state);

    if (!hasDates) {
        applyDateRangeFromState(state);
    }

    $giaiDoan.on("change", function () {
        state.giaiDoan = $(this).val();
        renderControls(state);
        applyDateRangeFromState(state);
        saveState();
    });

    function renderControls(state) {
        container.empty();

        if (state.giaiDoan === "Nam") {
            container.css('justify-content', 'flex-start');
            container.append(yearSelectHTML(state.nam));
            $("#selectNam").on("change", function () {
                state.nam = parseInt(this.value, 10);
                setDateRangeYear(state.nam);
                saveState();
            });
        }
        else if (state.giaiDoan === "Quy") {
            container.css('justify-content', 'space-around');
            container.append(yearSelectHTML(state.nam));
            container.append(quySelectHTML(state.quy));
            $("#selectNam").on("change", function () {
                state.nam = parseInt(this.value, 10);
                setDateRangeQuarter(state.nam, state.quy);
                saveState();
            });
            $("#selectQuy").on("change", function () {
                state.quy = parseInt(this.value, 10);
                setDateRangeQuarter(state.nam, state.quy);
                saveState();
            });
        }
        else if (state.giaiDoan === "Thang") {
            container.css('justify-content', 'space-around');
            container.append(yearSelectHTML(state.nam));
            container.append(thangSelectHTML(state.thang));
            $("#selectNam").on("change", function () {
                state.nam = parseInt(this.value, 10);
                setDateRangeMonth(state.nam, state.thang);
                saveState();
            });
            $("#selectThang").on("change", function () {
                state.thang = parseInt(this.value, 10);
                setDateRangeMonth(state.nam, state.thang);
                saveState();
            });
        }
        else if (state.giaiDoan === "Ngay") {
            container.css('justify-content', 'space-around');
        }
    }

    function yearSelectHTML(selectedYear) {
        let html = `<div style="width: 48%;">
            <label class="form-label" style="display: block; margin-bottom: 5px; white-space: nowrap;">Năm</label>
            <select id="selectNam" class="form-select" style="width: 100%;">`;
        for (let y = currentYear; y >=2000; y--) {
            html += `<option value="${y}" ${y === Number(selectedYear) ? 'selected' : ''}>${y}</option>`;
        }
        html += `</select></div>`;
        return html;
    }

    function quySelectHTML(selectedQuy) {
        const html = `
        <div style="width: 50%;">
            <label class="form-label" style="display: block; margin-bottom: 5px; white-space: nowrap;">Quý</label>
            <select id="selectQuy" class="form-select" style="width: 100%;">
                ${Array.from({ length: 4 }, (_, i) =>
                `<option value="${i + 1}" ${i + 1 === Number(selectedQuy) ? 'selected' : ''}>${i + 1}</option>`
            ).join('')}
            </select>
        </div>
        `;
        return html;
    }

    function thangSelectHTML(selectedThang) {
        const html = `
    <div style="width: 50%;">
        <label class="form-label" style="display:block; margin-bottom:5px; white-space:nowrap;">Tháng</label>
        <select id="selectThang" class="form-select" style="width:100%;">
            ${Array.from({ length: 12 }, (_, i) =>
                `<option value="${i + 1}" ${i + 1 === new Date().getMonth() + 1 ? 'selected' : ''}>${i + 1}</option>`
            ).join('')}
        </select>
    </div>
    `;
        return html;
    }

    function setDateRangeYear(year) {
        $('#myDate').datepicker('setDate', new Date(year, 0, 1));
        $('#myDate2').datepicker('setDate', new Date(year, 11, 31));
    }

    function setDateRangeQuarter(year, quarter) {
        const startMonth = (quarter - 1) * 3;
        $('#myDate').datepicker('setDate', new Date(year, startMonth, 1));
        $('#myDate2').datepicker('setDate', new Date(year, startMonth + 3, 0));
    }

    function setDateRangeMonth(year, month) {
        $('#myDate').datepicker('setDate', new Date(year, month - 1, 1));
        $('#myDate2').datepicker('setDate', new Date(year, month, 0));
    }
    function setDateRangeDay() {
        const currentDate = new Date();
        $('#myDate').datepicker('setDate', currentDate);
        $('#myDate2').datepicker('setDate', currentDate);
    }

    function applyDateRangeFromState(state) {
        if (state.giaiDoan === "Nam") {
            setDateRangeYear(state.nam);
        } else if (state.giaiDoan === "Quy") {
            setDateRangeQuarter(state.nam, state.quy);
        } else if (state.giaiDoan === "Thang") {
            setDateRangeMonth(state.nam, state.thang);
        } else if (state.giaiDoan === "Ngay") {
            setDateRangeDay();
        }
    }

    function saveState() {
        localStorage.setItem("filterState", JSON.stringify(state));
    }

    $('#datepicker').on('changeDate', function (e) {
        let startDate = $('#myDate').datepicker('getDate');
        let endDate = $('#myDate2').datepicker('getDate');

        if (endDate && startDate > endDate) {
            $('#myDate2').datepicker('setDate', startDate);
        }
    });

    $('#datepicker2').on('changeDate', function (e) {
        let startDate = $('#myDate').datepicker('getDate');
        let endDate = $('#myDate2').datepicker('getDate');

        if (startDate && endDate < startDate) {
            $('#myDate').datepicker('setDate', endDate);
        }
    });
});