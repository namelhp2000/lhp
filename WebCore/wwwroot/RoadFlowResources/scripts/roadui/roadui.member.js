//组织机构选择
; RoadUI.Member = function () {
    var instance = this;
    this.init = function ($members) {
        $members.each(function (index) {
            var $_member = $members.eq(index);
            var id = $_member.attr("id") || "";
            var name = $_member.attr("name") || "";
            var value = $_member.val() || "";
            var title = $_member.attr("title") || "";
            var ismobile = "1" == ($_member.attr("ismobile") || RoadUI.Core.queryString("ismobile"));
            var disabled = $_member.prop("disabled");

            $_member.prop("readonly", true);
            var $hide = $('<input type="hidden" id="' + id + '" name="' + name + '" value="' + (value || "") + '" />');
            var $but = $('<input type="button" ' + (disabled ? 'disabled="disabled"' : '') + ' class="mybutton" style="margin:0;border-radius: 0 4px 4px 0; -webkit-border-radius: 0 4px 4px 0;" value="选择" />');
            $_member.attr("id", id + "_text");
            $_member.attr("name", name + "_text");
            $_member.css({
                "border-right": "0", "border-radius": "4px 0 0 4px", "-webkit-border-radius":"4px 0 0 4px"
            });
            $_member.removeClass().addClass("mytext");
            if (value && $.trim(value).length > 0) {
                $.ajax({
                    url: "/RoadFlowCore/Controls/Member_GetNames",
                    data: { value: $.trim(value) },
                    type: "post",
                    async: false,
                    success: function (txt) {
                        $_member.val(txt);
                    }
                });
            }

            if ($_member.prop("disabled")) {
                $but.prop("disabled", true);
                $but.removeClass().addClass("buttondisabled");
            }
            else {
                $but.bind("click", function () {
                    var $obj = $(this).prev().prev();
                    var val = $obj.val();
                    var $obj1 = $(this).prev();
                    var isunit = $obj1.attr("unit") || "";
                    var isdept = $obj1.attr("dept") || "";
                    var isstation = $obj1.attr("station") || "";
                    var isuser = $obj1.attr("user") || "";
                    var ismore = $obj1.attr("more") || "";
                    var isgroup = $obj1.attr("group") || "";
                    var isrole = $obj1.attr("role") || "";
                    var rootid = $obj1.attr("rootid") || "";
                    var isall = $obj1.attr("all") || "";
                    var isexecuteflow = $obj1.attr("isChangeType") || "1";
                    var isChangeType = "1" == isexecuteflow ? "1" : "0";
                    var isselect = $obj1.attr("isselect") || "";
                    var openerattr = $obj1.attr("opener") || "";
                    var openerid = $obj1.attr("openerid") || (RoadUI.Core.queryString("tabid") || "");
                    id = $obj.attr("id") || "";

                    var params = "eid=" + id + "&isunit=" + isunit + "&isdept=" + isdept + "&isstation=" + isstation + "&isuser=" + isuser + "&ismore=" + ismore + "&isall=" + isall + "&isgroup=" + isgroup + "&isrole=" + isrole + "&rootid=" + rootid + "&ischangetype=" + isChangeType + "&isselect=" + isselect + "&ismobile=" + (ismobile ? "1" : "0");
                    new RoadUI.Window().open({
                        id: "member_" + id,
                        url: "/RoadFlowCore/Controls/Member_Index?" + params,
                        width: ismobile ? 350 : 500,
                        height: ismobile ? 360 : 470,
                        resize: false,
                        data: { "value": val },
                        title: "选择组织架构成员",
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
        if (value && $.trim(value).length > 0) {
            $.ajax({
                url: "/RoadFlowCore/Controls/Member_GetNames",
                data: { value: value },
                type: "post",
                async: false,
                success: function (txt) {
                    $obj.next().val(txt);
                }
            });
        }
        else {
            $obj.next().val('');
        }

    };
};