var AjaxParam = function (object) {
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
};

var SendAjax = function (url, obj, callback) {
    if (url !== undefined) {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', url, true);
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');

        xhr.onload = function () {
            if (xhr.status === 200) {
                if (callback !== undefined) {
                    try {
                        callback(JSON.parse(xhr.responseText));
                    }
                    catch (error) {
                        callback(xhr.responseText);
                    }
                }
            }
            else {
                alert("Ajax Request Error. Status " + xhr.status);
            }
        };

        if (obj !== undefined) {
            xhr.send(encodeURI(AjaxParam(obj)));
        }
        else {
            xhr.send();
        }
    }
    else {
        alert("No url!");
    }
};