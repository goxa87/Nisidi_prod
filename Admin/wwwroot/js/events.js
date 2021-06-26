
$(document).ready(function () {
    console.log('Executed');
    getEventsMarkup(null, placeMarkup);
});

function getRequestBodyForEventsMarkup() {

}

function getEventsMarkup(param, callback) {
    $.post({
        url: '/Events/get-events-page',
        data: param,
        success: function (data) {
            console.log('data1', data)
            callback(data);
        },
        error: function () {
            console.log('Error ...');
        }
    })
}

function placeMarkup(markup) {
    $('#ev-page-content').html(markup);
}

