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
        // let eveType = $(this).parent().children('.lk-event-state').text();
        // Удалить при имплементации реального апи
        let type = 0;
        //let type = eveType.indexOf('Global') === -1 ? 0 : 1;

        // Запрос на изменение статуса.
        function changeStatus(){
            window.open('/MarketRoom/Pay');
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
        getModelWindow('PRESSED', true, changeStatus);
        // Контент блока модалки.
        let content =`<div class="lk-sale-message flex-vsc">
            <p>Сейчас ваше событие находится в статусе "Private". Это значит, что оно ну будет отображатся в списке рекомендаций по интересам, дате и локации.</p>
            <p>Вы можете перевести это событие  статус "Global" оно будет попадать в список рекомендаций.</p>
            <p>После нажатия кнопки ОК, вы будете перенаправлены на окно оплаты. После поступления оплаты событие сразу будет попадать в выборки для показа.</p>
            <img src="/resourses/sale.png" alt="sale" />
            <p>Внимание! Сейчас действует специальная цена: 0р. на публикацию глобальных событий. И даже на форму оплаты переходить не прийдется.</p>
            <p>Специальная цена действует до 31.10.20</p></div>
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
                    alert('Событие не удалено. Запрещено дибо не доступно.');
                }
            });
        }
        
        let data = { id: eveId, container: eventContainer }
        console.log(data)
        getModelWindow('Удаление события', true, onConfirmDelete);
        let content = `<h3>Внимание!</h3>
            <p>Удалив данное событие вы не сможете его восстановить.</p>
            <p>также будут удалены все отметки о визитах и чат.</p>`;
        $('.modal-body').html(content);
    });
});