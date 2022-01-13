import { renderMessage, getModelWindow, iensSearchByText, GetNotification } from './Controls.js';
import { GetBanner } from './Banner.js';
import { GetCookieByName, UpdateCookieValue, TryRemoveCookie } from './CookieService.js';

var IsAuthorizedUser = true;
const INTERES_COOKIE_NAME = 'intereses';
// Начало исполнения
$(document).ready(function ()
{
    // Флаг прекращения загрузки.
    var dynamicLoadStopper = true;
    var block = false;
    // Сразу вызов для загрузки.
    if ($('#end-marker').length) {
        prepareDynamicLoad(); 
    }

    GetBanner("Hello.cshtml", "ev-hello-banner");
    InitStartLocalData();

    // Start постранично при нажатии загрузить еще.
    $('#ev-load-more').click(function () {        
            if (dynamicLoadStopper == true) {
                if (block == false) {
                    block == true;
                    $('#noTrespassingOuterBarG').removeClass('display-none');
                    $('#ev-load-more').addClass('display-none');
                    prepareDynamicLoad();
                }
            }
        });
    // Формирование аргументов и вызов.
    function prepareDynamicLoad() {
        $('#noTrespassingOuterBarG').removeClass('display-none');
        $('#flag-dwnl-more').val('false');
        var args = {
            UserId: $('#args-user-id').val(),
            Title: $('#args-title').val(),
            City: $('#args-city').val(),
            Tegs: $('#args-teg').val(),
            DateSince: $('#args-date-s').val(),
            DateDue: $('#args-date-e').val(),
            Skip: $('#args-skip').val(),
            Take: $('#args-take').val()
        }
        
        LoadEvents(args);
    }
    // Непосредственно загрузка контента и рассувать по ДОМ
    function LoadEvents(args)
    {
        var responce = $.ajax({
            url: '/Events/LoadDynamic',
            data: args
        });

        responce.then(function (data, stat, jqXHR) {
            if (jqXHR.status == 200) {
                $('#event-list').append(data);
                $('#args-skip').val(Number($('#args-skip').val()) + Number($('#args-take').val()));
                block = false;
                $('#noTrespassingOuterBarG').addClass('display-none');
                $('#ev-load-more').removeClass('display-none');
            } else if (jqXHR.status == 206) {
                $('#event-list').append(data);
                block = false;
                $('#noTrespassingOuterBarG').addClass('display-none');
            }
            else {
                dynamicLoadStopper = false
                block = false;
                $('#noTrespassingOuterBarG').addClass('display-none');
            }            
        });
    }
    // DETAILS
    // Загрузка истории изменений и сообщений автоматически
    if ($('#ed-changes').length) {
        let eve = $('#event-id').val();
        var user = $('#user-id').val();
        $.ajax({
            url: '/Evetns/GetEventMessages',
            data: {
                eventId: eve
            },
            success: function(data){
                // Загрузка в изменения
                let changes = data.filter(e=> e.eventState == true )
                let block = renderMessage(changes, user);
                $('#ed-changes').html(block);
                // Загрузка в комментарии
                block = renderMessage(data, user);
                $('.ed-message-list').html(block);
            }
        });
    }
    // клик на элементе меню внизу страницы
    $('#btn-changes').click(function ()
    {
        $('.bottom-menu-item').removeClass('bottom-menu-item-selected');
        $(this).addClass('bottom-menu-item-selected');
        $('.bottom-page').addClass('display-none');
        $('#ed-changes').removeClass('display-none');
    });
    $('#btn-chat').click(function () {
        $('.bottom-menu-item').removeClass('bottom-menu-item-selected');
        $(this).addClass('bottom-menu-item-selected');
        $('.bottom-page').addClass('display-none');
        $('#chat').removeClass('display-none');
    });
    $('#btn-vizitors').click(function () {
        $('.bottom-menu-item').removeClass('bottom-menu-item-selected');
        $(this).addClass('bottom-menu-item-selected');
        $('.bottom-page').addClass('display-none');
        $('#vizitors').removeClass('display-none');
    });
          
    // Отправить сообщение.
    $('#btn-send').click(function (event)
    {
        event.preventDefault();
        if ($('#user-id').val() == '0') {
            alert('Авторизуйтесь для отправки сообщений');
            return;
        }

        if ($('#chat-id').val() == '0') {
            // создаем чат отправляем сообщение(включает отправку)
            $('.message-item').empty();
        }
        else
        {
            //Отправляем сообщение
            SendMessage();
        }
    });

    // Отправка сообщения
    function SendMessage()
    {
        let user = $('#user-id').val();
        let chat = $('#chat-id').val();
        let messageText = $('#text').val();
        let name = $('#user-name').val();

        if (messageText == '') return;

        $.ajax({
            url: '/Events/SendMessage',
            data:
            {
                userId: user,
                userName: name,
                chatId: chat,
                text: messageText
            },
            success: function () {
                $('#text').val('');
                AddMessageToListMessage(messageText)
            },
            error: function () {
                GetNotification('Отправка сообщений в этот чат заблокирована администратором чата', 1, 3);
            }
        });

    }
    // Добавление элемента в дом (сообщение чата).
    function AddMessageToListMessage(text1)
    {
        let date1 = new Date();
        let userId = $('#user-id').val();
        let data = { personId: userId, text: text1, postDate: date1, eventState:false, eventLink:false }
        let block = renderMessage(data, userId);
        $('.ed-message-list').prepend(block);

    }
    
    // Нажатие кнопки пойду
    $('#btn-come').click(function ()
    {
        let id = $('#event-id').val();
        let button = $('#btn-come');
        $.ajax({
            url: '/Event/SubmitToEvent',
            data:
            {
                eventId: id
            },
            success: function (ans) {
                console.log(ans)
                if (ans.isSuccess) {
                    // Состояние проверяем по классу.
                    if (button.hasClass('calltoaction-btn')) {

                        button.removeClass('calltoaction-btn');
                        button.addClass('calltoaction-prime-btn');
                        button.text('Подтвердить участие');
                        button.siblings().toggleClass('display-none');
                        GetNotification('Отметка о визите удалена', 3, 3)
                        // Отписаться.
                    }
                    else {
                        button.removeClass('calltoaction-prime-btn');
                        button.addClass('calltoaction-btn');
                        button.text('Отменить участие');
                        button.siblings().toggleClass('display-none');
                        GetNotification('Отметка о визите сохранена', 3, 3)
                        // Подписаться.
                    }
                } else {
                    getModelWindow("Действие недоступно", false, null, null, null);
                    let content = '<h3>Необходима авторизация</h3><p> Для успешного выполнения этого действия необходимо войти в систему.<br> Если ты еще не зарагистрирован, то ты можешь сделать это перейдя по <a href="/Account/Register">ССЫЛКЕ</a></p>';
                    $('.modal-body').html(content);
                }
            },
            error: function () {
                GetNotification("Что-то пошло не так(", 1, 2);
                }
        });
    });

    // Секция Пориглашения.
    // Запрос с сервера на получение данных д доступных для приглашения друзьях.
    function GetFriends()
    {
        let eve = $('#event-id').val();
        $.ajax({
            url: '/Events/GetFriendslist',
            data:
            {
                eventId: eve
            },
            success: function (ans) {
                if (ans.isSuccess) {
                    let data = ans.content;
                    // Парс в HTML списка друзей доступных для приглашения
                    var block = '';
                    $(data).each(function (i, v) {
                        block +=
                            `<div class="invite-item flex-hsbc">
                        <img src="${v.photo}" />
                        <div class="invite-data">
                            <div class="flex-hsbc flex-wr-reverse">
                                <div>Текст приглашения</div>
                                <div class="invite-title flex-hec">
                                    <h5>${v.name}</h5>
                                    <input class="inv-button" type="button" title="Отметить" />
                                </div>
                            </div>                            
                            <textarea class="ev-d-invite-text"></textarea>
                            <input type="hidden" id="friend-id" value="${v.userId}"/>
                        </div>
                </div>`
                    });
                    let helpText = `
В этом окне Вы можете пригласить Ваших друзей на событие. Здесь отображаются только те пользователи, которые подтвердили добавление в друзья.
Для выбора пользователей, которым будет отправлено приглашение, нажмите на кружок рядом с именем, что бы тот стал жёлтым.
Вы можете указать текст приглашения длинной до 1000 символов. <br>Также есть возможность скопировать текст приглашения от порвого ко всем остальным пользователям и отметить или снять выделение со всех 
пользователей, которым будет отправлено приглашение.<hr> Если вы не нашли нужных пользователей, то либо они не приняли вашу заявку в друзья, либо у них уже есть приглашение на это событие.
`
                    getModelWindow('Пригласить', false, onInviteClick, null, helpText);
                    let firstBlock = `
                <div class="invite-menu">
                    <div class="form-submit inline" id="invite-copy" title="Скопировть текст из первого сообщения пользователю ко всем пользователям">Скопировать текст</div>
                    <div class="form-submit inline" id="inv-selectall">Отметить всех</div>
                    <div class="form-submit inline" id="inv-invite">Пригласить</div>
                </div>
                <div class="invite-list">
                </div>`
                    $('.modal-body').html(firstBlock);
                    $('.invite-list').html(block);
                } else {
                    getModelWindow("Действие недоступно", false, null, null, null);
                    let content = '<h3>Необходима авторизация</h3><p> Для успешного выполнения этого действия необходимо войти в систему.<br> Если ты еще не зарагистрирован, то ты можешь сделать это перейдя по <a href="/Account/Register">ССЫЛКЕ</a></p>';
                    $('.modal-body').html(content);
                }
            },
            error: function () {
                GetNotification("Что-то пошло не так(", 1, 2);
            }
        });
    }
    // Нажатие на форме детали кнопки пригласить.
    $('#btn-invite').click(function ()
    {
        // Получение с сервера InviteOutVM списка.
        GetFriends();       
    });
    // Нажатие галочки напротив имени.
    $('body').on('click', '.inv-button', function () {
        $(this).toggleClass('checked-inv');
    });
    // Выбрать всех кнопка.
    $('body').on('click', '#inv-selectall', function () {
        if ($('.inv-button').hasClass('checked-inv'))
            $('.inv-button').removeClass('checked-inv');
        else
            $('.inv-button').addClass('checked-inv');
    });
    // Скопировать текст приглашения.
    $('body').on('click', '#invite-copy', function () {
        let text1 = $('.ev-d-invite-text:first').val();
        $('.ev-d-invite-text').val(text1);
    });
    // Кнопка пригласить всех. InviteFriendsIn(int eventId, InviteInVm[] invites)
    $('body').on('click', '#inv-invite', function () {
        onInviteClick();
    });

    // Секция Отправить ссылку в чат.
    $('#btn-details-send-link').click(function () { 
        getChats();
    })
    //Запрос и вставка чатов
    function getChats() {
        //var rez;
        $.ajax({
            url: '/Event/GetAvailableChats',
            success: function (ans) {
                let content = `<div class="s-filter-container">
                    <span class="small-label">Фильтр</span>
                    <div class="flex-hsbc">                            
                        <input id="ev-send-link-filter" class="s-filter" />
                        <img src="/resourses/cancel.png" class="s-filter-clear" />
                    </div>
                </div>` + ans;
                getModelWindow('Отправить ссылку', false, null, null, "Вы можете отправить ссылку на собыие собеседнику. В списке отображаются активные приватные чаты. Если вы не видите нужный вам чат, начните его отправив любое сообщение.");
                $('.modal-body').html(content);               
            },
            error: function(){
                getModelWindow("Действие недоступно", false, null, null, null);
                let content = '<h3>Необходима авторизация</h3><p> Для успешного выполнения этого действия необходимо войти в систему.<br> Если ты еще не зарагистрирован, то ты можешь сделать это перейдя по <a href="/Account/Register">ССЫЛКЕ</a></p>';
                $('.modal-body').html(content);
            }
        });
    }
    // поиск на отправке ссылки
    $(document).on('keyup', function () {
        if ($('#ev-send-link-filter').is(':focus')) {
            let searchText = $('#ev-send-link-filter').val();
            let items = $('.item-element-user-chat-small');
            iensSearchByText(items, '.ev-link-title', searchText);
        }
    });
    $('body').on('click', '.s-filter-clear', function () {
        $('#ev-send-link-filter').val('');
        $('.item-element-user-chat-small').removeClass('display-none');
    }); 

    // Скрытие по серому полю.
    $('#ev-over-link').on('click', function (event) {
        $(this).css('display', 'none');
    });
    $('.ev-link-cont').click(function (e) {
        e.stopPropagation();
    });
    // Нажатие на кнопу отправить
    $('body').on('click', '.ev-link-send-btn', function () {
        var evId = $('#event-id').val();
        var chatId = $(this).parent().children('.ev-link-chat-id').val();
        $.ajax({
            url: '/Event/SendLink',
            data: {
                eventId: evId,
                userChatId: chatId
            }, success: function () {
                $('#ev-over-link').css('display', 'none');
                $('.modal-shadow').remove();
                GetNotification('ССЫЛКА ОТПРАВЛЕНА', 3, 3);
            }, error: () => alert('Что-то пошло не так(') 
        })
    });

    // Поиск по чатам
    // Динамический поиск.
    $(document).on('keyup', function () {
        if ($('#ev-txt-search').is(':focus')) {
            searchOpponent();
        }
    });

    // Отображение чатов результатам поиска.
    function searchOpponent() {
        // получить список того что в опп лист
        // добавить класс дисплей none тем кто не подходит
        let searchText = $('#ev-txt-search').val();
        console.log(searchText)
        if (searchText == '') {
            $('.chat-list-small').removeClass('display-none');
            return;
        }
        $('.chat-list-small').addClass('display-none');
        let opps = $(".chat-list-small:contains(" + searchText + ")");
        $(opps).removeClass('display-none');
    };

    //Беннер рекомендации
    /** Закрыть баннер рекомендации*/
    $('.bnrre-close').click(function () {
        $('.bnrre-body').slideUp(400);
    });

    /** Удалить ТЕГ для регистрированного пользователя */
    $('.bnrre-delete-teg').click(function () {
        let Teg = $(this).parent().children('.bnrre-interers-value').text();
        console.log('newT', Teg);
        if (IsAuthorizedUser) {
            DeleteTegForAuthUser(Teg, $(this));
        } else {
            DeleteTegForNotAuthUser(Teg, $(this));
        }
    });

    /** Добавить ТЕГ для регистрированного пользователя */
    $('.bnrre-add-teg').click(function () {
        let tegValue = $('.bnrre-input').val();

        if (tegValue == undefined || tegValue == '')
            return;

        if (IsAuthorizedUser) {
            AddTegForAuthUser(tegValue);
        } else {
            AddTegForNotAuthUser(tegValue);
        }
    });

    /**Выполнить фильтрацию */
    $('.bnrre-btn-filter').click(function () {
        if (IsAuthorizedUser) {
            $.ajax({
                url: '/Events/GetLinqToFilterByTegs',
                success: function (data) {
                    window.location.replace(data);
                },
                error: function () {
                    console.log('ошибка перехода по рекомендациям')
                }
            })
        } else {
            let cookie = GetCookieByName(INTERES_COOKIE_NAME);
            let city = $('#args-city').val().toUpperCase();
            let newUrl = '/Events/SearchEventlist?City=' + city + '&Tegs=' + cookie + '&Skip=0&Take=30';
            window.location.replace(newUrl);
        }
    });

    //Добавление
    // Превью для картинки
    $('#eve-add-image').change(function () {
        var input = $(this)[0];

        if (input.files && input.files[0]) {
            if (input.files[0].type.match('image.*')) {
                var reader = new FileReader();
                reader.onload = function (e) { $('#eve-add-image_preview').attr('src', e.target.result); }
                reader.readAsDataURL(input.files[0]);
            }
        }
    });

    // заголовок
    $('#eve-add-title').change(function () {
        $('#eve-add-title_preview').text($(this).val());
    });

    // Дата
    $('#eve-add-date').change(function () {
        let dateTime = $(this).val();
        let dateDouble = dateTime.split('T');
        let date = dateDouble[0].split('-')[2] + '.' + dateDouble[0].split('-')[1];
        $('#eve-add-date_preview').text(date);
        let time = dateDouble[1];
        $('#eve-add-time_preview').text(time);
    });

    // Кнопка "сохранить" пропадает после нажатия по ней, и на её месте появляется загрузка 
    // при условии, что поля заполнены верно
    $('#save-btn').click(function () {
        var titleLength = $('#eve-add-title').val().length;
        var bodyLength = $('#eve-add-body').val().length;
        var tegsLength = $('#eve-add-tegs').val().length;
        var cityLength = $('#eve-add-city').val().length;
        var placeLength = $('#eve-add-place').val().length;
        var dateIsPicked = $('#eve-add-date').val();
        var ticketsDescLength = $('#eve-add-ticketsDesc').val().length;
        var phoneLength = $('#eve-add-phone').val().length;

        if (titleLength == 0 || titleLength > 1000) return;
        if (bodyLength > 4000) return;
        if (tegsLength > 1000) return;
        if (cityLength == 0 || cityLength > 100) return;
        if (placeLength == 0 || placeLength > 200) return;
        if (!dateIsPicked) return;
        if (ticketsDescLength > 1000) return;
        if (phoneLength > 25) return;
        else {
            $(this).addClass('display-none');

            if ($('#noTrespassingOuterBarG').hasClass('display-none')) {
                $('#noTrespassingOuterBarG').removeClass('display-none');
            }
        }
    });
});

