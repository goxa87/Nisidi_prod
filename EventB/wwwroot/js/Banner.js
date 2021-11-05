/**
    добавляем куку с именем banners
    значения в формате <имя_баннера>:<значение_булево># 
 */

/**
 * Получить разметку баннера по названию файла банера
 * @param {any} bannerName имя файла из папки
 * @param {any} tegIdSelector селктор на верстке (передавать через id без #)
 */
export function GetBanner(bannerName, tegIdSelector) {
    $.ajax({
        url: "/Banner/GetBannerByName",
        data: {
            name: bannerName
        },
        success: function (markup) {
            let selector = "#" + tegIdSelector;
            $(selector).html(markup);
            $(selector).removeClass('display-none');
            // добавляем класс название баннера без расширения и цепляем обработчик добавление куки на закрытие

            let closeBnSelector = GetCookieName(bannerName);
            $(selector).children().find('.btn-close').addClass(closeBnSelector);
            $('body').on('click', '.' + closeBnSelector, function () {
                CloseBanner(bannerName);
            });
        },
        error: function () {
            console.log('Ошибка получения разметки баннера');
        }
    });
}

/**
 * Отправит запрос на закрытие баннера чтобы он не открывался (действует для зарегестрированных)
 * @param {any} bannerName
 */
function CloseBanner(bannerName) {
    $.ajax({
        url: "/Banner/CloseBanner",
        data: {
            bannerName: bannerName
        },
        success: function (response) { console.log(response) }
    });
}

/**
 * Вернет название куки и название класса для обработчика закрытия баннера
 * @param {any} source сюда передавать название файла партиалки с баннером вернет 1 до точки
*/
function GetCookieName(source) {
    let closeBnSelector = source.split('.');
    return closeBnSelector[0];
}

