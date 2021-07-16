import { renderMessage, getModelWindow, iensSearchByText, GetNotification } from './Controls.js';

$(document).ready(() => {
    let btnHtml = '<div id="h-small-menu-btn"></div>';
    $('#menu-left-container').append(btnHtml);

    $('body').on('click', '#h-small-menu-btn', function () {
        $('.h-left-menu').toggleClass('h-hide-menu');
    });
    $('body').on('click', '.h-menu-item', function () {
        $('.h-left-menu').toggleClass('h-hide-menu');
    });
    // Начальная прокрутка до конца чата.
    ScrollToEnd();

    /** Кнопка отправить сообщение*/
    $('#h-message-send').click(function () {
        var text = $('#h-message-text').val();
        var userId = $('#userId').val();
       
        $.ajax({
            url: '/help/message-support',
            data:{
                text:text
            },
            success: function () {
                let date1 = new Date();
                let data = { personId: userId, text: text, postDate: date1, eventState: false, eventLink: false }
                let block = renderMessage(data, userId);
                var triger = $('#vertical-trigger');
                $('#h-support-content').remove(triger);
                $('#h-support-content').append(block);
                $('#h-support-content').append(triger);
                ScrollToEnd();
                $('#h-message-text').val('');
            },
            error: function() {
                GetNotification('не удалось отправить ссобщение', 1, 5)
            }
        });
    });
});

/** Прокрутит до конца списка сообщений */
function ScrollToEnd() {
    let startoffset = $('#vertical-trigger').offset().top;
    $('#h-support-content').scrollTop(startoffset);
}