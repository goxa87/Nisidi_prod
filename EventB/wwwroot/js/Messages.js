$(document).ready(function () {
    // для парсинга дат с сервера.
    var formatter = new Intl.DateTimeFormat("ru", {
        day: "numeric",
        month: "numeric",
        year: "numeric",
        hour: "2-digit",
        minute: "2-digit",
        second: "2-digit"
    });

    // Начальная прокрутка до конца чата.
    let startoffset = $('#vertical-trigger').offset().top;
    $('.message-list').scrollTop(startoffset);
    // Нажатие на кнопу отправить сообщение.
    $('#btn-send').on('click', function (event) {
        event.preventDefault();
        if ($('#message').val() == '') { return; }
        let currentId = $('#user-id').val();
        let chatId = $('#chat-id').val();
        let opponentId = $('#opponent-id').val();
        if (chatId == '0' || chatId == '') {
            if (opponentId == '0') {
                alert('Выбирите собеседника');
                return;
            }
            else {
                // есть id но чата нет
                // Создать чат и отправить сообщения (когда новый чат и не было сообщений)
                createPrivateChat(currentId, opponentId);
            }
        }
        else {
            // Отправить сообщене в чат
            // Подразумевается что id чата был найден по id собеседника так что id уже есть.
            // если прилетело по ссылке.
            // или выбран вручную из списка.
            SendMessage();
        }
    });

    // Запрос к АПИ на отправление сообщения.
    //[Route("SendTo")]
    //async Task SaveMessage(int chatId, string senderId, string senderName, string text)
    function SendMessage() {
        let senderId = $('#user-id').val();
        let chat = $('#chat-id').val();
        let senderNam = $('#user-name').val();
        let opponentId = $('#opponent-id').val();
        let message = $('#message').val();
        let date1 = new Date();

        $.ajax({
            url: '/Api/SendTo',
            data: {
                chatId: chat,
                senderId: senderId,
                senderName: senderNam,
                text: message,
                reciverId: opponentId
            },
            success: function () {
                // Заполнить форму на странице новым сообщением.
                let data = { personId: senderId, text: message, postDate: date1, eventState: false, eventLink: false }
                let block = renderMessage(data, senderId);
                $('#vertical-trigger').remove();
                $('.message-list').append(block);
                $('.message-list').append('<div id="vertical-trigger"></div>');
                // Прокрутка лучше не придумал.(( 
                // Здесь vertical-trigger находится относительно окна а нужно относительно родителя. исправить
                var list = $('.message-list');
                list.scrollTop($('#vertical-trigger').offset().top + $(list).height());
                $('#message').val('');
            }
        });
    };

    // Запрос к АПИ на Создание чата с новым собеседником.
    //[Route("CreatePrivateChat")]
    //async Task < int > CreateUserChat(string id, string opponentId) 
    function createPrivateChat(Id, Opponent) {
        $.ajax({
            url: '/Api/CreatePrivateChat',
            data:
            {
                id: Id,
                opponentId: Opponent
            },
            complete: function (response) {
                $('#chat-id').val(response.responseJSON);
                SendMessage();
            }
        });
    };

    // Выбор собеседника из левой колонки.
    $('.opponent-container').on('click', function () {
        // Картинку и имя в заголовок.

        $('.opponent-photo').children('img').prop('src', $(this).children('.opponent-photo-value').val());
        $('.opponent-name').text($(this).children('.opponent-name-value').text());

        // убрать с выделенного класс
        $('.selected-opponent').removeClass('selected-opponent');
        // поставить на этот класс
        $(this).addClass('selected-opponent');
        // отчистить сообщения с листа
        $('.message-list').empty();
        // убрать количество новых сообщений
        $(this).children('.new-message-flag').text('');

        // скопировать значения id b chat id в форму
        let newChat = $(this).children('.opponent-chat-id').val();
        let newOppId = $(this).children('.opponent-id').val();
        $('#chat-id').val(newChat);
        $('#opponent-id').val(newOppId);

        $('.mes-remove-chat').removeClass('display-none');

        // загрузить сообщения для этого чата
        $.ajax({
            url: '/Api/GetMessageHistory',
            data: {
                chatId: newChat
            },
            success: function (rezult) {
                // Рендеринг сообщений.
                buildMessagesContent(rezult);
            }
        });

    });

    // Построение результатов запроса в блок content - List<Message>
    // Добавление результата в конец и скролл.
    function buildMessagesContent(content) {
        // Рендеринг ответа в блоки        
        let userId = $('#user-id').val();
        let block = renderMessage(content, userId);
        $('#vertical-trigger').remove();
        $('.message-list').html(block);
        $('.message-list').append('<div id="vertical-trigger"></div>');
        var list = $('.message-list'); 
        let offset = $('#vertical-trigger').offset().top;
        list.scrollTop(offset);
    };    

    // Клик на кнопке Отчистить.
    $('#btn-search').on('click', function (event) {
        event.preventDefault();
        $('#txt-search').val('');
        $('.mes-opponent-container').removeClass('display-none');
    });
    // Динамический поиск.
    $(document).on('keyup', function () {
        if ($('#txt-search').is(':focus')) {
            let searchText = $('#txt-search').val();
            let items = $('.mes-opponent-container');
            iensSearchByText(items, '.opponent-name-value', searchText);
        }
    });

    // Удаление чата
    // /messages/delete-user-chat
    $('.mes-remove-chat').click(() => {
        let chatId = $('#chat-id').val();
        console.log(chatId);
        $.ajax({
            url: '/messages/delete-user-chat',
            data: {
                chatId: chatId
            },
            success: () => {                
                $('.selected-opponent').parent().remove();
                $('.opponent-photo').children('img').prop('src', '');
                $('.opponent-name').text('');
                $('.mes-remove-chat').addClass('display-none');
                $('.message-list').html('');
                $('#chat-id').val('0');
                GetNotification("Вы покинули этот чат", 2, 3);
            },
            error: () => {
                GetNotification("Удаление не удалось", 2, 10);
            }
        });
    });

    //[Route("GetNewMessages")]
    //public async Task < List < Message >> GetNewMessages(int chatId, string opponentId)
    // Интервальный запрос на получение новых непрочитанных сообщений для чата. 
    setInterval(function () {
        // Если нет чата выходим не нагружаем сервер.
        if ($('#chat-id').val() == 0) { return; }
        let opponent_Id = $('#opponent-id').val();
        let chat_Id = $('#chat-id').val();
        $.ajax({
            url: '/Api/GetNewMessages',
            data: {
                chatId: chat_Id,
                opponentId: opponent_Id
            },
            success: function (rezult) {
                if (rezult.length != 0) {
                    let block = renderMessage(rezult);
                    $('#vertical-trigger').remove();
                    $('.message-list').append(block);
                    $('.message-list').append('<div id="vertical-trigger"></div>');
                    var list = $('.message-list');
                    let offset = $('#vertical-trigger').offset().top;
                    list.scrollTop(offset);
                }                
            }
        });
    }, 5000);

    // отправка запроса на получение количества новых сообщений в чатах в списке.
    getNewMessageCount();
    setInterval(function () {
        getNewMessageCount();
    }, 7000);
    function displayNewMessageFlag(count, chatId) {
        if (count > 0) {
            $('.opponent-chat-id').each((ind, chat) => {
                if ($(chat).val() == String(chatId)) {
                    $(chat).parent('.opponent-container').children('.new-message-flag').text(count);
                    $(chat).parent('.opponent-container').detach().prependTo('.opponents-list');
                    return true;
                }
            })
        }
    }
    function getNewMessageCount() {
        $.ajax({
            url: '/Api/GetNewMessagesCount',
            success: function (rezult) {
                $(rezult).each(function (i, value) {
                    displayNewMessageFlag(value.countNew, value.chatId)
                });
            }
        });
    }
});

