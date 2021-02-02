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
            var date = Date.parse(e.value);
            console.log(date);
            date = new Date(date += (1 * 24 * 60 * 60 * 1000));
            console.log(date.toDateString());
            let dateStr = `${date.getFullYear()}-${(date.getMonth() + 1) < 10 ? ('0' + date.getMonth() + 1) : (date.getMonth() + 1)}-${(date.getDate()) < 10 ? ('0' + date.getDate()) : (date.getDate())}`;
            console.log(dateStr)
            $('#ser-date-due').val(dateStr);
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