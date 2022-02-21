import { renderMessage, getModelWindow, iensSearchByText, GetNotification } from './Controls.js';
import { DateFromObjectToDateTime } from './Services.js';
$(document).ready(function ()
{
    // Кнопка сменить статус.
    $('.right-columnn').on('click', '.lk-change-event-status', function () {
        let eveId = $(this).parent().children('.kibnet-event-id').val();
        // Удалить при имплементации реального апи
        let type = 0;

        // Запрос на изменение статуса.
        function changeStatus(){
            // window.open('/MarketRoom/Pay');
            $.ajax({
                url: '/api/MarketKibnet/change-event-type',
                data: {
                    targetType: type,
                    eventId: eveId
                },
                success: () => {
                    //let newLabel = type === 0 ? 'Статус: Global' : 'Статус: Private'
                    //$(this).parent().children('.lk-event-state').text(newLabel);
                },
                error: function () {
                    alert('Ошибка сервера. Обратитесь в поддержку.');
                }
            });
        };
        getModelWindow('ПУБЛИКАЦИЯ', true, changeStatus, null, null);
        // Контент блока модалки.
        let content =`<div class="lk-sale-message flex-vsc">
            <p>Сейчас ваше событие находится в статусе "Частное".</p>
            <p> Это значит, что оно не будет отображатся в списке рекомендаций по интересам, дате и локации.</p>
            <p>Вы можете перевести это событие в статус "Публичное". После этого оно будет попадать в список рекомендаций.</p>
            <img src="/resourses/sale.png?v=3" alt="sale" />
            <p>Внимание! Сейчас действует специальная цена: 0р. на публикацию глобальных событий.</p>
            <p>Специальная цена действует до 01.03.2022г.</p></div>
        `;
        $('.modal-body').html(content);
        
    });

    // Кнопка удалить событие 
    $('.right-columnn').on('click', '.lk-delete-event', function () {
        let eveId = $(this).parent().children('.kibnet-event-id').val();
        let eventContainer = $(this);
        console.log(eveId)
        console.log(eventContainer)

        function onConfirmDelete() {
            $.ajax({
                url: '/api/MarketKibnet/delete-event',
                data: {                    
                    eventId: eveId
                },
                success: () => {
                    
                    console.log($(eventContainer))
                    $(eventContainer).parents('.lk-event').html('<p>Удалено</p>');
                },
                error: function () {
                    alert('Событие не удалено. Запрещено либо не доступно.');
                }
            });
        }
        
        let data = { id: eveId, container: eventContainer }
        getModelWindow('УДАЛЕНИЕ СОБЫТИЯ', true, onConfirmDelete, null, null);
        let content = `<h3>Внимание!</h3>
            <p>Удалив данное событие вы не сможете его восстановить.</p>
            <p>также будут удалены все отметки о визитах и чат.</p>`;
        $('.modal-body').html(content);
    });

    // Кнопка блокировать пользователей в чате
    $('.right-columnn').on('click', '.lk-block-users-chat', function () {
        let eveId = $(this).parent().children('.kibnet-event-id').val();
        
        $.ajax({
            url: '/api/MarketKibnet/get-event-chat-users',
            data: {
                eventId: eveId
            },
            success: (usersChat) => {
                console.log('usersChat', usersChat);
                getModelWindow('Блокировка пользователей', false, null, null, "В этом списке управляйте возможностью пользователей отправлять сообщения в чат события.");
                let content = `<h3>Участники чата события</h3><input type="hidden" id="lk-block-users-event-id" value="${eveId}"><div>`;
                $(usersChat).each(function (index, user) {
                    let block = `
<div class="flex-hsbc flex-wr">
<input type="hidden" class="lk-block-users-user-chat-id" value="${user.userChatId}" />
<div class="lk-block-user-name"><img class="lk-block-user-ava" src="${user.userPhoto}">${user.userName}&nbsp<span class="lk-isBlock"><u>${user.isBlocked ? "Заблокирован" : "НЕ&nbsp;заблокирован"}</u></span></div>`
                    if (user.isBlocked) {
                        block += `
                        <div class="lk-block-user-btn lk-block-user-btn-unblock">Разблокировать</div>
                        </div >
                        <hr>`;
                    }
                    else {
                        block += `
                        <div class="lk-block-user-btn lk-block-user-btn-block">Блокировать</div>
                        </div >
                        <hr>`;
                    };
                    content += block;
                    content += '</div>';
                });    

                $('.modal-body').html(content);
            },
            error: function () {
                GetNotification('Не удалось получить список участников чата',1,3)
            }
        });
    });
    // обработчики событий блокировки пользователей в чатах
    $('body').on('click', '.lk-block-user-btn', function () {
        let eveId = $('#lk-block-users-event-id').val();
        let userChatId = $(this).parent().children('.lk-block-users-user-chat-id').val();
        let curentButton = $(this);
        $.ajax({
            url: '/api/MarketKibnet/switch-user-chat-block',
            data: {
                userChatId: userChatId,
                eventId: eveId
            },
            success:  function() {
                $(curentButton).toggleClass('lk-block-user-btn-unblock');
                $(curentButton).toggleClass('lk-block-user-btn-block');
                if ($(curentButton).text() == "Блокировать") {
                    $(curentButton).text("Разблокировать");
                    $(curentButton).parent().children('.lk-block-user-name').children('.lk-isBlock').text('Заблокирован');
                }
                else {
                    $(curentButton).text("Блокировать");
                    $(curentButton).parent().children('.lk-block-user-name').children('.lk-isBlock').text('НЕ заблокирован');
                }
            },
            error: function (ex) {
                if (ex.status == 401) {
                    GetNotification(`Вам запрещено действие (${ex.status})`, 1, 3);
                } else if (ex.status == 400) {
                    GetNotification(`Пользователь не найден в чате (${ex.status})`, 1, 3);
                } else {
                    GetNotification(`Не удалось выполнить действие (${ex.status})`, 1, 3);
                }
            }
        });
    });

    // Фильтр событий
    // Поиск из списка друзей, по части имени.
    $(document).on('keyup', function () {
        if ($('#lk-eve-filter').is(':focus')) {
            let searchText = $('#lk-eve-filter').val();
            let items = $('.lk-eve-item');
            iensSearchByText(items, '.eve-title', searchText);
        }
    });
    $('#lk-eve-filter-clear').click(function () {
        clearSearch();
    });
    function clearSearch() {
        $('#lk-eve-filter').val('');
        $('.lk-eve-item').removeClass('display-none');
    }

    // Переключение в меню страницы
    $('.bottom-menu-item').click(function () {
        $('.bottom-menu-item').removeClass('bottom-menu-item-selected');
        $(this).addClass('bottom-menu-item-selected');

        if ($(this).attr('id') == 'mr-btn-events') {
            $('#mr-main-events-tab').removeClass('display-none');
            $('#mr-main-tickets-tab').addClass('display-none');
        }
        if ($(this).attr('id') == 'mr-btn-tickets') {
            $('#mr-main-events-tab').addClass('display-none');
            $('#mr-main-tickets-tab').removeClass('display-none');
        }
    });

    // Отправить новый тикет
    $('#mr-t-send-new').click(function () {
        var data = $('#mr-t-message-text').val();

        if (!data || data.length == 0) {
            GetNotification('Введите текст вашего вопроса', 1, 2);
            return;
        }

        $.ajax({
            url: '/MarketRoom/CreateNewTicket',
            method: 'POST',
            data: { messageText: data},
            success: function (response) {
                if (response.isSuccess) {
                    GetNotification('Обращение отправлено.', 3, 2)
                    $('#mr-t-message-text').val('');
                } else {
                    GetNotification('Что-то пошло не так, попробуйте еще раз.', 1, 2);
                    console.log('Чтото пошло не так ' + response.errorMessage)
                }
            },
            error: function (jqXHR, status) {
                GetNotification('Что-то пошло не так, попробуйте еще раз.', 1, 2);
                console.log('Чтото пошло не так ' + jqXHR.statusText);
            }
        });
    });

    // Получить и разместить тикеты пользователя
    GetAndNestTickets();

    // Cпрятаь секцию для ввода письма в тех. поддержку
    HideNewRequestSection();

    //Показать секцию для ввода письма в тех. поддержку и перевернуть шеврон
    $('#expand-more-icon').click(function () {
        $('#expand-more-icon').hide();
        $('#expand-less-icon').css('display', 'inline-block');
        ShowNewRequestSection();
    });

    //Спрятать секцию для ввода письма в тех. поддержку и перевернуть шеврон в исходное положение
    $('#expand-less-icon').click(function () {
        $('#expand-more-icon').show();
        $('#expand-less-icon').css('display', 'none');
        HideNewRequestSection();
    });
});

