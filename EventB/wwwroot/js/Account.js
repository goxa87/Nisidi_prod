window.onload = function () {
    console.log($('.form-entry-title[name=City]').val());
    if ($('.form-entry-title[name=City]').val() == '') {
        $('.form-entry-title[name=City]').val(ymaps.geolocation.city)
    }
}

$(document).ready(function () {
    // начальная загрузка
    setTimeout(function () {
        $('.acc-preview').fadeOut(800, function () {
            $('.acc-form-menu').fadeIn(500);
            $('.acc-form-menu').addClass('flex-vcc');
        });
    }, 500);

    // Генерация пароля
    GetAndSetRandomPassword();

    // переключение вход регистрация
    $('body').on('click', '.acc-btn-switch', function () {
        
        if ($(this).hasClass('acc-sw-log')) {
            $('.acc-sw-log').addClass('acc-switch-selected');
            $('.acc-sw-reg').removeClass('acc-switch-selected');
            $('.acc-reg-form').fadeOut(250, function () {
                $('.acc-login-form').fadeIn(250);
            });
        } else {
            $('.acc-sw-log').removeClass('acc-switch-selected');
            $('.acc-sw-reg').addClass('acc-switch-selected');
            $('.acc-login-form').fadeOut(250, function () {
                $('.acc-reg-form').fadeIn(250)
            });            
        }
    });

    // Генерация пароля
    $('body').on('click', '.acc-generate-password', function () {
        GetAndSetRandomPassword();
    });

    //Получает интересы из куки
    GetInterestsFromCookie();
});

/**Устанавливает в поле пароль регистрации рандомный пароль */
function GetAndSetRandomPassword() {
    if ($('#acc-reg-password-input') == undefined) {
        return;
    }

    $.ajax({
        method: 'get',
        url: '/Account/GetNewRandomPassword',
        success: function (newPassword) {
            $('#acc-reg-password-input').val(newPassword);
        },
        error: function () {
            console.log('Не удалось получить сгенерированный пароль');
        }
    })
    
}

/** Сохраняет интересы пользователя в скрытое поле при регистрации */
function GetInterestsFromCookie() {
    var cookieInterests = document.cookie.replace(/(?:(?:^|.*;\s*)intereses\s*\=\s*([^;]*).*$)|^.*$/, "$1");
    $("#interestsCookie").val(cookieInterests);
}