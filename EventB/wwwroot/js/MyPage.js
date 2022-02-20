import { renderMessage, getModelWindow, iensSearchByText, GetNotification } from './Controls.js';
import { GetBanner } from './Banner.js';

$(document).ready(function () {
    // Кнпоки Нижнего меню.
    $('.bottom-menu-item').on('click', function () {
        $('.bottom-menu-item').removeClass('bottom-menu-item-selected');
        $(this).addClass('bottom-menu-item-selected');
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
    $('body').on('click', '.mp-will-go', function () {
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
    $('body').on('click', '.mp-will-notgo', function () {
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

    // Загрузит визиты
    GetVizitsBlock('');
    // Загрузка созданных событий
    GetCreatedBlock('');
    // Загрузка приглашений
    GetInviteBlock('');

    // Пагинация в визитах
    $('body').on('click', '.mp-tab-vizit-paging', function () {
        $('.mp-tab-vizit-paging').removeClass('paging-selected');
        $(this).addClass('paging-selected');
        GetVizitsPage($(this).text())
    });

    // пагинация в созданных
    $('body').on('click', '.mp-tab-created-paging', function () {
        $('.mp-tab-created-paging').removeClass('paging-selected');
        $(this).addClass('paging-selected');
        GetCreatedPage($(this).text())
    });

    // фильтры
    $('body').on('click', '#mp-created-clear-filter', function () {
        $('#my-events-body').html('загрузка...');
        GetCreatedBlock('')
    });

    $('body').on('click', '#mp-created-search-filter', function () {
        var filter = $('#mp-created-ev-filter').val();
        $('#my-events-body').html('загрузка...');
        GetCreatedBlock(filter);
    });

    // фильтры
    $('body').on('click', '#mp-vizits-clear-filter', function () {
        $('#events-willgo-body').html('загрузка...');
        GetVizitsBlock('')
    });

    $('body').on('click', '#mp-vizits-search-filter', function () {
        var filter = $('#mp-vizits-filter').val();
        $('#events-willgo-body').html('загрузка...');
        GetVizitsBlock(filter);
    });
});

/**Загрузит блок с визитами ОСНОВНОЙ*/
function GetVizitsBlock(filter) {
    $.ajax({
        url: `/MyPage/GetVizits?filter=${filter}`,
        success: function (markup) {
            $('#events-willgo-body').html(markup);
        },
        error: function () { GetNotification("Что-то пошло не так (", 1, 2); }
    })
}

/**
 * Загрузит выбранную страницу
 * @param {any} selectedPage
 */
function GetVizitsPage(selectedPage) {
    $('#mp-vizits-tab-container').html("загрузка...");
    var filter = $('#mp-vizits-filter-applyed').val();
    $.ajax({
        url: `/MyPage/GetVizitsPage?currentPage=${selectedPage}&filter=${filter}`,
        success: function (markup) {
            $('#mp-vizits-tab-container').html(markup);
        },
        error: function () {
            $('#mp-vizits-tab-container').html("Ошибка (");
            GetNotification("Что-то пошло не так (", 1, 2);
        }
    })
}

/**Загрузит блок с созданными */
function GetCreatedBlock(filter) {
    $.ajax({
        url: `/MyPage/GetCreatedTab?filter=${filter}`,
        success: function (markup) {
            $('#my-events-body').html(markup);
        },
        error: function () { GetNotification("Что-то пошло не так (", 1, 2); }
    })
}

/**
 * Загрузит выбранную страницу для созданных
 * @param {any} selectedPage
 */
function GetCreatedPage(selectedPage) {
    $('#mp-created-tab-container').html("загрузка...");
    var filter = $('#mp-created-ev-filter-applyed').val();
    $.ajax({
        url: `/MyPage/GetCreatedPage?currentPage=${selectedPage}&filter=${filter}`,
        success: function (markup) {
            $('#mp-created-tab-container').html(markup);
        },
        error: function () {
            $('#mp-created-tab-container').html("Ошибка (");
            GetNotification("Что-то пошло не так (", 1, 2);
        }
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




