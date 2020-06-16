$(document).ready(function ()
{
    // Переключение вкладок.
    $('#lk-kibnet-selector').click(() => {
        $('.lk-selector').removeClass('selected-selector');
        $('#lk-kibnet-selector').addClass('selected-selector');
        $('.lk-page').addClass('display-none');
        $('#lk-kibnet-page').removeClass('display-none');
    });
    $('#lk-event-selector').click(() => {
        $('.lk-selector').removeClass('selected-selector');
        $('#lk-event-selector').addClass('selected-selector');
        $('.lk-page').addClass('display-none');
        $('#lk-event-page').removeClass('display-none');
    });
    $('#lk-cards-selector').click(() => {
        $('.lk-selector').removeClass('selected-selector');
        $('#lk-cards-selector').addClass('selected-selector');
        $('.lk-page').addClass('display-none');
        $('#lk-cards-page').removeClass('display-none');
    });
    $('#lk-banners-selector').click(() => {
        $('.lk-selector').removeClass('selected-selector');
        $('#lk-banners-selector').addClass('selected-selector');
        $('.lk-page').addClass('display-none');
        $('#lk-banners-page').removeClass('display-none');
    });
});