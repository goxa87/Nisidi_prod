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
    // Кнопка сменить статус.
    $('.right-columnn').on('click', '.lk-change-event-status', function () {        
        let eveId = $(this).parent().children('.kibnet-event-id').val();
        let eveType = $(this).parent().children('.lk-event-state').text();
        let type = eveType.indexOf('Global') === -1 ? 0 : 1; 

        $.ajax({
            url: '/api/MarketKibnet/change-event-type',
            data: {
                targetType: type,
                eventId: eveId
            },
            success: () => {
                let newLabel = type === 0 ? 'Статус: Global' : 'Статус: Private'
                $(this).parent().children('.lk-event-state').text(newLabel);
                alert('ИЗМЕНЕНО');
            },
            error: function () {
                alert('Не получилось изменить статус');
            }
        });
    });
});