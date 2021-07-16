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

/*


/**
 * Проверит нужно ли показывать баннер с заданным именем в этом окне
 * @param {any} name заданное имя баннера (= селектор)
 
export function CheckNeedShowBanner(name) {
    let cookieName = GetCookieName(name);
    let cookie = GetCookie('banners');
    let banValue = GetBannerValue(cookie, cookieName);

    if (banValue == undefined || banValue == 'false') {
        return true;
    } else {
        return false;
    }
}

/**
 * Установит указанную куку на значение true
 * @param {any} bannerName название баннера в перечне банеров
 
function SetCookie(bannerName) {
    let banCookieValue = GetCookie('banners');

    if (banCookieValue == undefined) banCookieValue = '';
    console.log('ban c v', banCookieValue);
    if (banCookieValue.indexOf(bannerName) !== -1) {
        
        banCookieValue = banCookieValue.replace(bannerName + ':false#', bannerName + ':true#')
        console.log('ban c 1', banCookieValue);
    } else {
        
        banCookieValue += bannerName + ':true#';
        console.log('ban c 2', banCookieValue);
    }
    document.cookie ='banners=' + banCookieValue;
    
}

function GetCookie(name) {
    let matches = document.cookie.match(new RegExp(
        "(?:^|; )" + name.replace(/([\.$?*|{}\(\)\[\]\\\/\+^])/g, '\\$1') + "=([^;]*)"
    ));
    return matches ? decodeURIComponent(matches[1]) : undefined;
}

function GetBannerValue(cook, banner) {
    if (cook == undefined || cook == null || cook == '') return undefined;

    let arr = cook.split('#');
    let returned = '';
    arr.forEach(val => {
        if (val.indexOf(banner) != -1) {
            var ret = val.split(':');
            console.log('c1', ret)
            returned = ret[1];
        }
    });
    return returned;
}
*/