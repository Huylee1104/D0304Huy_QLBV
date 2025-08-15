document.addEventListener("DOMContentLoaded", () => {
    const input = document.getElementById('comboBox');
    const dropdown = document.getElementById('dropdownList');
    const hiddenId = document.getElementById('IDHTTT');

    // set mặt định
    hiddenId.value = 0;
    // Nếu người dùng đã chọn mà không chọn thì set như mặc định
    input.addEventListener('input', () => {
        if (input.value.trim() === "") {
            hiddenId.value = 0;
            dropdown.style.display = "none";
        } else {
            hiddenId.value = "";
            renderOptions(input.value);
        }
    });

    let highlightedIndex = -1;
    let currentOptions = [];

    // Hàm bỏ dấu tiếng Việt
    function removeAccents(str) {
        return str.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
    }

    // Hàm highlight chữ trùng
    function highlightMatch(text, keyword) {
        if (!keyword) return text;

        const normalizedText = removeAccents(text).toLowerCase();
        const normalizedKeyword = removeAccents(keyword).toLowerCase();

        const startIndexNormalized = normalizedText.indexOf(normalizedKeyword);
        if (startIndexNormalized === -1) return text;

        // Map index từ chuỗi bỏ dấu sang chuỗi gốc
        let startIndexOriginal = 0;
        let count = 0;
        for (let i = 0; i < text.length; i++) {
            if (removeAccents(text[i]).toLowerCase() !== '') {
                if (count === startIndexNormalized) {
                    startIndexOriginal = i;
                    break;
                }
                count++;
            }
        }
        // Tìm endIndex dựa vào độ dài keyword
        let endIndexOriginal = startIndexOriginal;
        let count2 = 0;
        for (let i = startIndexOriginal; i < text.length; i++) {
            if (removeAccents(text[i]).toLowerCase() !== '') {
                count2++;
            }
            if (count2 === normalizedKeyword.length) {
                endIndexOriginal = i + 1;
                break;
            }
        }

        return (
            text.substring(0, startIndexOriginal) +
            '<span class="highlight-text">' +
            text.substring(startIndexOriginal, endIndexOriginal) +
            '</span>' +
            text.substring(endIndexOriginal)
        );
    }

    // Render danh sách gợi ý
    function renderOptions(filter = "") {
        dropdown.innerHTML = "";
        highlightedIndex = 0;

        const normalizedFilter = removeAccents(filter.toLowerCase());

        currentOptions = provincesDataHTTT.filter(item =>
            removeAccents((item.ten || '').toLowerCase()).includes(normalizedFilter)
        );

        currentOptions.forEach((item, index) => {
            const option = document.createElement('div');
            option.classList.add('option-item');

            const nameSpan = document.createElement('span');
            nameSpan.innerHTML = highlightMatch(item.ten, filter);
            nameSpan.style.flex = "1";

            option.appendChild(nameSpan);

            if (index === highlightedIndex) option.classList.add('highlight');

            option.addEventListener('click', () => selectOption(index));

            dropdown.appendChild(option);
        });

        dropdown.style.display = currentOptions.length ? "block" : "none";
    }

    // Cập nhật highlight khi dùng phím
    function updateHighlight() {
        const options = dropdown.querySelectorAll('.option-item');
        options.forEach((opt, idx) => {
            opt.classList.toggle('highlight', idx === highlightedIndex);
        });
    }

    // Chọn option
    function selectOption(index) {
        if (index >= 0 && index < currentOptions.length) {
            input.value = currentOptions[index].ten;
            hiddenId.value = currentOptions[index].id;
            dropdown.style.display = "none";
        }
    }

    // Sự kiện focus + input
    input.addEventListener('focus', () => renderOptions());
    input.addEventListener('input', () => {
        renderOptions(input.value);
    });

    window.addEventListener('load', () => {
        if (hiddenId.value && !input.value) {
            const selected = provincesDataHTTT.find(x => x.id == hiddenId.value);
            if (selected) {
                input.value = selected.ten;
            }
        }
    });

    // Điều hướng bằng bàn phím
    input.addEventListener('keydown', (e) => {
        if (dropdown.style.display === "block") {
            if (e.key === "ArrowDown") {
                e.preventDefault();
                highlightedIndex = (highlightedIndex + 1) % currentOptions.length;
                updateHighlight();
            } else if (e.key === "ArrowUp") {
                e.preventDefault();
                highlightedIndex = (highlightedIndex - 1 + currentOptions.length) % currentOptions.length;
                updateHighlight();
            } else if (e.key === "Enter") {
                e.preventDefault();
                selectOption(highlightedIndex);
            }
        }
    });

    // Click ngoài dropdown để đóng
    document.addEventListener('click', (e) => {
        if (!e.target.closest('#comboBox') && !e.target.closest('#dropdownList')) {
            dropdown.style.display = "none";
        }
    });

    console.log("combobox.js loaded");
    console.log("provincesDataHTTT:", provincesDataHTTT);
})

