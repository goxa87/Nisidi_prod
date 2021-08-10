import { renderMessage, getModelWindow, iensSearchByText, GetNotification } from './Controls.js';
import { GetBanner } from './Banner.js';

$(document).ready(function () {
    // Кнпоки Нижнего меню.
    $('.mp-my-selector').on('click', function () {
        $('.selected-label').removeClass('selected-label');
        $(this).addClass('selected-label');
    });
    // Показ Пойду.
    $('#events-willgo').click(function () {
        $('.mp-my-content').addClass('display-none');
        $('.mp-my-content').removeClass('display-block');
        $('#events-willgo-body').removeClass('display-none');
        $('#events-willgo-body').addClass('display-block');
    });
    // Показ Мои события.
    $('#my-events').click(function () {

        $('.mp-my-content').addClass('display-none');
        $('.mp-my-content').removeClass('display-block');
        $('#my-events-body').removeClass('display-none');
        $('#my-events-body').addClass('display-block');
    });    
    // Показ приглашения.
    $('#invites').click(function () {
        $('.mp-my-content').addClass('display-none');
        $('.mp-my-content').removeClass('display-block');
        $('#invites-body').removeClass('display-none');
        $('#invites-body').addClass('display-block');
    });
    // Нажате на кнопку пойду в контейнере приглашения.
    $('.mp-will-go').click(function ()
    {
        let eId = $(this).parent().children('#eveId').val();
        let invId = $(this).parent().children('#inviteId').val();
        $.ajax({
            url: '/MyPage/SubmitInvite',
            data: {
                eventId: eId,
                inviteId: invId
            },
            success: () => {
                $(this).parents('.total-invite').fadeOut(600);
            },
            error: data => { console.log(data);}
        });
    });

    // Нажате на кнопку НЕпойду в контейнере приглашения.
    $('.mp-will-notgo').click(function () {
        let invId = $(this).parent().children('#inviteId').val();
        $.ajax({
            url: '/MyPage/RefuseInvite',
            data: {
                inviteId: invId
            },
            success: () => {
                $(this).parents('.total-invite').fadeOut(600);
            },
            error: data => { console.log(data); }
        });
    });

    // Поиски на странице. Филтьруем за раз все 3 поля
    $(document).on('keyup', function () {
        if ($('#mp-vizits-filter').is(':focus')) {
            
            let searchText = $('#mp-vizits-filter').val();
            $('#mp-created-ev-filter').val(searchText);
            $('#mp-invites-filter').val(searchText);

            let items = $('.mp-details-figure');
            iensSearchByText(items, '.small-figure-title', searchText);
            let invitesItems = $('.total-invite');
            iensSearchByText(invitesItems, '.mp-invites-title', searchText);
        }
        if ($('#mp-created-ev-filter').is(':focus')) {
            let searchText = $('#mp-created-ev-filter').val();

            $('#mp-vizits-filter').val(searchText);
            $('#mp-invites-filter').val(searchText);

            let items = $('.mp-details-figure');
            iensSearchByText(items, '.small-figure-title', searchText);
            let invitesItems = $('.total-invite');
            iensSearchByText(invitesItems, '.mp-invites-title', searchText);
        }
        if ($('#mp-invites-filter').is(':focus')) {
            let searchText = $('#mp-invites-filter').val();

            $('#mp-vizits-filter').val(searchText);
            $('#mp-created-ev-filter').val(searchText);

            let items = $('.mp-details-figure');
            iensSearchByText(items, '.small-figure-title', searchText);
            let invitesItems = $('.total-invite');
            iensSearchByText(invitesItems, '.mp-invites-title', searchText);
        }
    });
    $('.s-filter-clear').click(function () {
        $('#mp-vizits-filter').val('');
        $('#mp-created-ev-filter').val('');
        $('#mp-invites-filter').val('');

        $('.mp-details-figure').removeClass('display-none');
        $('.total-invite').removeClass('display-none');
    });

    // Загрузит визиты
    GetVizitsBlock();
    // Загрузка созданных событий
    GetCreatedBlock();
    // Загрузка приглашений
    GetInviteBlock();
    
});

/**Загрузит блок с визитами */
function GetVizitsBlock() {
    $.ajax({
        url: "/MyPage/GetVizits",
        success: function (markup) {
            $('#events-willgo-body').html(markup);
        },
        error: function () { GetNotification("Что-то пошло не так (", 1, 2); }
    })
}

/**Загрузит блок с созданными */
function GetCreatedBlock() {
    $.ajax({
        url: "/MyPage/GetCreated",
        success: function (markup) {
            $('#my-events-body').html(markup);
        },
        error: function () { GetNotification("Что-то пошло не так (", 1, 2); }
    })
}

/**Загрузит блок с приглашениями */
function GetInviteBlock() {
    $.ajax({
        url: "/MyPage/GetInvites",
        success: function (markup) {
            $('#invites-body').html(markup);
        },
        error: function () { GetNotification("Что-то пошло не так (", 1, 2); }
    })
}




