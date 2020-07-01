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
    $('.btn-block').click(function () {
        let thisCard = $(this).parents('.friend-list-container');
        // Данные запроса
        let friendid = $(thisCard).children('#friend-entity-id').val();
        // Запрос
        let req = $.ajax({
            url: '/Api/BlockUser',
            data:
            {
                friendId: friendid
            }
        });
        // в зависмости от результата запроса 
        req.then((data, statusText, jqXHR) => {
            if (jqXHR.status == 200) {
                // от вида кнопки сменить стили и надписаь
                if ($(this).hasClass('action-button')) {
                    console.log('1')
                    $(this).toggleClass('action-button');
                    $(this).toggleClass('red-btn-alert');
                    $(this).text('Заблокирован');
                }
                else {
                    // просто в обратном поярдке.
                    console.log('2')
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
        let thisCard = $(this).parents('.friend-list-container');
        // Данные запроса
        let friendid = $(thisCard).children('#friend-entity-id').val();
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

    function getUsersFriends() {
        // Собирание данных
        let nameThis = $('#fr-search-name').val();
        let cityThis = $('#fr-search-city').val();
        let tegThis = $('#fr-search-teg').val();
        // Запрос
        $.ajax({
            url: '/Friends/SearchFriend',
            data: {
                name: nameThis,
                city: cityThis,
                teg: tegThis
            }, success: (data) => {
                // Рендеринг
                displayFriendSearch(data)
            }
        });       
        
    }

    // Поиск
    $('.fr-search-modal').click(function () {
        if ($(this).text() === 'ПОИСК') {
            
            let content =`
                <div>
                    <label class="form-label">Имя:</label><br>
                    <input id="fr-search-name" class="form-entry-title" type="text" /><br>

                    <label class="form-label">Теги (если несколько, то поиск только для первого):</label><br>
                    <input id="fr-search-teg" class="form-entry-title" type="text"/><br>

                    <label class="form-label">Город (полностью, если не указан, будет поиск для вашего города)</label><br>
                    <input id=" fr-search-city" class="form-entry-title" type="text"/><br>
                </div>  
                `
            ;
            getModelWindow('Параметры для поиска пользователей', true, getUsersFriends, cancelModal);
            $('.modal-body').html(content);
        } else {
            $(this).text('ПОИСК');
            $('.fr-current-friends').removeClass('display-none');
            $('.fr-current-friends').addClass('flex-hsac');
            $('.fr-filter-container').removeClass('display-none');
            $('.fr-search-list').addClass('display-none');
            $('.fr-search-list').removeClass('flex-hsc');
        }

    });
    // Вставка результата запроса
    function displayFriendSearch(friends) {
        $('.fr-current-friends').addClass('display-none');
        $('.fr-current-friends').removeClass('flex-hsac');
        $('.fr-filter-container').addClass('display-none');
        $('.fr-search-list').removeClass('display-none');
        $('.fr-search-list').addClass('flex-hsc');
        $('.fr-search-list').html(friends);
    }
    function cancelModal() {
        $(this).text('ПОИСК');
    }

});
