﻿// Начало исполнения
$(document).ready(function ()
{
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
            console.log('клик - чата нет');
            // создаем чат отправляем сообщение(включает отправку)
            $('.message-item').empty();
            CreateChat();
           
        }
        else
        {
            console.log('клик - чата есть');
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
        console.log('создание чата');
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
                console.log('выполнгение создания чата');
                console.log(responce.responseJSON);
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
        console.log('отправка сообщения');
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
                console.log('успешная отправка');
                $('#text').val('');
                AddMessageToListMessage(message);
            }
        });

    }
    // Добавление элекмента в дом (сообщение чата).
    function AddMessageToListMessage(text)
    {
        console.log('добавление сообщения');
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
});
