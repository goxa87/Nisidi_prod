import { ExecuteAjaxRequest } from './requestTool.js';

let originDetailDescription = '';
let originDetailNote = '';

$(document).ready(function () {
    originDetailDescription = $('#sup-detail-description').val();
    originDetailNote = $('#sup-detail-note').val();
    $('#sup-list-options').on('change', '', function () {
        GetTicketListPartial($('#sup-list-options').val())
    });

    // детали тикета Назначить себе
    $('#sup-detail-assengre').click(function () {
        AssengreTicketDetails();
    });

    // детали тикета редактировать
    $('#sup-detail-edit').click(function () {
        EditDetails();
    });

    // детали тикета Сохранить изменения
    $('body').on('click', '#sup-detail-save-changes', function () {
        SaveChangesDetails();
    });

    // детали тикета Отменить изменения
    $('body').on('click', '#sup-detail-cancel-changes', function () {
        console.log('xx1')
        CancelChangesDetails();
    });

    // детали тикета Закрыть
    $('#sup-detail-close').click(function () {
        CloseTicket();
    });
});

/**
 * Получить партиалку с списком тикетов по фильтру
 * @param {any} type
 */
function GetTicketListPartial(type) {
    $.ajax({
        url: `/Support/GetPartialTaskListByType?type=${type}`,
        success: function (responce) {
            $('#supp-lis-container').html(responce);
        },
        error: function (error) {
            console.log('ERROR Support/GetPartialTaskListByType?type=', error)
        }
    });
}

/** Назначить себе */
function AssengreTicketDetails() {
    var data = {
        ticketId: $('#ticket-id').val()
    };
    ExecuteAjaxRequest('/Support/AssengreTicket', 'post', data, true, function () {
        $('#sup-detail-assengre').addClass('display-none');
    });
}

/**Редактировать тикет */
function EditDetails() {
    ToggleDetailsButtonsMode();
    $('#sup-detail-description').prop("disabled", false);
    $('#sup-detail-note').prop("disabled", false);
}

/**Сохранить изменения по тикету */
function SaveChangesDetails() {
    var data = {
        ticketId: $('#ticket-id').val(),
        description: $('#sup-detail-description').val(),
        note: $('#sup-detail-note').val()
    };
    ExecuteAjaxRequest('/Support/SaveTicketDetails', 'post', data, true, function () {
        ToggleDetailsButtonsMode();
        $('#sup-detail-description').val(data.description);
        $('#sup-detail-description').prop("disabled", true);
        $('#sup-detail-note').val(data.note);
        $('#sup-detail-note').prop("disabled", true);
        originDetailDescription = $('#sup-detail-description').val();
        originDetailNote = $('#sup-detail-note').val();
    });
}

/**Отменить изменения по тикету */
function CancelChangesDetails() {
    ToggleDetailsButtonsMode();
    $('#sup-detail-description').val(originDetailDescription);
    $('#sup-detail-description').prop("disabled", true);
    $('#sup-detail-note').val(originDetailNote);
    $('#sup-detail-note').prop("disabled", true);

}

/**закрыть задачу */
function CloseTicket() {
    var data = {
        ticketId: $('#ticket-id').val()
    };
    ExecuteAjaxRequest('/Support/CloseTicket', 'post', data, true, function () {
        $('#sup-detail-close').addClass('display-none');
    });
}

/**Скроет и покажет кнопкаи действий  */
function ToggleDetailsButtonsMode() {
    $('#sup-detail-edit').toggleClass('display-none');
    $('#sup-detail-save-changes').toggleClass('display-none');
    $('#sup-detail-cancel-changes').toggleClass('display-none');
}

