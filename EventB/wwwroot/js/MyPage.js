console.log('log');
$('.mp-my-selector').on('click', function () {
    //$('.selected-label').each(function () { $(this).css('color','#e4dcc5') });
    $('.selected-label').removeClass('selected-label');

    $(this).addClass('selected-label');
    $('.mp-my-content').toggleClass('display-none');
    console.log('clicked');
});