document.addEventListener("DOMContentLoaded", () => {
    const input = document.getElementById('comboBox2');
    const dropdown = document.getElementById('dropdownList2');
    const hiddenId = document.getElementById('IDNhanVien'); // IDNhanVien từ form
    // set mặt định
    hiddenId.value = 0;
    // Nếu người dùng đã chọn mà không chọn thì set như mặc định
    input.addEventListener('input', () => {
        if (input.value.trim() === "") {
            hiddenId.value = 0;
            dropdown.style.display = "none";
        } else {
            hiddenId.value = "";
            renderOptions(input.value);
        }
    });

    let highlightedIndex = -1; // Chỉ số đang được highlight
    let currentOptions = []; // Lưu các options hiện tại

    // Hàm loại bỏ dấu tiếng Việt
    function removeAccents(str) {
        return str.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
    }
    function highlightMatch(text, keyword) {
        if (!keyword) return text;

        const normalizedText = removeAccents(text).toLowerCase();
        const normalizedKeyword = removeAccents(keyword).toLowerCase();

        const startIndexNormalized = normalizedText.indexOf(normalizedKeyword);
        if (startIndexNormalized === -1) return text;

        // Map index từ chuỗi bỏ dấu sang chuỗi gốc
        let startIndexOriginal = 0;
        let count = 0;
        for (let i = 0; i < text.length; i++) {
            if (removeAccents(text[i]).toLowerCase() !== '') {
                if (count === startIndexNormalized) {
                    startIndexOriginal = i;
                    break;
                }
                count++;
            }
        }
        // lâm đồng
        // Tìm endIndex dựa vào độ dài keyword (không dấu) trong chuỗi gốc
        let endIndexOriginal = startIndexOriginal;
        let count2 = 0;
        for (let i = startIndexOriginal; i < text.length; i++) {
            if (removeAccents(text[i]).toLowerCase() !== '') {
                count2++;
            }
            if (count2 === normalizedKeyword.length) {
                endIndexOriginal = i + 1;
                break;
            }
        }

        return (
            text.substring(0, startIndexOriginal) +
            '<span class="highlight-text">' +
            text.substring(startIndexOriginal, endIndexOriginal) +
            '</span>' +
            text.substring(endIndexOriginal)
        );
    }
    // Hàm render danh sách gợi ý
    function renderOptions(filter = "") {
        dropdown.innerHTML = "";
        highlightedIndex = 0; // Reset highlight về dòng đầu tiên
        const normalizedFilter = removeAccents(filter.toLowerCase());

        currentOptions = provincesDataNhanVien.filter(item =>
            removeAccents((item.Ten || '').toLowerCase()).includes(normalizedFilter) ||
            removeAccents((item.Viettat || "").toLowerCase()).startsWith(normalizedFilter)
        );
        currentOptions.forEach((item, index) => {
            const option = document.createElement('div');
            option.classList.add('option-item');
            // Tạo HTML highlight cho tên 
            const highlightedTenNhanVien = highlightMatch(item.Ten, filter);
            const highlightedVietTat = highlightMatch(item.Viettat || "", filter);
            // Container bên trái: tên 
            const nameSpan = document.createElement('span');
            nameSpan.innerHTML = highlightedTenNhanVien;
            nameSpan.style.flex = "1"; // Đẩy sang trái

            // Container bên phải: viết tắt
            const abbrSpan = document.createElement('span');
            abbrSpan.innerHTML = highlightedVietTat;
            abbrSpan.style.marginLeft = "10px";
            abbrSpan.style.color = "#888"; // Màu xám nhạt cho VietTat
            abbrSpan.style.fontSize = "12px";

            // Thêm các phần tử vào option
            option.appendChild(nameSpan);
            option.appendChild(abbrSpan);
            // Highlight nếu là item đang được chọn
            if (index === highlightedIndex) option.classList.add('highlight');
            option.addEventListener('click', () => selectOption(index));
            dropdown.appendChild(option);
        });

        dropdown.style.display = currentOptions.length ? "block" : "none";
    }
    // Cập nhật highlight khi nhấn phím
    function updateHighlight() {
        const options = dropdown.querySelectorAll('.option-item');
        options.forEach((opt, idx) => {
            opt.classList.toggle('highlight', idx === highlightedIndex);
        });
    }
    // Chọn option và ẩn các option khác
    function selectOption(index) {
        if (index >= 0 && index < currentOptions.length) {
            input.value = currentOptions[index].Ten;
            hiddenId.value = currentOptions[index].Id;
            dropdown.style.display = "none";
        }
    }
    // Sự kiện focus và lọc danh sách gợi ý theo nội dung nhập
    input.addEventListener('focus', () => renderOptions());
    input.addEventListener('input', () => {
        renderOptions(input.value);
    });

    window.addEventListener('load', () => {
        if (hiddenId.value && !input.value) {
            const selected = provincesDataHTTT.find(x => x.id == hiddenId.value);
            if (selected) {
                input.value = selected.ten;
            }
        }
    });
    // Điều hướng bằng bàn phím
    input.addEventListener('keydown', (e) => {
        if (dropdown.style.display === "block") {
            if (e.key === "ArrowDown") {
                e.preventDefault();
                highlightedIndex = (highlightedIndex + 1) % currentOptions.length;
                updateHighlight();
            } else if (e.key === "ArrowUp") {
                e.preventDefault();
                highlightedIndex = (highlightedIndex - 1 + currentOptions.length) % currentOptions.length;
                updateHighlight();
            } else if (e.key === "Enter") {
                e.preventDefault();
                selectOption(highlightedIndex);
            }
        }
    });
    // Click ngoài dropdown
    document.addEventListener('click', (e) => {
        if (!e.target.closest('#comboBox2') && !e.target.closest('#dropdownList2')) {
            dropdown.style.display = "none";
        }
    });

    console.log("combobox.js loaded");
    console.log("provincesDataNhanVien:", provincesDataNhanVien); // Sử dụng biến từ View
});

