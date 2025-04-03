
// Функція для переключення між вкладками
function showTab(tabId) {
    console.log("Активація вкладки: " + tabId);

    // Отримуємо всі вкладки
    var storesTab = document.getElementById('stores-tab');
    var specsTab = document.getElementById('specs-tab');
    var reviewsTab = document.getElementById('reviews-tab');

    // Спеціальна поведінка для "Опис" - показати всі вкладки
    if (tabId === 'description-tab') {
        // Показуємо всі вкладки
        if (storesTab) storesTab.style.display = 'block';
        if (specsTab) specsTab.style.display = 'block';
        if (reviewsTab) reviewsTab.style.display = 'block';
    }
    // Поведінка для інших вкладок - показати тільки вибрану
    else {
        // Приховуємо всі вкладки
        if (storesTab) storesTab.style.display = 'none';
        if (specsTab) specsTab.style.display = 'none';
        if (reviewsTab) reviewsTab.style.display = 'none';

        // Показуємо вибрану вкладку
        var selectedTab = document.getElementById(tabId);
        if (selectedTab) {
            selectedTab.style.display = 'block';
            console.log("Показую вкладку: " + tabId);
        } else {
            console.log("Вкладка не знайдена: " + tabId);
        }
    }

    // Оновлюємо активний стан у навігації
    var navItems = document.querySelectorAll('.nav-item');
    navItems.forEach(function (item) {
        item.classList.remove('active');
    });

    // Додаємо активний клас до поточного пункту
    var currentNav = document.querySelector('[onclick="showTab(\'' + tabId + '\')"]');
    if (currentNav) {
        currentNav.classList.add('active');
    }
}

// Функція для розгортання/згортання опису товару в картках магазинів
function toggleDescription(btn) {
    const descriptionBox = btn.closest('.store-card-info-box-4');
    descriptionBox.classList.toggle('expanded');
    btn.textContent = descriptionBox.classList.contains('expanded')
        ? 'Згорнути'
        : '...';
}

// Функція для відображення модального вікна з зображенням
function openImageModal(imageUrl) {
    const modal = document.getElementById('image-modal');
    const modalImg = document.getElementById('modal-image');

    if (modal && modalImg) {
        modalImg.src = imageUrl;
        modal.style.display = 'block';

        // Закриття модального вікна при кліку на хрестик
        const closeBtn = document.getElementsByClassName('close-modal')[0];
        if (closeBtn) {
            closeBtn.onclick = function () {
                modal.style.display = 'none';
            };
        }

        // Закриття модального вікна при кліку поза зображенням
        modal.onclick = function (event) {
            if (event.target === modal) {
                modal.style.display = 'none';
            }
        };
    }
}

// Ініціалізація при завантаженні сторінки
document.addEventListener("DOMContentLoaded", function () {
    // Виводимо в консоль всі вкладки для діагностики
    console.log("Доступні вкладки:");
    document.querySelectorAll('.tab-content').forEach(function (tab) {
        console.log(tab.id);
    });

    // Ініціалізація каруселі зображень
    const track = document.querySelector(".custom-thumbs-track");

    // Якщо карусель існує на сторінці
    if (track) {
        // Використовуємо контейнер для кожного слайда
        const slides = track.querySelectorAll(".custom-thumb-image-container");
        const originalCount = slides.length; // Кількість оригінальних слайдів
        const prevBtn = document.querySelector(".custom-thumb-prev");
        const nextBtn = document.querySelector(".custom-thumb-next");
        // Задані значення: ширина контейнера слайда (283px) плюс gap (12px)
        const slideWidth = 283 + 12; // 295px
        // Початкове зсування встановлюємо, щоб перший оригінальний слайд був видимим
        let position = -slideWidth;
        // Функція для дублювання першого та останнього слайдів для безперервного циклу
        function duplicateSlides() {
            const firstClone = slides[0].cloneNode(true);
            const lastClone = slides[originalCount - 1].cloneNode(true);
            track.appendChild(firstClone);                // додаємо клон першого слайда в кінець
            track.insertBefore(lastClone, track.firstChild); // додаємо клон останнього слайда на початок
        }

        // Дублюємо слайди тільки якщо є щонайменше один слайд
        if (originalCount > 0) {
            duplicateSlides();
            // Встановлюємо початковий зсув треку
            track.style.transform = `translateX(${position}px)`;
        }

        // При натисканні кнопки "Наступний" переміщаємо карусель вліво
        if (nextBtn) {
            nextBtn.addEventListener("click", function () {
                position -= slideWidth;
                track.style.transition = "transform 0.3s ease-in-out";
                track.style.transform = `translateX(${position}px)`;
            });
        }

        // При натисканні кнопки "Попередній" переміщаємо карусель вправо
        if (prevBtn) {
            prevBtn.addEventListener("click", function () {
                position += slideWidth;
                track.style.transition = "transform 0.3s ease-in-out";
                track.style.transform = `translateX(${position}px)`;
            });
        }

        // Обробник події завершення переходу
        track.addEventListener("transitionend", function () {
            // Якщо позиція досягла клона першого елемента (тобто після останнього оригінального)
            if (position <= -((originalCount + 1) * slideWidth)) {
                track.style.transition = "none";
                position = -slideWidth;
                track.style.transform = `translateX(${position}px)`;
                void track.offsetWidth; // Примусове reflow
                track.style.transition = "transform 0.3s ease-in-out";
            }
            // Якщо позиція досягла клона останнього елемента (тобто перед першим оригінальним)
            if (position >= 0) {
                track.style.transition = "none";
                position = -(originalCount * slideWidth);
                track.style.transform = `translateX(${position}px)`;
                void track.offsetWidth;
                track.style.transition = "transform 0.3s ease-in-out";
            }
        });
    }

    // Ініціалізація форми відгуків
    initReviewsForm();

    // За замовчуванням активуємо вкладку "Опис", що показує всі секції
    showTab('description-tab');
});

