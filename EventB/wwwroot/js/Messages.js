const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatroom")
    .build();
/**
 * Добавляет сообщение в список сообщений
 * @param {any} data
 * @param {any} senderId
 */
function AddRenderedMessages(data, senderId) {
    let block = renderMessage(data, senderId);
    $('#vertical-trigger').remove();
    $('.message-list').append(block);
    $('.message-list').append('<div id="vertical-trigger"></div>');
    // Прокрутка лучше не придумал.(( 
    // Здесь vertical-trigger находится относительно окна а нужно относительно родителя. исправить
    scrollDown();    
}

/** Отправляет сообщение в хаб*/
function SendMessage() {
    let senderId = $('#user-id').val();
    let opponentId = $('#opponent-id').val();
    let chat = Number.parseInt($('#chat-id').val());
    let senderNam = $('#user-name').val();
    let message = $('#message').val();
    let date1 = new Date();
    let DataObject = {
        personId: senderId,
        chatId: chat,
        senderName: senderNam,
        reciverId: opponentId,
        text: message,
        postDate: date1
    }
    connection.invoke("SendToChat", DataObject);
    $('#message').val('');
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
        success: function (response) {
            if (response.isSuccess == true) {
                $('#chat-id').val(response.content);
                SendMessage();
            } else {
                GetNotification(response.errorMessage, 2, duration = 3)
            }
        },
        error: function () {
            GetNotification('Что-то пошло не так (', 2, duration = 3)
        }
    });
};

// Перенос строки шифт энтер
function getCaret(el) {
    if (el.selectionStart) {
        return el.selectionStart;
    } else if (document.selection) {
        el.focus();
        var r = document.selection.createRange();
        if (r == null) {
            return 0;
        }
        var re = el.createTextRange(), rc = re.duplicate();
        re.moveToBookmark(r.getBookmark());
        rc.setEndPoint('EndToStart', re);
        return rc.text.length;
    }
    return 0;
}
function MessageSender() {
    if ($('#message').val() == '') { return; }
    let chatId = $('#chat-id').val();
    let opponentId = $('#opponent-id').val();
    if (chatId == '0' || chatId == '') {
        if (opponentId == '0') {
            GetNotification('Выбирите собеседника', 2, duration = 3)
            return;
        }
        else {
            // есть id но чата нет
            // Создать чат и отправить сообщения (когда новый чат и не было сообщений)
            let currentId = $('#user-id').val();
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
}
// Нажатие энетра
$('#message').keyup(function (event) {
    if (event.keyCode == 13) {
        var content = this.value;
        var caret = getCaret(this);
        if (event.shiftKey) {
            this.value = content.substring(0, caret - 1) + "\n" + content.substring(caret, content.length);
            event.stopPropagation();
        } else {
            MessageSender();
        }
    }
});
// Нажатие на кнопу отправить сообщение.
$('#btn-send').on('click', function (event) {
    event.preventDefault();
    MessageSender();
});

// получение сообщения с хаба
connection.on('reciveChatMessage', function (message) {
    // Если открыт этот чат добавляем на экран нет то ставим фифорку
    let chatId = $('#chat-id').val();    
    if (message.chatId == chatId) {
        AddRenderedMessages(message, $('#user-id').val())
    } else {
        let opponent = $('.opponent-chat-id[value="' + message.chatId + '"]').parent()
        let curentValue = $(opponent).children('.new-message-flag').text();
        if (curentValue === '' || curentValue == undefined) {
            curentValue = '1'
        } else {
            curentValue = Number.parseInt(curentValue) + 1;
        }
        $(opponent).children('.new-message-flag').text(curentValue);
        $(opponent).detach().prependTo('.opponents-list');
    }
});

connection.on('responceForBlockUser', function (message) {
    GetNotification(message, 1, 5);
});
connection.start();
// Конец хаба

$(document).ready(function () {
    let btnHtml = '<div id="ch-small-menu-btn"></div>';
    $('#menu-left-container').append(btnHtml);

    getNewMessageCount();

    let curentChatId = $('#chat-id').val();
    console.log(curentChatId)
    if (curentChatId && curentChatId != '0') {
        $.ajax({
            url: '/Api/GetMessageHistory',
            data: {
                chatId: curentChatId
            },
            success: function (rezult) {
                console.log('rezult', rezult)
                buildMessagesContent(rezult);
                scrollDown(); 
            }
        });
    }

    scrollDown();

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
        // Сделать неактивной если чат недоступен

        if ($(this).children('.opponent-is-blocked-chat').val() == 1) {
            $('#btn-send').attr('disabled', true);
            $('#message').attr('disabled', true);
            $('#message').val('заблокировано');
        }
        else {
            $('#btn-send').attr('disabled', false);
            $('#message').attr('disabled', false);
            $('#message').val('');
        }

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
        // Прячем узкое меню если оно есть
        if ($('.left-column').hasClass('ch-hide-menu')) {
            $('.left-column').toggleClass('ch-hide-menu');
        }
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
        scrollDown();
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

    // узкое меню
    $('body').on('click', '#ch-small-menu-btn', function () {
        $('.left-column').toggleClass('ch-hide-menu');
    });
});


function scrollDown() {
    let position = $('#vertical-trigger').position();
    $('.message-list').scrollTop(position.top + 2000);
}

// Получение значков новых сообщений
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
        },
        error: () => {
            GetNotification('Ошибка загрузки', 3, duration = 3)
        }
    });
}