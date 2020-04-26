$('.mp-my-selector').on('click', function () {
    $('.selected-label').removeClass('selected-label');
    $(this).addClass('selected-label');
});

$('#events-willgo').click(function ()
{
    $('.mp-my-content').addClass('display-none');
    $('.mp-my-content').removeClass('display-block');
    $('#events-willgo-body').removeClass('display-none');
    $('#events-willgo-body').addClass('display-block');
});

$('#my-events').click(function () {

    $('.mp-my-content').addClass('display-none');
    $('.mp-my-content').removeClass('display-block');
    $('#my-events-body').removeClass('display-none');
    $('#my-events-body').addClass('display-block');
});

$('#friends').click(function () {
    $('.mp-my-content').addClass('display-none');
    $('.mp-my-content').removeClass('display-block');
    $('#friends-body').removeClass('display-none');
    $('#friends-body').addClass('display-block');
});

$('#invites').click(function () {
    $('.mp-my-content').addClass('display-none');
    $('.mp-my-content').removeClass('display-block');
    $('#invites-body').removeClass('display-none');
    $('#invites-body').addClass('display-block');
});
