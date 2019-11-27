//选择图标
; RoadUI.SelectIco = function (options) {
    var instance = this;
    var defaults = {
        obj: null,
        x: true
    };

    this.opts = $.extend(defaults, options);
    var $source = $(this.opts.obj);
    var validate = $source.attr('validate');
    var val = $source.val();
    var id = $source.attr('id');
    var name = $source.attr('name');
    var more = $source.attr('more');
    var source = $source.attr("source");
    var title = $source.attr("title");
    var isimg = $source.attr("isimg") || "1";//是否选图片图标
    var isfont = $source.attr("isfont") || "1";//是否选字体图标
    if (title === null || title == undefined) title = "选择图标";


    if (name === null || name == undefined) name = id;
    more = more === null || more == undefined ? "0" : more.toLowerCase() === "true" || more == "1" ? "1" : "0";
    source = source === null || source == undefined ? "/RoadFlowResources/images/ico" : source;

    $source.removeClass().addClass("mytext");
    $source.css({
        "border-right": "0", "border-radius": "4px 0 0 4px", "-webkit-border-radius": "4px 0 0 4px"
    });
    //$source.prop("readonly", true);
    //$source.css({ "background-position-x": "3px", "color": "#666", "background-image": "url(" + RoadUI.Core.rooturl() + RoadUI.Core.rfPath + val + ")", "background-repeat": "no-repeat", "padding-left": "23px", "background-position-y": "center" });
    var $but = $('<input type="button" class="mybutton" style="margin:0;border-radius: 0 4px 4px 0; -webkit-border-radius: 0 4px 4px 0;" value="选择" />');
    var $label = $('<label type="msg"></label>');

    //if (!val || val.substr(0, 2) == "fa")
    //{
    //$source.css({ "padding-left": "3px", "background-image": "" });
    //}

    if ($source.prop("disabled")) {
        $but.prop("disabled", true);
    }
    else {
        $but.bind('click', function () {
            var $obj1 = $(this).prev();
            var openerattr = $obj1.attr("opener") || "";
            var openerid = $obj1.attr("openerid") || (RoadUI.Core.queryString("tabid") || "");
            new RoadUI.Window().open({
                id: "ico_" + id, title: title, width: 800, height: 506,
                openerid: openerid,
                opener: openerattr ? eval(openerattr) : null,
                url: '/RoadFlowCore/Controls/SelectIco_Index?more=' + more + '&id=' + id + '&source=' + source + '&values=' + val + "&isfont=" + isfont + "&isimg=" + isimg, openerid: RoadUI.Core.queryString("tabid")
            });
        });
    }

    $source.after($but, $label);
    this.setIco = function ($obj, ico) {
        if (!ico) {
            ico = $obj.val();
        }

        /* if (!ico || ico.substr(0, 2) == "fa")
         {
             $source.css({ "padding-left": "3px", "background-image": "" });
         }
         else
         {
             $obj.css({ "padding-left": "20px", "background-image": "url(" + RoadUI.Core.rooturl() + ico + ")" });
         }*/
    }
};