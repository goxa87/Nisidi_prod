// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(function () {
    $('.eventlist-btn-come').on('click', function () {
        
        $(this).parent().children('.will-go-img').css('visibility', 'visible');
    });

    $('.eventitem-vizitors-list-arrow').on('click', function () {

        console.log($('.eventitem-vizitors-list-body').css('display'));
        
        if ($('.eventitem-vizitors-list-body').css('display')=='none')
        {
            $('.eventitem-vizitors-list-body').css({ 'display': 'block' });
            $(this).css({ 'transform': 'rotate(90deg)' });
        }
        else
        {
            $('.eventitem-vizitors-list-body').css({ 'display': 'none' });
            $(this).css({ 'transform': 'rotate(0deg)' });
        }
    });

});
