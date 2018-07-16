function setCookie(key, value, time_second) {
    var expires = new Date();
    expires.setTime(expires.getTime() + (time_second * 1000));
    document.cookie = key + '=' + value + ';expires=' + expires.toUTCString();
}


function ExecuteServiceSyns(urli, type, callback, isJson) {
    try {
        var lstData = null;
        var syns = false;
        if (callback != null && callback != '')
            syns = true;
        if (isJson)

            $.ajax({
                url: urli,
                type: type,
                contentType: "application/json; charset=utf-8",
                //beforeSend: function (xhr) {
                //    xhr.setRequestHeader("access_token", "Basic fff" );
                //    xhr.setRequestHeader("X-Mobile", "false");
                //},
                //crossDomain: true,
                //headers: headers,
                //xhrFields: {
                //    withCredentials: true
                //},
                cache: false,
                crossDomain: true,
                dataType: "json",
                //data: JSON.stringify({ para: "123", config: "345" }),
                async: syns,
                success: function (data, textStatus, jqXHR) {
                    if (syns)
                        eval(callback)(data);
                    else {
                        lstData = data;
                        return data;
                    }
                },
                complete: function () {
                },
                error: function (jqXHR, textStatus, errorThrown) {
                }
            });
        else
            $.ajax({
                url: urli,
                type: type,
                data: "user_id=dfsf&token=gasd",
                async: syns,
                datatype: "json",
                contentType: "application/x-www-form-urlencoded; charset=utf-8",
                success: function (data, textStatus, jqXHR) {
                    if (syns)
                        eval(callback)(data);
                    else {
                        lstData = data;
                        return data;
                    }
                },
                complete: function () {
                },
                error: function (jqXHR, textStatus, errorThrown) {
                }
            });
        return lstData;
    } catch (e) {
        console.log(e);
        return null;
    }
}



function ExecuteServicePostForm(idForm,urli,callback) {
    try {
        var lstData = null;
        var syns = false;
        if (callback != null && callback != '')
            syns = true;

        $.ajax({
            url: urli,
            type: "POST",
            data: new FormData(document.getElementById(idForm)),
            contentType: !1,
            cache: !1,
            syns: syns,
            processData: !1,
                 success: function (data, textStatus, jqXHR) {
                    console.log(data);
                    if (syns)
                        eval(callback)(data);
                    else {
                        lstData = data;
                        return data;
                    }
            },
            error: function (jqXHR, textStatus, errorThrown) {
                console.log(jqXHR);
                console.log(textStatus);
                console.log(errorThrown);
                if (syns)
                    eval(callback)(null);
                else {
                    lstData = null;
                    return null;
                }
                }
        });
            //$.ajax({
            //    url: urli,
            //    type: type,
            //    data: new FormData(document.getElementById(idForm)),
            //    async: syns,
            //    datatype: "json",
            //    contentType: "application/x-www-form-urlencoded; charset=utf-8",
            //    success: function (data, textStatus, jqXHR) {
            //        console.log(data);
            //        if (syns)
            //            eval(callback)(data);
            //        else {
            //            lstData = data;
            //            return data;
            //        }
            //    },
            //    complete: function () {
            //    },
            //    error: function (jqXHR, textStatus, errorThrown) {
            //    }
            //});
        return lstData;
    } catch (e) {
        console.log(e);
        return null;
    }
}



function getTimeToday(date) {
   
    var day = date.getDate();
    var m = (date.getMonth() + 1);
    var y = date.getFullYear();
    return (day.toString().length == 1 ? ("0" + day + "-") : (day + "-"))
        + (m.toString().length == 1 ? ("0" + m + "-"): (m + "-"))
        + date.getFullYear();
}

function dateToTimestamp(date) {
    return date.getTime();
};

function stringToDate(dateStr) {
        var parts = dateStr.split("-")
        return new Date(parts[2], parts[1] - 1, parts[0])
}

function Base64Encode(str, encoding = 'utf-8') {
    var bytes = new (TextEncoder || TextEncoderLite)(encoding).encode(str);
    return base64js.fromByteArray(bytes);
}

function Base64Decode(str, encoding = 'utf-8') {
    var bytes = base64js.toByteArray(str);
    return new (TextDecoder || TextDecoderLite)(encoding).decode(bytes);
}