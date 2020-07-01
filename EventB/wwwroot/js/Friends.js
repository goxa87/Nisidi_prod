$(document).ready(function ()
{
    // добавить друга.
    $('.btn-submit').on('click', function () {
        let friendId = $(this).parent().children('input').val();
        console.log(friendId);

        $.ajax({

            url: '/Api/AddFriend',
            data: {
                userId: friendId
            },
            success: function (responce) {
                
            },
            fail: function () {
                console.log("ОШИБКА ЗАГРУЗКИ");
            }
        });
        $(this).text('Добавлен(а)');
    });

    // Поиск из списка друзей, по части имени.
    $(document).on('keyup', function () {
        if ($('#fr-filter').is(':focus')) {
            searchFriend();
        }
    });
    $('.fr-filter-clear').click(function () {
        clearSearch();
    });
    function clearSearch() {
        $('#fr-filter').val('');
        $('.friend-list-container').removeClass('display-none');
    }
    function searchFriend() {
        let searchText = $('#fr-filter').val();
        if (searchText == '') {
            $('.friend-list-container').removeClass('display-none');
            return;
        }
        $('.friend-list-container').addClass('display-none');
        let opps = $(".friend-list-container:contains(" + searchText + ")");
        $(opps).removeClass('display-none');
        return;
    };

    // блокировка пользователя
    $('#btn-block').click(function (event) {
        event.preventDefault();
        // Данные запроса
        let userid = $('#user-id').val();
        let friendid = $('#friend-id').val();
        // Запрос
        let req = $.ajax({
            url: '/Api/BlockUser',
            data:
            {
                currentUserId: userid,
                friendId: friendid
            }
        });
        // в зависмости от результата запроса 
        req.then(function (data, statusText, jqXHR) {
            if (jqXHR.status == 200) {
                // от вида кнопки сменить стили и надписаь
                if ($('#btn-block').hasClass('form-submit')) {
                    $('#btn-block').toggleClass('form-submit');
                    $('#btn-block').toggleClass('red-btn-alert');
                    $('#btn-block').text('Заблокирован');
                }
                else {
                    // просто в обратном поярдке.
                    $('#btn-block').toggleClass('form-submit');
                    $('#btn-block').toggleClass('red-btn-alert');
                    $('#btn-block').text('Заблокировать');
                }
            }
            else {
                // Предупереждение об ошибке.
                if (jqXHR.status == 400)
                    alert('Что-то пошло не так(');
                if (jqXHR.status == 205)
                    alert('Разблокировать может только оппонент');
            }
        });
    });

    // Согласиться принять в друзья.
    //agree-friend
    $('#agree-friend').click(function (event)
    {
        event.preventDefault();
        let id = $(this).parents('.friend-list-container').children('#friend-entity-id').val();
        let button = $(this);

        let req = $.ajax({
            url: '/Api/SubmitFriend',
            data: {
                friendEntityId:id
            }
        });
        req.then(function (data, stat,jqXHR) {
            if (jqXHR.status = 200) {
                button.removeClass('form-submit');
                button.text('Добавлен(а)');
            }
            else
            {
                alert('Что-то пошло не так(');
            }
        });
    });

    // Нажатие на кнопку заявки.
    $('.orders').click(function ()
    {
        if ($(this).text() == 'заявки') {
            $('.friend-list-container').slideUp();
            $(this).text('все');
            $('#agree-friend').parents('.friend-list-container').slideDown();
            clearSearch();
        }
        else {
            $('.friend-list-container').slideDown();
            $(this).text('заявки');
            clearSearch();
        }
        
    });

    // Предотвращение нажатия кнопы.
    $('.red-btn-alert').click(function (event)
    {
        event.preventDefault();
    });

    // События страницы userInfo
    // переключение по вкладкам
    $('.fr-selector').click(function ()
    {
        // Селекторы меняем цветами.
        $('.fr-selector').removeClass('fr-checked');
        $(this).addClass('fr-checked');
        $('.content').addClass('display-none');
        let selector = $(this).prop('id');
        console.log(selector);
        // В зависимости от того чт нажали добавляем удаляем класс видимости.
        if (selector == 'will-go') {
            $('.go-content').removeClass('display-none');
        }
        if (selector === 'created') {
            $('.create-content').removeClass('display-none');
        }
        if (selector == 'friends') {
            $('.friends-content').removeClass('display-none');
        }
        if (selector == 'photos') {
            $('.photos-content').removeClass('display-none');
        }
    });




});
