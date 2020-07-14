function SendAjax(obj, callback, url) {
    if (url === undefined) url = "/Form/Common/AjaxAuto.aspx";
    var xhr = new XMLHttpRequest();
    xhr.open('POST', url, true);
    xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
    xhr.onload = function () {
        if (xhr.status === 200) {            
            try {
                callback(JSON.parse(xhr.responseText));
            } catch(error){
                callback(xhr.responseText);
            }
        }
        else {
            alert("Ajax Request Error. Status " + xhr.status);            
        }
    };
    xhr.send(encodeURI(param(obj)));
}

function param(object) {
    var encodedString = '';
    for (var prop in object) {
        if (object.hasOwnProperty(prop)) {
            if (encodedString.length > 0) {
                encodedString += '&';
            }
            encodedString += encodeURI(prop + '=' + object[prop]);
        }
    }
    return encodedString;
}