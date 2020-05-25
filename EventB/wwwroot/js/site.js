$(document).ready(function () {
    // Передвижение меню при скролле.
    $(window).scroll(function () {       
        if ($(window).scrollTop() > $('#title-container').height()+16) {
            $('.main-menu-container').addClass('float-menu');
            $('#menu-voider').removeClass('display-none');            
            $('#menu-voider').css('height', $('.main-menu-container').height());
        }
        else {
            $('.main-menu-container').removeClass('float-menu');
            $('#menu-voider').addClass('display-none');
            $('#menu-voider').css('height', '0');
        }
    });
});

