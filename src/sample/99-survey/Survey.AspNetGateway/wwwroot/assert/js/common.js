/*********************动态载入JS Satrt************************/
function ansyloadJS(url, onload) {
    var domscript = document.createElement('script');
    domscript.src = url;
    if (!!onload) {
        domscript.afterLoad = onload;
        domscript.onreadystatechange = function () {
            if (domscript.readyState == "loaded" || domscript.readyState == "complete" || domscript.readyState == "interactive") {
                domscript.afterLoad();
            }
        }
        domscript.onload = function () {
            if (!!domscript.afterLoad)
                domscript.afterLoad();
        }
    }
    document.getElementsByTagName('head')[0].appendChild(domscript);
}
/*********************动态载入JS End************************/

function OpenModalDialog(url, option) {
    if ($.ShowIfrmDailog) {
        $.ShowIfrmDailog(url, option);
    }
}
function CloseModalDialog(callback, dooptioncallback, userstate) {
    if (parent && parent.$.closeIfrm) {
        parent.$.closeIfrm(callback, dooptioncallback, userstate);
    }
}


function StrFormat(temp, dataarry) {
    return temp.replace(/\{([\d]+)\}/g, function (s1, s2) { var s = dataarry[s2]; if (typeof (s) != "undefined") { if (s instanceof (Date)) { return s.getTimezoneOffset() } else { return encodeURIComponent(s) } } else { return "" } });
}
function StrFormatNoEncode(temp, dataarry) {
    return temp.replace(/\{([\d]+)\}/g, function (s1, s2) { var s = dataarry[s2]; if (typeof (s) != "undefined") { if (s instanceof (Date)) { return s.getTimezoneOffset() } else { return (s); } } else { return ""; } });
}
function guidGenerator() {
    var S4 = function () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    };
    return (S4() + S4() + S4() + S4() + S4() + S4() + S4() + S4());
}
function idGenerator() {
    var S4 = function () {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    };
    return (S4() + S4() + S4() + S4() );
}

function StrToDate(date) {
    var reg = /^(\d{1,4})(-|\/|.)(\d{1,2})\2(\d{1,2})(\040+(\d{1,2}):(\d{1,2}):(\d{1,2}))?$/;
    var arr = date.match(reg);
    if (arr == null) {
        return NaN;
    }
    else {
        return new Date(arr[1], parseInt(arr[3], 10) - 1, arr[4], arr[6] || 0, arr[7] || 0, arr[8] || 0);
    }
}
function buildAjaxHandlerUrl(controller, action, paras) {
    var url = "/{0}/{1}.do{2}";
    var qstring = [];
    if (paras) {
        qstring.push("?");
        for (var a in paras) {
            qstring.push(a, "=", encodeURIComponent(paras[a]));
        }
    }
    return StrFormatNoEncode(url, [controller, action, qstring.join("")]);
}

function FormSubmit(form, callback) {
    $.ajax({
        url: form.action,
        type: form.method,
        data: $(form).serialize(),
        success: function (result) {
            callback(result);
        },
        error: function (result) {
            console.error(result);
            alert("提交表单失败：" + result);
        }
    });

}

function post(url, data, callback, failcallback) {
    $.ajax({
        url: url,
        type: 'POST',
        data: data,
        success: callback,
        error: failcallback || function (request, status, error) {
            alert('异步请求发生异常，请刷新后重试' + error);
        }
    });
}

function get(url, data, callback, failcallback) {
    $.ajax({
        url: url,
        type: 'GET',
        data: data,
        success: callback,
        error: failcallback || function (request, status, error) {
            alert('异步请求发生异常,请刷新后重试');
        }
    });
}

function processGridJson(res,option){
    var data = {};

    var fields = option.cols.split(",");
    var keyFiled =  option.colkey ;
    if(res.return_code == 0){
        data.total = -1;
        data.page = option.page;
        data.rows =[];

        if(!res.data){
            return data;
        }

        data.total = res.data.total;
        if(res.data.list){
            for(var i = 0 ,l = res.data.list.length ; i<l ;i++){
                var item = res.data.list[i];
                var row = {cell:[]};
                row.id = item[keyFiled] || "";
                for(var j=0,k=fields.length ;j<k;j++){
                    row.cell.push(item[fields[j]] ) //字段名
                }
                data.rows.push(row);
            }
        }

    }
    else{
        data.error = res.return_message|| "意外错误";
    }
    return data;
}

function getUrlParameter(name) {
    name = name.replace(/[\[]/, '\\[').replace(/[\]]/, '\\]');
    var regex = new RegExp('[\\?&]' + name + '=([^&#]*)');
    var results = regex.exec(location.search);
    return results === null ? '' : decodeURIComponent(results[1].replace(/\+/g, ' '));
};
