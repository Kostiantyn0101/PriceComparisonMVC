// Функції для роботи з куками
function setCookie(name, value, days) {
    var expires = "";
    if (days) {
        var date = new Date();
        date.setTime(date.getTime() + (days * 24 * 60 * 60 * 1000));
        expires = "; expires=" + date.toUTCString();
    }
    document.cookie = name + "=" + (value || "") + expires + "; path=/";
}

function getCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) == 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

// Функції для управління списками продуктів
function getProductList(cookieName) {
    var list = getCookie(cookieName);
    return list ? JSON.parse(list) : [];
}

function addProductToList(productId, cookieName, maxCount) {
    var list = getProductList(cookieName);

    // Перевіряємо, чи продукт вже в списку
    if (!list.includes(productId)) {
        // Обмежуємо кількість продуктів у списку
        if (list.length >= maxCount) {
            list.shift(); // Видаляємо найстаріший продукт
        }
        list.push(productId);
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

// Функції для додавання продуктів до різних списків
function addToComparison(productId) {
    if (addProductToList(productId, 'comparison_products', 5)) {
        showNotification('Товар додано до порівняння', true);
    } else {
        showNotification('Товар вже в списку порівняння', false);
    }
}

function addToFavorites(productId) {
    if (addProductToList(productId, 'favorite_products', 20)) {
        showNotification('Товар додано до обраного', true);
    } else {
        showNotification('Товар вже в списку обраного', false);
    }
}