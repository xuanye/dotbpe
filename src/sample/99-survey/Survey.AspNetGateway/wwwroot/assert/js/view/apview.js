define(function (require, exports, module) {
    var __itemtmp = "<div class='qitem line' id='{2}' qtype='{3}' qnic='{4}'><h6>{0}</h6><div class='rowctl'>{1}</div></div>";
    exports.init = function () {
        initData();
    }

    function initData(){
        var id = getUrlParameter("id");
        if(id){ // 编辑
            get("/api/apaper/get?paperId="+id,{},function(res){
                if(res.return_code ==0){
                    var qpaper = res.data.qpaper;
                    $("#subject").text(qpaper.subject);
                    $("#description").text(qpaper.description);
                    if(qpaper.questions){ //显示问题
                        var cache = [] ;
                        for(var i =0,l=qpaper.questions.length; i<l ;i++){
                            var q = qpaper.questions[i];

                            var lpd = displayQuestionDetail(q.questionType,q.itemDetail,q.id,q.extendInput);
                            cache.push(StrFormatNoEncode(__itemtmp, [q.topic, lpd, q.id, q.questionType, q.extendInput]));                        }
                        $("#qitemlist").html(cache.join(""))
                    }

                    var apaper =  res.data.apaper;
                    $("#userId").text(apaper.userId);
                    var cache = {};
                    if(apaper.answers){
                        for(var i=0,l=apaper.answers.length ;i<l;i++){
                            var a = apaper.answers[i];
                            cache[a.questionId] = {
                                ov : a.objectiveAnswer,
                                sv : a.subjectiveAnswer
                            }
                        }
                    }
                    setValues(cache)
                }
            });
        }
        else{
            alert("参数错误！");
            window.close();
        }
    }
    function setValues(cache){
        $("#qitemlist input,textarea").each(function (i) {
            var item = cache[this.name];
            if(!item){
                return ;
            }
            if (this.tagName.toLowerCase() == "input" && this.type.toLowerCase() != "text") { //checkbox ,radiobox
                var v = parseInt($(this).val());
                if( (item.ov & v) >0)
                {
                    this.checked = true;
                }
            }
            else{
                $(this).val(item.sv);
            }

        });

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
                tmp.push("<div class='atomctl'><input type='text' class='nic' value=‘1’/></div>");
            }

            return tmp.join("");

        }

    }
});
