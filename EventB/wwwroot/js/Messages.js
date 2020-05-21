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
    let startoffset = $('#vertical-trigger').offset().top + 1000;
    $('.message-list').scrollTop(startoffset);
    // окраска собщений в цвет отправителя (темнее если отправитель текущий).
    function colored(item) {
        $(item).addClass('my-message');
    }
    colorAllItems();
    function colorAllItems() {
        let curentId = $('#user-id').val();
        let startMes = $('.message-info');
        startMes.each(function (i, value) {
            if ($(value).text() === curentId) {
                $(value).parent().addClass('my-message');
            }
        })
    }
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
        let date = new Date();

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
                let renderMessage = `<div class="message-item my-message">
                                        <div class="message-sender">${senderNam}</div >
                                        <div class="message-text">${message}</div>
                                        <div class="message-date">${date}</div>
                                     </div >`;
                $('.message-list').append(renderMessage);

                // Прокрутка лучше не придумал.((
                var list = $('.message-list');
                list.scrollTop(10000);
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

        // загрузить сообщения для этого чата
        $.ajax({
            url: '/Api/GetMessageHistory',
            data: {
                chatId: newChat
            },
            success: function (rezult) {
                // Рендеринг сообщений.
                buildMessagesContent(rezult);
                $('.message-list').append('<div id="vertical-trigger"></div>');
                colorAllItems();
            }
        });

    });

    // Построение результатов запроса в блок content - List<Message>
    // Добавление результата в конец и скролл.
    function buildMessagesContent(content) {
        // Рендеринг ответа в блоки
        let block = '';
        $(content).each(function (index, value) {
            let date = formatter.format(new Date(value.postDate));
            if (value.eventState == false) {
                block += '<div class="message-item"><div class="message-sender">' + value.senderName +
                    '</div><div class="message-text">' + value.text + '</div ><div class="message-date">' + date + '</div >' +
                    '<div class="message-info display-none">' + value.personId + '</div ></div > ';
            }
            else {
                block += '<div class="message-item message-item-event"><div class="message-sender-event">' + value.senderName +
                    '</div><div class="message-text-event">' + value.text + '</div ><div class="message-date-event">' + date + '</div >' +
                    '<div class="message-info display-none">' + value.personId + '</div ></div > ';
            }
        });

        $('.message-list').html(block);
        var list = $('.message-list');
        let offset = $('.vertical-trigger').offset().top = 1000;
        list.scrollTop(offset);
    };

    function addBuildMessagesContent(content) {
        // Рендеринг ответа в блоки
        let block = '';
        $(content).each(function (index, value) {
            block += '<div class="message-item"><div class="message-sender">' + value.senderName +
                '</div><div class="message-text">' + value.text + '</div ><div class="message-date">' + value.postDate + '</div >' +
                '<div class="message-info display-none">' + value.PersonId + '</div></div > ';
        });

        $('.message-list').append(block);
        let offset = $('.vertical-trigger').offset().top = 1000;
        list.scrollTop(offset);
    };

    // Клик на кнопке Отчистить.
    $('#btn-search').on('click', function (event) {
        event.preventDefault();
        $('#txt-search').val('');
        $('.opponent-container').removeClass('hidden');
    });
    // Динамический поиск.
    $(document).on('keyup', function () {
        if ($('#txt-search').is(':focus')) {
            searchOpponent();
        }
    });

    // Отображение собеседников в левой части согласно результатам поиска.
    function searchOpponent() {
        // получить список того что в опп лист
        // добавить класс дисплей none тем кто не подходит
        let searchText = $('#txt-search').val();
        if (searchText == '') {
            $('.opponent-container').removeClass('hidden');
            return;
        }
        $('.opponent-container').addClass('hidden');
        let opps = $(".opponent-container:contains(" + searchText + ")");
        $(opps).removeClass('hidden');
    };

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
                addBuildMessagesContent(rezult);
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

