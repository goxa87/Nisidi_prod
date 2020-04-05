/// <reference path="mypage.js" />
$('.btn-submit').on('click', function ()
{
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
        fail: function ()
        {
            console.log("ОШИБКА ЗАГРУЗКИ");
        }
    });
});