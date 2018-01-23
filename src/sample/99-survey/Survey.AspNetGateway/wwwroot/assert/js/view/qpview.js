define(function (require, exports, module) {
    exports.init = function () {
        $("#btnSave").click(function (e) {
            //移除样式
            $("div.error").each(function (i) {
                $(this).removeClass("error");
            });
            var data = {};
            $("input,textarea").each(function (i) {
                var v = data[this.name];
                var cv = $(this).val();
                if (this.tagName.toLowerCase() == "input" && this.type.toLowerCase() != "text") { //checkbox ,radiobox                    
                    cv = this.checked ? cv : "";
                }
                if (v != undefined && v != "") {
                    if (cv != "") {
                        v += ",";
                        v += cv;
                    }
                }
                else {
                    v = cv;
                }
                data[this.name] = v;
            });
            //            var isallcompleted = true;

            //            for (var p in data) {
            //                if (p != "UserId" && p != "Remark" && data[p] == "") {
            //                    $("#" + p).addClass("error");
            //                    isallcompleted = false;
            //                }
            //            }
            //            if (!isallcompleted) {
            //                alert("请填写所有问题！");
            //            }
            //else 
            {
                var url = $("#fmEdit").attr("action");
                $.ajax({
                    type: "POST",
                    url: url,
                    data: data,
                    dataType: "json",
                    success: function (ret) {
                        if (ret.IsSuccess) {
                            alert("提交成功！");
                            //window.close();
                            if (ret.Data && ret.Data > 0) {
                                var _href = buildAjaxHandlerUrl('QManage', 'APaperView');
                                window.location.href = _href + '/' + ret.Data;
                            }
                        }
                        else {
                            alert("提交失败！\r\n" + ret.Msg);
                        }
                    },
                    error: function (err) { }
                });
            }
        });
    }
});