import { CarouselMovePrev, CarouselMoveNext } from './Controls.js';

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
        CarouselMoveNext($(this));
    });

    $('body').on('click', '.carousel-btn-prev', function () {
        CarouselMovePrev($(this));
    });

    $('body').on('click', '.btn-close', function () {
        $(this).parent().hide();
    });
});

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


