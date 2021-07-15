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
        //if ($(window).scrollTop() > $('#title-container').height() + 16) { 
        let X = $('body').width() > 783 ? ($('#title-container').height() + 16) : 0;
        if ($(window).scrollTop() > X) {           
            $('#menu-voider').removeClass('display-none');
            $('#menu-voider').css('height', $('.main-menu-container').height());
            $('.main-menu-container').addClass('float-menu');
        }
        else {
                       
            $('#menu-voider').css('height', '0');
            $('#menu-voider').addClass('display-none');
            $('.main-menu-container').removeClass('float-menu'); 
        }
    });

    GetUpdates();

    // Узкое меню
    $('#thin-menu').click(function () {
        $('.thin-menu-items-container').toggleClass('display-none');
    });

    // Согласиться принять в друзья. моя страница и друзья
    //agree-friend
    $('.agree-friend').click(function () {
        let id = $(this).siblings('#friend-entity-id').val();
        let button = $(this);

        let req = $.ajax({
            url: '/Friends/SubmitFriend',
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

    // динамическая  справка
    $('body').on('click', '.help-close', function (e) {
        e.stopPropagation();
        $(this).parent().first().addClass('display-none');   
    });
    $('body').on('click', '.help-btn', function (e) {
        e.stopPropagation(); 
        // подвигаем налево если вылазит за пределы
        let thisFrame = $(this).children();
        thisFrame.removeClass('display-none');
        let rightEnd = thisFrame.offset().left + thisFrame.width()
        if (rightEnd > $(window).width()) {
            thisFrame.offset({ left: ($(window).width() - thisFrame.width() - 12), top: thisFrame.offset().top })
        }
    });

    //Карусель кнопки
    $('body').on('click', '.carousel-btn-next', function () {
        carouselMoveNext($(this));
    });

    $('body').on('click', '.carousel-btn-prev', function () {
        carouselMovePrev($(this));
    });

    $('body').on('click', '.btn-close', function () {
        $(this).parent().hide();
    });
});

/**Карусель перейти к следующему */
function carouselMoveNext(element) {
    let carouselItems = $(element).siblings('.carousel-item-block').children('.my-carousel-item');
    let selected = 100;
    carouselItems.each((ind, e) => {
        if ($(e).hasClass('carousel-selected')) {
            selected = ind;
        }
    });

    $(carouselItems).removeClass('carousel-selected');

    if (selected == carouselItems.length - 1) {
        $(carouselItems).first().addClass('carousel-selected');
    }
    else {
        carouselItems.each((ind, e) => {
            if (ind === selected + 1) {
                $(e).fadeIn();
                $(e).addClass('carousel-selected');
            }
        });
    }
}

/**Карусель перейти к прошлому */
function carouselMovePrev(element) {
    let carouselItems = $(element).siblings('.carousel-item-block').children('.my-carousel-item');
    let selected = 100;
    carouselItems.each((ind, e) => {
        if ($(e).hasClass('carousel-selected')) {
            selected = ind;
        }
    });

    $(carouselItems).removeClass('carousel-selected');

    if (selected == 0) {
        $(carouselItems).last().addClass('carousel-selected');
    }
    else {
        carouselItems.each((ind, e) => {
            if (ind === selected - 1) {
                $(e).fadeIn();
                $(e).addClass('carousel-selected');
            }
        });
    }
}

// Звездочки в меню
function GetUpdates() {
    $.ajax({
        url: '/api/get-updates-for-menu',
        success: (function (updates) {
            if (updates.content.hasNewFriends == true) {
                $('#mm-li-users').removeClass('display-none')
                $('#mm-li-users-th').removeClass('display-none')
                $('#fr-new-star').removeClass('display-none')
            }
            if (updates.content.hasNewInvites == true) {
                $('#mm-li-mypage').removeClass('display-none')
                $('#mm-li-mypage-th').removeClass('display-none')
                $('#mp-invites-star').removeClass('display-none')
            }
            if (updates.content.hasNewMessages == true) {
                $('#mm-li-messages').removeClass('display-none')
                $('#mm-li-messages-th').removeClass('display-none')
            }
        }),
        error: function (ex) { console.error('при получении обновлений для меню:', ex); }
    });
}

/**
 * вернет сообщения в виде html
 * @param {any} content ответ с сервера
 * @param {any} userId id текущего пользователя (выбрать со страницы)
 */
function renderMessage(content, userId)
{
    // Рендеринг ответа в блоки
    let block = '';
    $(content).each(function (index, value) {
        let date = formatter.format(new Date(value.postDate));
        if (value.eventState && value.eventState != false) {
            block += '<div class="message-item message-item-event"><div class="message-sender-event">' + value.senderName +
                '</div><div class="message-text-event">' + value.text + '</div ><div class="message-date-event">' + date + '</div >' +
                '<div class="message-info display-none">' + value.personId + '</div ></div > ';
        }
        else if (value.eventLink && value.eventLink != 0) {
            block += `<a href="/Events/Details/${value.eventLink}">
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
                '</div><div class="message-text formatted-body">' + value.text + '</div ><div class="message-date">' + date + '</div >' +
                '<div class="message-info display-none">' + value.personId + '</div ></div > ';
        } 
        else {
            block += '<div class="message-item"><div class="message-sender">' + value.senderName +
            '</div><div class="message-text formatted-body">' + value.text + '</div ><div class="message-date">' + date + '</div >' +
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
 * @param {any} helpInTitle Справка, которая отображается в заголовке в виде help элемента
 */
function getModelWindow(title, okCancel, okCallback, cancelCollback, helpInTitle) {
    let mainBlock = `<p>МОДАЛКА ${title}</p>`;
    let helpBlock = '';
    //Вы можете отправить ссылку на собыие собеседнику. В списке отображаются активные приватные чаты. Если вы не видите нужный вам чат, начните его отправив любое сообщение.
    if (helpInTitle && helpInTitle.length > 0) {
        helpBlock = `
            <div class="help-btn help-btn-light ">
                                <div class="help-content display-none">
                                    <div class="help-close"></div>
                                    <p class="help-container">
                                        ${helpInTitle}
                                    </p>
                                </div>        
                            </div>
        `;
    }
    mainBlock =`<div class="modal-shadow">
                    <div class="modal-content">
                        <div class="top-page-header flex-hsbc"> <div class="flex-hsc"></span>${title}</span>${helpBlock}</div> <div class="modal-cancel modal-close"></div></div>
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
        if (!(okCallback == undefined || okCallback == null)) okCallback();
        $('.modal-shadow').remove();
    });
    // Нажатие отмены.
    $('.modal-cancel').click(() => {
        if (!(cancelCollback !== undefined || okCallback == null)) cancelCollback();
        $('.modal-shadow').remove();
    });  
}   

/**
 * Оставит видимым только те из items , которые в selector содердат textToSearch без учета регистра
 * @param {any} items элементы для рендеринга
 * @param {any} selector где будет искть строку
 * @param {any} textToSearch что будем искать
 */
function iensSearchByText(items, selector, textToSearch) {
    if (textToSearch === '') {
        items.removeClass('display-none');
        return;
    }
    items.addClass('display-none');    
    items.each((index, element) => {
        var Val = $(element).find(selector);
        if (Val.text().toUpperCase().indexOf(textToSearch.toUpperCase()) !== -1) {
            $(element).removeClass('display-none');
        }
    });
}

/**
 * Вызывает уведомление слева снизу на n секунд
 * @param {any} text - текст сообщения
 * @param {any} type - тип окна 0 - белое 1 - красное 2 - желтое 3 - зеленое
 * @param {any} duration - продолжительность в секундах
 */
function GetNotification(text, type, duration = 2) {
    block = `<div class="s-fixed-notification">${text}</div>`;
    $('body').append(block);
    if (type === 1) $('.s-fixed-notification').addClass('s-fixed-notification-red');
    else if (type === 2) $('.s-fixed-notification').addClass('s-fixed-notification-yellow');
    else if (type === 3) $('.s-fixed-notification').addClass('s-fixed-notification-green');
    setTimeout(() => { $('.s-fixed-notification').remove(); }, duration * 1000);
}


