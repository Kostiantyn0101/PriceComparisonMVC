document.addEventListener('DOMContentLoaded', function () {
    // Знаходимо поле пошуку в шапці
    const searchField = document.querySelector('.custom-search-field');
    const searchForm = document.getElementById('search-form');

    // Змінні для debounce
    let searchTimeout = null;
    const debounceDelay = 1500; // 1.5 секунди

    // Мінімальна кількість символів для запуску пошуку
    const minSearchLength = 3;

    // Перевіряємо наявність поля пошуку
    if (!searchField) return;

    // Створюємо контейнер для випадаючих результатів, якщо його ще немає
    let searchResultsDropdown = document.getElementById('search-results-dropdown');
    if (!searchResultsDropdown) {
        searchResultsDropdown = document.createElement('div');
        searchResultsDropdown.id = 'search-results-dropdown';
        searchResultsDropdown.className = 'search-results-dropdown';
        searchResultsDropdown.style.display = 'none';

        // Додаємо стилі для випадаючого списку
        const style = document.createElement('style');
        style.textContent = `
            .search-results-dropdown {
                position: absolute;
                top: 100%;
                left: 0;
                right: 0;
                z-index: 1000;
                background-color: white;
                border: 1px solid #ddd;
                border-radius: 0 0 4px 4px;
                box-shadow: 0 2px 5px rgba(0,0,0,0.2);
                max-height: 400px;
                overflow-y: auto;
            }
            .search-result-item {
                padding: 10px 15px;
                border-bottom: 1px solid #eee;
                cursor: pointer;
                display: flex;
                align-items: center;
            }
            .search-result-item:hover {
                background-color: #f8f9fa;
            }
            .search-result-item img {
                width: 40px;
                height: 40px;
                object-fit: contain;
                margin-right: 10px;
            }
            .search-result-name {
                flex-grow: 1;
            }
            .search-result-price {
                color: #28a745;
                font-weight: bold;
                white-space: nowrap;
            }
            .search-view-all {
                text-align: center;
                padding: 12px;
                background-color: #f8f9fa;
                border-top: 1px solid #ddd;
                font-weight: bold;
                cursor: pointer;
            }
            .search-view-all:hover {
                background-color: #e9ecef;
            }
            .search-no-results {
                padding: 15px;
                text-align: center;
                color: #6c757d;
            }
            .search-loading {
                padding: 15px;
                text-align: center;
                color: #6c757d;
            }
        `;
        document.head.appendChild(style);

        // Вставляємо випадаючий список після батьківського контейнера поля пошуку
        const searchContainer = searchField.closest('.col-5');
        searchContainer.style.position = 'relative';
        searchContainer.appendChild(searchResultsDropdown);
    }

    // Обробник введення тексту в поле пошуку
    searchField.addEventListener('input', function () {
        const query = this.value.trim();

        // Очищаємо попередній таймер debounce
        if (searchTimeout) {
            clearTimeout(searchTimeout);
        }

        // Якщо запит менше мінімальної довжини, приховуємо випадаючий список
        if (query.length < minSearchLength) {
            searchResultsDropdown.style.display = 'none';
            return;
        }

        // Показуємо індикатор завантаження
        searchResultsDropdown.style.display = 'block';
        searchResultsDropdown.innerHTML = '<div class="search-loading">Пошук...</div>';

        // Встановлюємо таймер debounce
        searchTimeout = setTimeout(function () {
            // Виконуємо AJAX запит до API
            fetchSearchResults(query);
        }, debounceDelay);
    });

    // Функція для отримання результатів пошуку
    function fetchSearchResults(query) {
        fetch(`/Search/Autocomplete?query=${encodeURIComponent(query)}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error('Помилка при отриманні результатів пошуку');
                }
                return response.json();
            })
            .then(data => {
                renderSearchResults(data, query);
            })
            .catch(error => {
                console.error('Error:', error);
                searchResultsDropdown.innerHTML = `
                    <div class="search-no-results">
                        Сталася помилка при пошуку. Спробуйте ще раз пізніше.
                    </div>
                `;
            });
    }

    // Функція для відображення результатів пошуку у випадаючому списку
    function renderSearchResults(results, query) {
        // Очищаємо випадаючий список
        searchResultsDropdown.innerHTML = '';

        // Перевіряємо, чи є результати
        if (!results || results.length === 0) {
            searchResultsDropdown.innerHTML = `
                <div class="search-no-results">
                    За запитом "${query}" нічого не знайдено.
                </div>
            `;
            return;
        }

        // Беремо лише перші 5 результатів
        const limitedResults = results.slice(0, 5);

        // Створюємо елементи для кожного результату
        limitedResults.forEach(product => {
            const resultItem = document.createElement('div');
            resultItem.className = 'search-result-item';
            resultItem.innerHTML = `
                <img src="${product.imageUrl || '/images/placeholder.jpg'}" alt="${product.fullName}">
                <div class="search-result-name">${product.fullName}</div>
                <div class="search-result-price">${product.minPrice ? `${product.minPrice.toLocaleString()} грн` : ''}</div>
            `;

            // Додаємо обробник кліку для переходу на сторінку товару
            resultItem.addEventListener('click', function () {
                window.location.href = `/Product/Index?firstProductId=${product.id}`;
            });

            searchResultsDropdown.appendChild(resultItem);
        });

        // Додаємо посилання на повну сторінку результатів із зазначенням кількості
        const viewAllLink = document.createElement('div');
        viewAllLink.className = 'search-view-all';
        viewAllLink.textContent = `Переглянути всі результати (${results.length}) для "${query}"`;
        viewAllLink.addEventListener('click', function () {
            window.location.href = `/Search/Results?query=${encodeURIComponent(query)}`;
        });
        searchResultsDropdown.appendChild(viewAllLink);
    }

    // Обробник події натискання клавіші в полі пошуку
    searchField.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') {
            e.preventDefault();

            const query = this.value.trim();
            if (query.length >= minSearchLength) {
                window.location.href = `/Search/Results?query=${encodeURIComponent(query)}`;
            }
        }
    });

    // Закриваємо випадаючий список при кліку поза ним
    document.addEventListener('click', function (e) {
        if (!searchField.contains(e.target) && !searchResultsDropdown.contains(e.target)) {
            searchResultsDropdown.style.display = 'none';
        }
    });

    // Фокус на полі пошуку
    searchField.addEventListener('focus', function () {
        const query = this.value.trim();
        if (query.length >= minSearchLength) {
            searchResultsDropdown.style.display = 'block';
        }
    });
});