// Функція для ініціалізації форми відгуків
function initReviewsForm() {
    // Показ/приховування форми відгуку
    const showReviewFormButton = document.getElementById('show-review-form');
    const formContainer = document.getElementById('feedback-form-container');
    const cancelButton = document.getElementById('cancel-review');

    if (showReviewFormButton && formContainer) {
        showReviewFormButton.addEventListener('click', function () {
            formContainer.style.display = 'block';
            showReviewFormButton.style.display = 'none';
        });
    }

    if (cancelButton && formContainer && showReviewFormButton) {
        cancelButton.addEventListener('click', function () {
            formContainer.style.display = 'none';
            showReviewFormButton.style.display = 'block';
        });
    }

    // Попередній перегляд зображень
    const imageInput = document.getElementById('FeedbackImages');
    const previewContainer = document.getElementById('image-preview');

    if (imageInput && previewContainer) {
        imageInput.addEventListener('change', function () {
            previewContainer.innerHTML = '';

            if (this.files) {
                const maxFiles = 5;
                const filesCount = Math.min(this.files.length, maxFiles);

                for (let i = 0; i < filesCount; i++) {
                    const file = this.files[i];
                    if (!file.type.startsWith('image/')) continue;

                    const reader = new FileReader();

                    reader.onload = function (e) {
                        const imgContainer = document.createElement('div');
                        imgContainer.className = 'preview-image-container';

                        const img = document.createElement('img');
                        img.src = e.target.result;
                        img.className = 'preview-image';

                        const removeBtn = document.createElement('span');
                        removeBtn.className = 'remove-preview-image';
                        removeBtn.innerHTML = '&times;';
                        removeBtn.onclick = function () {
                            imgContainer.remove();
                        };

                        imgContainer.appendChild(img);
                        imgContainer.appendChild(removeBtn);
                        previewContainer.appendChild(imgContainer);
                    };

                    reader.readAsDataURL(file);
                }

                if (this.files.length > maxFiles) {
                    alert(`Ви можете завантажити максимум ${maxFiles} зображень. Перші ${maxFiles} були обрані.`);
                }
            }
        });
    }

    // Стрілки для розгортання коментарів
    const dropdownArrows = document.querySelectorAll('.dropdown-arrow');
    if (dropdownArrows && dropdownArrows.length > 0) {
        dropdownArrows.forEach(arrow => {
            arrow.addEventListener('click', function () {
                const reviewCard = this.closest('.review-card');
                if (reviewCard) {
                    const reviewComment = reviewCard.querySelector('.review-comment-text');
                    if (reviewComment) {
                        if (reviewComment.style.display === 'none' || !reviewComment.style.display) {
                            reviewComment.style.display = 'block';
                            this.src = '/images/arrow-down-02.png';
                        } else {
                            reviewComment.style.display = 'none';
                            this.src = '/images/arrow-up-02.png';
                        }
                    }
                }
            });
        });
    }
}





