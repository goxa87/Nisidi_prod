/**
 * Выполнит запрос, тело должно быть WebResponse
 * @param {any} url адрес
 * @param {any} method метод
 * @param {any} data данные
 * @param {any} showNotify показывать ли уведомления 
 * @param {any} successCallback что делаем потом
 */
export function ExecuteAjaxRequest(url, method, data, showNotify, successCallback) {
    let sendData = (data && data != null) ? data : null;
    console.log('data', sendData);
    $.ajax({
        url: url,
        data: sendData,
        method: method,
        success: function (resp) {
            if (resp.isSuccess) {
                if (showNotify) {
                    DevExpress.ui.notify("Успешно", 'success', 2000);
                }
                if (successCallback && typeof(successCallback) === 'function') {
                    successCallback(resp.content);
                }
            } else {
                if (showNotify) {
                    DevExpress.ui.notify("Ошибка бизнес логики. " + resp.errorMessage, 'error', 2000);
                }
                console.log("Ошибка удаления. " + resp.errorMessage);
            }
        },
        error: function (jqXHR) {
            if (showNotify) {
                DevExpress.ui.notify("Ошибка сети Выполенения запроса. " + jqXHR.status, 'error', 2000);
            }
            console.log('Ошибка сети Выполенения запроса.', jqXHR);
        }
    });
}
