var formatter = new Intl.DateTimeFormat("ru", {
    day: "numeric",
    month: "numeric",
    year: "numeric",
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit"
});

$(document).ready(function () {    
    // Передвижение меню при скролле.
    $(window).scroll(function () {       
        if ($(window).scrollTop() > $('#title-container').height()+16) {
            $('.main-menu-container').addClass('float-menu');
            $('#menu-voider').removeClass('display-none');            
            $('#menu-voider').css('height', $('.main-menu-container').height());
        }
        else {
            $('.main-menu-container').removeClass('float-menu');
            $('#menu-voider').addClass('display-none');
            $('#menu-voider').css('height', '0');
        }
    });

    // Согласиться принять в друзья. иоя страница и друзья
    //agree-friend
    $('.agree-friend').click(function () {
        let id = $(this).parents('.friend-list-container').children('#friend-entity-id').val();
        let button = $(this);

        let req = $.ajax({
            url: '/Api/SubmitFriend',
            data: {
                friendId: id
            }
        });
        req.then(function (data, stat, jqXHR) {
            if (jqXHR.status = 200) {
                button.removeClass('.agree-friend');
                button.text('Добавлен(а)');
                $(this).removeClass('.agree-friend');
            }
            else {
                alert('Что-то пошло не так(');
            }
        });
    });
});

/**
 * вернет сообщения в виде html
 * @param {ответ с сервера} content 
 * @param {id текущего пользователя} userId 
 */
function renderMessage(content, userId)
{
    // Рендеринг ответа в блоки
    let block = '';
    $(content).each(function (index, value) {
        let date = formatter.format(new Date(value.postDate));
        if (value.eventState != false) {
            block += '<div class="message-item message-item-event"><div class="message-sender-event">' + value.senderName +
                '</div><div class="message-text-event">' + value.text + '</div ><div class="message-date-event">' + date + '</div >' +
                '<div class="message-info display-none">' + value.personId + '</div ></div > ';
        }
        else if( value.eventLink != 0) {
            console.log('link' , value)
            block += `<a href="https://localhost:44344/Events/Details/${value.eventLink}">
                        <div class="message-item message-item-event message-item-event-link">
                            <img src="${value.eventLinkImage}" class="message-display-inline message-link-img" />
                            <div class="message-text-event message-display-inline">${value.text}</div>
                            <div class="message-date-event">${date}</div>
                            <div class="message-info display-none">${value.personId}</div >
                        </div >
                      </a>`;
        } 
        else if(userId == value.personId){
            block += '<div class="message-item  my-message"><div class="message-sender">' + 'ВЫ' +
                '</div><div class="message-text">' + value.text + '</div ><div class="message-date">' + date + '</div >' +
                '<div class="message-info display-none">' + value.personId + '</div ></div > ';
        } 
        else {
            block += '<div class="message-item"><div class="message-sender">' + value.senderName +
            '</div><div class="message-text">' + value.text + '</div ><div class="message-date">' + date + '</div >' +
            '<div class="message-info display-none">' + value.personId + '</div ></div > ';
        }
    });
    return block;
}

/**
 * Создает в теге body модальное окно, которое fixed и отображает контент.
 * @param {any} title Заголовок модального окна.
 * @param {any} okCancel Признак наличия кнопки отмена. true - ok cancell  false - ok.
 * @param {any} okCallback Функция, вызываемая принажатии ОК.
 * @param {any} cancelCollback Функция, вызываемая принажатии отмена.
 */
function getModelWindow(title, okCancel, okCallback, cancelCollback) {
    let mainBlock =`<p>МОДАЛКА ${title}</p>`;
    mainBlock =`<div class="modal-shadow">
                    <div class="modal-content">
                        <div class="top-page-header"> <span>${title}</span> </div>
                        <div class="modal-body"></div>
                        <div class="modal-my-footer">`;

    if (okCancel){
        mainBlock += `<span class="modal-ok">oк</span><span class="modal-cancel">отмена</span>`;
    } else {
        mainBlock += `<span class="modal-ok">oк</span>`;
    }
    mainBlock += `</div></div></div>`;                
    $('body ').prepend(mainBlock)
      // нажатие кнопы ОК
    $('.modal-ok').click(() => {
        if(okCallback !== undefined) okCallback();
        okCallback();
        $('.modal-shadow').remove();
    });
    // Нажатие отмены.
    $('.modal-cancel').click(() => {
        if(cancelCollback !== undefined) cancelCollback();
        $('.modal-shadow').remove();
    });  
}

