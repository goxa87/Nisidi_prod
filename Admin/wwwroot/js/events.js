DevExpress.localization.locale("ru");
// Настройка DE элементов
$(function () {

    var DateStartCreate = $('#evelist-createstartdate').val();
    if (DateStartCreate != undefined && DateStartCreate != '') {
        DateStartCreate = new Date(DateStartCreate);
    }
    else {
        DateStartCreate = undefined;
    }
    $("#dateCreateStartDE").dxDateBox({
        type: "datetime",
        displayFormat: "dd.MM.yyyy HH:mm:ss",
        dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
        showClearButton: true,
        value: DateStartCreate,
        onValueChanged: function (e) {
            $('#evelist-createstartdate').val(e.value);
        }
    });

    DateStartCreate = $('#evelist-createenddate').val();
    if (DateStartCreate != undefined && DateStartCreate != '') {
        DateStartCreate = new Date(DateStartCreate);
    }
    else {
        DateStartCreate = undefined;
    }
    $("#dateCreateEndDE").dxDateBox({
        type: "datetime",
        displayFormat: "dd.MM.yyyy HH:mm:ss",
        dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
        showClearButton: true,
        value: DateStartCreate,
        onValueChanged: function (e) {
            $('#evelist-createenddate').val(e.value);
        }
    });

    $("#dateStartDE").dxDateBox({
        type: "datetime",
        displayFormat: "dd.MM.yyyy HH:mm:ss",
        dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
        showClearButton: true,
        onValueChanged: function (e) {
            $('#evelist-startdate').val(e.value);
        }
    });

    $("#dateEndDE").dxDateBox({
        type: "datetime",
        displayFormat: "dd.MM.yyyy HH:mm:ss",
        dateSerializationFormat: "yyyy-MM-ddTHH:mm:ss",
        showClearButton: true,
        onValueChanged: function (e) {
            $('#evelist-enddate').val(e.value);
        }
    });
});

$(document).ready(function () {
    // Фильтр
    $('.eve-list-filter').click(function () {
        let params = GetDataForFilter();
        $.ajax({
            method: 'POST',
            url: "/Events/GetEventsTable",
            data: params,
            success: function (data) {
                $('#evelist-table').html(data);
            },
            error: function () {
                alert('Ошибка получения событий');
            }
        });
    })

    // Подтвердить
    $('#ev-d-confirm').click(function () {
        ConfirmEvent($('#ev-d-eventid').val());
    });

    // заблокировать
    $('#ev-d-ban').click(function () {
        $('#ev-d-message').toggleClass('display-none');
    });

    $('body').on('change', '#ev-d-message-select', function () {
        var optionSelected = $(this).find("option:selected");
        var valueSelected = optionSelected.val();
        $('#ev-d-message-text').text(valueSelected);
    });

    $('body').on('click', '#ev-d-ban-event', function () {
        BanEvent($('#ev-d-eventid').val(), $('#ev-d-message-text').text());
    });
});

/**Получить параметры для фильтра событий */
function GetDataForFilter() {
    let isGlobal = $('#flexCheckGlobal').prop('checked');
    let onlyRequereCheck = $('#flexCheckStatus').prop('checked');
    return {
        EventCreateStartDate: $('#evelist-createstartdate').val(),
        EventCreateEndDate: $('#evelist-createenddate').val(),
        StartDate: $('#evelist-startdate').val(),
        EndDate: $('#evelist-enddate').val(),
        EventTitle: $('#EventTitle').val(),
        UserName: $('#UserName').val(),
        IsGlobal: isGlobal,
        OnlyRequereCheck: onlyRequereCheck
    }
};

/** Отправить запрос на одобрение события */
function ConfirmEvent(eventId) {
    $.ajax({
        url: '/Events/ConfirmEvent?eventId=' + eventId,
        success: function (resp) {
            if (resp.isSuccess) {
                DevExpress.ui.notify("Успешно одобрено", 'success', 2000);
            } else {
                DevExpress.ui.notify("Ошибка одобрения. " + resp.errorMessage, 'error', 2000);
                console.log("Ошибка одобрения. " + resp.errorMessage);
            }
        },
        error: function () {
            console.log('Одобрение события. Чтото пошло не так.');
        }
    });
}

/** Отправить запрос на одобрение события */
function BanEvent(eventId, message) {
    $.ajax({
        url: '/Events/BanEvent?eventId=' + 15500 + '&message=' + message,
        success: function (resp) {
            if (resp.isSuccess) {
                DevExpress.ui.notify("Успешно забанено", 'success', 2000);
            } else {
                DevExpress.ui.notify("Ошибка забанено. " + resp.errorMessage, 'error', 2000);
                console.log("Ошибка забанено. " + resp.errorMessage);
            }
        },
        error: function () {
            console.log('Бан события. Чтото пошло не так.');
        }
    });
}


