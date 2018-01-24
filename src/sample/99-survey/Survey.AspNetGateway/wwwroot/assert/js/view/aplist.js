define(function (require, exports, module) {
    require("flexigrid");
    require("dailog");
    exports.init = function (opt) {
        var maiheight = document.documentElement.clientHeight;
        var mainWidth = document.documentElement.clientWidth - 2; // 减去边框和左边的宽度
        var otherpm = 113;
        var gh = maiheight - otherpm;
        var option = {
            height: gh,
            width: mainWidth,
            url: opt.gridUrl,
            colModel: [
                    { display: '答卷ID', name: 'apaperId', width: 50, sortable: false, align: 'left', iskey: true },
                    { display: '问卷ID', name: 'qpaperId', width: 50, sortable: false, align: 'left',hide:true },
                    { display: '问卷', name: 'qpaperSubject', width: 150, sortable: false, align: 'left' },
                    { display: '用户标识', name: 'userId', width: 100, sortable: false, align: 'left' },
                    { display: '答卷时间', name: 'createTime', width: 150, sortable: false, align: 'left' },
                    { display: '操作', name: 'apaperId', width: 120, sortable: false, align: 'center', process: formatOp, toggle: false }
            ],
            preProcess:processGridJson,
            sortname: "",
            sortorder: "",
            title: false,
            rp: 20,
            usepager: true,
            showcheckbox: true
        };

        $("#gridList").flexigrid(option);
        function formatOp(value, id, cell) {
            var a = [];
            a.push("<a class='imgbtn' href=\"javascript:void(0);\" onclick=\"javascript:_utils.veiwapaper('", value, "')\" title='查看答卷'><span class='view'>查看</span></a>");
            return a.join("");
        }

        function formatSubject(value, id,cell) {
            var a = [];
            a.push("<a class='imgbtn' href=\"javascript:void(0);\" onclick=\"javascript:_utils.view('", cell[1], "')\" title='查看问卷'> [", cell[1] , "]&nbsp;", value, "</a>");
            return a.join("");
        }
        //
        $("#qtext").keypress(function (e) {
            if (e.keyCode == 13) {
                Query();
            }
        });
        $("#abtnQuery").click(Query);
        function Query() {
            var v = $("#qtext").val();
            var p = { extParam: [{ name: "qtext", value: v}] };
            $("#gridList").flexOptions(p).flexReload();
        }

        _utils.view = function (id) {
            var url = opt.viewUrl + "?id=" + id;
            window.open(url);
        }
        _utils.veiwapaper = function (id) {
            var url = opt.viewPaperUrl + "?id=" + id;
            window.open(url);
        }

    }

});
