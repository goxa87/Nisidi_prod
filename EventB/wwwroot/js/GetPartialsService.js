/**
 * Вернет разметку для модалки поиска событий
 * @param {any} body
 * @param {any} callback функция которая вставит разметку куда надо
 */
export function GetSearchPartial(body, callback) {
    $.ajax({
        url: '/Search/GetSearchPartial',
        method: 'POST',
        data: {
            body: body
        },
        success: function (response) {
            callback(response);
        },
        error: function () {
            callback("Ошибка получения разметки")
        }
    });
}