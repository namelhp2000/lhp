//数据字典选择
; RoadUI.Dict = function () {
    var instance = this;
    this.init = function ($members) {
        $members.each(function (index) {
            var $_member = $members.eq(index);
            var id = $_member.attr("id") || "";
            var name = $_member.attr("name") || id;
            var value = $_member.val() || "";
            var title = $_member.attr("title") || "";
            var datasource = $_member.attr("datasource") || "0";
            var disabled = $_member.prop("disabled");
            var ismobile = "1" == ($_member.attr("ismobile") || RoadUI.Core.queryString("ismobile"));
            $_member.prop("readonly", true);
            var $hide = $('<input type="hidden" id="' + id + '" name="' + name + '" value="' + (value || "") + '" />');
            var $but = $('<input type="button" ' + (disabled ? 'disabled="disabled"' : '') + ' title="' + title + '" class="mybutton" style="margin:0;border-radius: 0 4px 4px 0; -webkit-border-radius: 0 4px 4px 0;" value="选择" />');
            $_member.attr("id", id + "_text");
            $_member.attr("name", name + "_text");
            $_member.css({
                "border-right": "0", "border-radius": "4px 0 0 4px", "-webkit-border-radius": "4px 0 0 4px"
            });
            $_member.removeClass().addClass("mytext");
            if (value && $.trim(value).length > 0) {
                switch (datasource) {
                    case "0":
                    default:
                        $.ajax({
                            url: "/RoadFlowCore/Controls/Dictionary_GetNames",
                            type: "post",
                            data: { "values": value },
                            async: false,
                            success: function (txt) {
                                $_member.val(txt);
                            }
                        });
                        break;
                    case "1":
                        var dbconn = $_member.attr("dbconn");
                        var sql = $_member.attr("sql");
                        $.ajax({
                            url: "/RoadFlowCore/Controls/Dictionary_GetNames_SQL?values=" + encodeURIComponent(value) + "&dbconn=" + dbconn + "&sql=" + sql,
                            type: "get",
                            async: false,
                            success: function (txt) {
                                $_member.val(txt);
                            }
                        });
                        break;
                    case "2": //url
                        var url = $_member.attr("url2");
                        url = url.indexOf('?') >= 0 ? url + "&values=" + encodeURIComponent(value) : url + "?values=" + encodeURIComponent(value);
                        url = $.trim(url).substr(0, 1) == "/" ? RoadUI.Core.rooturl() + url : url;
                        $.ajax({
                            url: url, type: "get", async: false, success: function (txt) {
                                $_member.val(txt);
                            }
                        });
                        break;
                    case "3": //table
                        var dbconn = $_member.attr("dbconn") || "";
                        var dbtable = $_member.attr("dbtable") || "";
                        var valuefield = $_member.attr("valuefield") || "";
                        var titlefield = $_member.attr("titlefield") || "";

                        url = "/RoadFlowCore/Controls/Dictionary_GetNames_Table?values=" + encodeURIComponent(value);
                        url += "&dbconn=" + dbconn + "&dbtable=" + dbtable + "&valuefield=" + valuefield + "&titlefield=" + titlefield;
                        $.ajax({
                            url: url, type: "get", async: false, success: function (txt) {
                                $_member.val(txt);
                            }
                        });
                        break;
                }
            }

            if ($_member.prop("disabled")) {
                $but.prop("disabled", true);
            }
            else {
                $but.bind("click", function () {
                    var $obj = $(this).prev().prev();
                    var val = $obj.val();
                    var $obj1 = $(this).prev();
                    var ismore = ($obj1.attr("more") || $obj1.attr("ismore")) || "1";
                    var isparent = ($obj1.attr("parent") || $obj1.attr("isparent")) || "1";//是否可以选择父节点
                    var root = $obj1.attr("rootid") || "";
                    var isroot = $obj1.attr("isroot") || "1";//是否可以选择根
                    var ischild = $obj1.attr("ischild") || "1";//是否加载所有子节点
                    var dialogtitle = $obj1.attr("dialogtitle") || "选择数据字典";
                    var dbconn = $obj1.attr("dbconn") || "";
                    var sql = $obj1.attr("sql") || "";
                    var url0 = $obj1.attr("url0") || "";
                    var url1 = $obj1.attr("url1") || "";
                    var url2 = $obj1.attr("url2") || "";
                    //var dbconn = $obj1.attr("dbconn") || "";
                    var dbtable = $obj1.attr("dbtable") || "";
                    var valuefield = $obj1.attr("valuefield") || "";
                    var titlefield = $obj1.attr("titlefield") || "";
                    var valuefield = $obj1.attr("valuefield") || "";
                    var parentfield = $obj1.attr("parentfield") || "";
                    var openerattr = $obj1.attr("opener") || "";
                    var openerid = $obj1.attr("openerid") || (RoadUI.Core.queryString("tabid") || "");
                    var where = $obj1.attr("where") || "";
                    id = $obj.attr("id") || "";
                    var params = "eid=" + id + "&datasource=" + datasource + "&dbconn=" + dbconn + "&sql=" + sql
                        + "&url0=" + encodeURIComponent(url0) + "&url1=" + encodeURIComponent(url1) + "&url2=" + encodeURIComponent(url2)
                        + "&ismore=" + ismore + "&isparent=" + isparent + "&root=" + root + "&isroot=" + isroot + "&ischild=" + ischild + "&values=" + encodeURIComponent(val)
                        + "&dbtable=" + dbtable + "&valuefield=" + valuefield + "&titlefield=" + titlefield
                        + "&parentfield=" + parentfield + "&where1=" + encodeURIComponent(where) + "&ismobile=" + (ismobile ? "1" : "0");

                    new RoadUI.Window().open({
                        id: "dict_" + id,
                        url: "/RoadFlowCore/Controls/Dictionary_Index?" + params,
                        width: ismobile ? 350 : 500,
                        height: ismobile ? 350 : 470,
                        resize: false,
                        title: dialogtitle,
                        openerid: openerid,
                        opener: openerattr ? eval(openerattr) : null
                    });
                });
            }

            $_member.after($but).before($hide);
        });
    };
    this.setValue = function (objorid) {
        var $obj;
        if (typeof (objorid) == "string") {
            $obj = $("#" + objorid);
        }
        else {
            $obj = $(objorid);
        }
        if (!$obj || $obj.size() === 0) return;
        var value = $obj.val();
        var datasource = $obj.next().attr("datasource");
        if (value && $.trim(value).length > 0) {
            switch (datasource) {
                case "0": //数据字典
                default:
                    $.ajax({
                        url: "/RoadFlowCore/Controls/Dictionary_GetNames",
                        data: { values: value },
                        type: "post",
                        async: false,
                        success: function (txt) {
                            $obj.next().val(txt);
                        }
                    });
                    break;
                case "1": //sql
                    dbconn = $obj.next().attr("dbconn");
                    var sql = $obj.next().attr("sql");
                    $.ajax({
                        url: "/RoadFlowCore/Controls/Dictionary_GetNames_SQL?values=" + encodeURIComponent(value) + "&dbconn=" + dbconn + "&sql=" + sql,
                        type: "get",
                        async: false,
                        success: function (txt) {
                            $obj.next().val(txt);
                        }
                    });
                    break;
                case "2": //url
                    var url = $obj.next().attr("url2");
                    url = url.indexOf('?') >= 0 ? url + "&values=" + encodeURIComponent(value) : url + "?values=" + encodeURIComponent(value);
                    url = $.trim(url).substr(0, 1) == "/" ? RoadUI.Core.rooturl() + url : url;
                    $.ajax({
                        url: url, type: "get", async: false, success: function (txt) {
                            $obj.next().val(txt);
                        }
                    });
                    break;
                case "3": //table
                    dbconn = $obj.next().attr("dbconn") || "";
                    var dbtable = $obj.next().attr("dbtable") || "";
                    var valuefield = $obj.next().attr("valuefield") || "";
                    var titlefield = $obj.next().attr("titlefield") || "";

                    url = "/RoadFlowCore/Controls/Dictionary_GetNames_Table?values=" + encodeURIComponent(value);
                    url += "&dbconn=" + dbconn + "&dbtable=" + dbtable + "&valuefield=" + valuefield + "&titlefield=" + titlefield;
                    $.ajax({
                        url: url, type: "get", async: false, success: function (txt) {
                            $obj.next().val(txt);
                        }
                    });
                    break;
            }
        }
        else {
            $obj.next().val('');
        }
    };
};