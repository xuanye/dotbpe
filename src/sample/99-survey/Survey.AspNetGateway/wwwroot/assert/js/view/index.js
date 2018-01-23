define(function (require, exports, module) {
    require("tree");
    require("tabpanel");
    var menudata = [{
        "id": "1",
        "text": "问卷调查",
        "value": "",
        "showcheck": false,
        "isexpand": true,
        "checkstate": 0,
        "hasChildren": true,
        "complete": true,
        "classes": null,
        "data": null,
        "ChildNodes": [{
            "id": "1.1",
            "text": "问卷管理",
            "value": "/html/qpaperlist.html",
            "showcheck": false,
            "isexpand": false,
            "checkstate": 0,
            "hasChildren": false,
            "complete": true,
            "classes": null,
            "data": null,
            "ChildNodes": []
        }, {
            "id": "1.2",
            "text": "答卷列表",
            "value": "/html/apaperlist.html",
            "showcheck": false,
            "isexpand": false,
            "checkstate": 0,
            "hasChildren": false,
            "complete": true,
            "classes": null,
            "data": null,
            "ChildNodes": []
        }]
    }]

    exports.init = function(){
        checkLogin(loadLayout)
    }

    var header = 43;
    var bottom = 3;

    function loadLayout() {


        var mainh = document.documentElement.clientHeight;
        var mainw = document.documentElement.clientWidth;

        var op = {
            data: menudata,
            onnodeclick: navi
        };
        $("#treepanel").treeview(op);
        var tabo = {};
        tabo.items = [{
            id: "home",
            text: "主页",
            classes: "icon_home",
            isactive: false,
            content: "<div style='padding:10px;'>欢迎光临!</div>"
        }];
        tabo.width = mainw - 196;
        tabo.height = mainh - header - bottom - 1;
        $("#tab-container").tabpanel(tabo);

        setpanelsize();

    }
    function navi(item) {
        if (item.value) {
            $("#tab-container").opentabitem({
                id: idreplace(item.id),
                text: item.text,
                url: item.value,
                isactive: true,
                closeable: true
            }, true);
        } else {
            item.expand();
        }
    }

    function idreplace(id) {
        return id.replace(/[^a-zA-Z\d_]/ig, "_")
    }


    function setpanelsize() {
        var panelheader = $("#panel-header").outerHeight();
        mainh = document.documentElement.clientHeight;
        mainw = document.documentElement.clientWidth;
        var h = mainh - header - panelheader - bottom;
        var w = mainw - 196;
        $("#panel-wrap").height(h);
        $("#tab-container").width(w);
    }
    $(window).resize(window_resize);

    function window_resize() {
        setpanelsize();
        $("#tab-container").resizetabpanel(mainw - 196, mainh - header - bottom - 1);
    }

    function checkLogin(callback){
        $.ajax({
            type: 'GET',
            url: '/api/gate/check',
            dataType: "json",
            success: function (res) {
                if (res && res.return_code == 0) {
                   $("#fullname").text(res.data.fullName);
                   callback();
                }
                else {
                   location.href = "/html/login.html"
                }
            },
            error: function (err) { location.href = "/html/login.html" }
        });
    }
});
