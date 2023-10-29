// Copyright 2019 AppSoln Technology (https://appsoln.com)
function calcDelay(str) {
    var res = str.length * 100;
    return res;
}
var toast;
function showToast(type, message, title, subtitle, hide = false) {
    alert(title + ': ' + message);
} 
function showSuccessToast(message, title, subtitle, hide = true) {
    showToast('bg-success', message, title, subtitle, hide);
}

function showInfoToast(message, title, subtitle, hide = true) {
    showToast('bg-info', message, title, subtitle, hide);
}

function showErrorToast(message, title, subtitle, hide = true) {
    showToast('bg-danger', message, title, subtitle, hide);
}

function showWarningToast(message, title, subtitle, hide = true) {
    showToast('bg-warning', message, title, subtitle);
}

function getLoader() {
    return '<i class="fa fa-spinner fa-spin fa-lg fa-fw"></i>';//'Working...';// '<div class="lds-css ng-scope"><div style="width:100%;height:100%" class="lds-facebook"><div></div><div></div><div></div></div>';
}
function ajaxRequest(requestUrl, datafunc, beforeSend, success, error) {
    $.ajax(ajaxOptions(requestUrl, datafunc, beforeSend, success, error, 'post'));
}
function ajaxGetRequest(requestUrl, datafunc, beforeSend, success, error) {
    $.ajax(ajaxOptions(requestUrl, datafunc, beforeSend, success, error, 'get'));
}
function ajaxOptions(requestUrl, datafunc, beforeSend, success, error, type) {
    return {
        url: requestUrl,
        data: datafunc(),
        type: type,
        beforeSend: function (xhr) {
            beforeSend(xhr);
            var token = $('#apitoken').val();
            xhr.setRequestHeader("Authorization", "bearer " + token);
        },
        contentType: 'application/json',
        success: success,
        error: error
    };
}
var waiting = false;
function displayModelErrors(title, errors) {
    if (errors) {
        var errorMsg = "";
        for (var i = 0; i < errors.length; i++) {
            errorMsg += errors[i].Value[0] + '<br/>';
        }
        showErrorToast(errorMsg, title, null);
    }
}
function onClickAjaxRequest(selector, requestUrl, datafunc, success, error) {
    var control = $(selector);
    var url = requestUrl;
    var prevHTML = control.html();
    if (waiting === false) {
        waiting = true;
        ajaxRequest(url, datafunc, function () {
            control.html(getLoader());
        },
            function (data) {
                waiting = false;
                control.html(prevHTML);
                if (success)
                    success(data);
                else {
                    if (data.Code == -1)
                        showErrorToast(data.Message, 'Error', null);
                    if (data.Code == 0)
                        showSuccessToast(data.Message, 'Success', null);
                    if (data.Code == -3) {
                        if (data.ModelErrors) {
                            displayModelErrors(data.Message, data.ModelErrors);
                        }
                    }
                    if (data.Url) {
                        setTimeout(function () { window.location.href = data.Url }, 1000);
                    }
                }
            },
            function (xhr, status, error) {
                showErrorToast(err, "Server Error", null);
                console.log(xhr);
                console.log(status);
                console.log(error);
                waiting = false;
                control.html(prevHTML);
                var err = JSON.parse(xhr.responseText);
                $('.modal').modal('hide');
                $('#modalerrmsg').text(err.Message);
            }
        );
    }
    //else
    //    alert('Please wait. Another request is processing.');
}