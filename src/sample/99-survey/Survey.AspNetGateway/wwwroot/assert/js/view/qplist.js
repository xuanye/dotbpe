define(function (require, exports, module) {
    require("flexigrid");
    require("dailog");
    exports.init = function (opt) {
        var maiheight = document.documentElement.clientHeight;
        var mainWidth = document.documentElement.clientWidth - 2; // 减去边框和左边的宽度
        var otherpm = 113;
        var gh = maiheight - otherpm;
        //"qpaperId": 1, "subject": "test1", "startTime": "", "endTime": "", "description": "啊啊啊啊啊", "createUserId": ""
        var option = {
            height: gh,
            width: mainWidth,
            url: opt.gridUrl,
            colModel: [
                    { display: '问卷ID', name: 'qpaperId', width: 60, sortable: false, align: 'left', iskey: true },
                    { display: '主题', name: 'subject', width: 150, sortable: false, align: 'left', process: formatSubject },
                    { display: '说明', name: 'description', width: 250, sortable: false, align: 'left' },
                    { display: '答卷数', name: 'apaperCount', width: 50, sortable: false, align: 'center', process: formatAPC },
            // { display: '状态', name: 'Status', width: 50, sortable: false, align: 'left', hide: true, process: formatState },
                    {display: '操作', name: 'qpaperId', width: 220, sortable: false, align: 'center', process: formatOp, toggle: false }
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
            a.push("<a class='imgbtn' href=\"javascript:void(0);\" onclick=\"javascript:_utils.edit('", value, "',", cell[3], ")\" title='修改'><span class='edit'>修改</span></a>");
            a.push("<a class='imgbtn' href=\"javascript:void(0);\" onclick=\"javascript:_utils.viewstatistic('", value, "')\" title='统计'><span class='statistic'>统计</span></a>");
            return a.join("");
        }

        function formatSubject(value, id) {
            var a = [];
            a.push("<a class='imgbtn' href=\"javascript:void(0);\" onclick=\"javascript:_utils.view('", id, "')\" title='查看问卷详细'>", value, "</a>");
            return a.join("");
        }
        function formatAPC(value, id, cell) {
            var url = opt.alistUrl + "?id=" + id;
            var a = [];
            a.push("<a href=\"", url, "\" title='查看答卷列表'>", value, "</a>");
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
            var p = { extParam: [{ name: "subject", value: v}] };
            $("#gridList").flexOptions(p).flexReload();
        }
        $("#btnAdd").click(function (e) {
            var url = opt.editUrl;
            OpenModalDialog(url, { caption: "新增问卷", type: 2, width: 700, height: maiheight - 50, onclose: function () {
                $("#gridList").flexReload();
            }
            });

        });
        _utils.edit = function (id, state) {
            if (state > 0) {
                alert("问卷已存在答卷,不能被修改");
                return;
            }
            var url = opt.editUrl + "?id=" + id;
            OpenModalDialog(url, { caption: "修改调查问卷", type: 2, width: 700, height: maiheight - 50, onclose: function () {
                $("#gridList").flexReload();
            }
            });
        }
        _utils.view = function (id) {
            var url = opt.viewUrl + "?id=" + id;
            window.open(url);
        }
        _utils.viewstatistic = function (id) {
            var url = opt.statisticUrl + "?id=" + id;
            OpenModalDialog(url, { caption: "答卷统计", type: 2, width: 700, height: maiheight - 50, onclose: function () {
            }
            });
        }
        _utils.viewadetaillist = function (id) {
            var url = opt.answerListUrl + "?id=" + id;
            window.open(url);
        }
    }

});
