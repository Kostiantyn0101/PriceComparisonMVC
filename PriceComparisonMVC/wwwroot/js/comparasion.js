/**
 * JavaScript для сторінки порівняння товарів
 */
document.addEventListener('DOMContentLoaded', function () {
    // Обробка кнопок фільтра для відображення всіх або тільки відмінних характеристик
    initFilterButtons();

    // Налаштування рівномірної ширини колонок
    initEqualColumnWidths();

    // Ініціалізація Bootstrap компонентів
    initBootstrapComponents();

    // Обробка кнопок додавання до обраного
    initFavoritesButtons();
});

/**
 * Ініціалізація функціональності кнопок фільтрації (всі / відмінні характеристики)
 */
function initFilterButtons() {
    const showAllBtn = document.getElementById('show-all');
    const showDifferentBtn = document.getElementById('show-different');
    const specRows = document.querySelectorAll('.specs-table tr');

    if (!showAllBtn || !showDifferentBtn) return;

    showAllBtn.addEventListener('click', function () {
        showAllBtn.classList.add('active');
        showDifferentBtn.classList.remove('active');
        specRows.forEach(row => row.style.display = '');
    });

    showDifferentBtn.addEventListener('click', function () {
        showDifferentBtn.classList.add('active');
        showAllBtn.classList.remove('active');

        specRows.forEach(row => {
            const cells = Array.from(row.querySelectorAll('td:not(.feature-name)'));
            // Видаляємо порожні елементи
            const nonEmptyCells = cells.filter(cell => cell.textContent.trim() !== '');

            if (nonEmptyCells.length > 1) {
                let allSame = true;
                const firstValue = nonEmptyCells[0].textContent.trim();

                for (let i = 1; i < nonEmptyCells.length; i++) {
                    if (nonEmptyCells[i].textContent.trim() !== firstValue) {
                        allSame = false;
                        break;
                    }
                }

                row.style.display = allSame ? 'none' : '';
            }
        });
    });
}

/**
 * Функція для встановлення однакової ширини стовпців
 */
function initEqualColumnWidths() {
    function setEqualWidths() {
        // Отримуємо контейнер
        const container = document.querySelector('.comparison-container');
        if (!container) return;

        // Отримуємо кількість товарів для порівняння (без порожніх колонок)
        const productColumns = Array.from(document.querySelectorAll('.product-column'))
            .filter(col => !col.querySelector('.empty-product'));

        const productsCount = productColumns.length;

        if (productsCount === 0) return;

        // Отримуємо ширину контейнера
        const containerWidth = container.offsetWidth;

        // Ширина першої колонки з назвами характеристик
        const firstColumnWidth = 200;

        // Доступна ширина для розподілу між колонками товарів
        const availableWidth = containerWidth - firstColumnWidth;

        // Розраховуємо ширину для кожної колонки товару
        const columnWidth = Math.floor(availableWidth / productsCount);

        // CSS-клас для встановлення однакової ширини
        const styleElement = document.createElement('style');
        styleElement.id = 'dynamic-column-styles';

        // Видаляємо попередній динамічний стиль, якщо він існує
        const existingStyle = document.getElementById('dynamic-column-styles');
        if (existingStyle) {
            existingStyle.remove();
        }

        // Створюємо CSS правила для однакової ширини стовпців
        styleElement.innerHTML = `
            /* Стилі для першої колонки з назвами характеристик */
            .add-product-column, .feature-name {
                width: ${firstColumnWidth}px !important;
                min-width: ${firstColumnWidth}px !important;
                max-width: ${firstColumnWidth}px !important;
            }
            
            /* Стилі для колонок товарів */
            .product-column:not(.empty-column) {
                width: ${columnWidth}px !important;
                min-width: ${columnWidth}px !important;
                max-width: ${columnWidth}px !important;
                flex: 0 0 ${columnWidth}px !important;
            }
            
            /* Стилі для комірок таблиці характеристик */
            .specs-table td:not(.feature-name) {
                width: ${columnWidth}px !important;
                min-width: ${columnWidth}px !important;
                max-width: ${columnWidth}px !important;
            }
            
            /* Приховуємо порожню колонку якщо товарів менше 3 */
            ${productsCount < 3 ? '.empty-column { display: none !important; }' : ''}
        `;

        document.head.appendChild(styleElement);

        // Ще один підхід - напряму застосувати стилі до кожного елемента
        productColumns.forEach(column => {
            column.style.width = `${columnWidth}px`;
            column.style.minWidth = `${columnWidth}px`;
            column.style.maxWidth = `${columnWidth}px`;
            column.style.flex = `0 0 ${columnWidth}px`;
        });

        // Додатково додаємо клас для порожньої колонки
        const emptyColumns = document.querySelectorAll('.product-column:has(.empty-product)');
        emptyColumns.forEach(col => {
            col.classList.add('empty-column');
            if (productsCount < 3) {
                col.style.display = 'none';
            } else {
                col.style.width = `${columnWidth}px`;
                col.style.minWidth = `${columnWidth}px`;
                col.style.maxWidth = `${columnWidth}px`;
            }
        });

        // Встановлюємо рівну ширину для комірок таблиці
        const tableValueCells = document.querySelectorAll('.specs-table td:not(.feature-name)');
        tableValueCells.forEach((cell, index) => {
            const actualColumnIndex = index % (productsCount + (productsCount < 3 ? 0 : 1));
            if (actualColumnIndex < productsCount) {
                cell.style.width = `${columnWidth}px`;
                cell.style.minWidth = `${columnWidth}px`;
                cell.style.maxWidth = `${columnWidth}px`;
            } else {
                // Для додаткових колонок (порожніх)
                if (productsCount < 3) {
                    cell.style.display = 'none';
                }
            }
        });
    }

    // Виконуємо при завантаженні
    setEqualWidths();

    // Переналаштовуємо при зміні розміру вікна з debounce
    let resizeTimeout;
    window.addEventListener('resize', function () {
        clearTimeout(resizeTimeout);
        resizeTimeout = setTimeout(setEqualWidths, 100);
    });
}

/**
 * Ініціалізація Bootstrap компонентів
 */
function initBootstrapComponents() {
    // Ініціалізація випадаючих меню (dropdowns)
    const dropdownElementList = [].slice.call(document.querySelectorAll('.dropdown-toggle'));
    if (dropdownElementList.length > 0 && typeof bootstrap !== 'undefined') {
        dropdownElementList.map(function (dropdownToggleEl) {
            return new bootstrap.Dropdown(dropdownToggleEl);
        });
    }
}

/**
 * Обробка кнопок додавання до обраного
 */
function initFavoritesButtons() {
    const favButtons = document.querySelectorAll('.favorites-btn');

    favButtons.forEach(button => {
        button.addEventListener('click', function (e) {
            e.preventDefault();

            // Отримуємо іконку всередині кнопки
            const icon = button.querySelector('i');

            // Перемикаємо стан обраного
            if (icon.classList.contains('fas')) {
                // Якщо вже в обраному, видаляємо
                icon.classList.remove('fas');
                icon.classList.add('far');
                icon.style.color = '#ddd';
            } else {
                // Якщо не в обраному, додаємо
                icon.classList.remove('far');
                icon.classList.add('fas');
                icon.style.color = '#ff6b6b';
            }
        });
    });
}