
/**Получить значения куки по имени куки */
export function GetCookieByName(cookieName) {
    let matches = document.cookie.match(new RegExp(
        "(?:^|; )" + cookieName.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
    ));
    return matches ? decodeURIComponent(matches[1]) : undefined;
}

/**
 * Сохранит в куки новое значение
 * @param {any} cookieName
 * @param {any} newValue
 */
export function UpdateCookieValue(cookieName, newValue) {

    document.cookie = cookieName + "=" + newValue;
}

/**
 * Пробует удалить значение из куки
 * @param {any} cookieName
 * @param {any} removingValue
 * Верент тру если удалось 
 */
export function TryRemoveCookie(cookieName, removingValue) {

}