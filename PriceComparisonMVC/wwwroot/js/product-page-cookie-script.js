// Функції для роботи з куками



function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + encodeURIComponent(value || "") + expires + "; path=/";
}

function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i].trim();
        if (c.indexOf(nameEQ) == 0) {
            // Декодуємо значення куки
            return decodeURIComponent(c.substring(nameEQ.length));
        }
    }
    return null;
}

// Функції для управління списками продуктів
function getProductList(cookieName) {
    var list = getCookie(cookieName);
    if (!list) return [];

    try {
        var parsed = JSON.parse(list);

        // Перевіряємо, чи це масив об'єктів (новий формат) чи просто масив чисел (старий формат)
        if (Array.isArray(parsed) && parsed.length > 0) {
            if (typeof parsed[0] === 'object' && parsed[0] !== null && parsed[0].hasOwnProperty('productId')) {
                // Новий формат - масив об'єктів з productId та categoryId
                return parsed;
            } else if (typeof parsed[0] === 'number' || typeof parsed[0] === 'string') {
                // Старий формат - масив ID продуктів
                // Конвертуємо його у новий формат
                var newFormat = parsed.map(function (id) {
                    return { productId: parseInt(id), categoryId: 1 }; // Припускаємо категорію 1 як запасний варіант
                });

                // Оновлюємо кукі до нового формату
                setCookie(cookieName, JSON.stringify(newFormat), 30);

                return newFormat;
            }
        }
        return parsed;
    } catch (e) {
        console.error("Помилка при парсингу списку продуктів:", e);
        return [];
    }
}

// Модифікована функція для додавання продукту в список разом з categoryId
function addProductToList(productId, categoryId, cookieName, maxCount) {
    var list = getProductList(cookieName);

    // Створюємо об'єкт з productId та categoryId
    var productItem = {
        productId: productId,
        categoryId: categoryId
    };

    // Перевіряємо, чи продукт вже в списку (порівнюємо за productId)
    var exists = false;
    for (var i = 0; i < list.length; i++) {
        if (list[i].productId === productId) {
            exists = true;
            break;
        }
    }

    if (!exists) {
        // Обмежуємо кількість продуктів у списку
        if (list.length >= maxCount) {
            list.shift(); // Видаляємо найстаріший продукт
        }
        list.push(productItem);
        setCookie(cookieName, JSON.stringify(list), 30); // Зберігаємо на 30 днів
        return true;
    }
    return false;
}

// Функція для відображення повідомлення про успішне додавання
function showNotification(message, isSuccess) {
    var notification = document.createElement('div');
    notification.className = isSuccess ? 'notification success' : 'notification info';
    notification.innerHTML = message;
    document.body.appendChild(notification);

    // Анімація з'явлення
    setTimeout(function () {
        notification.classList.add('show');
    }, 10);

    // Автоматичне зникнення через 3 секунди
    setTimeout(function () {
        notification.classList.remove('show');
        setTimeout(function () {
            document.body.removeChild(notification);
        }, 300);
    }, 3000);
}

// Модифіковані функції для додавання продуктів до різних списків
function addToComparison(productId, categoryId) {
    if (addProductToList(productId, categoryId, 'comparison_products', 5)) {
        showNotification('Товар додано до порівняння', true);
    } else {
        showNotification('Товар вже в списку порівняння', false);
    }
}

//function addToFavorites(productId, categoryId) {
//    if (addProductToList(productId, categoryId, 'favorite_products', 20)) {
//        showNotification('Товар додано до обраного', true);
//    } else {
//        showNotification('Товар вже в списку обраного', false);
//    }
//}


function addToFavorites(productId, categoryId) {
    console.log(`addToFavorites called with: productId=${productId}, categoryId=${categoryId}`);
    // Перевірка типу і значення categoryId
    console.log(`categoryId type: ${typeof categoryId}, value: ${categoryId}`);

    if (addProductToList(productId, categoryId, 'favorite_products', 20)) {
        showNotification('Товар додано до обраного', true);
    } else {
        showNotification('Товар вже в списку обраного', false);
    }
}


// Функція для оновлення лічильників у хедері
function updateHeaderCounters() {
    // Оновлення лічильника порівняння
    var comparisonList = getProductList('comparison_products');
    var comparisonBadge = document.getElementById('comparison-badge');

    if (comparisonBadge) {
        if (comparisonList.length > 0) {
            comparisonBadge.textContent = comparisonList.length;
            comparisonBadge.style.display = 'inline-block';
        } else {
            comparisonBadge.style.display = 'none';
        }
    }

    // Оновлення лічильника обраного
    var favoritesList = getProductList('favorite_products');
    var favoritesBadge = document.getElementById('favorites-badge');

    if (favoritesBadge) {
        if (favoritesList.length > 0) {
            favoritesBadge.textContent = favoritesList.length;
            favoritesBadge.style.display = 'inline-block';
        } else {
            favoritesBadge.style.display = 'none';
        }
    }
}

// Викликаємо функцію при завантаженні сторінки
document.addEventListener('DOMContentLoaded', updateHeaderCounters);