//下拉选择
; RoadUI.Select = function () {
    var instance = this;
    this.init = function ($selects) {
        initElement($selects, "select");
    };
    this.init2 = function ($selects) {
        $selects.each(function () {
            var dropdownwidth = $(this).attr("dropdownwidth") || "";
            //var dropdownheight=$(this).attr("dropdownheight")||"";

            $(this).select2({
                allowClear: false,
                placeholder: $(this).attr("placeholder"),
                dropdownWidth: dropdownwidth,
                otherAttr: ''
            });
        });
    };
};