DevExpress.localization.locale("ru");
$(function () {
    $("#dateSinseDE").dxDateBox({
        type: "date",
        displayFormat: "dd.MM.yyyy",
        dateSerializationFormat: "yyyy-MM-dd",
        showClearButton: true,
        onValueChanged: function (e) {
            $('#ser-date-since').val(e.value);
        }
    });
    $("#dateDueDE").dxDateBox({
        type: "date",
        displayFormat: "dd.MM.yyyy",
        dateSerializationFormat: "yyyy-MM-dd",
        showClearButton: true,
        onValueChanged: function (e) {
            $('#ser-date-due').val(e.value);
        }
    });
});

$(document).ready(function ()
{
    $('input[type="submit"]:last').on('click', function (event)
    {
        if ($('input[name="name"]').val() == '' && $('input[name="city"]').val() == '' && $('input[name="teg"]').val() == '') {
            event.preventDefault();
            $('.validation-summary-valid:last').removeClass('validation-summary-valid');
            $('.validation-summary-errors:last').text('Ни одно из полей не заполнено.');
        }
    });
});