/**
 * Вернет параметры поиска событий
 * */
function getSearchParams() {
    return {
        Title: $('#search-param-title').text(),
        Tegs: $('#search-param-tegs').text(),
        City: $('#search-param-city').text()
    }
}

/**Пригласить друщей (в содержимоми на ОК) */
function onInviteClick() {
    var ids = [];
    $('.checked-inv').each(function (i, v) {
        let id = $(v).parents('.invite-data').children('#friend-id').val();
        let mess = $(v).parents('.invite-data').children('.ev-d-invite-text').val();
        let inv = { userId: id, message: mess };
        ids.push(inv);
    });
    let eventid = $('#event-id').val();
    $.ajax({
        type: 'post',
        url: '/Events/InviteFriendsIn',
        data:
        {
            eventId: eventid,
            invites: ids
        }, success: () => {
            GetNotification('Приглашения отправлены', 3, 3)
            $('.modal-shadow').remove();
        }
    });
}

/**Инициализация локальных данных */
function InitStartLocalData() {
    if ($('#bnrre-user-id').val() == undefined || ($('#bnrre-user-id').val() == '')){
        IsAuthorizedUser = false;
    } else {
        IsAuthorizedUser = true;
    }
    var intereses = GetCookieByName(INTERES_COOKIE_NAME);
    if (intereses) {
        var interesArray = intereses.split('@');
        if (interesArray.length > 0) {
            interesArray.forEach(function (element) {
                if (element && element.length > 0) {
                    RenderTegToBunner(element);
                }
            });
        }
    }
}

