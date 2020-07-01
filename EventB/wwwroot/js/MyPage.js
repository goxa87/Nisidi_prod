$(document).ready(function () {
    // Кнпоки Нижнего меню.
    $('.mp-my-selector').on('click', function () {
        $('.selected-label').removeClass('selected-label');
        $(this).addClass('selected-label');
    });
    // Показ Пойду.
    $('#events-willgo').click(function () {
        $('.mp-my-content').addClass('display-none');
        $('.mp-my-content').removeClass('display-block');
        $('#events-willgo-body').removeClass('display-none');
        $('#events-willgo-body').addClass('display-block');
    });
    // Показ Мои события.
    $('#my-events').click(function () {

        $('.mp-my-content').addClass('display-none');
        $('.mp-my-content').removeClass('display-block');
        $('#my-events-body').removeClass('display-none');
        $('#my-events-body').addClass('display-block');
    });
    // Показ Друзей.
    $('#friends').click(function () {
        $('.mp-my-content').addClass('display-none');
        $('.mp-my-content').removeClass('display-block');
        $('#friends-body').removeClass('display-none');
        //$('#friends-body').addClass('');
    });
    // Показ приглашения.
    $('#invites').click(function () {
        $('.mp-my-content').addClass('display-none');
        $('.mp-my-content').removeClass('display-block');
        $('#invites-body').removeClass('display-none');
        $('#invites-body').addClass('display-block');
    });
    // Нажате на кнопку пойду в контейнере приглашения.
    $('#will-go').click(function ()
    {
        //[Route("SubmitInvite")]
        //public async Task SubmitInvite(int eventId, int inviteId)
        let eId = $(this).parent().children('#eveId').val();
        let invId = $(this).parent().children('#inviteId').val();
        $.ajax({
            url: '/MyPage/SubmitInvite',
            data: {
                eventId: eId,
                inviteId: invId
            },
            success: function () {
                $(this).parents('.total-invite').fadeOut(600);
            },
            error: data => { console.log(data);}
        });
    });

    // Нажате на кнопку НЕпойду в контейнере приглашения.
    $('#will-notgo').click(function () {
         //[Route("RefuseInvite")]
        //public async Task RefuseInvite(int inviteId)
        let invId = $(this).parent().children('#inviteId').val();
        $.ajax({
            url: '/MyPage/RefuseInvite',
            data: {
                inviteId: invId
            },
            success: function () {
                $(this).parents('.total-invite').fadeOut(600);
            },
            error: data => { console.log(data); }
        });
    });
});



