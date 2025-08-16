document.addEventListener("DOMContentLoaded", () => {
    const input = document.getElementById('comboBox');
    const dropdown = document.getElementById('dropdownList');
    const hiddenId = document.getElementById('IDHTTT');
    let isMouseDownOnDropdown = false; 

    hiddenId.value = 0;
    input.addEventListener('input', () => {
        if (input.value.trim() === "") {
            hiddenId.value = 0;
            dropdown.style.display = "none";
        } else {
            hiddenId.value = "";
            renderOptions(input.value);
        }
    });

    dropdown.addEventListener('mousedown', () => {
        isMouseDownOnDropdown = true;
    });

    input.addEventListener('blur', () => {
        setTimeout(() => {
            if (!isMouseDownOnDropdown) {
                if (hiddenId.value === "" && input.value.trim() !== "") {
                    input.value = "";
                    hiddenId.value = 0;
                }
            }
            isMouseDownOnDropdown = false; 
            dropdown.style.display = "none";
        }, 100);
    });

    function removeAccents(str) {
        return str.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
    }

    function highlightMatch(text, keyword) {
        if (!keyword) return text;

        const normalizedText = removeAccents(text).toLowerCase();
        const normalizedKeyword = removeAccents(keyword).toLowerCase();

        const startIndexNormalized = normalizedText.indexOf(normalizedKeyword);
        if (startIndexNormalized === -1) return text;

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

            option.addEventListener('mousedown', (e) => {
                e.preventDefault(); 
                selectOption(index);
            });

            dropdown.appendChild(option);
        });

        dropdown.style.display = currentOptions.length ? "block" : "none";
    }

    function updateHighlight() {
        const options = dropdown.querySelectorAll('.option-item');
        options.forEach((opt, idx) => {
            opt.classList.toggle('highlight', idx === highlightedIndex);
        });
    }

    function selectOption(index) {
        if (index >= 0 && index < currentOptions.length) {
            input.value = currentOptions[index].ten;
            hiddenId.value = currentOptions[index].id;
            dropdown.style.display = "none";
        }
    }

    document.addEventListener('click', (e) => {
        if (!e.target.closest('#comboBox') && !e.target.closest('#dropdownList')) {
            if (hiddenId.value === "" && input.value.trim() !== "") {
                input.value = "";
                hiddenId.value = 0;
            }
            dropdown.style.display = "none";
        }
    });

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

    document.addEventListener('click', (e) => {
        const isClickInsideCombo = e.target.closest('#comboBox') || e.target.closest('#dropdownList');

        if (!isClickInsideCombo) {
            if (hiddenId.value === "" && input.value.trim() !== "") {
                input.value = "";
                hiddenId.value = 0;
            }
            dropdown.style.display = "none";
        }
    });
});

document.addEventListener("DOMContentLoaded", () => {
    const input = document.getElementById('comboBox2');
    const dropdown = document.getElementById('dropdownList2');
    const hiddenId = document.getElementById('IDNhanVien');
    let isMouseDownOnDropdown = false; 

    hiddenId.value = 0;
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

    dropdown.addEventListener('mousedown', () => {
        isMouseDownOnDropdown = true;
    });

    input.addEventListener('blur', () => {
        setTimeout(() => {
            if (!isMouseDownOnDropdown) {
                if (hiddenId.value === "" && input.value.trim() !== "") {
                    input.value = "";
                    hiddenId.value = 0;
                }
            }
            isMouseDownOnDropdown = false;
            dropdown.style.display = "none";
        }, 100);
    });

    function removeAccents(str) {
        return str.normalize("NFD").replace(/[\u0300-\u036f]/g, "");
    }

    function highlightMatch(text, keyword) {
        if (!keyword) return text;

        const normalizedText = removeAccents(text).toLowerCase();
        const normalizedKeyword = removeAccents(keyword).toLowerCase();

        const startIndexNormalized = normalizedText.indexOf(normalizedKeyword);
        if (startIndexNormalized === -1) return text;

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

    function renderOptions(filter = "") {
        dropdown.innerHTML = "";
        highlightedIndex = 0;
        const normalizedFilter = removeAccents(filter.toLowerCase());

        currentOptions = provincesDataNhanVien.filter(item =>
            removeAccents((item.Ten || '').toLowerCase()).includes(normalizedFilter) ||
            removeAccents((item.Viettat || "").toLowerCase()).startsWith(normalizedFilter)
        );

        currentOptions.forEach((item, index) => {
            const option = document.createElement('div');
            option.classList.add('option-item');

            const highlightedTenNhanVien = highlightMatch(item.Ten, filter);
            const highlightedVietTat = highlightMatch(item.Viettat || "", filter);

            const nameSpan = document.createElement('span');
            nameSpan.innerHTML = highlightedTenNhanVien;
            nameSpan.style.flex = "1";

            const abbrSpan = document.createElement('span');
            abbrSpan.innerHTML = highlightedVietTat;
            abbrSpan.style.marginLeft = "10px";
            abbrSpan.style.color = "#888";
            abbrSpan.style.fontSize = "12px";

            option.appendChild(nameSpan);
            option.appendChild(abbrSpan);

            if (index === highlightedIndex) option.classList.add('highlight');

            option.addEventListener('mousedown', (e) => {
                e.preventDefault();
                selectOption(index);
            });

            dropdown.appendChild(option);
        });

        dropdown.style.display = currentOptions.length ? "block" : "none";
    }

    function updateHighlight() {
        const options = dropdown.querySelectorAll('.option-item');
        options.forEach((opt, idx) => {
            opt.classList.toggle('highlight', idx === highlightedIndex);
        });
    }

    function selectOption(index) {
        if (index >= 0 && index < currentOptions.length) {
            input.value = currentOptions[index].Ten;
            hiddenId.value = currentOptions[index].Id;
            dropdown.style.display = "none";
        }
    }

    input.addEventListener('focus', () => renderOptions());
    input.addEventListener('input', () => {
        renderOptions(input.value);
    });

    window.addEventListener('load', () => {
        if (hiddenId.value && !input.value) {
            const selected = provincesDataNhanVien.find(x => x.Id == hiddenId.value);
            if (selected) {
                input.value = selected.Ten;
            }
        }
    });

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

    document.addEventListener('click', (e) => {
        const isClickInsideCombo = e.target.closest('#comboBox2') || e.target.closest('#dropdownList2');

        if (!isClickInsideCombo) {
            if (hiddenId.value === "" && input.value.trim() !== "") {
                input.value = "";
                hiddenId.value = 0;
            }
            dropdown.style.display = "none";
        }
    });
});

