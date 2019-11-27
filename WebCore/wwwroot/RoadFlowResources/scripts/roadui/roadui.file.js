//文件上传
; RoadUI.File = function () {
    var instance = this;
    this.init = function ($files) {
        $files.each(function () {
            var $file = $(this);
            var id = $file.attr("id") || "";
            var name = $file.attr("name") || "";
            var filetype = $file.attr("filetype") || "";
            var value = $file.val();
            var validate = $file.attr("validate") || "";
            var ismobile = "1" == ($file.attr("ismobile") || RoadUI.Core.queryString("ismobile"));
            var isselectuserfile = $file.attr("selectuserfile") || "";
            if (name.length == 0) {
                name = id;
            }
            var $hide = $('<input type="hidden" id="' + id + '" name="' + name + '" value="" ' + (validate && validate.length > 0 ? 'validate="' + validate + '"' : '') + '/>');
            var $but = $('<input type="button" class="mybutton" style="margin:0;border-radius: 0 4px 4px 0; -webkit-border-radius: 0 4px 4px 0;" value="附件" />');
            $file.attr("id", id + "_text");
            $file.attr("name", name + "_text");
            $file.attr("readonly", "readonly");
            $file.removeClass().addClass("mytext");
            $file.css({
                "border-right": "0", "border-radius": "4px 0 0 4px", "-webkit-border-radius": "4px 0 0 4px"
            });
            $hide.val(value);

            if (value.length > 0) {

                $file.val('共' + value.split('|').length + '个文件');
            }
            $but.bind("click", function () {
                var $obj = $(this).prev();
                var $obj1 = $(this).prev().prev();
                var val = $obj1.val();
                var eid = $obj1.attr("id");
                var openerattr = $obj.attr("opener") || "";
                var openerid = $obj.attr("openerid") || (RoadUI.Core.queryString("tabid") || "");
                var url = "/RoadFlowCore/Controls/UploadFiles_Index?eid=" + eid + "&filetype=" + filetype + "&ismobile=" + (ismobile ? "1" : "0") + "&isselectuserfile=" + (isselectuserfile || "1");
                new RoadUI.Window().open({
                    id: "file_" + id,
                    url: url,
                    width: ismobile ? 350 : 790,
                    height: ismobile ? 350 : 498,
                    title: "附件",
                    showclose: true,
                    openerid: openerid,
                    data: { value: val },
                    opener: openerattr ? eval(openerattr) : null
                });
            });
            if (validate && validate.length > 0) {
                $file.removeAttr("validate");
                //$but.after('<span class="msg"></span>');
            }
            $file.after($but).before($hide);
        });
    };
};