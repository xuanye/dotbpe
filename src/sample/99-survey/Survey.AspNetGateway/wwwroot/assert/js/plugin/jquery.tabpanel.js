/// <reference path="../intellisense/jquery-1.4.1-vsdoc.js" />
; (function($) {
    $.fn.tabpanel = function(option) {
        var dfop = {
            items: [], //选项卡数据项 {id,text,classes,disabled,closeable,content,url,cuscall,onactive}
            width: 500,
            height: 400,
            showcloseall: 5, //当item数量大于5个时，显示关闭所有的按钮
            scrollwidth: 100, //如果存在滚动条，点击按钮次每次滚动的距离
            autoscroll: true, //当选项卡宽度大于容器时自动添加滚动按钮
            onfullscreen: false,
            onrestorescreen: false
        };
        var headerheight = 29;
        $.extend(dfop, option);
        var me = $(this).width(dfop.width);
        innerwidth = dfop.width - 2;
        //构建Tab的Html    
        var tcs = dfop.autoscroll ? "x-tab-scrolling-top" : "";
        var header = $("<div class='x-tab-panel-header x-unselectable " + tcs + "' unselectable='on' style='width:" + innerwidth + "px;MozUserSelect:none;KhtmlUserSelect:none;'></div>");
        var stripwrap = $("<div class='x-tab-strip-wrap'/>");
        var scrollerright = $("<div class='x-tab-scroller-right x-unselectable' style='height: 24px; visibility: hidden; mozuserselect: none; khtmluserselect: none;' unselectable='on'/>");
        var scrollerleft = $("<div class='x-tab-scroller-left x-unselectable' style='height: 24px; visibility: hidden; mozuserselect: none; khtmluserselect: none;' unselectable='on'/>");
        var ulwrap = $("<ul class='x-tab-strip x-tab-strip-top'></ul>");
        var stripspacer = $("<div class='x-tab-strip-spacer'/>");
        var litemp = [];
        for (var i = 0, l = dfop.items.length; i < l; i++) {
            var item = dfop.items[i];
            builditemlihtml(item, litemp);
        }
        if (dfop.items.length >= dfop.showcloseall) {
            buildcloseallbtnhtml(litemp);
        }
        litemp.push("<li class='x-tab-edge'/><div class='x-clear'></div>");

        ulwrap.html(litemp.join(""));
        litemp = null;
        stripwrap.append(ulwrap);
        if (dfop.autoscroll) {
            header.append(scrollerright).append(scrollerleft);
        }
        header.append(stripwrap).append(stripspacer);
        var bodyheight = dfop.height - headerheight;
        var bodywrap = $("<div class='x-tab-panel-bwrap'/>");
        var body = $("<div class='x-tab-panel-body x-tab-panel-body-top'/>").css({ width: innerwidth, height: bodyheight });
        var bodytemp = [];
        for (var i = 0, l = dfop.items.length; i < l; i++) {
            var item = dfop.items[i];
            builditembodyhtml(item, bodytemp);
        }

        var awrap = [];
        awrap.push("<a href='javascript:void(0)' id='tab_fa_", item.id, "'  class='x-tab-fullscreen-a' title='点击切换全屏状态'><img alt='点击切换全屏状态' src='../assert/image/icons/s.gif'/></a>");

        body.html(bodytemp.join("")).appendTo(bodywrap);
        me.append(header).append(bodywrap).append(awrap.join(""));

        initevents();
        function builditemlihtml(item, parray) {
            parray.push("<li id='tab_li_", item.id, "' class='", item.isactive ? "x-tab-strip-active" : "", item.disabled ? "x-tab-strip-disabled" : "", item.closeable ? " x-tab-strip-closable" : "", item.classes ? " x-tab-with-icon" : "", "'>");
            parray.push("<a class='x-tab-strip-close' onclick='return false;'/>");
            parray.push("<a class='x-tab-right' onclick='return false;' href='javascript:void(0)'>");
            parray.push("<em class='x-tab-left'><span class='x-tab-strip-inner'><span class='x-tab-strip-text ", item.classes || "", "'>", item.text, "</span></span></em>");
            parray.push("</a></li>");
        }
        function buildcloseallbtnhtml(parray) {
            parray.push("<li id='tab_li_closeall' class='x-tab-closeall'>");
            parray.push("<a hideFocus='hideFocus' title='关闭所有' href='javascript:void(0)'>&nbsp;</a>");
            parray.push("</li>");
        }
        function builditembodyhtml(item, parray) {
            parray.push("<div class='x-panel x-panel-noborder", item.isactive ? "" : " x-hide-display", "' id='tab_item_", item.id, "' style='width:", innerwidth, "px'>");
            parray.push("<div class='x-panel-bwrap'>");
            parray.push("<div class='x-panel-body x-panel-body-noheader x-panel-body-noborder'  id='tab_item_content_", item.id, "' style='position:relative;  width:", innerwidth, "px; height:", bodyheight, "px; overflow: auto;'>");
            if (item.url) {
                parray.push("<iframe name='tab_item_frame_", item.id, "' width='100%' height='100%'  id='tab_item_frame_", item.id, "' src='about:blank' frameBorder='0' />");
                parray.push("<div id='tab_mask_", item.id, "' class=\"x-el-mask\"/>");
                parray.push("<div id='tab_loadingmsg_", item.id, "' class=\"x-el-mask-msg x-mask-loading\"><div>正在加载", item.text, "...</div></div>");
            }
            else if (item.cuscall) {
                parray.push("<div class='loadingicon'/>");
            }
            else {
                parray.push(item.content);
            }
            parray.push("</div></div></div>");
        }
        function initevents() {
            //reset scoller
            resetscoller(); //设置默认是否出现滚动掉
            scollerclick(); //滚动条的点击事件，如果存在的话
            ulwrap.find("li:not(.x-tab-edge)").each(function(e) {
                inititemevents(this); //给每个选项卡 添加事件
            });
            //全屏按钮事件
            $("#tab_fa_" + item.id).click(function() {
                if ($(this).hasClass("x-tab-restorescreen-a")) {
                    restorescreen();
                    $(this).removeClass("x-tab-restorescreen-a");
                }
                else {
                    fullscreen();
                    $(this).addClass("x-tab-restorescreen-a");
                }
            }).focus(function() { $(this).blur() })
            .hover(function() { $(this).animate({ "right": 15, "margin-top": "-31px" }, 200) }, function() { $(this).animate({ "right": 10, "margin-top": "-26px" }, 200) });

        }

        function fullscreen() {
            if (dfop.onfullscreen) {
                dfop.onfullscreen();
            }
        }
        function restorescreen() {
            if (dfop.onrestorescreen) {
                dfop.onrestorescreen();
            }
        }
        function inititemevents(liitem) {
            if (liitem.id == "tab_li_closeall") {
                return;
            }
            liswaphover.call(liitem); //选项卡的鼠标hover效果
            liclick.call(liitem); //选项卡的点击事件
            closeitemclick.call(liitem); // 点击关闭按钮的事件
        }
        function scollerclick() {
            if (dfop.autoscroll) {
                scrollerleft.click(function(e) { scolling("left") });
                scrollerright.click(function(e) { scolling("right") });
            }
        }
        function resetscoller() {
            if (dfop.autoscroll) {
                var edge = ulwrap.find("li.x-tab-edge");
                var eleft = edge.position().left;
                var sleft = stripwrap.scrollLeft(); //.attr("scrollLeft");              
                if (sleft + eleft > innerwidth) {

                    header.addClass("x-tab-scrolling");
                    scrollerleft.css("visibility", "visible");
                    scrollerright.css("visibility", "visible");
                    if (sleft > 0) {
                        scrollerleft.removeClass("x-tab-scroller-left-disabled");
                    }
                    else {
                        scrollerleft.addClass("x-tab-scroller-left-disabled");
                    }
                    if (eleft > innerwidth) {
                        scrollerright.removeClass("x-tab-scroller-right-disabled");
                    }
                    else {
                        scrollerright.addClass("x-tab-scroller-right-disabled");
                    }
                    dfop.showscrollnow = true;

                }
                else {
                    header.removeClass("x-tab-scrolling");
                    stripwrap.animate({ "scrollLeft": 0 }, "fast");
                    scrollerleft.css("visibility", "hidden");
                    scrollerright.css("visibility", "hidden");
                    dfop.showscrollnow = false;
                }
            }
        }
        //
        function scolling(type, max) {
            //debugger;
            if (!dfop.autoscroll || !dfop.showscrollnow) {
                return;
            }
            //debugger;
            //var swidth = stripwrap.attr("scrollWidth");
            var sleft = stripwrap.scrollLeft(); //.attr("scrollLeft");
            var edge = ulwrap.find("li.x-tab-edge");
            var eleft = edge.position().left;
            if (type == "left") {
                if (scrollerleft.hasClass("x-tab-scroller-left-disabled")) {
                    return;
                }
                if (sleft - dfop.scrollwidth - 20 > 0) {
                    sleft -= dfop.scrollwidth;
                }
                else {
                    sleft = 0;
                    scrollerleft.addClass("x-tab-scroller-left-disabled");
                }
                if (scrollerright.hasClass("x-tab-scroller-right-disabled")) {
                    scrollerright.removeClass("x-tab-scroller-right-disabled");
                }
                stripwrap.animate({ "scrollLeft": sleft }, "fast");
            }
            else {
                if (scrollerright.hasClass("x-tab-scroller-right-disabled") && !max) {
                    return;
                }
                //left + ;
                if (max || (eleft > innerwidth && eleft - dfop.scrollwidth - 20 <= innerwidth)) {
                    //debugger;
                    sleft = sleft + eleft - (innerwidth - 38);
                    scrollerright.addClass("x-tab-scroller-right-disabled");
                    // sleft = eleft-innerwidth;
                }
                else {
                    sleft += dfop.scrollwidth;
                }
                if (sleft > 0) {
                    if (scrollerleft.hasClass("x-tab-scroller-left-disabled")) {
                        scrollerleft.removeClass("x-tab-scroller-left-disabled");
                    }
                }
                stripwrap.animate({ "scrollLeft": sleft }, "fast");
            }
        }
        function scollingToli(liitem) {
            var sleft = stripwrap.scrollLeft(); //.attr("scrollLeft");
            var lleft = liitem.position().left;
            var lwidth = liitem.outerWidth();
            var edge = ulwrap.find("li.x-tab-edge");
            var eleft = edge.position().left;
            if (lleft <= 0) {
                sleft += (lleft - 2);
                if (sleft < 0) {
                    sleft = 0;
                    scrollerleft.addClass("x-tab-scroller-left-disabled");
                }
                if (scrollerright.hasClass("x-tab-scroller-right-disabled")) {
                    scrollerright.removeClass("x-tab-scroller-right-disabled");
                }
                stripwrap.animate({ "scrollLeft": sleft }, "fast");
            }
            else {
                if (lleft + lwidth > innerwidth - 40) {
                    sleft = sleft + lleft + lwidth + -innerwidth + 40; // 40 =scrollerleft and scrollerrightwidth;
                    if (scrollerleft.hasClass("x-tab-scroller-left-disabled")) {
                        scrollerleft.removeClass("x-tab-scroller-left-disabled");
                    }
                    //滚到最后一个了，那么就要禁用right;
                    if (eleft - (lleft + lwidth + -innerwidth + 40) <= innerwidth) {
                        scrollerright.addClass("x-tab-scroller-right-disabled");
                    }
                    stripwrap.animate({ "scrollLeft": sleft }, "fast");

                }
            }
            liitem.click();

        }
        function liswaphover() {
            $(this).hover(function(e) {
                if (!$(this).hasClass("x-tab-strip-disabled")) {
                    $(this).addClass("x-tab-strip-over");
                }
            }, function(e) {
                if (!$(this).hasClass("x-tab-strip-disabled")) {
                    $(this).removeClass("x-tab-strip-over");
                }
            });
        }
        function closeitemclick() {
            if ($(this).hasClass("x-tab-strip-closable")) {
                $(this).find("a.x-tab-strip-close").click(function() {
                    deleteitembyliid($(this).parent().attr("id"));
                });
            }
        }
        function closeallitem() {
            if (!confirm("你确认要关闭所有选项卡吗？")) {
                return;
            }
            ulwrap.find("li").remove(".x-tab-strip-closable"); //移除所有可以关闭的tab项
            var t = [];
            var nid = "";
            for (var i = 0, j = dfop.items.length; i < j; i++) {
                if (!dfop.items[i].closeable) {
                    t.push(dfop.items[i]);
                    if (!dfop.items[i].disabled) {
                        nid = dfop.items[i].id;
                    }
                }
            }
            dfop.items = t;
            if (nid != "") {
                $("#tab_li_" + nid).click();
            }
            $("#tab_li_closeall").remove();
            resetscoller();
            scolling("right", true);
        }
        function liclick() {
            $(this).click(function(e) {
                var itemid = this.id.substr(7);
                var curr = getactiveitem();
                if (curr != null && itemid == curr.id) {
                    return;
                }
                var clickitem = getitembyid(itemid);
                if (clickitem && clickitem.disabled) {
                    return;
                }
                if (curr) {
                    $("#tab_li_" + curr.id).removeClass("x-tab-strip-active");
                    $("#tab_item_" + curr.id).addClass("x-hide-display");
                    curr.isactive = false;
                }
                if (clickitem) {

                    $(this).addClass("x-tab-strip-active");
                    $("#tab_item_" + clickitem.id).removeClass("x-hide-display");
                    if (clickitem.url) {
                        frm = document.getElementById("tab_item_frame_" + clickitem.id)
                        var cururl = frm.src;
                        if (cururl == "about:blank") {
                            frm.src = clickitem.url;
                            if ($.browser.msie) {
                                frm.onreadystatechange = function() {
                                    if (frm.readyState == "complete") {
                                        $("#tab_mask_" + clickitem.id).remove();
                                        $("#tab_loadingmsg_" + clickitem.id).remove();
                                        frm.onreadystatechange = null;
                                    }
                                };
                            } else {
                                frm.onload = function() {
                                    $("#tab_mask_" + clickitem.id).remove();
                                    $("#tab_loadingmsg_" + clickitem.id).remove();
                                    frm.onload = null;
                                };
                            }

                            var parent = $(frm).parent();
                            var loadingpanel = $("#tab_loadingmsg_" + clickitem.id);
                            var pos = { left: parent.width() / 2 - loadingpanel.outerWidth() / 2, top: parent.height() / 2 - loadingpanel.outerHeight() / 2 };
                            loadingpanel.css(pos);
                        }
                    }
                    else if (clickitem.cuscall && !clickitem.cuscalled) {
                        var panel = $("#tab_item_content_" + clickitem.id);
                        var ret = clickitem.cuscall(this, clickitem, panel);
                        clickitem.cuscalled = true;
                        if (ret) //如果存在返回值，且不为空
                        {
                            clickitem.content = ret;
                            panel.html(ret);
                        }
                    }
                    clickitem.isactive = true;
                    if (clickitem.onactive) {
                        clickitem.onactive.call(this, clickitem);
                    }
                }
            });
        }
        //获取当前活跃项
        function getactiveitem() {
            for (var i = 0, j = dfop.items.length; i < j; i++) {
                if (dfop.items[i].isactive) {
                    return dfop.items[i];
                    break;
                }
            }
            return null;
        }
        //根据ID获取Item数据
        function getitembyid(id) {
            for (var i = 0, j = dfop.items.length; i < j; i++) {
                if (dfop.items[i].id == id) {
                    return dfop.items[i];
                    break;
                }
            }
            return null;
        }
        function getIndexbyId(id) {
            for (var i = 0, j = dfop.items.length; i < j; i++) {
                if (dfop.items[i].id == id) {
                    return i;
                    break;
                }
            }
            return -1;
        }
        //添加项
        function addtabitem(item) {
            var chkitem = getitembyid(item.id);
            if (!chkitem) {
                var isactive = item.isactive;
                item.isactive = false;
                var lastitem = dfop.items[dfop.items.length - 1];
                dfop.items.push(item);

                var lastli = $("#tab_li_" + lastitem.id);
                var lastdiv = $("#tab_item_" + lastitem.id);
                var litemp = [];
                var bodytemp = [];
                builditemlihtml(item, litemp);
                builditembodyhtml(item, bodytemp);
                var liitem = $(litemp.join(""));
                var bodyitem = $(bodytemp.join(""));
                lastli.after(liitem);
                lastdiv.after(bodyitem);
                //事件
                var li = $("#tab_li_" + item.id);
                inititemevents(li);
                if (isactive) {
                    li.click();
                }
                //debugger;

                if (dfop.items.length >= dfop.showcloseall) {
                    var cabtn = $("#tab_li_closeall");
                    if (cabtn.length == 0) {
                        var cabtntmp = [];
                        buildcloseallbtnhtml(cabtntmp);
                        li.after(cabtntmp.join(""));
                        $("#tab_li_closeall").click(closeallitem);
                    }
                }
                resetscoller();
                scolling("right", true);
            }
            else {
                alert("指定的tab项已存在!");
            }
        }
        function openitemOrAdd(item, allowAdd) {
            var checkitem = getitembyid(item.id);
            if (!checkitem && allowAdd) {
                addtabitem(item);
            }
            else {
                var li = $("#tab_li_" + item.id);
                scollingToli(li);
                var ifrm = $("#tab_item_frame_" + item.id);
                if (ifrm.length > 0 && ifrm[0].src != item.url) {
                    ifrm[0].src = item.url;
                }
            }
        }
        //移除一个tab 项
        function deleteitembyliid(liid) {
            var id = liid.substr(7);
            $("#" + liid).remove();
            $("#tab_item_" + id).remove();
            var index = getIndexbyId(id);
            if (index >= 0) {
                var nextcur;
                if (index < dfop.items.length - 1) {
                    nextcur = dfop.items[index + 1];
                }
                else if (index > 0) {
                    nextcur = dfop.items[index - 1];
                }
                if (nextcur) {
                    $("#tab_li_" + nextcur.id).click();
                }
                dfop.items.splice(index, 1);
                if (dfop.items.length < dfop.showcloseall) {
                    var clbtn = $("#tab_li_closeall");
                    if (clbtn.length > 0) {
                        clbtn.remove();
                    }
                }
                resetscoller();
                scolling("right", true);
            }
        }
        function resize(width, height) {
            if (width == dfop.width && height == dfop.height) {
                return;
            }
            if (width) { dfop.width = width };
            if (height) { dfop.height = height; }
            innerwidth = width - 2;
            bodyheight = dfop.height - headerheight;
            me.css("width", dfop.width);
            header.css("width", innerwidth);
            body.css({ width: innerwidth, height: bodyheight });
            for (var i = 0, j = dfop.items.length; i < j; i++) {
                var item = dfop.items[i];
                $("#tab_item_" + item.id).css({ width: innerwidth });
                $("#tab_item_content_" + item.id).css({ width: innerwidth, height: bodyheight });
            }
            resetscoller();
        }
        //设置选项卡项是否disabled
        function setdisabletabitem(itemId, disabled) {
            var chitem = getitembyid(itemId);
            if (!chitem || chitem.disabled == disabled) {
                return;
            }
            if (disabled) {
                chitem.disabled = true;
                $("#tab_item_" + item.id).addClass("x-tab-strip-disabled");
            }
            else {
                chitem.disabled = false;
                $("#tab_item_" + item.id).removeClass("x-tab-strip-disabled");
            }
        }
        me[0].tab = {
            addtabitem: addtabitem,
            opentabitem: openitemOrAdd,
            resize: resize,
            setdisabletabitem: setdisabletabitem
        };
    };
    $.fn.addtabitem = function(item) {
        if (this[0].tab) {
            return this[0].tab.addtabitem(item);
        }
        return false;
    }
    $.fn.opentabitem = function(item, orAdd) {
        if (this[0].tab) {
            return this[0].tab.opentabitem(item, orAdd);
        }
        return false;
    }
    $.fn.resizetabpanel = function(w, h) {
        if (this[0].tab) {
            return this[0].tab.resize(w, h);
        }
        return false;
    }
    $.fn.setdisabletabitem = function(itemId, disabled) {
        if (this[0].tab) {
            return this[0].tab.setdisabletabitem(itemId, disabled);
        }
        return false;
    }
})(jQuery);
