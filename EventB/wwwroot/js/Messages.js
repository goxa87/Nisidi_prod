﻿const connection = new signalR.HubConnectionBuilder()
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
    var list = $('.message-list');
    list.scrollTop($('#vertical-trigger').offset().top + $(list).height());
    $('#message').val('');
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
    AddRenderedMessages(DataObject, senderId);
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

// Нажатие на кнопу отправить сообщение.
$('#btn-send').on('click', function (event) {
    event.preventDefault();
    if ($('#message').val() == '') { return; } 
    let chatId = $('#chat-id').val();
    let opponentId = $('#opponent-id').val();
    if (chatId == '0' || chatId == '') {
        if (opponentId == '0') {
            GetNotification('Выбирите собеседника',2 , duration = 3)
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
connection.start();
// Конец хаба

$(document).ready(function () {
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
    getNewMessageCount();

    // Начальная прокрутка до конца чата.
    let startoffset = $('#vertical-trigger').offset().top;
    $('.message-list').scrollTop(startoffset);         

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

    
});