/** Получить и разместить тикеты пользователя */
function GetAndNestTickets() {
    $.ajax({
        url: '/MarketRoom/GetAllTicketsForUser',
        success: function (response) {
            console.log('resp', response);
            if (response.isSuccess) {
                response.content.forEach(function (item) {
                    let markup = RenderSupportTicket(item);
                    $('#mr-tickets-list').append(markup);
                });
            } else {
                console.log('Чтото пошло не так ' + response.errorMessage)
            }
        },
        error: function (jqXHR, ex) {
            console.log('Чтото пошло не так ' + jqXHR.statusText);
        }
    });
}

/**
 * Сгенерирует разметку тикета техподдержки
 * @param {any} item SupportTicket
 */
function RenderSupportTicket(item) {
    let castStatus = 'неизвестно';
    if (item.status == 0) { castStatus = 'новая'; }
    if (item.status == 1) { castStatus = 'в работе'; }
    if (item.status == 2) { castStatus = 'закрыта'; }
    if (item.status == 3) { castStatus = 'удалена'; }

    let markup = `<div class="mr-t-container">
            <h4 class="mr-t-item-header"><span><b># ${item.supportTicketId}</b>&nbsp;&nbsp;</span>${item.theme}</h4>
            <p class="mr-t-description">${item.description}</p><br />
            <div>
                <div class="mr-t-state">Статус: ${castStatus}</div>
                <div>Открыто: ${DateFromObjectToDateTime(item.openDate)}</div>
                <div>Закрыто: ${DateFromObjectToDateTime(item.closeDate)}</div>
            </div>
            <div class="flex-hec">
                <a href="/Messages/Index?opponentId=${item.nisidiEmployeeId}" class="mr-t-tochat">Перейти к чату с сотрудником</a>
            </div>
        </div>`;

    return markup;
}

/** Прячет секцию для ввода письма в тех. поддержку */
function HideNewRequestSection(requestSection = "#mr-new-request-section") {
    $(requestSection).hide();
}

/** Показывает секцию для ввода письма в тех. поддержку */
function ShowNewRequestSection(requestSection = "#mr-new-request-section") {
    $(requestSection).show();
}