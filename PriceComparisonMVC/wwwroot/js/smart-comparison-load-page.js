document.addEventListener('DOMContentLoaded', function () {
    // Отримання параметрів з URL
    const urlParams = new URLSearchParams(window.location.search);
    const productIdA = urlParams.get('productIdA');
    const productIdB = urlParams.get('productIdB');

    // Запускаємо запит на отримання даних порівняння
    fetch(`/Compare/SmartComparisonData?productIdA=${productIdA}&productIdB=${productIdB}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Помилка при отриманні даних порівняння');
            }
            // Починаємо періодичну перевірку статусу
            checkComparisonStatus();
        })
        .catch(error => {
            console.error('Помилка:', error);
            // Навіть при помилці, починаємо перевіряти статус
            checkComparisonStatus();
        });

    // Функція для періодичної перевірки завершення порівняння
    function checkComparisonStatus() {
        fetch('/Compare/CheckComparisonStatus')
            .then(response => response.json())
            .then(data => {
                if (data.isComplete) {
                    // Якщо порівняння завершено, перенаправляємо на сторінку результатів
                    window.location.href = `/Compare/SmartComparisonResult?productIdA=${productIdA}&productIdB=${productIdB}`;
                } else {
                    // Якщо порівняння ще не завершено, перевіряємо знову через 1 секунду
                    setTimeout(checkComparisonStatus, 1000);
                }
            })
            .catch(error => {
                console.error('Помилка при перевірці статусу:', error);
                // При помилці чекаємо 1 секунду перед повторною спробою
                setTimeout(checkComparisonStatus, 1000);
            });
    }
});