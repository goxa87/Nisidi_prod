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

