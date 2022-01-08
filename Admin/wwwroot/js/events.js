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
    $('.eve-list-filter').click(function () {
        let params = GetDataForFilter();
        $.ajax({
            method: 'POST',
            url: "/Events/GetGetEventsTable",
            data: params,
            success: function (data) {
                console.log('eves', data)
                $('#evelist-table').html(data);
            },
            error: function () {
                alert('Ошибка получения событий');
            }
        });
    })
});


function GetDataForFilter() {
    let isGlobal = $('#IsGlobal').prop('checked');// ? true : false;
    console.log('isGlobal')

    return {
        EventCreateStartDate: $('#evelist-createstartdate').val(),
        EventCreateEndDate: $('#evelist-createenddate').val(),
        StartDate: $('#evelist-startdate').val(),
        EndDate: $('#evelist-enddate').val(),
        EventTitle: $('#EventTitle').val(),
        UserName: $('#UserName').val(),
        IsGlobal: isGlobal,
    }
};


