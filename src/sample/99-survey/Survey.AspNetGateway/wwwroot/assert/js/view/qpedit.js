define(function (require, exports, module) {
    var __itemtmp = "<div class='qitem line' id='{2}' qtype='{3}' qnic='{4}'><a class='remove'></a><a class='edit'></a><h6>{0}</h6><div class='rowctl'>{1}</div></div>";
    var __mctmp = "<div class='atomctl'><label class='checkbox'><input type='checkbox' value='{0}'/>{1}</label></div>";
    var __sctmp = "<div class='atomctl'><label class='checkbox'><input type='radio' value='{0}' name='{2}_sc'/>{1}</label></div>";
    var __intmp = "<div class='atomctl'><textarea class='qitemdetail'></textarea></div>";
    var __nictmp = "<div class='atomctl'><input type='text' class='nic'/></div>";
    exports.init = function () {

        initData();

        $(".line").live({
            mouseover: function () {
                $(this).addClass("line_hover");
            },
            mouseout: function () {
                $(this).removeClass("line_hover");
            }
        });
        $("a.mclose").click(function (e) {
            CloseModalDialog(null, false, null);
        });
        $("#editItemType_p .qtypecheck").each(function (i) {
            $(this).click(function () {
                $("#editItemType_p .checked").removeClass("checked");
                $(this).addClass("checked");
                $("#hdQType").val(i);
                if (i == 2) {
                    $("#input_p").hide();
                    $("#needcinput_p").hide();
                }
                else {
                    $("#input_p").show();
                    $("#needcinput_p").show();
                }
            });
        });
        $("div.qitem>a.edit").live("click", function () {
            //判断是否处于编辑状态
            var isshow = $("#qitemedit").css("display") != "none";
            if (isshow) {
                alert("请先结束当年编辑项")
                return;
            }
            var qitem = $(this).parent();
            var qtype = parseInt(qitem.attr("qtype"));
            var qnic = parseInt(qitem.attr("qnic"));
            qitem.hide();
            //重置数据
            $("#hdEditId").val(qitem.attr("id"));
            $("#hdQType").val(qtype); //问题类型

            $("#editItemType_p .qtypecheck").each(function (i) {
                if (i == qtype) {
                    $(this).addClass("checked");
                }
                else {
                    $(this).removeClass("checked");
                }
            });
            if (qtype == 0 || qtype == 1) {
                var arrdetail = [];
                $(".atomctl", qitem).each(function (i) {
                    var itext = $(this).text().replace(/(^\s*)|(\s*$)|(\n*)|(\r*)/g, "");
                    arrdetail.push(itext);
                });
                $("#qdetail").val(arrdetail.join('\r'));
                $("#input_p").show();
                $("#needcinput_p").show();
                if (qnic == 1) {
                    $("#needcinput")[0].checked = true;
                }
                else {
                    $("#needcinput")[0].checked = false;
                }

            }
            else {
                $("#input_p").hide();
                $("#needcinput_p").hide();
            }
            $("#qitemsubject").val($("h6", qitem).text());

            qitem.after($("#qitemedit").show());
        });
        $("div.qitem>a.remove").live("click", function () {
            $(this).parent().detach();
        });
        // 完成添加或者编辑
        $("#btnEndAdd").click(function (e) {
            //debugger;
            var isshow = $("#qitemedit").css("display") != "none";
            if (isshow) { // 处于编辑状态
                //判断是更新还是删除
                var eid = $("#hdEditId").val();
                var qtype = $("#hdQType").val(); //问题类型
                var qdetail = $("#qdetail").val();
                var subject = $("#qitemsubject").val();

                var qnic = $("#needcinput")[0].checked ? 1 : 0;

                var delist = [];
                var arrqdetail = qdetail.split(/[\r\n]+/);
                if (qtype == "0") {
                    var tempid = idGenerator();
                    for (var i = 0, l = arrqdetail.length; i < l; i++) {
                        //TODO:排除空行
                        delist.push(StrFormatNoEncode(__sctmp, [Math.pow(2, i), arrqdetail[i], tempid]));
                    }
                }
                else if (qtype == "1") {
                    for (var i = 0, l = arrqdetail.length; i < l; i++) {
                        //排除空行
                        delist.push(StrFormatNoEncode(__mctmp, [Math.pow(2, i), arrqdetail[i]]));
                    }
                }
                else {
                    delist.push(__intmp);
                }
                if (qnic == 1) {
                    delist.push(__nictmp);
                }
                if (eid != "") { //更新
                    var editelement = $("#" + eid);
                    editelement.attr("qtype", qtype);
                    editelement.attr("qnic", qnic);
                    $("h6", editelement).text(subject);
                    $(".rowctl", editelement).html(delist.join(""));
                    editelement.show();
                }
                else {  // 新增
                    var addhtml = StrFormatNoEncode(__itemtmp, [subject, delist.join(""), idGenerator(), qtype, qnic]);
                    $("#qitemedit").before(addhtml);
                }
                //把自己隐藏掉
                $("#qitemedit").hide();
            }
        });

        //添加问题
        $("#btnAddQuestion").click(function (e) {
            var isshow = $("#qitemedit").css("display") != "none";
            if (!isshow) {
                $("#hdEditId").val("");
                $("#hdQType").val("0"); //问题类型
                $("#needcinput")[0].checked = false;
                $("#editItemType_p .qtypecheck").each(function (i) {
                    if (i == 0) {
                        $(this).addClass("checked");
                    }
                    else {
                        $(this).removeClass("checked");
                    }
                });
                $("#qdetail").val("");
                $("#qitemsubject").val("无标题问题");

                $("#qitemlist").append($("#qitemedit").show());

            }
            else {
                alert("请先结束当前编辑状态")
            }
        });
        //编辑时的删除按钮
        $("#btnitemremove").click(function () {
            var eid = $("#hdEditId").val();
            if (eid != "") { //更新
                $("#" + eid).detach();
            }
            $("#qitemedit").hide();
        });

        $("#btnSave").click(function () {
            var id = getUrlParameter("id");

            var qpaper = {};
            qpaper.qpaperId = id || 0;
            qpaper.subject = $("#subject").val();
            qpaper.description = $("#description").val();
            qpaper.questions = [];

            //获取明细数据
            $("#qitemlist div.qitem").each(function (i) {
                var qitem = {};
                qitem.id = this.id;
                qitem.topic = $("h6", this).text();
                qitem.questionType = parseInt($(this).attr("qtype"));
                qitem.extendInput = parseInt($(this).attr("qnic")) ==1;
                if (qitem.questionType != 2) {
                    var arrdetail = [];
                    $("label.checkbox", this).each(function (i) {
                        arrdetail.push($(this).text().replace(/(^\s*)|(\s*$)|(\n*)|(\r*)/g, ""));
                    });
                    qitem.itemDetail = arrdetail.join('\r');
                }
                qpaper.questions.push(qitem);
            });
            //表单验证
            if (qpaper.subject == "") {
                qpaper.subject = "无标题问卷";
            }
            if (qpaper.description == "问卷说明") {
                qpaper.description = "";
            }
            if (qpaper.questions && qpaper.questions.length > 0) {
                // json to str ;

                //post to server
                var url = $("#fmEdit").attr("action");
                //alert(url);
                //return;
                $.ajax({
                    type: "POST",
                    url: url,
                    data:JSON.stringify(qpaper),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (res) {
                        if (res.return_code == 0) {
                            alert("操作成功！");
                            CloseModalDialog(null, true, null);
                        }
                        else {
                            alert("操作失败！\r\n" + ret.Msg);
                        }
                    },
                    error: function (err) { }
                });
            }
            else {
                alert("该问卷没有任何问题！你要闹哪样啊");
            }
        });
    }


    function initData(){
        var id = getUrlParameter("id");
        if(id){ // 编辑
            get("/api/qpaper/get?qpaperId="+id,{},function(res){
                if(res.return_code ==0){
                    var data = res.data.qpaper;
                    $("#subject").val(data.subject);
                    $("#description").val(data.description);
                    if(data.questions){ //显示问题
                        var cache = [] ;
                        for(var i =0,l=data.questions.length; i<l ;i++){
                            var q = data.questions[i];
                            var lpd = displayQuestionDetail(q.questionType,q.itemDetail,q.id,q.extendInput);
                            cache.push(StrFormatNoEncode(__itemtmp, [q.topic, lpd, q.id, q.questionType, q.extendInput]));
                        }
                        $("#qitemlist").html(cache.join(""))
                    }
                }
            });
        }
    }

    function displayQuestionDetail(type,stringDetail,qid,qnic)
    {
        if(type ==2)
        {
            return "<div class='atomctl'><textarea class='qitemdetail' rows='5' cols='10'></textarea></div>"
        }
        else
        {
            var arrDetail  ;
            if(stringDetail)
            {
               arrDetail = stringDetail.split(/[\r\n]+/ig);
            }
            else
            {
               arrDetail = [];
            }

            var tmp = [];
            for (var i = 0, l = arrDetail.length; i < l;i++)
            {
                tmp.push("<div class='atomctl'><label class='checkbox'>");
                if (type == 0)
                {
                    tmp.push("<input type='radio' value='",Math.pow(2,i),"' name='",qid,"_sc'/>",arrDetail[i]);
                }
                else
                {
                    tmp.push("<input type='checkbox' value='",Math.pow(2,i),"'/>",arrDetail[i])
                }
                tmp.push("</label></div>")
            }
            if(qnic)
            {
                tmp.push("<div class='atomctl'><input type='text' class='nic' value=‘1’/></div>");
            }

            return tmp.join("");

        }


    }
});
