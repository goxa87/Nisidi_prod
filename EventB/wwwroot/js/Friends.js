$(document).ready(function ()
{
    $('.btn-submit').on('click', function () {
        console.log('pressed');
        let friendId = $(this).parent().children('input').val();
        console.log(friendId);

        $.ajax({

            url: '/Api/AddFriend',
            data: {
                userId: friendId
            },
            success: function (responce) {
                console.log('отправлено');
            },
            fail: function () {
                console.log("ОШИБКА ЗАГРУЗКИ");
            }
        });
    });

    $('.search-submit').click(function (event) {
        //event.preventDefault();
        if ($('.search-entry').val() == '') {
            event.preventDefault();
        }
    });

    // блокировка пользователя
    $('#btn-block').click(function (event) {
        event.preventDefault();

    });
});
