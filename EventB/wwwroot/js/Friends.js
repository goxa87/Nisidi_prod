import { renderMessage, getModelWindow, iensSearchByText, GetNotification } from './Controls.js';
import { GetBanner } from './Banner.js';

$(document).ready(function ()
{
    // добавить друга.
    $('.add-as-friend').on('click', function () {
        let friendId = $(this).parent().children('input').val();
        console.log(friendId);

        $.ajax({
            url: '/Friends/AddFriend',
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
            let searchText = $('#fr-filter').val();
            let items = $('.friend-list-container');
            iensSearchByText(items, '.central-title', searchText);
        }
    });
    $('.s-filter-clear').click(function () {
        $('#fr-filter').val('');
        $('.friend-list-container').removeClass('display-none');
    }); 

    // блокировка пользователя
    $('.btn-block').click(function () {
        let friendid = $(this).parent().children('#friend-entity-id').val();
        let req = $.ajax({
            url: '/Api/BlockUser',
            data:
            {
                friendId: friendid
            }
        });
        req.then((data, statusText, jqXHR) => {
            if (jqXHR.status == 200) {
                // от вида кнопки сменить стили и надписаь
                if ($(this).hasClass('action-button')) {
                    $(this).toggleClass('action-button');
                    $(this).toggleClass('red-btn-alert');
                    $(this).text('Заблокирован');
                }
                else {
                    // просто в обратном поярдке.
                    $(this).toggleClass('action-button');
                    $(this).toggleClass('red-btn-alert');
                    $(this).text('Заблокировать');
                }
            }
            else {
                // Предупереждение об ошибке.
                if (jqXHR.status == 400)
                    alert('Что-то пошло не так(');
                if (jqXHR.status == 205) {
                    var content = '<p>Разблокировать может только оппонент</p>'
                    getModelWindow('Невозможно разблокировать.', false);
                    $('.modal-body').html(content);
                }
            }
        });
    });
    // Удаление друга
    $('.delete-friend').click(function () {
        let friendid = $(this).parent().children('#friend-entity-id').val();
        let req = $.ajax({
            url: '/Api/DeleteFriend',
            data:
            {
                friendId: friendid
            }
        });
        req.then((data, statusText, jqXHR) => {
            if (jqXHR.status == 200) {
                $(this).toggleClass('action-button');
                $(this).toggleClass('red-btn-alert');
                $(this).text('Удален');
                $(this).removeClass('delete-friend');
            }
            else {
                // Предупереждение об ошибке.
                if (jqXHR.status == 400)
                    alert('Что-то пошло не так( Обратитесь в поддержку.');
            }
        });
    });
    function clearSearch() {
        $('#fr-filter').val('');
    }
    // Нажатие на кнопку заявки.
    $('.orders').click(function ()
    {
        if ($(this).text() == 'заявки') {
            $('.friend-list-container').slideUp();
            $(this).text('все');
            $('.agree-friend').parents('.friend-list-container').slideDown();
            clearSearch();
        }
        else {
            $('.friend-list-container').slideDown();
            $(this).text('заявки');
            clearSearch();
        }
        
    });

    // Предотвращение нажатия кнопы.
    $('.red-btn-alert').click(function (event) {
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
    });

    // Поиск из недрузей глобально (модальное окно)
    $('.fr-search-modal').click(function () {
        let content =`
            <div>
                <label class="form-label">Имя:</label><br>
                <input id="fr-search-name" class="form-entry-title" type="text" /><br>

                <label class="form-label">Теги (если несколько, то поиск только для первого):</label><br>
                <input id="fr-search-teg" class="form-entry-title" type="text"/><br>

                <label class="form-label">Город (полностью, если не указан, будет поиск для вашего города)</label><br>
                <input id="fr-search-city" class="form-entry-title" type="text"/><br>
            </div>  
            `
        ;
        getModelWindow('Параметры для поиска пользователей', true, getUsersFriends, null, "Поиск среди всех пользователей.");
        $('.modal-body').html(content);
        
    });

    function getUsersFriends() {
        // Собирание данных
        let nameThis = $('#fr-search-name').val();
        let cityThis = $('#fr-search-city').val();
        let tegThis = $('#fr-search-teg').val();
        let url = `/Friends/SearchFriend?name=${nameThis}&city=${cityThis}&teg=${tegThis}`;
        window.location.replace(url);
    }

});

function GetFriendBanner() {
    //...
    GetBanner("Friend.cshtml", "friend-banner");
}
