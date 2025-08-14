$(function () {
    var ngayBatDau = $('#myDate').data('ngay'); // Lấy từ data-ngay
    $("#myDate").datepicker({
        format: 'dd-mm-yyyy',         // Định dạng cố định
        autoclose: true,
        todayHighlight: true,
        language: 'vi' 
    }).datepicker('update', ngayBatDau);
    console.log("Giá trị ViewBag.NgayBatDau:", ngayBatDau);
});

document.getElementById('myDate').addEventListener('input', function (e) {
    const input = e.target;
    const rawValue = input.value;
    const cursorPos = input.selectionStart;

    // Xóa ký tự không phải số
    let digits = rawValue.replace(/\D/g, '').slice(0, 8);

    // Định dạng dd-mm-yyyy
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

    // Gán lại giá trị
    input.value = formatted;

    // Tính lại vị trí con trỏ
    let newCursorPos = cursorPos;

    // Nếu người dùng vừa gõ vào vị trí chèn dấu, dịch con trỏ thêm 1
    if (rawValue.length < formatted.length && formatted[cursorPos] === '-') {
        newCursorPos++;
    }

    // Khôi phục vị trí con trỏ
    input.setSelectionRange(newCursorPos, newCursorPos);
});

$('#datepicker-icon').on('click', function () {
    $('#myDate').datepicker('show');
});

// input đến ngày
$(function () {
    var ngayKetThuc = $('#myDate2').data('ngay2'); // Lấy từ data-ngay
    $("#myDate2").datepicker({
        format: 'dd-mm-yyyy',         // Định dạng cố định
        autoclose: true,
        todayHighlight: true,
        language: 'vi' 
    }).datepicker('update', ngayKetThuc);
});

document.getElementById('myDate2').addEventListener('input', function (e) {
    const input = e.target;
    const rawValue = input.value;
    const cursorPos = input.selectionStart;

    // Xóa ký tự không phải số
    let digits = rawValue.replace(/\D/g, '').slice(0, 8);

    // Định dạng dd-mm-yyyy
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

    // Gán lại giá trị
    input.value = formatted;

    // Tính lại vị trí con trỏ
    let newCursorPos = cursorPos;

    // Nếu người dùng vừa gõ vào vị trí chèn dấu, dịch con trỏ thêm 1
    if (rawValue.length < formatted.length && formatted[cursorPos] === '-') {
        newCursorPos++;
    }

    // Khôi phục vị trí con trỏ
    input.setSelectionRange(newCursorPos, newCursorPos);
});

$('#datepicker-icon2').on('click', function () {
    $('#myDate2').datepicker('show');
});

