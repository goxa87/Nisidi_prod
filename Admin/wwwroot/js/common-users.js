import { ExecuteAjaxRequest } from './requestTool.js';

$(document).ready(function () {
    // Подтвердить почту
    $('#cu-det-confirm-email').click(function () {
        confirmUserEmail();
    });

    //заблокировать пользователя
    $('#cu-det-toggle-block').click(function () {
        blockUser();
    });

    //показать созданные пользователем события
    $('#cu-det-show-created').click(function () {
        getAndShowUserEvents();
    });
});

/**Подтвердить почту */
function confirmUserEmail() {
    let data = {
        userId: $('#cu-det-userid').val()
    };
    ExecuteAjaxRequest('/CommonUsers/ConfirmUserEmail', 'get', data, true, null);
}

/**заблокировать пользователя */
function blockUser(){
    let data = {
        userId: $('#cu-det-userid').val()
    };
    ExecuteAjaxRequest('/CommonUsers/ToggleBlockUser', 'get', data, true, null);
}

/**показать созданные пользователем события */
function getAndShowUserEvents() {
    let data = {
        userId: $('#cu-det-userid').val()
    };

    $.ajax({
        url: '/CommonUsers/GetCreatedEventsByuser',
        data: data,
        success: function (content) {
            $('#cu-det-events-container').toggleClass('display-none');
            $('#cu-det-events-list').html(content);
        },
        error: function (jqXHR) {
            console.log(jqXHR);
        }
    })
}
