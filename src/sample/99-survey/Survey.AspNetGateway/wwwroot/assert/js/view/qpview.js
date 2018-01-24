define(function (require, exports, module) {
    var __itemtmp = "<div class='qitem line' id='{2}' qtype='{3}' qnic='{4}'><h6>{0}</h6><div class='rowctl'>{1}</div></div>";
    exports.init = function () {

        initData();

        $("#btnSave").click(function (e) {
            //移除样式
            $("div.error").each(function (i) {
                $(this).removeClass("error");
            });
            var data = {};
            data.userId = $("#userId").val();

            if(!data.userId){
                alert("请输入用户标识");
                return;
            }

            data.qpaperId = getUrlParameter("id");

            data.answers = [];

            var dictAnswer = {};
            $("#qitemlist input,textarea").each(function (i) {

                var item = dictAnswer[this.name] || {};

                var ov = 0;
                var sv = "";
                if (this.tagName.toLowerCase() == "input" && this.type.toLowerCase() != "text") { //checkbox ,radiobox
                    ov = this.checked ? parseInt($(this).val()) : 0;
                }
                else{
                    sv = $(this).val();
                }

                if (item.questionId) {
                    if(ov>0){
                        item.ov = (item.ov | ov) ;
                    }
                    if(!item.sv){
                        item.sv = sv;
                    }
                    else{
                        item.sv = item.sv+","+sv;
                    }
                }
                else {
                    item.ov = ov ;
                    item.sv = sv;
                    item.questionId = this.name;

                }
                dictAnswer[this.name] = item;
            });

            for(a in dictAnswer){
                if(dictAnswer.hasOwnProperty(a)){
                    data.answers.push({
                        questionId:dictAnswer[a].questionId,
                        objectiveAnswer:dictAnswer[a].ov,
                        subjectiveAnswer:dictAnswer[a].sv
                    })
                }
            }

            var url = "/api/apaper/save";
            $.ajax({
                type: "POST",
                url: url,
                data: JSON.stringify(data),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (ret) {
                    if (ret.return_code ==0) {
                        alert("提交成功！");
                        //window.close();
                        if (ret.data && ret.data.apaperId > 0) {

                            window.location.href =  '/html/viewapaper.html?id=' + ret.data.apaperId;
                        }
                    }
                    else {
                        alert("提交失败！\r\n" + ret.Msg);
                    }
                },
                error: function (err) { }
            });

        });
    }

    function initData(){
        var id = getUrlParameter("id");
        if(id){ // 编辑
            get("/api/qpaper/get?qpaperId="+id,{},function(res){
                if(res.return_code ==0){
                    var data = res.data.qpaper;
                    document.title = data.subject ;
                    $("#subject").text(data.subject);
                    $("#description").text(data.description);
                    if(data.questions){ //显示问题
                        var cache = [] ;
                        for(var i =0,l=data.questions.length; i<l ;i++){
                            var q = data.questions[i];
                            console.log(q)
                            var lpd = displayQuestionDetail(q.questionType,q.itemDetail,q.id,q.extendInput);
                            cache.push(StrFormatNoEncode(__itemtmp, [q.topic, lpd, q.id, q.questionType, q.extendInput]));
                        }
                        $("#qitemlist").html(cache.join(""))
                    }
                }
            });
        }
        else{
            alert("参数错误！");
            window.close();
        }
    }
    function displayQuestionDetail(type,stringDetail,qid,qnic)
    {
        if(type ==2)
        {
            return "<div class='atomctl'><textarea class='qitemdetail' rows='5' name='"+qid+"' cols='10'></textarea></div>";
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
                    tmp.push("<input type='radio' value='",Math.pow(2,i),"' name='",qid,"'/>",arrDetail[i]);
                }
                else
                {
                    tmp.push("<input type='checkbox' value='",Math.pow(2,i),"' name='",qid,"'/>",arrDetail[i])
                }
                tmp.push("</label></div>")
            }
            if(qnic)
            {
                tmp.push("<div class='atomctl'><input type='text' class='nic' /></div>");
            }

            return tmp.join("");

        }

    }
});
