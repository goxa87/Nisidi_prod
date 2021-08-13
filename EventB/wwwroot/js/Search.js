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