/**
 * Удаление тега у авторизованного пользователя
 * @param {any} value
 * @param {any} theThis
 */
function DeleteTegForAuthUser(value, theThis) {
    $.ajax({
        url: '/Account/DeleteUserInteres?value=' + value,
        success: (data) => {
            if (data.IsSuccess == false) {
                console.log(data.ErrorMessage);
            }
            $(theThis).parent().hide();
        },
        error: () => {
            console.log('Ошибка отправка запроса на удаление тега');
        }
    });
}


/**
 * Удаление тега у НЕ авторизованного пользователя
 * @param {any} value
 * @param {any} theThis
 */
function DeleteTegForNotAuthUser(value, theThis) {
    let cookie = GetCookieByName(INTERES_COOKIE_NAME);
    console.log('coockМ', value);
    console.log('coock1', cookie);
    if (cookie.indexOf(value) !== -1) {
        var source = cookie.split('@');
        cookie = '';
        source.forEach(function (element) {
            if (element.length > 0 && element != value) {
                cookie += (cookie.length == 0 ? '' : '@') + element;
            }
        });
        console.log('New ccok1', cookie);
        UpdateCookieValue(INTERES_COOKIE_NAME, cookie);
        $(theThis).parent().hide();
    }
}

/**
 * Добавить тег для авторизованного пользователя
 * @param {any} value
 */
