$(document).ready(function () {
    // Встановлюємо CSS змінну з кількістю товарів для правильного розміру клітинок
    var productCount = $(".mm-product-column").not(".mm-empty-column").length;
    document.documentElement.style.setProperty('--product-count', productCount);

    // Динамічно розраховуємо ширину колонок товарів
    function setDynamicWidths() {
        var totalColumns = productCount + 1; // +1 для колонки "Додати товар"
        var columnWidthPercentage = 100 / totalColumns;

        // Встановлюємо ширину колонок товарів
        $(".mm-product-column").css("width", columnWidthPercentage + "%");
        $(".mm-add-product-column").css("width", columnWidthPercentage + "%");

        // Встановлюємо ширину клітинок характеристик
        $(".mm-feature-value").css("width", columnWidthPercentage + "%");

        // Забезпечуємо однакову ширину заголовків і клітинок таблиці
        $(".mm-specs-table").each(function () {
            var tableWidth = $(this).width();
            var featureNameWidth = $(".mm-feature-name").first().width();
            var remainingWidth = tableWidth - featureNameWidth;
            var valueWidth = remainingWidth / productCount;
            $(this).find(".mm-feature-value").css("width", valueWidth + "px");
        });
    }

    // Викликаємо функцію при завантаженні та при зміні розміру вікна
    setDynamicWidths();
    $(window).on('resize', setDynamicWidths);

    // Обробка кнопок для фільтрації
    $("#show-different").click(function () {
        $(this).addClass('active');
        $("#show-all").removeClass('active');

        // Проходимося по всіх рядках таблиць
        $(".mm-specs-table tr").each(function () {
            var cells = $(this).find("td.mm-feature-value").not(".mm-empty-value");
            var values = [];

            // Збираємо всі непусті значення
            cells.each(function () {
                var value = $(this).text().trim();
                if (value !== "" && value !== "-") {
                    values.push(value);
                }
            });

            // Якщо всі значення однакові, ховаємо рядок
            var allSame = values.every(v => v === values[0]);
            if (allSame && values.length > 1) {
                $(this).hide();
            }
        });
    });

    $("#show-all").click(function () {
        $(this).addClass('active');
        $("#show-different").removeClass('active');

        // Показуємо всі рядки
        $(".mm-specs-table tr").show();
    });
});
