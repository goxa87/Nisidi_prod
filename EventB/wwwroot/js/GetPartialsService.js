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
            $("#dateSinseDE").dxDateBox({
                type: "date",
                displayFormat: "dd.MM.yyyy",
                dateSerializationFormat: "yyyy-MM-dd",
                showClearButton: true,
                onValueChanged: function (e) {
                    $('#ser-date-since').val(e.value);
                }
            });
            $("#dateDueDE").dxDateBox({
                type: "date",
                displayFormat: "dd.MM.yyyy",
                dateSerializationFormat: "yyyy-MM-dd",
                showClearButton: true,
                onValueChanged: function (e) {
                    $('#ser-date-due').val(e.value);
                }
            });
        },
        error: function () {
            callback("Ошибка получения разметки")
        }
    });
}