function AddTegForAuthUser(value) {
    $.ajax({
        url: '/Account/SaveUserInteres?value=' + value,
        success: (data) => {
            if (data.IsSuccess == false) {
                console.log(data.ErrorMessage);
            } else {
                $('.bnrre-input').val('');
                RenderTegToBunner(value);
            }
        },
        error: () => {
            console.log('Ошибка отправка запроса на удаление тега');
        }
    });
}

/**
 * Сохранить тег для НЕ авторизованног о пользователя
 * @param {any} value
 */
function AddTegForNotAuthUser(value) {

    let newString = value.replace(" ", "@");
    let arr = newString.split("@");
    console.log('arr', arr);
    let cookie = GetCookieByName(INTERES_COOKIE_NAME);
    if (!cookie) {
        cookie = '';
    }
    console.log('cookie', cookie);
    arr.forEach(function (value, index) {
        var arrayOfInteres = cookie.split('@')
        if (arrayOfInteres.indexOf(value) == -1) {
            cookie += (cookie.length > 0 ? '@' : '') + value;
        }
    });

    UpdateCookieValue(INTERES_COOKIE_NAME, cookie);
    $('.bnrre-input').val('');
    RenderTegToBunner(value);
}

/**
 * Срендерит новый блок тега в блок с тегами
 * @param {any} value
 */
function RenderTegToBunner(value) {
    var block = '<div class="bnrre-teg"><div class="bnrre-interers-value">' + value + '</div>&nbsp;<span class="material-icons bnrre-delete-teg bnrre-delete-teg">close</span ></div>';
    $('.bnrre-teg-container').append(block);
}