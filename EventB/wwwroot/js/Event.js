// Начало исполнения
$(document).ready(function ()
{
    
    // Флаг прекращения загрузки.
    var dynamicLoadStopper = true;
    var block = false;
    // Сразу вызов для загрузки.
    prepareDynamicLoad();    
    // Start постранично при скролле.
    $(window).scroll($.throttle(500,true,  function () {     
            if (dynamicLoadStopper == true) {
                if (block == false) {
                    $('#noTrespassingOuterBarG').removeClass('display-none');
                    block == true;
                    var endMarker = $('#end-marker').offset().top;
                    var currentPosition = $(this.window).scrollTop() + $(this.window).height();
                    if (endMarker < currentPosition) {
                        prepareDynamicLoad();
                    }
                }
            }
        }  )
    );
    // Формирование аргументов и вызов.
    function prepareDynamicLoad() {
        $('#noTrespassingOuterBarG').removeClass('display-none');
        $('#flag-dwnl-more').val('false');
        var args = {
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
            }
            else {
                dynamicLoadStopper = false
                block = false;
                $('#noTrespassingOuterBarG').addClass('display-none');
            }            
        });
    }
    // DETAILS
    
    // клик на элементе меню внизу страницы
    $('#btn-changes').click(function ()
    {
        // bottom-menu-item-selected
        $('.bottom-menu-item').removeClass('bottom-menu-item-selected');
        $(this).addClass('bottom-menu-item-selected');

        $('.bottom-page').addClass('display-none');
        $('#changes').removeClass('display-none');
    });
    $('#btn-chat').click(function () {
        // bottom-menu-item-selected
        $('.bottom-menu-item').removeClass('bottom-menu-item-selected');
        $(this).addClass('bottom-menu-item-selected');

        $('.bottom-page').addClass('display-none');
        $('#chat').removeClass('display-none');
    });
    $('#btn-vizitors').click(function () {
        // bottom-menu-item-selected
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
            alert('Авторизуйтесь для отправки сооsбщений');
            return;
        }

        if ($('#chat-id').val() == '0') {
            // создаем чат отправляем сообщение(включает отправку)
            $('.message-item').empty();
            CreateChat();
        }
        else
        {
            //Отправляем сообщение
            SendMessage();
        }
    });

    // [Route("CreateEventChat")]
    // [Authorize]
    //    public async Task < int > CreateEventChat(int eventId, string userId)
    //  создать чат для события
    // Создать чат.
    function CreateChat()
    {
        let user = $('#user-id').val();
        let event = $('#event-id').val();

        $.ajax({
            url: '/Events/CreateEventChat',
            data:
            {
                eventId: event,
                userId: user
            }, complete: function (responce)
            {
                $('#chat-id').val(responce.responseJSON);
                SendMessage();
            }
        });

    }
    
    // [Route("SendMessage")]
    //    public async Task SendMessage(string userId,string userName, int chatId, string text)

    // Отправка сообщения
    function SendMessage()
    {
        let user = $('#user-id').val();
        let chat = $('#chat-id').val();
        let message = $('#text').val();
        let name = $('#user-name').val();

        if (message == '') return;

        $.ajax({
            url: '/Events/SendMessage',
            data:
            {
                userId: user,
                userName: name,
                chatId: chat,
                text: message
            },
            success: function () {
                $('#text').val('');
                AddMessageToListMessage(message);
            }
        });

    }
    // Добавление элекмента в дом (сообщение чата).
    function AddMessageToListMessage(text)
    {
        let date = Date();
        let block ='<div class="message-item">'+
            '<div class="message-sender">Вы</div>'+
            '<div class="message-text">' + text + '</div>' +
            '<div class="message-date">' + date + '</div></div>';
        $('.message-list').prepend(block);

    }
    
    // Нажатие кнопки пойду
    $('#btn-come').click(function ()
    {
        let id = $('#event-id').val();

        let user = $('#user-id').val();
        console.log(`event ${id} user ${user}`);

        //[Route("/Event/SubmitToEvent")]
        // public async Task < StatusCodeResult > SubmitToEvent(int eventId)
        let req = $.ajax({
            url: '/Event/SubmitToEvent',
            data:
            {
                eventId: id
            }           
        });

        req.then(function (data, statusText,jqXHR)
        {
            let button = $('#btn-come');
            if (jqXHR.status == 200) {
                // Состояние проверяем по классу.
                if (button.hasClass('form-search-fade')) {

                    button.removeClass('form-search-fade');
                    button.addClass('form-submit');
                    button.text('Подтвердить участие');
                    button.siblings().toggleClass('display-none');
                    // Отписаться.
                }
                else {
                    button.removeClass('form-submit');
                    button.addClass('form-search-fade');
                    button.text('Отменить участие');
                    button.siblings().toggleClass('display-none');
                    // Подписаться.
                }
            }
            else {
                alert('Ошибка БД( Событие не найдено.');
            }

        });
    });

    // Секция Пориглашения.
    // Запрос с сервера на получение данных д доступных для приглашения друзьях.
    function GetFriends()
    {
        let eve = $('#event-id').val();
        var rez;
        var req = $.ajax({
            url: '/Events/GetFriendslist',
            data:
            {
                eventId: eve
            }
        });
        req.then(function (data, stat, jqXHR)
        {
            // Парс в HTML
            var block = '';
            $(data).each(function (i, v) {
                block +=
                    `<div class="invite-item flex-hsbc">
                <img src="${v.photo}" />
                <div class="invite-data">
                    <div class="invite-title flex-hec">
                        <h5>${v.name}</h5>
                        <input class="inv-button" type="button" title="Отметить" />
                    </div>
                    <textarea id="text"></textarea>
                    <input type="hidden" id="friend-id" value="${v.userId}"/>
                </div>
            </div>`
            });
            $('.invite-list').append(block);
        });
        return rez;
    }
    // Нажатие на форме детали кнопки пригласить.
    $('#btn-invite').click(function ()
    {
        // Получение с сервера InviteOutVM списка.
        $('.over-cont').css('display', 'flex');
        let friends = GetFriends();       
    });

    // Скрытие списка по нажатии на серое поле.
    $('.over-cont').on('click' ,function (event) {
        
        $(this).css('display', 'none');
    });
    $('.invite-cont').click(function (e) {
        e.stopPropagation();
    });
    // Выбрать всех кнопка.
    $('#inv-selectall').click(function () {
        if ($('.inv-button').hasClass('checked-inv'))
            $('.inv-button').removeClass('checked-inv');
        else
            $('.inv-button').addClass('checked-inv');
    });
    // Скопировать текст приглашения.
    $('#invite-copy').click(function () {
        let text1 = $('#text:first').val();
        console.log(text1);
        alert('not working');
    });

    // Нажатие пригласить в темном контейнере.
    // InviteFriendsIn(int eventId, InviteInVm[] invites)
    $('#inv-invite').click(function ()
    {
        var ids = [];

        $('.checked-inv').each(function (i, v)
        {
            let id = $(v).parents('.invite-data').children('#friend-id').val();
            let mess = $(v).parents('.invite-data').children('#text').val();
            let inv = { userId: id, message: mess };

            ids.push(inv);
        });

        let eventid = $('#event-id').val();
        console.log(eventid);
        console.log(ids);

        $.ajax({
            type: 'post',
            url: '/Events/InviteFriendsIn',
            data:
            {
                eventId: eventid,
                invites:ids
            }
        });

        $('.over-cont').css('display', 'none');

    });
});
// Нажатие галочки напротив имени.
$('.invite-list').on('click', '.inv-button', function () {
    $(this).toggleClass('checked-inv');
});
