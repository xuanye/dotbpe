/* File Created: 九月 11, 2013 */
define(function (require, exports, module) {
    exports.init = function () {
        $("a.mclose").click(function () {
            CloseModalDialog(null, false, null);
        });
        initData();
    };

    function initData(){
        var id = getUrlParameter("id");
        if(id){ // 编辑
            get("/api/qpaper/sta?qpaperId="+id,{},function(res){
                if(res.return_code ==0){
                    var qpaper = res.data.qpaper;
                    document.title = qpaper.subject ;
                    $("#subject").text(qpaper.subject);
                    $("#description").text(qpaper.description);
                    $("#apaperCount").text(qpaper.apaperCount);

                    var staDetail = res.data.staDetail;
                    var apcount = qpaper.apaperCount || 1;
                    var qSta = {};
                    for(var i=0,l=staDetail.length ; i<l ;i++){
                        qSta[staDetail[i].questionId] = staDetail[i].oa
                    }
                    console.log(qSta);
                    if(qpaper.questions){ //显示问题
                        var cache = [] ;
                        for(var i =0,l= qpaper.questions.length ; i<l ;i++){
                            var q = qpaper.questions[i];
                            console.log(q);
                            var typeName = q.questionType==0?"单选":"多选"

                            var answer = qSta[q.id] || [0,0,0,0,0,0,0,0,0,0,0];
                            console.log(answer);
                            cache.push("<div class='qitem line'>");
                            cache.push("<h6>",q.topic," (",typeName,")</h6>");

                            cache.push("<div class='rowctl'>");


                            cache.push("<table cellpadding='3' cellspacing='0' class='tablelist'>");

                            var arrItem = q.itemDetail.split(/[\r\n]+/ig);
                            for(var j=0,k= arrItem.length ; j<k ;j++){

                                var plenD = Math.round((answer[j] * 230.00 )/ apcount,2);
                                var plen  = parseInt(plenD);
                                var  pecent = Math.round(answer[j] * 100.00 / apcount,2);


                                cache.push("<tr>");
                                cache.push("<td style='width:300px'>");
                                cache.push("<div class='progress_p'><div class='progress_i' style='width:",plen,"px'></div></div> ");
                                cache.push("<div class='progress_s'>",pecent,"%</div>");
                                cache.push("</td>");
                                cache.push("<td>",arrItem[j],"</td>");
                                cache.push("<td style='width:40px'>",answer[j],"</td>");
                                cache.push("</tr>");
                            }


                            cache.push("</table>");
                            cache.push("</div>");
                            cache.push("</div>");
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
});
