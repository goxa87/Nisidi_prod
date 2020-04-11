// Нужно ли?
// Начальная прокрутка до конца чата.
var list = $('.message-list');
list.scrollTop(10000);

// Нажатие на кнопу отправить сообщение.
$('#btn-send').on('click', function (event)
{
    event.preventDefault();
    if ($('#message').val() == '') { return; }
    let currentId = $('#user-id').val();
    let chatId = $('#chat-id').val();
    let opponentId = $('#opponent-id').val();
    if (chatId == '0') {
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
    else
    {
        // Отправить сообщене в чат
        // Подразумевается что id чата был найден по id собеседника так что id уже есть.
        // если прилетело по ссылке.
        // или выбран вручную из списка.
        SendMessage();
    }  
    console.log('clicked');
});

// Запрос к АПИ на отправление сообщения.
//[Route("SendTo")]
//async Task SaveMessage(int chatId, string senderId, string senderName, string text)
function SendMessage()
{
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
            let renderMessage = `<div class="message-item"><div class="message-sender">${senderNam}</div ><div class="message-text">${message}</div><div class="message-date">${date}</div></div >`;
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
function createPrivateChat(Id, Opponent)
{
    $.ajax({
        url: '/Api/CreatePrivateChat',
        data:
        {
            id: Id,
            opponentId: Opponent
        },
        complete: function (response) {
            console.log(response.responseJSON);
            $('#chat-id').val(response.responseJSON);
            SendMessage();
        }
    });
};

// Выбор собеседника из левой колонки.
$('.opponent-container').on('click', function ()
{
    // убрать с выделенного класс
    $('.selected-opponent').removeClass('selected-opponent');
    // поставить на этот класс
    $(this).addClass('selected-opponent');
    // отчистить сообщения с листа
    $('.message-list').empty();

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
        }
    });

});

// Построение результатов запроса в блок content - List<Message>
// Добавление результата в конец и скролл.
function buildMessagesContent(content) {    
    // Рендеринг ответа в блоки
    let block = '';
    $(content).each(function (index, value)
    {
        block += '<div class="message-item"><div class="message-sender">' + value.senderName +
            '</div><div class="message-text">' + value.text + '</div ><div class="message-date">' + value.postDate + '</div ></div >';
    });
    
    $('.message-list').html(block);
    var list = $('.message-list');
    list.scrollTop(10000);
};

function addBuildMessagesContent(content) {
    // Рендеринг ответа в блоки
    let block = '';
    $(content).each(function (index, value) {
        block += '<div class="message-item"><div class="message-sender">' + value.senderName +
            '</div><div class="message-text">' + value.text + '</div ><div class="message-date">' + value.postDate + '</div ></div >';
    });

    $('.message-list').append(block);
    var list = $('.message-list');
    list.scrollTop(1000);
};

// Клик на кнопке поиска.
$('#btn-search').on('click', function(event)
{
    event.preventDefault();
    searchOpponent();
});

// Отображение собеседников в левой части согласно результатам поиска.
function searchOpponent()
{        
    // получить список того что в опп лист
    // добавить класс дисплей none тем кто не подходит
    let searchText = $('#txt-search').val();
    console.log('777'+searchText);
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
    console.log('interval');
    if ($('#chat-id').val() == 0) { return; }

    let opponent_Id = $('#opponent-id').val();
    let chat_Id = $('#chat-id').val();
    console.log('отправлен запрос на ' + chat_Id);
    $.ajax({
        url: '/Api/GetNewMessages',
        data: {
            chatId: chat_Id,
            opponentId: opponent_Id
        },
        success: function (rezult)
        {
            console.log(rezult);
            addBuildMessagesContent(rezult);
        }
    });

}, 5000);